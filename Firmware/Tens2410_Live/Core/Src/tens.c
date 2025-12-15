
#include "tens.h"

void TensInit(Tens_HandleTypeDef* dev, TIM_HandleTypeDef* TensLoopTimer, GPIO_TypeDef* VBoostPort, uint16_t VBoostPin,
				uint8_t VBoostActiveState, uint32_t fsaProgs, uint16_t ChanMinSpeed, uint16_t ChanMaxSpeed){
	dev->_vBoostPort = VBoostPort;
	dev->_vBoostPin = VBoostPin;
	dev->_vBoostActiveState = VBoostActiveState;
	TensvBoostEnable(dev, false);

	//init program storage
	uint8_t n=0;
	for(n=0; n<(NUM_CHANNELS); n++){
		TensClearProgStatus(dev, n);
	}

	dev->progFileVersion = (float)(TENS_PROGRAM_CUR_VER);
	dev->flashStartAddress_Progs = fsaProgs;
	TensReadFileAllocationTable(dev);

	//init the SystemMaster channel
	dev->numChans = 1;

	dev->tensChan[0].chanType = chanType_SystemMaster;
	dev->tensChan[0].chanSpeed = 100;
	dev->tensChan[0].chanMinSpeed = ChanMinSpeed; //20;
	dev->tensChan[0].chanMaxSpeed = ChanMaxSpeed; //5000;


	//testing==
	for(uint8_t n=0; n<NUM_CHANNELS; n++){
	//	dev->curProgNum[n] = 0;
		dev->curProgStatus[n].progState = progState_Empty;
	}
	//=========


	dev->masterSpeed = 100; //dev->speed = 100;
	dev->masterMinSpeed = 10;
	dev->masterMaxSpeed = 1000;

	dev->newDisplayRequest = false;
	memset(dev->displayReqChars, 0, sizeof(dev->displayReqChars));
	memset(dev->displayReqCharsLast, 0, sizeof(dev->displayReqCharsLast));
	dev->displayReqLen = 0;

	dev->imuRef = 0;
	dev->loopTimer = TensLoopTimer;


}

void TensInitWithVEnable(Tens_HandleTypeDef* dev, TIM_HandleTypeDef* TensLoopTimer, GPIO_TypeDef* VBoostPort, uint16_t VBoostPin, uint8_t VBoostActiveState,
		             	 GPIO_TypeDef* VEnablePort, uint16_t VEnablePin, uint8_t VEnableActiveState, uint32_t fsaProgs,
						 uint16_t ChanMinSpeed, uint16_t ChanMaxSpeed){

	dev->_vEnablePort = VEnablePort;
	dev->_vEnablePin = VEnablePin;
	dev->_vEnableActiveState = VEnableActiveState;
	TensInit(dev, TensLoopTimer, VBoostPort, VBoostPin, VBoostActiveState, fsaProgs, ChanMinSpeed, ChanMaxSpeed);

}

void TensReadFileAllocationTable(Tens_HandleTypeDef* dev){
	//FAT Structure:
	// [float] 						FAT Version
	// [uint16]						Number of programs in storage
	// [uint16 * NumberOfPrograms]	Number of lines in each program
	// ...[remaining bytes till the end of the last double word of the FAT]...
	// [program1 Filename][extra byte][structProgLine 0][structProgLine 1]...[structProgLine X][...remaining bytes of last doubleword]
	// [program2 Filename][extra byte][structProgLine 0][structProgLine 1]...[structProgLine X][...remaining bytes of last doubleword]
	// ...
	// [programX Filename][extra byte][structProgLine 0][structProgLine 1]...[structProgLine X][...remaining bytes of last doubleword]

	volatile uint32_t baseAddr = dev->flashStartAddress_Progs;
	volatile uint16_t addrOffset = 0;

	volatile float* pStoredFATVersion = (float *)(baseAddr + addrOffset);
	if(*pStoredFATVersion != dev->progFileVersion){
		dev->numProgFiles = 0;
		TensWriteFileAllocationTable(dev, 0, 0);
		return;
	}
	addrOffset += sizeof(*pStoredFATVersion);

	//Get the number of programs that are stored:
	volatile uint16_t* pNumFiles = (uint16_t *)(baseAddr + addrOffset);
	dev->numProgFiles = *pNumFiles;
	addrOffset += sizeof(dev->numProgFiles);

	//Get the number of lines for each program:
	volatile uint16_t* pNumLines = (uint16_t *)(baseAddr + addrOffset);
	for(uint8_t n=0; n<dev->numProgFiles; n++){
		dev->numProgramLines[n] = *pNumLines;
		pNumLines++;
		addrOffset += sizeof(uint16_t);
	}

	//Calculate the starting address of the first program:
	//uint16_t tmpFatSize = sizeof(float) + sizeof(uint16_t) + (dev->numProgFiles * sizeof(uint16_t) );
	//tmpFatSize += 8 - (tmpFatSize % 8);
	//dlProgStartAddr[0] = tens.flashStartAddress_Progs + TensCalculateFatSize(dlNumProgFiles);
	//dev->progFileAddress[0] = baseAddr + tmpFatSize;
	dev->progFileAddress[0] = baseAddr + TensCalculateFatSize(dev->numProgFiles);

	//Get the starting address of each additional program:
	for(uint8_t n=1; n<dev->numProgFiles; n++){
		//Calculate the space the previous program took up:
		uint32_t prevProgStart = dev->progFileAddress[n-1];
		//uint32_t prevProgByteSize = TENS_FILEHEADER_LEN + (dev->numProgramLines[n-1] * sizeof(structProgLine) );
		uint32_t prevProgByteSize = TensCalculateProgramSize(dev->numProgramLines[n-1]);
		//prevProgByteSize += prevProgByteSize % 8;
		dev->progFileAddress[n] = prevProgStart + prevProgByteSize;
	}
}

uint32_t TensPrepFlashForDownload(Tens_HandleTypeDef* dev){
	// 1. Clear the flash storage.
	volatile uint32_t baseAddr = dev->flashStartAddress_Progs;

	// ===== Clear the flash storage
	//Erase the entire tens program storage area of the flash
	static FLASH_EraseInitTypeDef EraseInitStruct;
	uint32_t PAGEError;

	HAL_FLASH_Unlock();  // Unlock the Flash to enable the flash control register access
	uint32_t StartPage = getFlashPage(baseAddr);

	// Fill EraseInit structure
	EraseInitStruct.TypeErase   = FLASH_TYPEERASE_PAGES;
	EraseInitStruct.Page 	   	= StartPage;
	EraseInitStruct.NbPages     = TENS_PROGRAM_STORAGE_NUMPAGES;

	if (HAL_FLASHEx_Erase(&EraseInitStruct, &PAGEError) != HAL_OK){
		//Error occurred while page erase
		return HAL_FLASH_GetError ();
	}
	HAL_FLASH_Lock();
	return HAL_OK;
}

uint32_t TensCalculateFatSize(uint16_t numFiles){
	uint32_t tmpFatSize = sizeof(float) + sizeof(uint16_t) + (numFiles * sizeof(uint16_t) );
	//tmpFatSize += 8 - (tmpFatSize % 8);
	tmpFatSize += (tmpFatSize % 8 > 0 ? (8- (tmpFatSize % 8) ) : 0);
	return tmpFatSize;
}

uint32_t TensCalculateProgramSize(uint16_t numLines){
	uint32_t curProgByteSize = TENS_FILEHEADER_LEN + (numLines * sizeof(structProgLine) );
	//curProgByteSize +=  8 - (curProgByteSize % 8);
	curProgByteSize += (curProgByteSize % 8 > 0 ? (8 - (curProgByteSize % 8) ) : 0);
	return curProgByteSize;
}

uint32_t TensWriteFileAllocationTable(Tens_HandleTypeDef* dev, uint16_t numFiles, uint16_t* numLines){
	// 1. Calculate the starting address of each program.
	// 2. Write the file allocation table.

	//File Structure:

	// --Start of FAT--  dev->flashStartAddress_Progs
	// [float] 						FAT Version
	// [uint16]						Number of programs in storage
	// [uint16 * NumberOfPrograms]	Number of lines in each program
	// ...[remaining bytes till the end of the last double word of the FAT]...
	// --End of FAT, Start of program storage--
	// [program1 Filename][extra byte][structProgLine 0][structProgLine 1]...[structProgLine X][...remaining bytes of last doubleword]
	// [program2 Filename][extra byte][structProgLine 0][structProgLine 1]...[structProgLine X][...remaining bytes of last doubleword]
	// ...
	// [programX Filename][extra byte][structProgLine 0][structProgLine 1]...[structProgLine X][...remaining bytes of last doubleword]

	volatile uint32_t baseAddr = dev->flashStartAddress_Progs;
/*
	// ===== Clear the flash storage
	//Erase the entire tens program storage area of the flash
	static FLASH_EraseInitTypeDef EraseInitStruct;
	uint32_t PAGEError;

	HAL_FLASH_Unlock();  // Unlock the Flash to enable the flash control register access
	uint32_t StartPage = getFlashPage(baseAddr);

	// Fill EraseInit structure
	EraseInitStruct.TypeErase   = FLASH_TYPEERASE_PAGES;
	EraseInitStruct.Page 	   	= StartPage;
	EraseInitStruct.NbPages     = TENS_PROGRAM_STORAGE_NUMPAGES;

	if (HAL_FLASHEx_Erase(&EraseInitStruct, &PAGEError) != HAL_OK){
		//Error occurred while page erase
		return HAL_FLASH_GetError ();
	}
	HAL_FLASH_Lock();
*/

//	//Load the fat data into a single array.  That array will then be fed to the flash programming function.
//	//uint16_t tmpFatSize = sizeof(float) + sizeof(uint16_t) + (numFiles * sizeof(uint16_t) );
//	//tmpFatSize += 8 - (tmpFatSize % 8);
//	uint16_t tmpFatSize = TensCalculateFatSize(numFiles);
//	uint8_t* fatBytes;
//	fatBytes = calloc(tmpFatSize, sizeof(uint8_t));

	uint16_t tmpFatSize = TensCalculateFatSize(numFiles);
	static uint8_t fatBytes[sizeof(float) + sizeof(uint16_t) + (TENS_PROGRAMS_MAX * 2) + 8];
	memset(&fatBytes, 0, sizeof(fatBytes));

	uint32_t tmpAddr = (uint32_t)fatBytes;
	float* pFatVer = (float *)tmpAddr;
	*pFatVer = dev->progFileVersion;
	tmpAddr += sizeof(float);
	uint16_t* pNumProgs = (uint16_t *)tmpAddr;
	*pNumProgs = numFiles;
	tmpAddr += sizeof(uint16_t);


	uint16_t* pNumLines = (uint16_t *)tmpAddr;
	//start of listing the number of lines for each program
	for(uint8_t n = 0; n<numFiles; n++){
		*pNumLines = numLines[n];
		pNumLines++;
		tmpAddr += sizeof(uint16_t);
	}


	//==== Program the user Flash area with the fat table
	HAL_FLASH_Unlock();
	uint32_t targetAddress = baseAddr;
	uint64_t* pSource = (uint64_t *)fatBytes;
	for(uint16_t i=0; i<(tmpFatSize / 8); i++){
		if (HAL_FLASH_Program(FLASH_TYPEPROGRAM_DOUBLEWORD, targetAddress, pSource[i]) != HAL_OK){
			// Error occurred while writing data in Flash memory
			return HAL_FLASH_GetError();
		}
		targetAddress += 8;
	}

	HAL_FLASH_Lock();
//	free(fatBytes);
	return HAL_OK;
}

