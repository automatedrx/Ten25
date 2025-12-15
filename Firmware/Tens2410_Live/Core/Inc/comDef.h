

#ifndef __COMDEF_H__
#define __COMDEF_H__

#define MAX_VAL_32BIT	2000000000


//ParamArray1 definitions:
#define PA1_JS1X	0
#define PA1_JS1Y	1
#define PA1_JS2X	2
#define PA1_JS2Y	3
#define PA1_POT1	4
#define PA1_POT2	5
#define PA1_PB2		6
#define PA1_PB3		7
#define PA1_PB4		8
#define PA1_PB5		9
#define PA1_PB6		10
#define PA1_PB7		11
#define PA1_PB8		12
#define PA1_PB9		13
#define PA1_PB11	14
#define PA1_PB12	15
#define PA1_PB13	16
#define PA1_PB14	17
#define PA1_ENC1	18
#define PA1_ENC2	19
#define PA1_ENC4	20
#define PA1_COUNT	21

#define DATAFIELD_LENGTH 20


 typedef enum{
     dtTens2410B1 = 0,
     dtTens2410E1 = 1,
     dtTens2503F1 = 2
 } DeviceType_t;

typedef enum {
	dfCommand 	= 0,
	dfChannel 	= 1,
	dfGotoTrue 	= 2,
	dfGotoFalse = 3,
	df81S 		= 4,
	df81V1 		= 5,
	df81V2 		= 6,
	df82S 		= 7,
	df82V1 		= 8,
	df82V2 		= 9,
	dfPolarity 	= 10,
	df321S 		= 11,
	df321V1 	= 12,
	df321V2 	= 13,
	df322S 		= 14,
	df322V1 	= 15,
	df322V2 	= 16,
	df323S 		= 17,
	df323V1 	= 18,
	df323V2 	= 19,
	//dfRepeats 	= 20
} dataFieldEnum;
#define DFCOMMAND	0
#define DFCHANNEL	1
#define DFGTT		2
#define DFGTF		3
#define DF81S		4
#define DF81V1		5
#define DF81V2		6
#define DF82S		7
#define DF82V1		8
#define DF82V2		9
#define DFPOLARITY	10
#define DF321S		11
#define DF321V1		12
#define DF321V2		13
#define DF322S		14
#define DF322V1		15
#define DF322V2		16
#define DF323S		17
#define DF323V1		18
#define DF323V2		19
//#define DFREPEATS	20

//Message Type:
	//If the commandType uses a capital letter, then the destination should return an ACK to the source.
	//If the commandType uses a lowercase letter then no ACK will be returned.
typedef enum {
	commandType_ACK				= 'A',
	commandType_NAK				= 'n',
	commandType_SetParamArray 	= 'b',	//Followed by an index number
	commandType_SetSingleParam 	= 'd',	//Followed by a pStatEnum
	commandType_GetParamArray	= 'e',	//Followed by an index number
	commandType_GetSingleParam 	= 'g',	//Followed by a pStatEnum
	commandType_SetFileData		= 'k',
	commandType_GetFileData		= 'p',
	commandType_Reset			= 'R',
	commandType_DeleteTensProg	= 'X'

} commandTypeEnum;

typedef enum {
	pStat_BattLevel				= '1',
	pStat_Charging				= '2',
	pStat_Charged				= '3',

	pStat_NumMotorChannels 		= 'a',
	pStat_NumTensChannels		= 'b',
	pStat_NumChannels			= 'c',
	pStat_NumInputs				= 'd',
	pStat_NumOutputs			= 'e',
	pStat_MotIndexStart			= 'f',
	pStat_TensIndexStart		= 'g',

	pStat_NumProgVariables		= 'h',
	pStat_NumProgTimers			= 'i',

	pStat_TensProgCurVer		= 'j',
	pStat_TensProgMinVer		= 'k',

	pStat_ChanType				= 'l',
	pStat_ChanEnabled			= 'm',
	pStat_ProgNumber			= 'n',
	pStat_ProgState				= 'o',
	pStat_CurLineNum			= 'p',
	pStat_ChanCurPWidthPct		= 'q',
	pStat_ChanCurIntensityPct	= 'r',
	pStat_CurSpeed				= 's',
	pStat_MinIntensity			= 't',
	pStat_MaxIntensity			= 'u',

	pStat_SwapPolarity			= 'v',



	pStat_NumPrograms			= 'A',	//'Includes chanProgs and sysProgs.
	pStat_ProgramLength			= 'B',	//'Index Required for program number. Indicates the number of lines in a program
	pStat_ProgramName			= 'C'	// Index required for program name.
} pStatEnum;

typedef enum {
 	pArray_Unknown 		= '0',
	pArray_DeviceInfo	= '1',	//	pArray_1			= '1',
	pArray_ChanMinMax	= '2',	//	pArray_2			= '2',
	pArray_ChanStats	= '3',	//	pArray_3			= '3',
//	pArray_SysStats		= '4',	//	pArray_4			= '4',
	pArray_ProgLineData	= '5',	//	This contains info about a single line of program data.

	pArray_7			= '7',
	pArray_8			= '8',
	pArray_9			= '9',
} pArrayEnum;