uint32_t TensSaveProgramToFlash(Tens_HandleTypeDef* dev, uint16_t progNum, uint32_t* progStartingAddress,
		char* progName, uint16_t numLines, structProgLine* progLines){
	//This function will write one program to the flash.
	//NOTE: The flash area must already be ready for writing (cleared) and the file allocation table must
	//	already be written for this new program prior to calling this function.

	//spot check the validity of the line data:
/*
	uint16_t errorFound = 0;
	for(uint16_t n=0; n< numLines; n++){
		if(progLines[n].command >= tensCommand_LastItem){
			errorFound += 1;
		}else if(progLines[n].channel > NUM_CHANNELS){  	//MUST be greater than numChannels.  If this is == NumChannels then it indicates the command is for the current channel.
			errorFound += 2;
		}else if(progLines[n].gotoFalse >= numLines){
			errorFound += 4;
		}else if(progLines[n].gotoTrue >= numLines){
			errorFound += 8;
		}else if(progLines[n].pi81S >= dataSource_LastItem){
			errorFound += 16;
		}else if(progLines[n].pi82S >= dataSource_LastItem){
			errorFound += 32;
		}else if(progLines[n].pi321S >= dataSource_LastItem){
			errorFound += 64;
		}else if(progLines[n].pi322S >= dataSource_LastItem){
			errorFound += 128;
		}else if(progLines[n].pi323S >= dataSource_LastItem){
			errorFound += 512;
		}else if(progLines[n].polarity >= polLastItem){
			errorFound += 256;
		}
		if(errorFound > 0){
			__NOP();
			while(1);
		}
	}
*/

	//Get the starting address the first program will be written to
	uint32_t targetAddress = progStartingAddress[progNum];

	//Calculate the program size
	uint32_t curProgByteSize = TensCalculateProgramSize(numLines);

	//Create and fill a byte array with the program data
	//uint8_t* progBytes;
	//progBytes = (uint8_t*)calloc(curProgByteSize, sizeof(uint8_t));
	static uint8_t progBytes[TENS_MAX_PROGRAM_BYTES];
	memset(&progBytes, 0, sizeof(progBytes));
	uint16_t pByteIndex = 0;
	memcpy(&progBytes[pByteIndex], progName, TENS_FILENAME_LEN);
	// There's extra header space here... Maybe fill it with your favorite pattern of binary data.
	pByteIndex += TENS_FILEHEADER_LEN;

	//Fill the working buffer with the line data from each line:
	for(uint16_t n=0; n<numLines; n++){
		memcpy(&progBytes[pByteIndex], &progLines[n], sizeof(structProgLine));
		pByteIndex += sizeof(structProgLine);
	}

	//Write the program to flash
	HAL_FLASH_Unlock();
	uint64_t* pSource = (uint64_t *)progBytes;
	for(uint16_t i=0; i<(curProgByteSize / 8); i++){
		if (HAL_FLASH_Program(FLASH_TYPEPROGRAM_DOUBLEWORD, targetAddress, pSource[i]) != HAL_OK){
			// Error occurred while writing data in Flash memory
			return HAL_FLASH_GetError ();
		}
		targetAddress += 8;
	}
	//free(progBytes);

//	}

	HAL_FLASH_Lock();	// Lock the Flash to disable the flash control register access
	return HAL_OK;
}
/*
uint32_t TensSaveDownloadToFlash2(Tens_HandleTypeDef* dev, uint16_t numFiles, void * FileNames, uint16_t* numLines, structProgLine* progs){
	// 1. Clear the flash storage.
	// 2. Calculate the starting address of each program.
	// 3. Write the file allocation table.
	// 4. Write the program data.

	char**	fileNames = FileNames;

	//FAT Structure:
	// [float] 						FAT Version
	// [uint16]						Number of programs in storage
	// [uint16 * NumberOfPrograms]	Number of lines in each program
	// ...[remaining bytes till the end of the last double word of the FAT]...
	// [program1 Filename][extra byte][structProgLine 0][structProgLine 1]...[structProgLine X][...remaining bytes of last doubleword]
	// [program2 Filename][extra byte][structProgLine 0][structProgLine 1]...[structProgLine X][...remaining bytes of last doubleword]
	// ...
	// [programX Filename][extra byte][structProgLine 0][structProgLine 1]...[structProgLine X][...remaining bytes of last doubleword]

	volatile uint32_t baseAddr = dev->flashStartAddress_Progs;

	// ===== Clear the flash storage
	//Erase the entire tens program storage area of the flash
	static FLASH_EraseInitTypeDef EraseInitStruct;
	uint32_t PAGEError;

	HAL_FLASH_Unlock();  // Unlock the Flash to enable the flash control register access
	uint32_t StartPage = getFlashPage(baseAddr);

	// Fill EraseInit structure
	EraseInitStruct.TypeErase   = FLASH_TYPEERASE_PAGES;
	EraseInitStruct.Page 	   	= StartPage;
	EraseInitStruct.NbPages     = TENS_PROGRAM_STORAGE_NUMPAGES;

	if (HAL_FLASHEx_Erase(&EraseInitStruct, &PAGEError) != HAL_OK){
		//Error occurred while page erase
		return HAL_FLASH_GetError ();
	}
	HAL_FLASH_Lock();

	//Load the fat data into a single array.  That array will then be fed to the flash programming function.
	uint16_t tmpFatSize = sizeof(float) + sizeof(uint16_t) + (numFiles * sizeof(uint16_t) );
	tmpFatSize += 8 - (tmpFatSize % 8);
	uint8_t* fatBytes;
	fatBytes = calloc(tmpFatSize, sizeof(uint8_t));


	uint32_t tmpAddr = (uint32_t)fatBytes;
	float* pFatVer = (float *)tmpAddr;
	*pFatVer = dev->progFileVersion;
	tmpAddr += sizeof(float);
	uint16_t* pNumProgs = (uint16_t *)tmpAddr;
	*pNumProgs = numFiles;
	tmpAddr += sizeof(uint16_t);


	uint16_t* pNumLines = (uint16_t *)tmpAddr;
	//start of listing the number of lines for each program
	for(uint8_t n = 0; n<numFiles; n++){
		*pNumLines = numLines[n];
		pNumLines++;
		tmpAddr += sizeof(uint16_t);
	}


	//==== Program the user Flash area with the fat table
	HAL_FLASH_Unlock();
	uint32_t targetAddress = baseAddr;
	uint64_t* pSource = (uint64_t *)fatBytes;
	for(uint16_t i=0; i<(tmpFatSize / 8); i++){
		if (HAL_FLASH_Program(FLASH_TYPEPROGRAM_DOUBLEWORD, targetAddress, pSource[i]) != HAL_OK){
			// Error occurred while writing data in Flash memory
			return HAL_FLASH_GetError ();
		}
		targetAddress += 8;
		//pSource += 8;
	}
	//HAL_FLASH_Lock();	// Lock the Flash to disable the flash control register access

	//==== Write the program info to flash, one program at a time:
	//Get the starting address the first program will be written to
	targetAddress = baseAddr + tmpFatSize;
	for(uint16_t n=0; n<numFiles; n++){

		//Calculate the program size
		uint16_t curProgByteSize = TENS_FILEHEADER_LEN + (numLines[n] * sizeof(structProgLine) );
		curProgByteSize += 8 - (curProgByteSize % 8);

		//Create and fill a byte array with the program data
		uint8_t* progBytes;
		progBytes = (uint8_t*)calloc(curProgByteSize, sizeof(uint8_t));
		uint16_t pByteIndex = 0;
		memcpy(&progBytes[pByteIndex], &fileNames[n], TENS_FILENAME_LEN);
		// There's extra header space here... Maybe fill it with your favorite pattern of binary data.
		pByteIndex += TENS_FILEHEADER_LEN;

		//Calculate the starting index of the first progLine of this program
		uint16_t dlLineIndex = 0;  //starting index of first line of first program.
		for(uint16_t c=0; c<n; c++){
			dlLineIndex += numLines[c];
		}
		//dlLineIndex is now at the starting index for the current program.


		//Fill the working buffer with the line data from each line:
		for(uint16_t pLine=0; pLine<numLines[n]; pLine++){
			memcpy(&progBytes[pByteIndex], &progs[dlLineIndex + pLine], sizeof(structProgLine));
			pByteIndex += sizeof(structProgLine);
		}

		//Write the program to flash
		pSource = (uint64_t *)progBytes;
		for(uint16_t i=0; i<(curProgByteSize / 8); i++){
			if (HAL_FLASH_Program(FLASH_TYPEPROGRAM_DOUBLEWORD, targetAddress, pSource[i]) != HAL_OK){
				// Error occurred while writing data in Flash memory
				return HAL_FLASH_GetError ();
			}
			targetAddress += 8;
		}
		free(progBytes);

	}

	HAL_FLASH_Lock();	// Lock the Flash to disable the flash control register access
	free(fatBytes);
	return HAL_OK;

}
*/

void TensClearProgStatus(Tens_HandleTypeDef* dev, uint8_t chanIndex){
	TensProgramStatus_HandleTypeDef* progStatus = &dev->curProgStatus[chanIndex];

	memset(&dev->curProgLine[chanIndex], 0, sizeof(structProgLine));
	memset(&dev->curProgName, 0, TENS_FILENAME_LEN);
	progStatus->curLineNum = 0;
	progStatus->lineStartedTime = 0;
	progStatus->modOutputDuration = 0;
	progStatus->modDelayDuration = 0;
	progStatus->nextLineNum = 0;
	progStatus->progState = progState_Empty;
}

bool TensFileExists(Tens_HandleTypeDef* dev, uint16_t fileNum){
	if(fileNum < dev->numProgFiles){
		return true;
	}else{
		return false;
	}
}

bool TensReadProgLineFromFlash(Tens_HandleTypeDef* dev, uint8_t chanIndex, uint16_t lineNum){
	uint16_t curProgNum = dev->curProgNum[chanIndex];
	//Verify the current program has enough lines for the requested lineNum
	if(lineNum >= dev->numProgramLines[curProgNum]) return false;

	uint32_t addr = dev->progFileAddress[curProgNum] + TENS_FILEHEADER_LEN;
	structProgLine* curLine = &dev->curProgLine[chanIndex];
	memcpy(curLine, (structProgLine *)(addr + (lineNum * sizeof(structProgLine))), sizeof(structProgLine));

	return true;
}

uint32_t TensGetProgLineAddressFromFlash(Tens_HandleTypeDef* dev, uint16_t progNum, uint16_t lineNum){
	if(progNum >= dev->numProgFiles) return 0;
	if(lineNum >= dev->numProgramLines[progNum]) return 0;
	structProgLine* pLine = (structProgLine*)(dev->progFileAddress[progNum] + TENS_FILEHEADER_LEN);
	pLine += lineNum;
	return (uint32_t)pLine;
}

uint32_t TensLoadFile(Tens_HandleTypeDef* dev, uint8_t chanIndex, uint8_t fileNum){
	TensClearProgStatus(dev, chanIndex);
	if(TensFileExists(dev, fileNum)){
		dev->curProgNum[chanIndex] = fileNum;
		uint32_t addr = dev->progFileAddress[fileNum];
		memcpy(&dev->curProgName[chanIndex], (char *)addr, TENS_FILENAME_LEN);
		TensReadProgLineFromFlash(dev, chanIndex, 0);
		dev->curProgStatus[chanIndex].progState = progState_Stopped;

		return HAL_OK;
	}else{
		return 1; //File does not exist
	}
}

//void TensInitTensChannel(TensChannel_HandleTypeDef* chan, GPIO_TypeDef* PolarityPort, uint16_t PolarityPin,
//						TIM_HandleTypeDef* PulseOutTimerHandle, __IO uint32_t* PulseOutTCCR, uint32_t PulseOutTimChan,
//						DAC_HandleTypeDef* PDAC, uint32_t DacChannel, uint16_t DacMin, uint16_t DacMax, uint16_t PulseWidthMax,
//						uint16_t ChanMinSpeed, uint16_t ChanMaxSpeed){
void TensInitTensChannel(Tens_HandleTypeDef* dev, uint8_t chanIndex, GPIO_TypeDef* PolarityPort, uint16_t PolarityPin,
						TIM_HandleTypeDef* PulseOutTimerHandle, __IO uint32_t* PulseOutTCCR, uint32_t PulseOutTimChan,
						DAC_HandleTypeDef* PDAC, uint32_t DacChannel, uint16_t DacMin, uint16_t DacMax, uint16_t PulseWidthMax,
						uint16_t ChanMinSpeed, uint16_t ChanMaxSpeed){

	TensChannel_HandleTypeDef* chan = &dev->tensChan[chanIndex];

	chan->chanType = chanType_Tens;
	chan->polarityPort = PolarityPort;
	chan->polarityPin = PolarityPin;
	chan->timerHandle = PulseOutTimerHandle;
	chan->pulseOutTCCR = PulseOutTCCR;
	chan->pulseOutTmrChan = PulseOutTimChan;
	chan->pdac = PDAC;
	chan->dacChan = DacChannel;
	chan->dacMin = DacMin;
	chan->dacMax = DacMax;
	chan->pulseWidthMax = PulseWidthMax;
	chan->chanSpeed = 100;
	chan->chanMinSpeed = ChanMinSpeed;
	chan->chanMaxSpeed = ChanMaxSpeed;

	chan->intensityMin = 0;
	chan->intensityMax = 30;	//setting this low during init.  The user's program can always increase it.

	TensSetChannelEnable_Worker(chan, false);
	TensSetPolarity(chan, polForward);
	//TensSetDacVal(chan, 0);
	TensSetTensIntensity(dev, chanIndex, 0);
	TensSetTensOrMotorOutput(chan, 0); //TensSetPulseWidth(chan, 0);

	HAL_TIM_Base_Start_IT(PulseOutTimerHandle);
	HAL_TIM_PWM_Start(PulseOutTimerHandle, PulseOutTimChan);
}

void TensInitAuxChannel(TensChannel_HandleTypeDef* chan, uint16_t ChanMinSpeed, uint16_t ChanMaxSpeed){

	chan->chanType = chanType_Aux;
	chan->chanSpeed = 100;
	chan->chanMinSpeed = ChanMinSpeed;
	chan->chanMaxSpeed = ChanMaxSpeed;

	TensSetChannelEnable_Worker(chan, false);
}

uint32_t randomAtMost(uint32_t max){
	unsigned long
	    // max <= RAND_MAX < ULONG_MAX, so this is okay.
	    num_bins = (unsigned long) max + 1,
	    num_rand = (unsigned long) RAND_MAX + 1,
	    bin_size = num_rand / num_bins,
	    defect   = num_rand % num_bins;

	long x;
	do {
		x = random();
	}
	// This is carefully written not to overflow
	while (num_rand - defect <= (unsigned long)x);

	// Truncated division is intentional
	return x/bin_size;
}

void TensLoop(Tens_HandleTypeDef* dev){
	TensProgramStatus_HandleTypeDef *curProgStatus;

	for(uint8_t n=0; n<NUM_CHANNELS; n++){
		curProgStatus = &dev->curProgStatus[n];

		switch(curProgStatus->progState){
		case progState_Unknown:
		case progState_Empty:
		case progState_Stopped:
		case progState_Paused:
			break;
		case progState_Running:
			TensCheckProgLine(dev, n);
			break;
		case progState_LineComplete:
			//advance to next line if current line is complete
			TensStartProgLine(dev, n, curProgStatus->nextLineNum);
			break;
		default:
			break;
		}
	}

//	TensTurnOffIdleMotors(dev);
}

void TensStartProgLine(Tens_HandleTypeDef* dev, uint8_t chanNum, uint16_t lineToRun){
	if(TensReadProgLineFromFlash(dev, chanNum, lineToRun) == false){
		//error reading program line.
		TensStop(dev, chanNum, progState_Empty);
		return;
	}
	TensProgramStatus_HandleTypeDef *curProg  = &dev->curProgStatus[chanNum];
	structProgLine *curLine = &dev->curProgLine[chanNum];

	curProg->curLineNum = lineToRun;
	curProg->lineStartedTime = HAL_GetTick();

	switch(curLine->command){
	case tensCommand_NoOp:
		//Nothing to do here.
		curProg->progState = progState_LineComplete;
		curProg->nextLineNum = lineToRun + 1;
		break;
	case tensCommand_TenMotOutput:
		//Start a tens output or motor pwm output as long as the channel is not bypassed
		if( (chanNum >= MOT_INDEX_START) && (chanNum < (TENS_INDEX_START + NUM_TENS_CHANNELS)) ){ //This is only available on motor or tens channels.
			if(dev->tensChan[chanNum].isBypassed == false){
				//Start the Tens or PWM output.
				TensStartTensOrPwmOutput(dev, chanNum, curLine);
				curProg->progState = progState_Running;
			}else{
				//Channel is bypassed.  Skip this line and move on to the next line.
				curProg->progState = progState_LineComplete;
			}
		}else{
			//Sysprogs can't directly output to a channel.  Move on to the next line.
			curProg->progState = progState_LineComplete;
		}
		curProg->nextLineNum = lineToRun + 1;
		break;
	case tensCommand_GoTo:
		curProg->progState = progState_LineComplete;
		curProg->nextLineNum = curLine->gotoTrue;
		break;
	case tensCommand_End:
		// Turn off all channels
		TensStop(dev, chanNum, progState_End);
		break;
	case tensCommand_Test:
		bool testResult = TensTestCommand(dev, chanNum, curLine);
		curProg->progState = progState_LineComplete;
		curProg->nextLineNum = (testResult == true) ? curLine->gotoTrue : curLine->gotoFalse;
		break;
	case tensCommand_Set:
		TensSetCommand(dev, chanNum, curLine);
		curProg->progState = progState_LineComplete;
		curProg->nextLineNum = lineToRun + 1;
		break;
	case tensCommand_Delay:
		//Capture the current time, then just let the TensCheckProgLine function monitor for the timer to run out.
		curProg->progState = progState_Running;
		if(chanNum < NUM_CHANNELS){
			uint16_t tmpSpeed = (chanNum > 0) ? dev->tensChan[chanNum].chanSpeed : 100;
			uint32_t tmpDuration = TensGetValue(dev, curLine->pi323S, curLine->pi323V1, curLine->pi323V2, chanNum, 0);
			dev->tensChan[chanNum].origDelayDuration = tmpDuration;
			curProg->modDelayDuration = TensCalculateModDurationVal(dev->masterSpeed, tmpSpeed, tmpDuration);	//grab the original
			//TensStartDelay(dev, chanNum, curLine);
		}
		curProg->nextLineNum = lineToRun + 1;
		break;
	case tensCommand_ProgramControl:
		//NOTE: Only set the progState and nextLineNum  if the program control command is performed on A DIFFERENT
		//CHANNEL THAN THE CURRENT CHANNEL RUNNING THE COMMAND.  If the program control command is being ran on this same
		//channel, the TensProgramControlCommand function will handle setting the necessary progState and nextLineNum statuses.
		if(chanNum != TensProgramControlCommand(dev, chanNum, curLine)){
			curProg->progState = progState_LineComplete;
			curProg->nextLineNum = lineToRun + 1;
		}
		break;
	case tensCommand_Display:
		TensDisplayRequestCommand(dev, chanNum, curLine);
		curProg->progState = progState_LineComplete;
		curProg->nextLineNum = lineToRun + 1;
		break;
	default:
		//ERROR!  Problem with program.
		TensStop(dev, 0, progState_End);
		//while(1);
		TensStart(dev, 0);
	}
}

void TensCheckProgLine(Tens_HandleTypeDef* dev, uint8_t chanNum){
	TensProgramStatus_HandleTypeDef *curProg = &dev->curProgStatus[chanNum];
	structProgLine *curLine = &dev->curProgLine[chanNum];

	switch(curLine->command){
	case tensCommand_NoOp:
		break;
	case tensCommand_TenMotOutput:
		if(dev->tensChan[chanNum].activeState == Active){
			//Nothing to do here.  The channel's timer handles all updates while the program line is running.
			//Update status variables:
			curProg->elapsedOpTime = HAL_GetTick() - curProg->lineStartedTime;
			curProg->remainingOpTime = (curProg->modOutputDuration > curProg->elapsedOpTime ? curProg->modOutputDuration - curProg->elapsedOpTime : 0);
			curProg->remainingTotTime = curProg->remainingOpTime + curProg->modDelayDuration;

		}else if(dev->tensChan[chanNum].activeState == PostDelay){
			//if(HAL_GetTick() - curProg->lineStartedTime > curProg->modDelayDuration){

			//Update status variables:
			curProg->elapsedPdTime = HAL_GetTick() - curProg->postDelayStartedTime;
			curProg->elapsedTotTime = curProg->elapsedOpTime + curProg->elapsedPdTime;
			curProg->remainingPdTime = (curProg->modDelayDuration > curProg->elapsedPdTime ? curProg->modDelayDuration - curProg->elapsedPdTime : 0);
			curProg->remainingTotTime = curProg->remainingPdTime;

			if(HAL_GetTick() - curProg->postDelayStartedTime > curProg->modDelayDuration){
				//PostDelay is complete.  Check for any repeats that need to be run.  If none, the repeat sub will mark the line as completed.
				TensRepeatTenMotLine(dev, chanNum);
			}
		}

		break;
	case tensCommand_GoTo:
	case tensCommand_End:
	case tensCommand_Test:
	case tensCommand_Set:
		break;
	case tensCommand_Delay:
		// Check to see if the delay has finished elapsing
		if(HAL_GetTick() - curProg->lineStartedTime > curProg->modDelayDuration){
			curProg->progState = progState_LineComplete;
		}
		float tmpPctComplete = 0.0f;
		if(curProg->modDelayDuration == 0){
			tmpPctComplete = 100;
		}else{
			tmpPctComplete = (HAL_GetTick() - curProg->lineStartedTime) / curProg->modDelayDuration * 100;
		}
		tmpPctComplete = (tmpPctComplete < 0 ? 0 : (tmpPctComplete > 100 ? 100 : tmpPctComplete) );
		dev->tensChan[chanNum].pctComplete = tmpPctComplete;

		break;
	case tensCommand_ProgramControl:
		break;
	case tensCommand_Display:
		break;
	default:
	}
}

/*
void TensStartTensOrPwmOutput(Tens_HandleTypeDef* dev, uint8_t chanNum, structProgLine *curLine){
	if(chanNum >= NUM_CHANNELS) return;
	TensChannel_HandleTypeDef *chan = &dev->tensChan[chanNum];

	//Make sure the vBoost is active:
	if( (chan->chanType == chanType_Tens) && (dev->_vBoostCurState == false) ){
		TensvBoostEnable(dev, true);
	}

	//Note: All speed and output values are based on 0-100%, NOT based on the corresponding timer channel pwm values for those percentages.

	chan->startTime = HAL_GetTick();
	chan->origDuration = TensGetValue(dev, curLine->pi321S, curLine->pi321V1, curLine->pi321V2, chanNum, 0);
	chan->startVal = (uint8_t)TensGetValue(dev, curLine->pi81S, curLine->pi81V1, curLine->pi81V2, chanNum, 100);
	chan->endVal = (uint8_t)TensGetValue(dev, curLine->pi82S, curLine->pi82V1, curLine->pi82V2, chanNum, 100);
	chan->deltaVal = chan->endVal - chan->startVal;
	chan->repeatCounter = (uint16_t)TensGetValue(dev, curLine->pi322S, curLine->pi322V1, curLine->pi322V2, chanNum, 65535);

	dev->curProgStatus[chanNum].modDuration = TensCalculateModDurationVal(dev->masterSpeed, chan->chanSpeed, chan->origDuration);
	chan->pctComplete = 0;
	TensCalculateTensMotorStepVals(chan, dev->curProgStatus[chanNum].modDuration);

	//Set polarity:
	TensSetPolarity(chan, curLine->polarity);

	chan->curVal = chan->startVal;  //startVal is a uint32 and curVal is a float-- check this in operation.
	TensSetTensOrMotorOutput(chan, (chan->startVal));
	chan->isActive = true;
}
*/

//void TensStartDelay(Tens_HandleTypeDef* dev, uint8_t chanNum, structProgLine *curLine){
//	uint16_t tmpSpeed = (chanNum > 0) ? dev->tensChan[chanNum].chanSpeed : 100;
//	uint32_t tmpDuration = TensGetValue(dev, curLine->pi323S, curLine->pi323V1, curLine->pi323V2, chanNum, 0);
//	//curProg->modDuration = TensCalculateModDurationVal(dev->masterSpeed, tmpSpeed, tmpDuration);	//grab the original
//	dev->curProgStatus[chanNum].modDelayDuration = TensCalculateModDurationVal(dev->masterSpeed, tmpSpeed, tmpDuration);	//grab the original
//}

void TensStartTensOrPwmOutput(Tens_HandleTypeDef* dev, uint8_t chanNum, structProgLine *curLine){
	if(chanNum >= NUM_CHANNELS) return;
	TensChannel_HandleTypeDef *chan = &dev->tensChan[chanNum];

	//Make sure the vBoost is active:
	if( (chan->chanType == chanType_Tens) && (dev->_vBoostCurState == false) ){
		TensvBoostEnable(dev, true);
	}

	//Note: All speed and output values are based on 0-100%, NOT based on the corresponding timer channel pwm values for those percentages.
	chan->startTime = HAL_GetTick();
	//chan->origDuration = TensGetValue(dev, curLine->pi321S, curLine->pi321V1, curLine->pi321V2, chanNum, 0);
	chan->origOutputDuration = TensGetValue(dev, curLine->pi321S, curLine->pi321V1, curLine->pi321V2, chanNum, 0);
	chan->origDelayDuration = TensGetValue(dev, curLine->pi323S, curLine->pi323V1, curLine->pi323V2, chanNum, 0);
	chan->repeatCounter = (uint16_t)TensGetValue(dev, curLine->pi322S, curLine->pi322V1, curLine->pi322V2, chanNum, 65535);

	//TODO: implement Post-Delay!
	//chan->postdelay = TensGetValue(dev, curLine->pi323S, curLine->pi323V2, curLine->pi323V2, chanNum, 0);

	chan->startVal = (uint8_t)TensGetValue(dev, curLine->pi81S, curLine->pi81V1, curLine->pi81V2, chanNum, 100);
	chan->endVal = (uint8_t)TensGetValue(dev, curLine->pi82S, curLine->pi82V1, curLine->pi82V2, chanNum, 100);
	chan->deltaVal = chan->endVal - chan->startVal;


	TensProgramStatus_HandleTypeDef *curProg = &dev->curProgStatus[chanNum];
	curProg->modOutputDuration = TensCalculateModDurationVal(dev->masterSpeed, chan->chanSpeed, chan->origOutputDuration);
	curProg->modDelayDuration = TensCalculateModDurationVal(dev->masterSpeed, chan->chanSpeed, chan->origDelayDuration);

	//=== update variables used for reporting out ChanSettings for program lines:
	curProg->remainingOpTime = curProg->modOutputDuration;
	curProg->elapsedOpTime = 0;
	curProg->postDelayStartedTime = 0;
	curProg->elapsedPdTime = 0;
	curProg->remainingPdTime = curProg->modDelayDuration;
	curProg->elapsedTotTime = 0;
	curProg->remainingTotTime = curProg->modOutputDuration + curProg->modDelayDuration;
	//== end update variables

	chan->pctComplete = 0;
	//TensCalculateTensMotorStepVals(chan, dev->curProgStatus[chanNum].modDuration);
	TensCalculateTensMotorStepVals(chan, dev->curProgStatus[chanNum].modOutputDuration);

	//Set polarity:
	TensSetPolarity(chan, curLine->polarity);

	chan->curVal = chan->startVal;  //startVal is a uint32 and curVal is a float-- check this in operation.
	TensSetTensOrMotorOutput(chan, (chan->startVal));
	//chan->isActive = true;
	chan->activeState = Active;


}