//Communication channel types:
typedef enum {
	comChan_Radio = 0,
	comChan_Usb,
	comChan_EspWifi,
	comChan_EspBt
} comChanEnum;

//Tens channel Types:
typedef enum {
	chanType_Unknown 		= 0,
	chanType_SystemMaster 	= 1,
	chanType_Motor 			= 2,
	chanType_Tens 			= 3,
	chanType_Aux			= 4
} devChanType;

typedef enum {				//				channel		gtTrue	gtFalse		pi8_1		pi8_2		polarity	pi32_1		pi32_2
	tensCommand_NoOp = 0,
	tensCommand_TenMotOutput,
	//tensCommand_PWMOutput,	//motor/vibe
	tensCommand_GoTo,
	tensCommand_End,
	tensCommand_Test,//tensCommand_Input,
	tensCommand_Set,//tensCommand_Output,
	tensCommand_Delay,
	tensCommand_ProgramControl,
	tensCommand_Display,
	tensCommand_LastItem
} enumTensCommand;

typedef enum {
	 wfNone 		= 0,
	 wfRamp 		= 1,
	 wfTriangle 	= 2,
	 wfSine 		= 3
}enumWaveForm;

typedef enum {
	progControl_Unknown				= 0,
	progControl_LoadProgramAndPause = 1,
	progControl_LoadProgramAndRun 	= 2,
	progControl_Start				= 3,
	progControl_Stop				= 4,
	progControl_Pause				= 5
} progControlEnum;

/*
typedef enum {
	dataSource_DirectInput = 0,
	dataSource_ProgramVariable,
	dataSource_SystemVariable,
	dataSource_DigitalInput,
	dataSource_DigitalOutput,
	dataSource_MathOperation
}enumDataSource;
*/
typedef enum {
	dataSource_DirectInput = 0,
	dataSource_ProgramVariable,
	dataSource_SystemVariable,
	dataSource_ChannelSetting,
	dataSource_SystemSetting,
	dataSource_DigitalInput,
	dataSource_DigitalOutput,
	dataSource_RandomNumber,
	dataSource_Timer,
	dataSource_LastItem
}enumDataSource;



typedef enum {
	 quadMidHi 		= 0,
	 quadMidHiMid 	= 1,
	 quadHiMid	 	= 2,
	 quadMidLow 	= 3,
	 quadMidLowMid 	= 4,
	 quadLowMid 	= 5,
	 quadLowHi 		= 6,
	 quadLowHiLow 	= 7,
	 quadHiLowHi 	= 8,

	 quadHiLow		= 9,
	 quadMidHiLowMid = 10,
	 quadMidLowHiMid = 11
}enumQuadrant;

typedef enum {
	dscSetting_Speed = 0,
	dscSetting_OrigOutputDuration,
	dscSetting_OrigPostDelayDuration,
	dscSetting_OrigTotalDuration,
	dscSetting_ModOutputDuration,
	dscSetting_ModPostDelayDuration,
	dscSetting_ModTotalDuration,
	dscSetting_ElapsedOutputDuration,
	dscSetting_ElapsedPostDelayDuration,
	dscSetting_ElapsedTotalDuration,
	dscSetting_RemainingOutputDuration,
	dscSetting_RemainingPostDelayDuration,
	dscSetting_RemainingTotalDuration,
	dscSetting_PercentComplete,
	dscSetting_StartVal,
	dscSetting_EndVal,
	dscSetting_CurVal,
	dscSetting_Polarity,
	dscSetting_Intensity,
	dscSetting_IntensityMin,
	dscSetting_IntensityMax,
	dscSetting_CurProgNum,
	dscSetting_CurLineNum,
	dscSetting_CurChanNum
}enumDataSourceChanSetting;

typedef enum {
	dssSetting_ZRotation = 0,
	dssSetting_UpDirection = 1,
	dssSetting_StepCount = 2,
	dssSetting_AudioTotal = 3,
//	dssSetting_AudioLeft = 3,
//	dssSetting_AudioRight = 4,
//	dssSetting_AudioLRAvg = 5,
//	dssSetting_AudioLRMin = 6,
//	dssSetting_AudioLRMax = 7,
	dssSetting_AudioLow = 4,
	dssSetting_AudioMid = 5,
	dssSetting_AudioHigh = 6
}enumDataSourceSysSetting;

typedef enum {				//				channel		gtTrue	gtFalse		pi8_1		pi8_2		polarity	pi32_1		pi32_2
	tensCompare_LessThan = 0,
	tensCompare_LessThanOrEqual,
	tensCompare_Equal,
	tensCompare_NotEqual,
	tensCompare_GreaterThanOrEqual,
	tensCompare_GreaterThan,
	tensCompare_IsBetween,
	tensCompare_IsBetweenOrEqual,
	tensCompare_IsNotBetween
} enumTensCompare;

typedef enum {
	math_None 		= 0,
	math_Add 		= 1,
	math_Subtract 	= 2,
	math_Multiply 	= 3,
	math_Divide 	= 4,
	math_Remainder 	= 5
} enumMathOperation;



#endif