void TensRepeatTenMotLine(Tens_HandleTypeDef* dev, uint8_t chanNum){
	TensChannel_HandleTypeDef *chan = &dev->tensChan[chanNum];
	TensProgramStatus_HandleTypeDef *curProg = &dev->curProgStatus[chanNum];
	//structProgLine *curLine = &dev->curProgLine[chanNum];

	//This output operation is complete.  Are there any repeats left?
	if(chan->repeatCounter > 0){
		chan->activeState = Active;
		chan->repeatCounter--;
		//reset the start time
		chan->startTime = HAL_GetTick();
		//handle polarity:
		if(!( (chan->polarity == polForward) || (chan->polarity == polReverse) )){
			//Toggle polarity:
			TensChangePolarityOutputs(chan, (chan->lastPolarity == polForward) ? polReverse : polForward);
		}
		//setup and start the output from the beginning:
		chan->curVal = chan->startVal;
		chan->pctComplete = 0;
		TensSetTensOrMotorOutput(chan, chan->startVal); //TensSetPulseWidth(chan, (chan->curVal1000 / 1000) );
	}else{
		//Operation is complete, no repeats left.  Shut off the output and mark it complete.
		TensSetTensOrMotorOutput(chan, 0);
		chan->activeState = notActive;
		curProg->progState = progState_LineComplete;
	}

}


void TensSetSpeed(Tens_HandleTypeDef* dev, uint8_t chanIndex, uint16_t newSpeed){
	// chanIndex: index num of the channel (chanIndex < NUM_MOTOR_CHANNELS + NUM_TENS_CHANELS), or masterSpeed
	// 				if chanIndex == NUM_MOTOR_CHANNELS + NUM_TENS_CHANELS
	// newSpeed: 10-1000, represents a percentage (10-99 = slow, 100 = normal, 101-1000=fast)

	TensChannel_HandleTypeDef *chan;
	//if(newSpeed < 1) newSpeed = 1;
	if(newSpeed < dev->tensChan[chanIndex].chanMinSpeed) newSpeed = dev->tensChan[chanIndex].chanMinSpeed;
	//if(newSpeed > 1000) newSpeed = 1000;
	if(newSpeed > dev->tensChan[chanIndex].chanMaxSpeed) newSpeed = dev->tensChan[chanIndex].chanMaxSpeed;

	if(chanIndex > 0){
		chan = &dev->tensChan[chanIndex];
		if(chan->chanSpeed != newSpeed){
			chan->chanSpeed = newSpeed;
			//Update the modDuration for this single channel, but only modify the stepVals if the channel is active.  (Note: modDuration applies to Tens/PWM outputs as well as Delay commands, but the Delay commands do not have the channel marked as 'Active' while the delay is running.)
			//dev->curProgStatus[chanIndex].modDuration = TensCalculateModDurationVal(dev->masterSpeed, chan->chanSpeed, chan->origDuration);
			dev->curProgStatus[chanIndex].modOutputDuration = TensCalculateModDurationVal(dev->masterSpeed, chan->chanSpeed, chan->origOutputDuration);
			dev->curProgStatus[chanIndex].modDelayDuration = TensCalculateModDurationVal(dev->masterSpeed, chan->chanSpeed, chan->origDelayDuration);
			TensCalculateTensMotorStepVals(chan, dev->curProgStatus[chanIndex].modOutputDuration);
		}
	}else{
		if(dev->tensChan[0].chanSpeed != newSpeed){
			dev->tensChan[0].chanSpeed = newSpeed;
			//If any channels are active then update their modified duration value:
			for(uint8_t n=1; n<NUM_CHANNELS; n++){
				chan = &dev->tensChan[n];
				//Update the modDuration for all channels, but only modify the motorStepVals for channels that are active.
				//dev->curProgStatus[chanIndex].modDuration = TensCalculateModDurationVal(newSpeed, chan->chanSpeed, chan->origDuration);
				dev->curProgStatus[chanIndex].modOutputDuration = TensCalculateModDurationVal(newSpeed, chan->chanSpeed, chan->origOutputDuration);
				dev->curProgStatus[chanIndex].modDelayDuration = TensCalculateModDurationVal(newSpeed, chan->chanSpeed, chan->origDelayDuration);
				//if(chan->isActive){
				if(chan->activeState != notActive){
					TensCalculateTensMotorStepVals(chan, dev->curProgStatus[chanIndex].modOutputDuration);
				}
			}
		}
	}
}

uint32_t TensCalculateTensMotorTimerVal(TensChannel_HandleTypeDef* chan, uint8_t outputPercentageVal){
	uint32_t retVal = 0;

	//return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
	outputPercentageVal = (outputPercentageVal > 100) ? 100 : outputPercentageVal;
	if(outputPercentageVal != 0){
		if(chan->chanType == chanType_Tens){
			//if(chan->dacVal > 0){
			if(chan->curIntensityPct > 0){
				retVal = outputPercentageVal * chan->pulseWidthMax / 100;
			}
		}else if(chan->chanType == chanType_Motor){
			retVal = outputPercentageVal * (chan->motorMaxOutput - chan->motorMinOutput) / 100 + chan->motorMinOutput;
		}
	}
	return retVal;
}

uint32_t TensCalculateModDurationVal(uint16_t masterSpeed, uint16_t chanSpeed, uint32_t origDuration){
	// Calculate the remaining duration based on the new speed value.  This function is used
	// for initial setup of an output function (tens pulse and/or motor output) and it's also
	// used when the system speed changes mid-operation.

	// To override the function from applying a master speed scaling, just pass in 100 for the masterSpeed.

	//Calculate speed multiplier
	uint32_t tmpSpeed = (chanSpeed * masterSpeed) / 100;
	if(tmpSpeed < 10) tmpSpeed = 10;
	if(tmpSpeed > 1000) tmpSpeed = 1000;

	//Calculate modified duration
	uint32_t tmpDuration = (origDuration * 100) / tmpSpeed;

//alex:  Maybe recalculate the startTime of the channel here?  That would help with calculating the elapsed time and remaining time in the getValue function.
//alex:  Not sure what else uses the startTime though- it might break something else?
	return tmpDuration;
}


void TensCalculateTensMotorStepVals(TensChannel_HandleTypeDef* chan, uint32_t modDuration){
	//calculate how many steps will be involved in an output operation and how much the output percentage should change per timer cycle.
	//NOTE: the startVal, endVal, and duration values should already have been set in the chan object before calling this function.

	uint32_t tmpNumLoops = 0;

	if(chan->chanType == chanType_Motor){
		//Motor timer is running at 80MHz with a prescaler of 800, and the counter period is 100, so: 80,000,000 / 800 / 100 = 1ms per loop.
		tmpNumLoops = modDuration;  //duration is in ms, so this is a direct conversion.
	}else if(chan->chanType == chanType_Tens){
		//Tens timer is running at 80MHz with a prescaler of 125, and the counter period is 10000, so: 80,000,000 / 125 / 10,000 = 15.625ms per loop.
		tmpNumLoops = (modDuration * 1000) / 15625;  // 15.625 * 1000
	}

	if(tmpNumLoops > 0){
		//Calculate the percentage change that will be applied each loop:
		//chan->pctStep = chan->deltaVal / tmpNumLoops;  ======This causes an error if the deltaVal = 0.  (e.g. StartVal=50, EndVal=50 --> deltaVal = 0)
		chan->pctCompletePerStep = (float)(100.0f / tmpNumLoops);
		chan->pctCompletePerStep = (chan->pctCompletePerStep < 0) ? -chan->pctCompletePerStep : chan->pctCompletePerStep;
	}else{
		chan->pctCompletePerStep = 100;
	}
}

/*
void TensChangeSystemState(Tens_HandleTypeDef* dev, enumProgState newState){
	dev->systemState = newState;
	for(uint8_t n=0; n<(NUM_CHANNELS + NUM_SYS_PROGS); n++){
		if(newState == progState_Running){
			//Start all channels that are not disabled
			TensStart(dev, n);
		}else if((newState == progState_Paused) || (newState == progState_Stopped)){
			//Set all running channels to "paused"
			TensStop(dev, n, newState);
		}
	}


	//Load a program into one of the tens channels.
	//TensLoadFile(dev, 2, 0);
}
*/

void TensStart(Tens_HandleTypeDef* dev, uint8_t chanNum){
//	if(chanNum == 0){
//		for(uint8_t n=1; n<NUM_CHANNELS; n++){
//			TensStart(dev, n);
//		}
//	}

	if(chanNum == 0){
		//start the loop timer
		HAL_TIM_Base_Start_IT(dev->loopTimer);
	}

	TensProgramStatus_HandleTypeDef *curProg = &dev->curProgStatus[chanNum];
	if(curProg->progState == progState_Empty){
		return;
	}

	if(curProg->progState == progState_End){
		TensStartProgLine(dev, chanNum, 0);
	}else{
		TensStartProgLine(dev, chanNum, curProg->curLineNum);
	}
}


void TensStop(Tens_HandleTypeDef* dev, uint8_t chanNum, enumProgState newState){
	//This function is used to pause or stop output activity for a single channel (progNum < NUM_CHANNELS)
	// or for all channels (progNum >= NUM_CHANNELS).

	// Pause: Channel output is stopped, progState marked as Paused, curLine is unchanged.
	// Stop:  Channel output is stopped, progState marked as Stopped, curLine reset to 0, digital outputs set to safe state.
	// End:   Channel output is stopped, progState marked as End, curLine is unchanged, digital outputs set to safe state.

	if(!( (newState == progState_Paused) || (newState == progState_Stopped) || (newState == progState_End) ) ){
		return;
	}

	if(chanNum == 0){
		// Stop all channels
		for(uint8_t n=1; n<NUM_CHANNELS; n++){
			TensStop(dev, n, newState);
		}

		//stop the loop timer
		HAL_TIM_Base_Stop_IT(dev->loopTimer);

	}

	TensProgramStatus_HandleTypeDef *curProg = &dev->curProgStatus[chanNum];

	// Stop 1 channel
	if( (curProg->progState == progState_Running)
			|| (curProg->progState == progState_Paused)
			|| (curProg->progState == progState_LineComplete) ){
		//dev->tensChan[chanNum].isActive = false;
		dev->tensChan[chanNum].activeState = notActive;
		curProg->progState = newState;
		TensSetTensOrMotorOutput(&dev->tensChan[chanNum], 0);
		if(newState == progState_Stopped){
			curProg->curLineNum = 0;
		}
	}
}

void TensSetPolarity(TensChannel_HandleTypeDef* chan, enumPolarity newVal){
	switch(newVal){
	case polForward:
	case polForward_ToggleCycle:
	case polForward_TogglePulse:
		TensChangePolarityOutputs(chan, polForward);
		break;
	case polReverse:
	case polReverse_ToggleCycle:
	case polReverse_TogglePulse:
		TensChangePolarityOutputs(chan, polReverse);
		break;
	default:
		return;
		break;
	}
	chan->polarity = newVal;
}

void TensSwapPolarity(TensChannel_HandleTypeDef* chan, bool SwapPolarity){
	chan->polaritySwapped = SwapPolarity;
}

void TensChangePolarityOutputs(TensChannel_HandleTypeDef* chan, enumPolarity newVal){
	//figure out if the polarity has been (temporarily) swapped in software to accommodate changing electrode polarity.
	uint8_t finalPolarityVal = 0;

	if(newVal == polForward){
		//set to forward
		finalPolarityVal = (chan->polaritySwapped == false ? 1 : 0);
		if(chan->chanType == chanType_Tens){
			HAL_GPIO_WritePin(chan->polarityPort, chan->polarityPin, finalPolarityVal);
		}else if(chan->chanType == chanType_Motor){
			HAL_GPIO_WritePin(chan->pmotorDirPort, chan->motorDirPin, finalPolarityVal);
		}
		chan->lastPolarity = polForward;
	}else{
		//set to reverse
		finalPolarityVal = (chan->polaritySwapped == false ? 0 : 1);
		if(chan->chanType == chanType_Tens){
			HAL_GPIO_WritePin(chan->polarityPort, chan->polarityPin, finalPolarityVal);
		}else if(chan->chanType == chanType_Motor){
			HAL_GPIO_WritePin(chan->pmotorDirPort, chan->motorDirPin, finalPolarityVal);
		}
		chan->lastPolarity = polReverse;
	}
}

void TensSetTensOrMotorOutput(TensChannel_HandleTypeDef* chan, uint8_t newVal){
	//Note1: the direction (polarity) should have already been set before calling this function.

	if(chan->timerHandle->Instance == TIM17){
		__NOP();
	}

	if(chan->chanType == chanType_Tens){
		//map the 0-100 percentage value to a timer pulsewidth value
		*chan->pulseOutTCCR = TensCalculateTensMotorTimerVal(chan, (chan->chanEnabled) ? newVal : 0);
	}else if(chan->chanType == chanType_Motor){
		*chan->pmotorTCCR = TensCalculateTensMotorTimerVal(chan, (chan->chanEnabled) ? newVal : 0);
	}
}

void TensEnableChannel(Tens_HandleTypeDef* dev, uint8_t index, bool EnableVal){
	//if(index == 0){
	//	for(uint8_t n=0; n<NUM_CHANNELS; n++){
	//		TensSetChannelEnable_Worker(&dev->tensChan[n], EnableVal);
	//	}
	//}else{
	//	TensSetChannelEnable_Worker(&dev->tensChan[index], EnableVal);
	//}

	if(index == 0){
		//Start or stop
		for(uint8_t n=0; n<NUM_CHANNELS; n++){
			if(EnableVal == true){
				TensStart(dev, n);
			}else{
				//TensStop(dev, n, progState_Paused);
				TensStop(dev, n, progState_Stopped);
			}
			TensSetChannelEnable_Worker(&dev->tensChan[n], EnableVal);
		}
	}else{
		if(EnableVal == true){
			TensStart(dev, index);
		}else{
			TensStop(dev, index, progState_Paused);
		}
		TensSetChannelEnable_Worker(&dev->tensChan[index], EnableVal);
	}
}

void TensSetChannelEnable_Worker(TensChannel_HandleTypeDef* chan, bool EnableVal){
	chan->chanEnabled = EnableVal;

	if(!EnableVal){
		TensSetTensOrMotorOutput(chan, 0);
	}

}




//void TensSetDacVal(Tens_HandleTypeDef* dev, uint8_t index, uint16_t newVal){
void TensSetTensIntensity(Tens_HandleTypeDef* dev, uint8_t index, uint16_t newVal){
	//The rest of the tens software uses 0-100 for dac values, but the actual DAC itself uses
	//12-bit (0-4096).  There is also a dacMaxVal that acts as the highest value we can set the DAC to.
	//So we'll map newVal (0-100) to a value of (0 - dacMaxVal) for the DAC setting.


	//Testing results on DB 2410B2.  Tested with 2x4 pads
	//Test					Pad V	Dac Val		Dac Voltage
	// Min Value					830			0.675
	// Start Feeling		8.4		840
	// Low end of regular	17.2	898
	// High end of regular	30		979
	// Very High			42		1055
	// Max					50		1190		0.968

	TensChannel_HandleTypeDef* chan = &dev->tensChan[index];
	newVal = (newVal > 100) ? 100 : newVal;

//	if(chan->chanType == chanType_Tens){
//		//Map function: return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
//		//Calculate the max dac value based on the user-defined intensityMax setting:
//		uint32_t scaledMaxDac = chan->intensityMax * (chan->dacMax - chan->dacMin) / 100 + chan->dacMin;
//		//Calculate the dac value based on our new dacMax scaled value
//		uint32_t tmpVal = (newVal - 0) * (scaledMaxDac - chan->dacMin) / (100) + chan->dacMin;
//		if( (tmpVal >= 0) && (tmpVal <= scaledMaxDac) ){
//			HAL_DAC_SetValue(chan->pdac, chan->dacChan, DAC_ALIGN_12B_R, tmpVal);
//			//Multiple channels might share the same DAC output.  Update the curIntensity for all channels that share this DAC.
//			for(uint8_t n=TENS_INDEX_START; n<(TENS_INDEX_START + NUM_TENS_CHANNELS); n++){
//				if( (dev->tensChan[n].pdac == chan->pdac) && (dev->tensChan[n].dacChan == chan->dacChan) ){
//					//We're either referencing ourself here (n == index) or this is another channel using the same dac.
//					dev->tensChan[n].curIntensityPct = newVal;
//				}
//			}
//		}
//	}
	if(chan->chanType == chanType_Tens){
		//1. Calculate temporary dacMin and dacMax values based on the channel's current intensityMin & intensityMax percentage settings.
		//2. Calculate a dac output value using the incoming percentVal (newVal) that is mapped against the temp scaledMinDac
		//		and scaledMaxDac values obtained in step 1.
		//3. Set the DAC to the new dac value created in step 2.
		//4. Determine whether any other channels share this same DAC.  If so, thier DAC value will simultaneously be updated when this
		//		channel's dac is updated (duh, same output!).  Update the curIntensityPct for those other channels so it will correctly
		//		represent that channel's new (current) output intensity.


		//1:
		//Map function: return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
		//Calculate the max dac value based on the user-defined intensityMax setting:
		uint32_t scaledMaxDacVal = (chan->intensityMax) * (chan->dacMax - chan->dacMin) / (100 - 0) + chan->dacMin;

		//Calculate the min dac value based on the user-defined intensityMin setting:
		uint32_t scaledMinDacVal = (chan->intensityMin) * (chan->dacMax - chan->dacMin) / (100 - 0) + chan->dacMin;

		//2:
		//Calculate the final dac value based on our new scaledMinDac and scaledMaxDac values
		uint32_t tmpScaledFinalDac = (newVal - 0) * (scaledMaxDacVal - scaledMinDacVal) / (100 - 0) + scaledMinDacVal;
		tmpScaledFinalDac = (tmpScaledFinalDac >= scaledMinDacVal ? tmpScaledFinalDac : scaledMinDacVal);
		tmpScaledFinalDac = (tmpScaledFinalDac <= scaledMaxDacVal ? tmpScaledFinalDac : scaledMaxDacVal);

		//3:
		HAL_DAC_SetValue(chan->pdac, chan->dacChan, DAC_ALIGN_12B_R, tmpScaledFinalDac);

		//4:
		//Multiple channels might share the same DAC output.  Update the curIntensity for all channels that share this DAC.
		for(uint8_t n=TENS_INDEX_START; n<(TENS_INDEX_START + NUM_TENS_CHANNELS); n++){
			if( (dev->tensChan[n].pdac == chan->pdac) && (dev->tensChan[n].dacChan == chan->dacChan) ){
				//We're either referencing ourself here (n == index) or this is another channel using the same dac.
				dev->tensChan[n].curIntensityPct = newVal;
			}
		}
	}
}
void TensSetTensMinIntensity(Tens_HandleTypeDef* dev, uint8_t index, uint16_t newVal){
	//Multiple channels might share the same DAC output.  Update the minIntensity for all channels that share this DAC.
	TensChannel_HandleTypeDef *chan = &dev->tensChan[index];
	for(uint8_t n=TENS_INDEX_START; n<(TENS_INDEX_START + NUM_TENS_CHANNELS); n++){
		if( (dev->tensChan[n].pdac == chan->pdac) && (dev->tensChan[n].dacChan == chan->dacChan) ){
			//We're either referencing ourself here (n == index) or this is another channel using the same dac.
			dev->tensChan[n].intensityMin = newVal;
		}
	}
}
void TensSetTensMaxIntensity(Tens_HandleTypeDef* dev, uint8_t index, uint16_t newVal){
	//Set the max intensity for this channel and all other channels that share the same DAC.
	//After setting the max intensity, check what the current DAC output is and calculate the current intensity value
	//  based on the new maxIntensity value.  As long as the DAC output is less than the new max value then the result
	//  is that the output voltage remains constant, but the curIntensity value will change to reflect the current output
	//  voltage with respect to the maxIntensity value.  If the output voltage is above the new max value, drop the
	//  output voltage to the new max allowed value.

	TensChannel_HandleTypeDef *chan = &dev->tensChan[index];

	//Multiple channels might share the same DAC output.  Update the maxIntensity for all channels that share this DAC.
	for(uint8_t n=TENS_INDEX_START; n<(TENS_INDEX_START + NUM_TENS_CHANNELS); n++){
		if( (dev->tensChan[n].pdac == chan->pdac) && (dev->tensChan[n].dacChan == chan->dacChan) ){
			//We're either referencing ourself here (n == index) or this is another channel using the same dac.
			dev->tensChan[n].intensityMax = newVal;
		}

		//Get the current Intensity value based on the new maxIntensity value:
		dev->tensChan[n].curIntensityPct = TensGetDacVal(&dev->tensChan[n]);
		if(dev->tensChan[n].curIntensityPct > 100){
			TensSetTensIntensity(dev, index, 100);
		}
	}
}


uint8_t TensGetDacVal(TensChannel_HandleTypeDef* chan){
	//Convert a DAC value to 0-100% value

	//Get the current DAC value
	uint16_t curDacVal = (chan->dacChan == DAC_CHANNEL_1 ? (uint16_t)chan->pdac->Instance->DHR12R1 : (uint16_t)chan->pdac->Instance->DHR12R2);

	//Calculate the max dac value based on the user-defined intensityMax setting:
	uint32_t scaledMaxDac = chan->intensityMax * (chan->dacMax - chan->dacMin) / 100 + chan->dacMin;

	//Calculate the 0-100% value by mapping the actual dac value (curDacVal) between dacMin and scaledMaxDac
	uint32_t dacPct = (curDacVal - chan->dacMin) * 100 / (scaledMaxDac - chan->dacMin) + 0;
	if(dacPct > 100){
		dacPct = 100;
	}
	return (uint8_t)dacPct;
}

void TensvBoostEnable(Tens_HandleTypeDef* dev, bool newVal){
	uint8_t tmpVBoostDeActiveState = (dev->_vBoostActiveState == 0) ? 1 : 0;
	uint8_t tmpVEnableDeActiveState = (dev->_vEnableActiveState == 0) ? 1 : 0;

#ifdef DEVICE_BOARD_B
	HAL_GPIO_WritePin(dev->_vBoostPort, dev->_vBoostPin, (newVal == true) ? dev->_vBoostActiveState : tmpDeActiveState);
#endif
#ifdef DEVICE_BOARD_E
	if( (dev->_vBoostCurState != newVal) || (dev->_vBoostCurState == false) ){
		if(newVal == false){
			//Turn off the vBoost.
			HAL_GPIO_WritePin(dev->_vBoostPort, dev->_vBoostPin, tmpVBoostDeActiveState);
			HAL_GPIO_WritePin(dev->_vEnablePort, dev->_vEnablePin, tmpVEnableDeActiveState);
			dev->_vEnableCurState = false;
		}else{
			//turn off the venable while the boost fires up
			HAL_GPIO_WritePin(dev->_vEnablePort, dev->_vEnablePin, tmpVEnableDeActiveState);
			//turn on the vboost
			HAL_GPIO_WritePin(dev->_vBoostPort, dev->_vBoostPin, dev->_vBoostActiveState);
			//Wait for it to stabilize
			HAL_Delay(100);
			//Turn on the vEnable
			HAL_GPIO_WritePin(dev->_vEnablePort, dev->_vEnablePin, dev->_vEnableActiveState);
			dev->_vEnableCurState = true;
		}
	}

#endif

	dev->_vBoostCurState = newVal;
}

void TensInitMotorChannel(TensChannel_HandleTypeDef* chan, GPIO_TypeDef* MotorDirPort, uint16_t MotorDirPin, TIM_HandleTypeDef* MotorTimerHandle,
						__IO uint32_t* MotorTCCR, uint32_t MotorTimChan, uint8_t MotorMinOutput, uint8_t MotorMaxOutput,
						uint16_t ChanMinSpeed, uint16_t ChanMaxSpeed){

	chan->chanType = chanType_Motor;
	chan->pmotorDirPort = MotorDirPort;
	chan->motorDirPin = MotorDirPin;
	//chan->pmotorModePort = MotorModePort;
	//chan->motorModePin = MotorModePin;
	chan->timerHandle = MotorTimerHandle;
	chan->pmotorTCCR = MotorTCCR;
	chan->motorTmrChan = MotorTimChan;
	chan->motorMaxOutput = MotorMaxOutput;
	chan->motorMinOutput = MotorMinOutput;
	chan->chanSpeed = 100;
	chan->chanMinSpeed = ChanMinSpeed;
	chan->chanMaxSpeed = ChanMaxSpeed;

	TensSetTensOrMotorOutput(chan, 0);
	HAL_TIM_Base_Start_IT(MotorTimerHandle);
	HAL_TIM_PWM_Start(MotorTimerHandle, MotorTimChan);
}



/*
void TensTurnOffIdleMotors(Tens_HandleTypeDef* dev){
	for(uint8_t n=0; n< dev->numChans; n++){
		TensChannel_HandleTypeDef* chan = &dev->tensChan[n];
		if(chan->chanType == chanType_Motor){
			if( (chan->motorEnabled == true) && (chan->motorSpeed == 0) ){
				if(HAL_GetTick() - chan->motorStoppedMovingTime > 1000){
					//motor has been idle for > 1 second.
					TensMotorEnable(chan, false);
				}
			}
		}
	}
}
*/

/*
void TensTimerPeriodElapsed(Tens_HandleTypeDef* dev, uint8_t index){
	TensChannel_HandleTypeDef* chan = &dev->tensChan[index];
	if(chan->isActive){
		if(index == 3){
			__NOP();
		}
		if(chan->pctComplete >= 100){
			//This output operation is complete.  Are there any repeats left?
			if(chan->repeatCounter > 0){
				chan->repeatCounter--;
				//reset the start time
				chan->startTime = HAL_GetTick();
				//handle polarity:
				if(!( (chan->polarity == polForward) || (chan->polarity == polReverse) )){
					//Toggle polarity:
					TensChangePolarityOutputs(chan, (chan->lastPolarity == polForward) ? polReverse : polForward);
				}
				//setup and start the output from the beginning:
				chan->curVal = chan->startVal;
				chan->pctComplete = 0;
				TensSetTensOrMotorOutput(chan, chan->startVal); //TensSetPulseWidth(chan, (chan->curVal1000 / 1000) );
			}else{
				//Operation is complete, no repeats left.  Shut off the output and mark it complete.
				TensSetTensOrMotorOutput(chan, 0);
				chan->isActive = false;
				dev->curProgStatus[index].progState = progState_LineComplete;
			}
		}else{
			//Operation not complete.  Toggle polarity if needed and add the stepVal:
			//Need to toggle polarity?
			if( (chan->polarity == polForward_TogglePulse) || (chan->polarity == polReverse_TogglePulse) ){
				TensChangePolarityOutputs(chan, (chan->lastPolarity == polForward) ? polReverse : polForward);
			}

			chan->pctComplete += chan->pctStep;
			if(chan->pctComplete > 100){
				chan->pctComplete = 100;
			}
			chan->curVal = chan->startVal + ((chan->pctComplete / 100) * chan->deltaVal);
			if(chan->curVal > 100){
				chan->curVal = 100;
			}
			TensSetTensOrMotorOutput(chan, (uint8_t)(floor)(chan->curVal));
		}
	}
}
*/
/*
void TensTimerPeriodElapsed(Tens_HandleTypeDef* dev, uint8_t index){
	TensChannel_HandleTypeDef* chan = &dev->tensChan[index];
	//if(chan->isActive){
	if(chan->activeState == Active){
		if(chan->pctComplete >= 100){
			//This output operation is complete.  Are there any repeats left?
			if(chan->repeatCounter > 0){
				chan->repeatCounter--;
				//reset the start time
				chan->startTime = HAL_GetTick();
				//handle polarity:
				if(!( (chan->polarity == polForward) || (chan->polarity == polReverse) )){
					//Toggle polarity:
					TensChangePolarityOutputs(chan, (chan->lastPolarity == polForward) ? polReverse : polForward);
				}
				//setup and start the output from the beginning:
				chan->curVal = chan->startVal;
				chan->pctComplete = 0;
				TensSetTensOrMotorOutput(chan, chan->startVal); //TensSetPulseWidth(chan, (chan->curVal1000 / 1000) );
			}else{
				//Operation is complete, no repeats left.  Shut off the output and mark it complete.
				TensSetTensOrMotorOutput(chan, 0);
				chan->isActive = false;
				dev->curProgStatus[index].progState = progState_LineComplete;
			}
		}else{
			//Operation not complete.  Toggle polarity if needed and add the stepVal:
			//Need to toggle polarity?
			if( (chan->polarity == polForward_TogglePulse) || (chan->polarity == polReverse_TogglePulse) ){
				TensChangePolarityOutputs(chan, (chan->lastPolarity == polForward) ? polReverse : polForward);
			}

			chan->pctComplete += chan->pctCompletePerStep;
			if(chan->pctComplete > 100){
				chan->pctComplete = 100;
			}

			//chan->curVal = chan->startVal + ((chan->pctComplete / 100) * chan->deltaVal);
			structProgLine *curLine = &dev->curProgLine[index];
			if(curLine->command == tensCommand_TenMotOutput){
				enumWaveForm curWave = curLine->gotoTrue;
				if(curWave == wfRamp){
					chan->curVal = chan->startVal + ((chan->pctComplete / 100) * chan->deltaVal);
				}else if(curWave == wfTriangle){
					uint8_t tmpDutyCycle = curLine->gotoFalse;
					//Are we going up or down?
					if(chan->pctComplete <= tmpDutyCycle){
						//Still going up.
						chan->curVal = chan->startVal + ((chan->pctComplete / tmpDutyCycle) * chan->deltaVal);
					}else{
						//Going down
						chan->curVal = chan->endVal - ( ((100 - chan->pctComplete) / (100 - tmpDutyCycle)) * chan->deltaVal);
					}
				}else if(curWave == wfSine){
					enumQuadrant tmpQuad = curLine->gotoFalse;
					float startRad = 0.0f;
					float endRad = 0.0f;
					float curRad = 0.0f;
					float curSin = 0.0f;

					switch(tmpQuad){
					case quadMidHi:
						startRad = 0;
						endRad = M_PI / 2.0;
						//curRad = (endRad - startRad) * (chan->pctComplete / 100);
						//curSin = sin(curRad);
						//chan->curVal = chan->startVal + (curSin * (chan->endVal - chan->startVal));
						break;
					case quadMidHiMid:
						startRad = 0;
						endRad = M_PI;
						break;
					case quadHiMid:
						startRad = M_PI / 2.0;
						endRad = M_PI;
						break;
					case quadMidLow:
						startRad = M_PI;
						endRad = M_PI * 1.5;
						break;
					case quadMidLowMid:
						startRad = M_PI;
						endRad = M_PI * 2.0;
						break;
					case quadLowMid:
						startRad = M_PI * 1.5;
						endRad = M_PI * 2.0;
						break;
					case quadLowHi:
						startRad = M_PI * 1.5;
						endRad = M_PI / 2.0;
						break;
					case quadLowHiLow:
						startRad = M_PI * 1.5;
						endRad = (M_PI * 1.5) + (M_PI * 2.0);
						break;
					case quadHiLowHi:
						startRad = M_PI / 2.0;
						endRad = (M_PI / 2.0) + (M_PI * 2.0);
						break;

					case quadHiLow:
						startRad = M_PI / 2.0;
						endRad = M_PI * 1.5;
						break;
					case quadMidHiLowMid:
						startRad = 0;
						endRad = M_PI * 2;
						break;
					case quadMidLowHiMid:
						startRad = M_PI;
						endRad = M_PI * 3;
						break;
					default:
						startRad = 0;
						endRad = 0;
					}

					curRad = (endRad - startRad) * (chan->pctComplete / 100);
					curSin = sin(curRad);
					chan->curVal = chan->startVal + (curSin * (chan->endVal - chan->startVal));
					chan->curVal = (chan->curVal < 0 ? 0 : (chan->curVal > 100 ? 100 : chan->curVal));

//Todo: Finish this.
				}else{
					//We shouldn't be here, but here we are...
					chan->curVal = 0;
				}
			}

			if(chan->curVal > 100){
				chan->curVal = 100;
			}
			TensSetTensOrMotorOutput(chan, (uint8_t)(floor)(chan->curVal));
		}
	}
}
*/
void TensTimerPeriodElapsed(Tens_HandleTypeDef* dev, uint8_t index){
	TensChannel_HandleTypeDef* chan = &dev->tensChan[index];
	structProgLine *curLine = &dev->curProgLine[index];
	if(chan->activeState == Active){
		if(chan->pctComplete >= 100){
			//This output operation is complete.  Does the program call for a post-Delay?
			if(dev->curProgStatus[index].modDelayDuration > 0){
				//Yep.  The delay parameters were already set up when the program line was initialized (TensStartTensOrPwmOutput).
				//Just need to shut off the output and change the active state.
				TensSetTensOrMotorOutput(chan, 0);
				chan->curVal = 0;
				chan->pctComplete = 0;
				chan->activeState = PostDelay;
				////curProg->lineStartedTime = HAL_GetTick();
				//dev->curProgStatus[index].lineStartedTime = HAL_GetTick();
				dev->curProgStatus[index].remainingOpTime = 0;
				dev->curProgStatus[index].elapsedOpTime = dev->curProgStatus[index].modOutputDuration;

				dev->curProgStatus[index].postDelayStartedTime = HAL_GetTick();
				dev->curProgStatus[index].elapsedPdTime = 0;
				dev->curProgStatus[index].remainingPdTime = dev->curProgStatus[index].modDelayDuration;

			}else{
				//This output operation is complete. The TensRepeatTenMotLine sub will check for any repeats.
				//If there are any repeats to be done, it'll initialize all the data and start the output.
				//If no more repeats to do then it'll mark the line as complete.
				TensRepeatTenMotLine(dev, index);
			}
		}else{
			//Operation not complete.  Toggle polarity if needed and add the stepVal:
			//Need to toggle polarity?
			if( (chan->polarity == polForward_TogglePulse) || (chan->polarity == polReverse_TogglePulse) ){
				TensChangePolarityOutputs(chan, (chan->lastPolarity == polForward) ? polReverse : polForward);
			}

			chan->pctComplete += chan->pctCompletePerStep;
			if(chan->pctComplete > 100){
				chan->pctComplete = 100;
			}

			//chan->curVal = chan->startVal + ((chan->pctComplete / 100) * chan->deltaVal);
			//structProgLine *curLine = &dev->curProgLine[index];
			if(curLine->command == tensCommand_TenMotOutput){
				enumWaveForm curWave = curLine->gotoTrue;
				if(curWave == wfRamp){
					chan->curVal = chan->startVal + ((chan->pctComplete / 100) * chan->deltaVal);
				}else if(curWave == wfTriangle){
					uint8_t tmpDutyCycle = curLine->gotoFalse;
					//Are we going up or down?
					if(chan->pctComplete <= tmpDutyCycle){
						//Still going up.
						chan->curVal = chan->startVal + ((chan->pctComplete / tmpDutyCycle) * chan->deltaVal);
					}else{
						//Going down
						chan->curVal = chan->endVal - ( ((100 - chan->pctComplete) / (100 - tmpDutyCycle)) * chan->deltaVal);
					}
				}else if(curWave == wfSine){
					enumQuadrant tmpQuad = curLine->gotoFalse;
					float startRad = 0.0f;
					float endRad = 0.0f;
					float curRad = 0.0f;
					float curSin = 0.0f;

					switch(tmpQuad){
					case quadMidHi:
						startRad = 0;
						endRad = M_PI / 2.0;
						//curRad = (endRad - startRad) * (chan->pctComplete / 100);
						//curSin = sin(curRad);
						//chan->curVal = chan->startVal + (curSin * (chan->endVal - chan->startVal));
						break;
					case quadMidHiMid:
						startRad = 0;
						endRad = M_PI;
						break;
					case quadHiMid:
						startRad = M_PI / 2.0;
						endRad = M_PI;
						break;
					case quadMidLow:
						startRad = M_PI;
						endRad = M_PI * 1.5;
						break;
					case quadMidLowMid:
						startRad = M_PI;
						endRad = M_PI * 2.0;
						break;
					case quadLowMid:
						startRad = M_PI * 1.5;
						endRad = M_PI * 2.0;
						break;
					case quadLowHi:
						startRad = M_PI * 1.5;
						endRad = M_PI / 2.0;
						break;
					case quadLowHiLow:
						startRad = M_PI * 1.5;
						endRad = (M_PI * 1.5) + (M_PI * 2.0);
						break;
					case quadHiLowHi:
						startRad = M_PI / 2.0;
						endRad = (M_PI / 2.0) + (M_PI * 2.0);
						break;

					case quadHiLow:
						startRad = M_PI / 2.0;
						endRad = M_PI * 1.5;
						break;
					case quadMidHiLowMid:
						startRad = 0;
						endRad = M_PI * 2;
						break;
					case quadMidLowHiMid:
						startRad = M_PI;
						endRad = M_PI * 3;
						break;
					default:
						startRad = 0;
						endRad = 0;
					}

					curRad = (endRad - startRad) * (chan->pctComplete / 100);
					curSin = sin(curRad);
					chan->curVal = chan->startVal + (curSin * (chan->endVal - chan->startVal));
					chan->curVal = (chan->curVal < 0 ? 0 : (chan->curVal > 100 ? 100 : chan->curVal));

//Todo: Finish this.
				}else{
					//We shouldn't be here, but here we are...
					chan->curVal = 0;
				}
			}

			if(chan->curVal > 100){
				chan->curVal = 100;
			}
			TensSetTensOrMotorOutput(chan, (uint8_t)(floor)(chan->curVal));
		}
	}
}

bool TensReadDigitalInput(Tens_HandleTypeDef* dev, uint8_t inNum, bool ignoreBypass){
	bool retVal = false;
	if(inNum < NUM_DINPUTS){
		if( (dev->inputs[inNum].isBypassed == true) && (ignoreBypass == false) ){
			//Input is bypassed and this function is not ignoring the bypass.  return the bypass value.
			retVal = dev->inputs[inNum].curState;
		}else if(dev->inputs[inNum].pPort){
			//input seems to exist and has been initialized.  Read the input.
			GPIO_PinState inState = HAL_GPIO_ReadPin(dev->inputs[inNum].pPort, dev->inputs[inNum].pin);
			if(dev->inputs[inNum].activeState == true){
				// Active High
				dev->inputs[inNum].curState = (inState == GPIO_PIN_SET);
			}else{
				// Active Low
				dev->inputs[inNum].curState = (inState == GPIO_PIN_RESET);
			}
			retVal = dev->inputs[inNum].curState;
		}
	}
	return retVal;
}

void TensSetDigitalOutput(Tens_HandleTypeDef* dev, uint8_t outNum, bool newVal, bool ignoreBypass){
	if(outNum < NUM_DOUTPUTS){
		if(dev->outputs[outNum].pPort){
			//output seems to exist and has been initialized.
			if( (dev->outputs[outNum].isBypassed == false) || (ignoreBypass == true) ){
				//Set the output.
				GPIO_PinState tmpOutVal;
				if(newVal == true){
					// "Activate" the output
					tmpOutVal = (dev->outputs[outNum].activeState == true) ? GPIO_PIN_SET : GPIO_PIN_RESET;
				}else{
					// "Deactivate" the output
					tmpOutVal = (dev->outputs[outNum].activeState == true) ? GPIO_PIN_RESET : GPIO_PIN_SET;
				}
				HAL_GPIO_WritePin(dev->outputs[outNum].pPort, dev->outputs[outNum].pin, tmpOutVal);
				dev->outputs[outNum].curState = newVal;
			}
		}
	}
}

void TensSetOutputsToSafeState(Tens_HandleTypeDef* dev){
	//Set all outputs to ProgramEnd state:
	for(uint8_t x=0; x<NUM_DOUTPUTS; x++){
		switch(dev->outputs[x].progEndState){
		case 0:	//Turn off
			TensSetDigitalOutput(dev, x, false, false);
			break;
		case 1: //Turn on
			TensSetDigitalOutput(dev, x, true, false);
			break;
		default:
			//Do nothing.
		}
	}
}

uint32_t TensGetValue(Tens_HandleTypeDef* dev, enumDataSource dataSource, uint32_t dataVal1, uint32_t dataVal2, uint8_t chanNum, uint32_t maxVal){
	//NOTE:  chanNum is the chanNum of the channel that is running the program line.  It's "me".
	//		 If a program is requesting data of another channel, the other channel's index will be in dataVal2.
	//	(e.g. Chan2 is running a program that requests the speed of Chan4:  chanNum == 2, dataVal2 == 4

	//TensChannel_HandleTypeDef 		*chan = &dev->tensChan[chanNum];
	//structProgLine 					*curLine = &dev->curProgLine[chanNum];
	//TensProgramStatus_HandleTypeDef *curProg = &dev->curProgStatus[chanNum];

	uint32_t retVal = 0;
	switch(dataSource){
	case dataSource_DirectInput:
		retVal = dataVal1;
		break;
	case dataSource_ProgramVariable:
		if(chanNum < NUM_CHANNELS){
			retVal = dev->curProgStatus[chanNum].variable[dataVal1];
		}
		break;
	case dataSource_SystemVariable:
		retVal = dev->curProgStatus[0].variable[dataVal1];
		break;
	case dataSource_ChannelSetting:
		//Any program can be run on any channel.  So if the program needs to specifically reference the channel that it's running
		//on, it will specify the channel index == NUM_CHANNELS.  So if dataVal2 (requested channel) = NUM_CHANNELS then it's looking for this channel.
		uint8_t targetChan = dataVal2;
		if(targetChan == NUM_CHANNELS){
			targetChan = chanNum;
		}
		switch(dataVal1){
		case dscSetting_Speed:
			retVal = dev->tensChan[targetChan].chanSpeed;
			break;
		case dscSetting_OrigOutputDuration:
			retVal = dev->tensChan[targetChan].origOutputDuration;
			break;
		case dscSetting_OrigPostDelayDuration:
			retVal = dev->tensChan[targetChan].origDelayDuration;
			break;
		case dscSetting_OrigTotalDuration:
			retVal = dev->tensChan[targetChan].origOutputDuration + dev->tensChan[targetChan].origDelayDuration;
			break;
		case dscSetting_ModOutputDuration:
			retVal = dev->curProgStatus[targetChan].modOutputDuration;
			break;
		case dscSetting_ModPostDelayDuration:
			retVal = dev->curProgStatus[targetChan].modDelayDuration;
			break;
		case dscSetting_ModTotalDuration:
			retVal = dev->curProgStatus[targetChan].modOutputDuration + dev->curProgStatus[targetChan].modDelayDuration;
			break;
		case dscSetting_ElapsedOutputDuration:
			retVal = dev->curProgStatus[targetChan].elapsedOpTime;
			break;
		case dscSetting_ElapsedTotalDuration:
			retVal = dev->curProgStatus[targetChan].elapsedTotTime;
			break;
		case dscSetting_RemainingOutputDuration:
			retVal = dev->curProgStatus[targetChan].remainingOpTime;
			break;
		case dscSetting_RemainingPostDelayDuration:
			retVal = dev->curProgStatus[targetChan].remainingPdTime;
			break;
		case dscSetting_RemainingTotalDuration:
			retVal = dev->curProgStatus[targetChan].remainingTotTime;
			break;
		case dscSetting_PercentComplete:
			retVal = dev->tensChan[targetChan].pctComplete;
			break;
		case dscSetting_StartVal:
			retVal = dev->tensChan[targetChan].startVal;
			break;
		case dscSetting_EndVal:
			retVal = dev->tensChan[targetChan].endVal;
			break;
		case dscSetting_CurVal:
			retVal = dev->tensChan[targetChan].curVal;
			break;
		case dscSetting_Polarity:
			retVal = dev->tensChan[targetChan].polarity;
			break;
		case dscSetting_Intensity:
			retVal = dev->tensChan[targetChan].curIntensityPct;
			break;
		case dscSetting_IntensityMin:
			retVal = dev->tensChan[targetChan].intensityMin;
			break;
		case dscSetting_IntensityMax:
			retVal = dev->tensChan[targetChan].intensityMax;
			break;
		case dscSetting_CurProgNum:
			retVal = dev->curProgNum[targetChan];
			break;
		case dscSetting_CurLineNum:
			retVal = dev->curProgStatus[targetChan].curLineNum;
			break;
		case dscSetting_CurChanNum:
			retVal = chanNum;
			break;
		default:
			retVal = 0;
		}
		break;
	case dataSource_SystemSetting:
		uint8_t startFreqBand = 0;				//used for audio levels
		uint8_t numBands = NUM_FREQ_BANDS / 3;	//used for audio levels
		uint16_t bandTotalVal = 0;				//used for audio levels
		uint8_t bandMinVal = 0;
		uint8_t bandMaxVal = 0;

		switch(dataVal1){
		case dssSetting_ZRotation:
			retVal = dev->imuRef->globalZRotation;
			break;
		case dssSetting_UpDirection:
			retVal = dev->imuRef->upDirection;
			break;
		case dssSetting_StepCount:
			retVal = dev->imuRef->stepCount;
			break;
		case dssSetting_AudioTotal:
			//retVal = dev->audioValTotal;
			retVal = dev->audioRef->audioValTotal;
			break;
//		case dssSetting_AudioLowAvg:
//			//return the avg of the lower 1/3 of the freq bands
//			startFreqBand = 0;
//			for(uint8_t n=0; n<numBands; n++){
//				bandTotalVal += dev->audioRef->freqBandVal[n+startFreqBand];
//			}
//			retVal = bandTotalVal / numBands;
//			retVal = (retVal <=100 ? retVal : 100);
//			break;
		case dssSetting_AudioLow:
			//return the highest val of the lower 1/3 of the freq bands
			startFreqBand = 0;
			for(uint8_t n=0; n<numBands; n++){
				if(dev->audioRef->freqBandVal[n+startFreqBand] > bandMaxVal){
					bandMaxVal = dev->audioRef->freqBandVal[n+startFreqBand];
				}
			}
			retVal = bandMaxVal;
			break;
//		case dssSetting_AudioMidAvg:
//			startFreqBand = numBands;  //low range is bands 0 to [numBands -1], so mid level starts on [numBands].
//			for(uint8_t n=0; n<numBands; n++){
//				bandTotalVal += dev->audioRef->freqBandVal[n+startFreqBand];
//			}
//			retVal = bandTotalVal / numBands;
//			retVal = (retVal <=100 ? retVal : 100);
//			break;
		case dssSetting_AudioMid:
			startFreqBand = numBands;  //low range is bands 0 to [numBands -1], so mid level starts on [numBands].
			for(uint8_t n=0; n<numBands; n++){
				if(dev->audioRef->freqBandVal[n+startFreqBand] > bandMaxVal){
					bandMaxVal = dev->audioRef->freqBandVal[n+startFreqBand];
				}
			}
			retVal = bandMaxVal;
			break;
//		case dssSetting_AudioHighMax:
//			startFreqBand = (NUM_FREQ_BANDS / 3) * 2;
//			numBands = NUM_FREQ_BANDS - (startFreqBand + 1);	//Give the remaining extra band to the high range.
//			for(uint8_t n=0; n<numBands; n++){
//				bandTotalVal += dev->audioRef->freqBandVal[n+startFreqBand];
//			}
//			retVal = bandTotalVal / numBands;
//			retVal = (retVal <=100 ? retVal : 100);
//			break;
		case dssSetting_AudioHigh:
			startFreqBand = (NUM_FREQ_BANDS / 3) * 2;
			numBands = NUM_FREQ_BANDS - (startFreqBand + 1);	//Give the remaining extra band to the high range.
			for(uint8_t n=0; n<numBands; n++){
				if(dev->audioRef->freqBandVal[n+startFreqBand] > bandMaxVal){
					bandMaxVal = dev->audioRef->freqBandVal[n+startFreqBand];
				}
			}
			retVal = bandMaxVal;
			break;
		}
		break;
	case dataSource_DigitalInput:
		retVal = (TensReadDigitalInput(dev, dataVal1, false) == true) ? 1 : 0;
		break;
	case dataSource_DigitalOutput:
		if(dataVal1 < NUM_DOUTPUTS){
			retVal = (dev->outputs[dataVal1].curState == true) ? 1 : 0;
		}else{
			retVal = 0;
		}
		break;

	case dataSource_RandomNumber:
		retVal = dataVal1;
		//dataVal1 is min, dataVal2 is max.
		if(dataVal2 > dataVal1){
			uint32_t tmpDelta = dataVal2 - dataVal1;
			retVal = randomAtMost(tmpDelta) + dataVal1;
		}
		break;
	case dataSource_Timer:
		if(chanNum < NUM_CHANNELS){
			retVal = HAL_GetTick() - dev->curProgStatus[chanNum].timer[dataVal1];
		}
		break;
	default:
		retVal = 0;
	}

	if( (retVal > maxVal) && (maxVal > 0) ){
		retVal = maxVal;
	}
	return retVal;
}

/*
bool TensTestCommand(Tens_HandleTypeDef* dev, uint8_t chanNum, structProgLine *curLine){
	if(chanNum >= NUM_CHANNELS) return false;
	//TensChannel_HandleTypeDef *chan = &dev->tensChan[chanNum];
	uint32_t testValLeft = TensGetValue(dev, curLine->pi81S, curLine->pi81V1, curLine->pi81V2, chanNum, 0);
	uint32_t testValRight = TensGetValue(dev, curLine->pi321S, curLine->pi321V1, curLine->pi321V2, chanNum, 0);
	//uint32_t testValRight2 = TensGetValue(dev, curLine->pi322S, curLine->pi322V1, curLine->pi322V2, chanNum, 0);
	uint32_t testValRightModifier = TensGetValue(dev, curLine->pi322S, curLine->pi322V1, curLine->pi322V2, chanNum, 0);

	switch(curLine->pi82V1){
	case tensCompare_LessThan:
		return (testValLeft < testValRight);
		break;
	case tensCompare_LessThanOrEqual:
		return (testValLeft <= testValRight);
		break;
	case tensCompare_Equal:
		return (testValLeft == testValRight);
		break;
	case tensCompare_GreaterThanOrEqual:
		return (testValLeft >= testValRight);
		break;
	case tensCompare_GreaterThan:
		return (testValLeft > testValRight);
		break;
	case tensCompare_IsBetween:
		return ( (testValLeft > testValRight) && (testValLeft < testValRight2) );
		break;
	case tensCompare_IsBetweenOrEqual:
		return ( (testValLeft >= testValRight) && (testValLeft <= testValRight2) );
		break;
	case tensCompare_IsNotBetween:
		return ( (testValLeft < testValRight) || (testValLeft > testValRight2) );
		break;
	default:
		return false;
	}
	return false;
}
*/
bool TensTestCommand(Tens_HandleTypeDef* dev, uint8_t chanNum, structProgLine *curLine){
	if(chanNum >= NUM_CHANNELS) return false;
	uint32_t testValLeft = TensGetValue(dev, curLine->pi81S, curLine->pi81V1, curLine->pi81V2, chanNum, 0);
	uint32_t testValRight = TensGetValue(dev, curLine->pi321S, curLine->pi321V1, curLine->pi321V2, chanNum, 0);
	uint32_t testValRight2 = TensGetValue(dev, curLine->pi322S, curLine->pi322V1, curLine->pi322V2, chanNum, 0);
	//uint32_t testValRight2 = TensGetValue(dev, curLine->pi322S, curLine->pi322V1, curLine->pi322V2, chanNum, 0);

	//uint32_t testMaxVal = MAX_VAL_32BIT;

	//Figure out if the right value gets modified and if so then how:
	switch(curLine->pi82V2){  //ModOperator is pi82V2
	case math_None:
		//testValRight = testValRight
		break;
	case math_Add:
		if( (testValRight < MAX_VAL_32BIT) && (testValRight2 < MAX_VAL_32BIT)){
			testValRight += testValRight2;
		}else{
			testValRight = MAX_VAL_32BIT;
		}
		break;
	case math_Subtract:
		if(testValRight >= testValRight2){
			testValRight -= testValRight2;
		}else{
			testValRight = 0;
		}
		break;
	case math_Multiply:
		testValRight = testValRight * testValRight2;
		break;
	case math_Divide:
		if( (testValRight >= testValRight2) && (testValRight2 > 0) ){
			testValRight = testValRight / testValRight2;
		}else{
			testValRight = 0;
		}
		break;
	case math_Remainder:
		if(testValRight2 > 0){
			testValRight = testValRight % testValRight2;
		}else{
			testValRight = 0;
		}
		break;
	default:
		//testValRight = testValRight;
		break;
	}


	switch(curLine->pi82V1){
	case tensCompare_LessThan:
		return (testValLeft < testValRight);
		break;
	case tensCompare_LessThanOrEqual:
		return (testValLeft <= testValRight);
		break;
	case tensCompare_Equal:
		return (testValLeft == testValRight);
		break;
	case tensCompare_NotEqual:
		return (testValLeft != testValRight);
		break;
	case tensCompare_GreaterThanOrEqual:
		return (testValLeft >= testValRight);
		break;
	case tensCompare_GreaterThan:
		return (testValLeft > testValRight);
		break;
	case tensCompare_IsBetween:
		return ( (testValLeft > testValRight) && (testValLeft < testValRight2) );
		break;
	case tensCompare_IsBetweenOrEqual:
		return ( (testValLeft >= testValRight) && (testValLeft <= testValRight2) );
		break;
	case tensCompare_IsNotBetween:
		return ( (testValLeft < testValRight) || (testValLeft > testValRight2) );
		break;
	default:
		return false;
	}
	return false;
}

uint32_t TensMinMax(uint32_t val, uint32_t min, uint32_t max){
	return (val < min ? min : (val > max ? max : val));
}

void TensSetCommand(Tens_HandleTypeDef* dev, uint8_t chanNum, structProgLine *curLine){
	if(chanNum >= NUM_CHANNELS) return;
	//Determine the math result (answer), then determine where to put it (variable, dig output value, etc.)
	uint32_t maxResult = 2000000000; //4294967295;

	uint32_t setResult = TensGetValue(dev, curLine->pi321S, curLine->pi321V1, curLine->pi321V2, chanNum, 0);
	if((uint8_t)curLine->pi82V2 > (uint8_t)math_None){
		uint32_t setModifierVal = TensGetValue(dev, curLine->pi322S, curLine->pi322V1, curLine->pi322V2, chanNum, 0);
		//NOTE: The math operations will not overflow; they will be limited at the extents.
		switch(curLine->pi82V2){
		case math_Add:
			if(maxResult - setResult > setModifierVal){
				setResult += setModifierVal;
			}else{
				setResult = maxResult;
			}
			break;
		case math_Subtract:
			if(setResult > setModifierVal){
				setResult -= setModifierVal;
			}else{
				setResult = 0;
			}
			break;
		case math_Multiply:
			if( (setResult == 0) || (setModifierVal == 0) ){
				setResult = 0;
			}else if(maxResult / setResult > setModifierVal){
				setResult = setResult * setModifierVal;
			}else{
				setResult = maxResult;
			}
			break;
		case math_Divide:
			if( (setResult >= setModifierVal) && (setModifierVal > 0) ){
				setResult = setResult / setModifierVal;
			}else{
				setResult = 0;
			}
			break;
		case math_Remainder:
			if(setModifierVal > 0){
				setResult += setResult % setModifierVal;
			}else{
				setResult = 0;
			}
			break;
		default:
			return;
		}
	}

	//Figure out what to do with the result we just calculated.
	uint8_t tmpIndex = curLine->pi81V1;

	switch(curLine->pi81S){
	case dataSource_DirectInput:
		//Nothing to do here.  This would be a dumb choice since its a useless statement.
		break;
	case dataSource_ProgramVariable:
		if(curLine->pi81V1 < NUM_VARIABLES){
			dev->curProgStatus[chanNum].variable[tmpIndex] = setResult;
		}
		break;
	case dataSource_SystemVariable:
		dev->curProgStatus[0].variable[tmpIndex] = setResult;
		break;

	case dataSource_ChannelSetting:
		//Any program can be run on any channel.  So if the program needs to specifically reference the channel that it's running
		//on, it will specify the channel index == NUM_CHANNELS.  So if dataVal2 (requested channel) = NUM_CHANNELS then it's looking for this channel.
		uint8_t targetChan = curLine->pi81V2;
		if(targetChan > NUM_CHANNELS){
			return;
		}else if(targetChan == NUM_CHANNELS){
			targetChan = chanNum;
		}


		switch(curLine->pi81V1){
		case dscSetting_Speed:
			TensSetSpeed(dev, targetChan, TensMinMax(setResult, dev->tensChan[targetChan].chanMinSpeed, dev->tensChan[targetChan].chanMaxSpeed));
			break;
		case dscSetting_OrigOutputDuration:
		case dscSetting_OrigPostDelayDuration:
		case dscSetting_OrigTotalDuration:
		case dscSetting_ModOutputDuration:
		case dscSetting_ModPostDelayDuration:
		case dscSetting_ModTotalDuration:
		case dscSetting_ElapsedOutputDuration:
		case dscSetting_ElapsedPostDelayDuration:
		case dscSetting_ElapsedTotalDuration:
		case dscSetting_RemainingOutputDuration:
		case dscSetting_RemainingPostDelayDuration:
		case dscSetting_RemainingTotalDuration:
		case dscSetting_StartVal:
		case dscSetting_EndVal:
		case dscSetting_CurVal:
		case dscSetting_Polarity:
			//This will change the polarity override of the channel.
			switch(setResult){
			case 0: //Normal polarity
				dev->tensChan[targetChan].polaritySwapped = false;
				break;
			case 1: //Reverse Polarity
				dev->tensChan[targetChan].polaritySwapped = true;
				break;
			default:  //Toggle Polarity
				dev->tensChan[targetChan].polaritySwapped = !dev->tensChan[targetChan].polaritySwapped;
				break;
			}
			TensSetPolarity(&dev->tensChan[targetChan], dev->tensChan[targetChan].polarity);
			break;
		case dscSetting_Intensity:
			if(dev->tensChan[targetChan].chanType == chanType_Tens){
				TensSetTensIntensity(dev, targetChan, (uint16_t)TensMinMax(setResult, 0, 100));
			} //else{ //motor:
			break;
		case dscSetting_IntensityMin:
			if(dev->tensChan[targetChan].chanType == chanType_Tens){
				TensSetTensMinIntensity(dev, targetChan, (uint16_t)TensMinMax(setResult, 0, 50));
			} //else{ //motor:
			break;
		case dscSetting_IntensityMax:
			if(dev->tensChan[targetChan].chanType == chanType_Tens){
				TensSetTensMaxIntensity(dev, targetChan, (uint16_t)TensMinMax(setResult, 0, 100));
			} //else{ //motor:
			break;
		case dscSetting_CurProgNum:
		case dscSetting_CurLineNum:
		case dscSetting_CurChanNum:
		default:
			break;
		}

		break;
	case dataSource_SystemSetting:
		switch(curLine->pi81V1){
		case dssSetting_ZRotation:
			ImuSetGlobalZRotation(dev->imuRef, (float)setResult);
			break;
		case dssSetting_UpDirection:
			//Can't change this.
			break;
		case dssSetting_StepCount:
			ImuResetStepCounter(dev->imuRef);
			break;
		default:
			break;
		}
		break;
	case dataSource_DigitalInput:
		//Nothing to do here.  We can't "set" an input to some arbitrary value.
		break;
	case dataSource_DigitalOutput:
		TensSetDigitalOutput(dev, tmpIndex, (setResult != 0), false);
		break;
	case dataSource_RandomNumber:
		//Nothing to do here.  The user has asked us to Set a random number to some value.  Invalid option in this case.
		break;
	case dataSource_Timer:
		//Regardless what value was passed in for the timer, we will set it to the current time, essentially "resetting" the timer.
		if(curLine->pi81V1 < NUM_VARIABLES){
			dev->curProgStatus[chanNum].timer[tmpIndex] = HAL_GetTick();
		}
	default:
		break;
	}


}

uint8_t TensGetChannelFromProgLine(Tens_HandleTypeDef* dev, uint8_t RequestingChanNum, structProgLine *curLine){
	uint8_t actualChanNum = curLine->channel;
	if(actualChanNum >= NUM_CHANNELS){
		actualChanNum = RequestingChanNum;
	}
	return actualChanNum;
}

uint8_t TensProgramControlCommand(Tens_HandleTypeDef* dev, uint8_t chanNum, structProgLine *curLine){
	//use this to load a program, or to start/stop/pause a program.
	//This can load/start/stop/pause a program on ANY channel, not just the channel that is running
	//this program command.
	//RETURN the channel number that the program control was performed on.  That way the calling code
	//can easily determine if the program control was performed on the calling channel or on a different
	//channel.

	//uint8_t progCtlCmd = TensGetValue(dev, curLine->pi8_1_Source, curLine->pi8_1_Val, chanNum, 255);
	uint8_t progCtlCmd = curLine->pi81V1;
	uint32_t tmpVal = TensGetValue(dev, curLine->pi321S, curLine->pi321V1, curLine->pi321V2, chanNum, 32767);
	//uint8_t targetChanNum = curLine->channel;
	uint8_t targetChanNum = TensGetChannelFromProgLine(dev, chanNum, curLine);

	switch(progCtlCmd){
	case progControl_LoadProgramAndPause:
		TensStop(dev, targetChanNum, progState_Stopped);
		TensLoadFile(dev, targetChanNum, tmpVal);
		break;
	case progControl_LoadProgramAndRun:
		TensStop(dev, targetChanNum, progState_Stopped);
		TensLoadFile(dev, targetChanNum, tmpVal);
		TensStart(dev, targetChanNum);
		break;
	case progControl_Start:
		TensStart(dev, targetChanNum);
		break;
	case progControl_Stop:
		TensStop(dev, targetChanNum, progState_Stopped);
		break;
	case progControl_Pause:
		TensStop(dev, targetChanNum, progState_Paused);
		break;
	default:
		break;
	}

	return targetChanNum;
}

void TensDisplayRequestCommand(Tens_HandleTypeDef* dev, uint8_t chanNum, structProgLine *curLine){




//	dev->newDisplayRequest = false;
//		memset(dev->displayReqChars, 0, sizeof(dev->displayReqChars));
//		memset(dev->displayReqCharsLast, 0, sizeof(dev->displayReqCharsLast));
//		dev->displayReqLen = 0;
}



