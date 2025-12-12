
// Each channel will have a channelProgram that can be around 50 lines long.
// The system will have 2 system programs that can be up to 200 lines long.

#ifndef _TENS_H_
#define _TENS_H_

#include "main.h"
#include <stdbool.h>
#include <string.h>
#include <math.h>
#include <stdlib.h>
#include "comDef.h"
#include "imu.h"

#define		NUM_MOTOR_CHANNELS	2
#ifdef DEVICE_BOARD_B
#define		NUM_TENS_CHANNELS	2
#endif
#ifdef DEVICE_BOARD_E
#define		NUM_TENS_CHANNELS	3
#endif
//#define		NUM_TENS_CHANNELS	2
#define		NUM_AUX_CHANNELS	2
#define		NUM_CHANNELS		(1 + NUM_MOTOR_CHANNELS + NUM_TENS_CHANNELS + NUM_AUX_CHANNELS)
#define		NUM_DINPUTS			1
#define		NUM_DOUTPUTS		1

#define 	NUM_VARIABLES 		40
#define 	NUM_TIMERS 			20

#define		MOT_INDEX_START		1							//Provides a starting number for motor channels.  e.g. chan[MOT_INDEX + n] will give you the nth motor
#define		TENS_INDEX_START	(MOT_INDEX_START + NUM_MOTOR_CHANNELS)	//Provides a starting number for tens channels.  e.g. chan[TENS_INDEX + n] will give you the nth tens channel
#define		AUX_INDEX_START		(TENS_INDEX_START + NUM_TENS_CHANNELS)

//#define 	CHAN_PROGLINES_MAX	30 //50 //#define PROGRAM_LINES_MAX	400
//#define 	SYS_PROGLINES_MAX	60 //100
//#define		NUM_SYS_PROGS		1//2

#define 	TENS_FILENAME_LEN				10
#define 	TENS_FILEHEADER_LEN				(TENS_FILENAME_LEN + sizeof(uint16_t))
#define 	TENS_PROGRAM_STORAGE_NUMPAGES	40	//24 pages of flash x 2048 bytes = 49,152 bytes
#define 	TENS_PROGRAMS_MAX				200
#define		TENS_MAX_PROGRAM_LINES			200
#define		TENS_MAX_PROGRAM_BYTES			(TENS_FILEHEADER_LEN + (TENS_MAX_PROGRAM_LINES * sizeof(structProgLine)) + 8)
#define 	TENS_PROGRAM_CUR_VER			2503	//File version




typedef enum {
	polForward = 0,
	polReverse,
	polForward_TogglePulse,		//Toggle polarity each pulse output.  Start with forward polarity
	polReverse_TogglePulse,		//Toggle polarity each pulse output.  Start with reverse polarity
	polForward_ToggleCycle,		//Toggle polarity each full cycle of a program line.  (toggle between repeats of the line). Start with forward
	polReverse_ToggleCycle,		//Toggle polarity each full cycle of a program line.  (toggle between repeats of the line)	Start with reverse
	polLastItem
} enumPolarity;


typedef enum {
	progState_Unknown = 0,
	progState_Empty,
	progState_Stopped,
	progState_Paused,
	progState_Running,
	progState_LineComplete,
	progState_End
} enumProgState;



/*Program Commands
	NoOp	= no operation (empty placeholder)

	TenMotOutput = perform a tens output or a motor pwm output.  When complete, move on to the next line number of the program automatically.

			All Waveforms:
				Command:	enumTensCommand.tensCommand_TenMotOutput
				Channel:	not used.
				GotoTrue:	Waveform.: Ramp, Triangle, Sine
				polarity:	All available (fwd, rev, toggle cycle, toggle pulse).
				pi321:		Duration in ms
				pi322:		Number of times to repeat the line.  (A value of 0 means: Run the line 1 time and repeat it 0 times.)
				pi323:		Post-Delay.  Length of delay (in ms) at the end of the line.

			Ramp:
				pi81:		Starting pwm percentage
				pi82:		Ending pwm percentage

			Triangle:
				pi81:		Start/End pwm percentage
				pi82:		Peak pwm percentage

			Sine:
			  quadMidHi, quadHiMid, quadMidLow, quadLowMid, quadLowHi:
				GotoFalse:	Quadrant (only used for Sine waveform).
				pi81:		Starting pwm percentage
				pi82:		Ending pwm percentage

	 	 	  quadMidHiMid, quadMidLowMid, quadLowHiLow, quadHiLowHi:
				pi81:		Start/End pwm percentage
				pi82:		Peak pwm percentage

	GoTo = Jump to a program line
				gotoTrue:	Program line to jump to.

	End = End of program.

	Test:  If (ValLeft) (TestType <, =, >,...) ( ValRight (ModOperator +, -, *, /, ...) Modifier ) Then...
			Command:		enumTensCommand.tensCommand_Test
			Channel:		not used.
			GotoTrue:		Goto this line if the test is true.
			GotoFalse:		Goto this line if the test is false.
			pi81: (S,V1,V2)	ValLeft.  This is the value on the left side of the test.
			pi82S:			not used
			pi82V1:			TestType.
			pi82V2:			ModOperator
			polarity:		not used
			pi321:			ValRight
			pi322:			Modifier

	Set:  Set (Target) = Source (ModOperator +, -, *, /, Mod, Random) Modifier
			Command:		enumTensCommand.tensCommand_Set
			Channel:		not used
			GotoTrue:		not used
			GotoFalse:		not used
			pi81:			Target
			pi82S,V1:		not used
			pi82V2:			ModOperator
			polarity:		not used
			pi321:			Source
			pi322:			Modifier

	Delay:	delay in ms
			Command:		enumTensCommand.tensCommand_Delay
			pi323:			Delay (in ms)

	ProgControl:
			Command:		enumTensCommand.tensCommand_ProgramControl
			Channel:		target channel
			pi81V1:			progControl Command
			pi81S,V2:		not used.
			pi82:			not used.
			polarity:		not used.
			pi321:			Program Number (0-based)

	Display:




*/


typedef struct {
	enumTensCommand	command;
	uint8_t			channel;	// [PWM: 0-3], [tens: 0], [outputs: 0-?], [inputs: 0-?]
	uint16_t		gotoTrue;
	uint16_t		gotoFalse;
	enumDataSource	pi81S;
	int8_t			pi81V1;		// [io state: 0=deactive, 1=active]	[tens/pwm: 	start pulsewidth 0-100]
	int8_t			pi81V2;
	enumDataSource	pi82S;
	int8_t			pi82V1;		//									[tens/pwm: 	end   pulsewidth 0-100]
	int8_t			pi82V2;
	enumPolarity	polarity;	//									[tens/pwm: polarity enum]					--doubles as enumMathOperation
	enumDataSource	pi321S;
	int32_t			pi321V1;		//delay(ms)							[tens/pwm: duration]
	int32_t			pi321V2;
	enumDataSource	pi322S;
	int32_t			pi322V1;		// [tens/PWM: 0-65536]				[tens/pwm: repeatCount for this line]
	int32_t			pi322V2;
	enumDataSource 	pi323S;
	int32_t			pi323V1;
	int32_t			pi323V2;
} structProgLine;


//#define MAX_VARIABLES 50
typedef struct {
	enumProgState		progState;
	uint16_t			curLineNum;
	uint16_t			nextLineNum;		//This is set while the line is being run to specify which line number should be ran next.  (e.g. "Goto True/False" or Jump commands will set the next line number in this var.)
	uint32_t			lineStartedTime;	//Used for delays
	uint32_t			modOutputDuration;		//Used for output channels and for delays.
	uint32_t			modDelayDuration;	//Used for actual delay period tracking
	uint32_t			variable[NUM_VARIABLES];
	uint32_t			timer[NUM_TIMERS];


	//uint32_t			outputStartedTime;	//used for tracking time since the output started
	uint32_t			postDelayStartedTime;	//used for tracking the elapsed time since the postDelay started

	uint32_t			elapsedOpTime;
	uint32_t			remainingOpTime;
	uint32_t			elapsedPdTime;
	uint32_t			remainingPdTime;
	uint32_t			elapsedTotTime;
	uint32_t			remainingTotTime;



} TensProgramStatus_HandleTypeDef;

typedef enum{
	notActive = 0,
	Active,
	PostDelay
}enumActiveState;


typedef struct {
	devChanType		chanType;

// chanEnabled, isBypassed, isActive:
//  chanEnabled: is a master OFF switch for the channel.  If a channel is not enabled then it will not be used.
//  isBypassed: determines whether a running program will be able to use the channel (isBypassed == false), or if the channel will be controlled externally to the running program.
//	isActive: indicates the channel is actively running an output command.
	bool				chanEnabled;
	bool				isBypassed;				// Used to indicate that manual (bypass) control is in use and running programs should not change the state.
//	bool				isActive;				// Indicates whether or not the channel is currently outputting (active).  Used to determine if action should be taken when the channel's timer overflow IRQ fires.
	enumActiveState		activeState;			// Indicates whether the channel is currently outputting (Active), performing post-delay wait period (PostDelay), or not active (notActive).  Used to determine what action should be taken when the channel's timer overflow IRQ fires, and also used for monitoring the end of the postDelay period.

	uint16_t 			chanMinSpeed;// = 20; 	//10
	uint16_t 			chanMaxSpeed;
	uint16_t			chanSpeed;				//10-1000, represents a percentage (10-99 = slow, 100 = normal, 101-1000=fast)

	uint32_t			startTime;				// Timestamp of when the output started.  Note: this will reset to current time whenever the speed value changes.
	uint32_t			origOutputDuration;		// This is the original duration specified in the program for the output period, based on 100% normal speed.
	uint32_t			origDelayDuration;		// This is the original duration specified in the program for a delay (or postDelay), based on 100% normal speed.
	uint16_t			repeatCounter;			// Number of times to repeat an output line.

	uint8_t				startVal;
	uint8_t				endVal;
	float				deltaVal;		//delta val can be negative if the startVal is higher than the endVal.  Could make it int32, but going with float to avoid conversions during ISR math.
	float				curVal;
	float				pctComplete;
	float				pctCompletePerStep;

	//== TENS Section ==
	GPIO_TypeDef*		polarityPort;
	uint16_t			polarityPin;
	__IO uint32_t* 		pulseOutTCCR;
	uint32_t			pulseOutTmrChan;
	DAC_HandleTypeDef*	pdac;
	uint32_t			dacChan;		//DAC_CHANNEL_1 or DAC_CHANNEL_2
	bool				polaritySwapped;	//used to "swap" the polarity of the channel in software rather than having to move physical wires/electrodes.
	enumPolarity		polarity;
	enumPolarity		lastPolarity;

	uint16_t			dacMin;		//Minimum value of dac for the physical hardware to start responding
	uint16_t			dacMax;		//Maximum value of dac the physical hardware can withstand
	uint8_t				curIntensityPct;	//Current intensity setting of the channel.  This is a 0-100% range WITHIN THE INTENSITYMIN/INTENSITYMAX BELOW.
	uint8_t				intensityMin;	//intensityMin and intensityMax allow for a smaller range of output intensities if the user is using
										//smaller electrodes.  The user's program should set the intensityMin/Max for each channel at the start
										//of the user program.  Example:  Device is capable of producing 100 volts max.  If the user sets the
										//intensityMin to 0 and intensityMax to 80, then when the user adjusts the output intensity to 50% the
										//device will output 40 volts.
	uint8_t				intensityMax;

	uint16_t			pulseWidthMax;

	//== MOTOR Section ==
	uint8_t				motorMaxOutput;
	uint8_t				motorMinOutput;
	TIM_HandleTypeDef*	timerHandle;				//Used for tens channel AND motor channel
	__IO uint32_t* 		pmotorTCCR;
	uint32_t			motorTmrChan;				//TIM_CHAN_0 - 4
	uint8_t				motorSpeed;

	GPIO_TypeDef* 		pmotorDirPort;
	uint16_t			motorDirPin;
	bool				motorDirection;
	uint32_t			motorStoppedMovingTime;		//Timestamp of when the motor stopped moving.  After a certain time delay (1 sec?) of motorEnabled==false and motorSpeed==0, the motor should be disabled to conserve batt power.
} TensChannel_HandleTypeDef;


typedef struct {
	GPIO_TypeDef*		pPort;
	uint16_t			pin;
	bool				curState;
	bool				activeState;
	uint8_t				progEndState;	//What state should the output be when the program ends? 0=deactive, 1=active, 2=no change.  Only used for outputs; ignored for inputs.
	bool				isBypassed;		// Used to indicate that manual (bypass) control is in use and running programs should not change the state.
} TensDigitalIO_HandleTypeDef;;

typedef struct {
	uint8_t				numChans;

	float				progFileVersion;
	uint16_t			numProgFiles;
	uint16_t			numProgramLines[TENS_PROGRAMS_MAX];
	uint32_t			flashStartAddress_Progs; //flashStartAddress that is accessible within this class.  Set in TensInit.
	uint32_t			progFileAddress[TENS_PROGRAMS_MAX];

	uint16_t			curProgNum[NUM_CHANNELS];
	structProgLine		curProgLine[NUM_CHANNELS];
	char				curProgName[NUM_CHANNELS][TENS_FILENAME_LEN];
	TensProgramStatus_HandleTypeDef		curProgStatus[NUM_CHANNELS];

	enumProgState				systemState;				//Only using stopped, paused, or running.
	uint16_t					masterSpeed;				//10-1000, represents a percentage (10-99 = slow, 100 = normal, 101-1000=fast)
	uint16_t 					masterMinSpeed;// = 20; 	//10
	uint16_t 					masterMaxSpeed;// = 500; 	//1000

	TensChannel_HandleTypeDef	tensChan[NUM_CHANNELS];
	TensDigitalIO_HandleTypeDef	inputs[NUM_DINPUTS];
	TensDigitalIO_HandleTypeDef	outputs[NUM_DOUTPUTS];

	GPIO_TypeDef*		_vBoostPort;
	uint16_t			_vBoostPin;
	uint8_t				_vBoostActiveState;
	bool				_vBoostCurState;

	GPIO_TypeDef*		_vEnablePort;
	uint16_t			_vEnablePin;
	uint8_t				_vEnableActiveState;
	bool				_vEnableCurState;


	bool				newDisplayRequest;
#define DISPLAY_REQ_MAXLEN 20
	char				displayReqChars[DISPLAY_REQ_MAXLEN];
	char				displayReqCharsLast[DISPLAY_REQ_MAXLEN];
	uint8_t				displayReqLen;


	uint8_t				audioValLow;	//Allows tens programs to access the audio level values
	uint8_t				audioValMid;
	uint8_t				audioValHigh;
	uint8_t				audioValTotal;

	IMU_HandleTypeDef*	imuRef;		//Allows tens programs to access the imu data
	TIM_HandleTypeDef* 	loopTimer;	//Reference to the timer that calls tensLoop.


} Tens_HandleTypeDef;


void TensInit(Tens_HandleTypeDef* dev, TIM_HandleTypeDef* TensLoopTimer, GPIO_TypeDef* VBoostPort, uint16_t VBoostPin,
						uint8_t VBoostActiveState, uint32_t fsaProgs, uint16_t ChanMinSpeed, uint16_t ChanMaxSpeed);
void TensInitWithVEnable(Tens_HandleTypeDef* dev, TIM_HandleTypeDef* TensLoopTimer, GPIO_TypeDef* VBoostPort, uint16_t VBoostPin, uint8_t VBoostActiveState,
		             	 GPIO_TypeDef* VEnablePort, uint16_t VEnablePin, uint8_t VEnableActiveState, uint32_t fsaProgs,
						 uint16_t ChanMinSpeed, uint16_t ChanMaxSpeed);

void TensReadFileAllocationTable(Tens_HandleTypeDef* dev);
uint32_t TensPrepFlashForDownload(Tens_HandleTypeDef* dev);
uint32_t TensCalculateFatSize(uint16_t numFiles);
uint32_t TensCalculateProgramSize(uint16_t numLines);
uint32_t TensWriteFileAllocationTable(Tens_HandleTypeDef* dev, uint16_t numFiles, uint16_t* numLines);
uint32_t TensSaveProgramToFlash(Tens_HandleTypeDef* dev, uint16_t progNum, uint32_t* progStartingAddress, char* progName, uint16_t numLines, structProgLine* progLines);
void TensClearProgStatus(Tens_HandleTypeDef* dev, uint8_t chanIndex);
bool TensFileExists(Tens_HandleTypeDef* dev, uint16_t fileNum);

bool TensReadProgLineFromFlash(Tens_HandleTypeDef* dev, uint8_t chanIndex, uint16_t lineNum);
uint32_t TensGetProgLineAddressFromFlash(Tens_HandleTypeDef* dev, uint16_t progNum, uint16_t lineNum);
uint32_t TensLoadFile(Tens_HandleTypeDef* dev, uint8_t chanIndex, uint8_t fileNum);

//void TensInitTensChannel(TensChannel_HandleTypeDef* chan, GPIO_TypeDef* PolarityPort, uint16_t PolarityPin,
//						TIM_HandleTypeDef* PulseOutTimerHandle, __IO uint32_t* PulseOutTCCR, uint32_t PulseOutTimChan,
//						DAC_HandleTypeDef* PDAC, uint32_t DacChannel, uint16_t DacMin, uint16_t DacMax, uint16_t PulseWidthMax,
//						uint16_t ChanMinSpeed, uint16_t ChanMaxSpeed);
void TensInitTensChannel(Tens_HandleTypeDef* dev, uint8_t chanIndex, GPIO_TypeDef* PolarityPort, uint16_t PolarityPin,
						TIM_HandleTypeDef* PulseOutTimerHandle, __IO uint32_t* PulseOutTCCR, uint32_t PulseOutTimChan,
						DAC_HandleTypeDef* PDAC, uint32_t DacChannel, uint16_t DacMin, uint16_t DacMax, uint16_t PulseWidthMax,
						uint16_t ChanMinSpeed, uint16_t ChanMaxSpeed);
void TensInitAuxChannel(TensChannel_HandleTypeDef* chan, uint16_t ChanMinSpeed, uint16_t ChanMaxSpeed);

uint32_t randomAtMost(uint32_t max);

void TensLoop(Tens_HandleTypeDef* dev);
void TensStartProgLine(Tens_HandleTypeDef* dev, uint8_t chanNum, uint16_t curLine);
void TensCheckProgLine(Tens_HandleTypeDef* dev, uint8_t chanNum);
//void TensStartDelay(Tens_HandleTypeDef* dev, uint8_t chanNum, structProgLine *curLine);
void TensStartTensOrPwmOutput(Tens_HandleTypeDef* dev, uint8_t chanNum, structProgLine *curLine);
void TensStartPwmOutput(Tens_HandleTypeDef* dev, uint8_t chanNum, int8_t startPulseWidth, int8_t endPulseWidth, enumPolarity polarity, uint32_t durationInMs);
void TensRepeatTenMotLine(Tens_HandleTypeDef* dev, uint8_t chanNum);

void TensSetSpeed(Tens_HandleTypeDef* dev, uint8_t chanIndex, uint16_t newSpeed);

uint32_t TensCalculateTensMotorTimerVal(TensChannel_HandleTypeDef* chan, uint8_t outputPercentageVal);
uint32_t TensCalculateModDurationVal(uint16_t masterSpeed, uint16_t chanSpeed, uint32_t origDuration);
void TensCalculateTensMotorStepVals(TensChannel_HandleTypeDef* chan, uint32_t modDuration);

//void TensChangeSystemState(Tens_HandleTypeDef* dev, enumProgState newState);
void TensStart(Tens_HandleTypeDef* dev, uint8_t chanNum);
void TensStop(Tens_HandleTypeDef* dev, uint8_t chanNum, enumProgState newState);

void TensSetPolarity(TensChannel_HandleTypeDef* chan, enumPolarity newVal);
void TensSwapPolarity(TensChannel_HandleTypeDef* chan, bool SwapPolarity);
void TensChangePolarityOutputs(TensChannel_HandleTypeDef* chan, enumPolarity newVal);

void TensSetTensOrMotorOutput(TensChannel_HandleTypeDef* chan, uint8_t newVal);
void TensEnableChannel(Tens_HandleTypeDef* dev, uint8_t index, bool EnableVal);
void TensSetChannelEnable_Worker(TensChannel_HandleTypeDef* chan, bool Enabled);

void TensSetTensIntensity(Tens_HandleTypeDef* dev, uint8_t index, uint16_t newVal);
void TensSetTensMinIntensity(Tens_HandleTypeDef* dev, uint8_t index, uint16_t newVal);
void TensSetTensMaxIntensity(Tens_HandleTypeDef* dev, uint8_t index, uint16_t newVal);
void TensSetDacVal(Tens_HandleTypeDef* dev, uint8_t index, uint16_t newVal);
uint8_t TensGetDacVal(TensChannel_HandleTypeDef* chan);
void TensvBoostEnable(Tens_HandleTypeDef* dev, bool newVal);

void TensInitMotorChannel(TensChannel_HandleTypeDef* chan, GPIO_TypeDef* MotorDirPort, uint16_t MotorDirPin, TIM_HandleTypeDef* MotorTimerHandle,
						__IO uint32_t* MotorTCCR, uint32_t MotorTimChan, uint8_t MotorMinOutput, uint8_t MotorMaxOutput,
						uint16_t ChanMinSpeed, uint16_t ChanMaxSpeed);

void TensMotorEnable(TensChannel_HandleTypeDef* chan, bool ENdisable);

void TensTimerPeriodElapsed(Tens_HandleTypeDef* dev, uint8_t index);

bool TensReadDigitalInput(Tens_HandleTypeDef* dev, uint8_t inNum, bool ignoreBypass);
void TensSetDigitalOutput(Tens_HandleTypeDef* dev, uint8_t outNum, bool activeVal, bool ignoreBypass);

void TensSetOutputsToSafeState(Tens_HandleTypeDef* dev);

uint32_t TensGetValue(Tens_HandleTypeDef* dev, enumDataSource dataSource, uint32_t dataVal1, uint32_t dataVal2, uint8_t chanNum, uint32_t maxVal);
bool TensTestCommand(Tens_HandleTypeDef* dev, uint8_t chanNum, structProgLine *curLine);
uint32_t TensMinMax(uint32_t val, uint32_t min, uint32_t max);
void TensSetCommand(Tens_HandleTypeDef* dev, uint8_t chanNum, structProgLine *curLine);
uint8_t TensGetChannelFromProgLine(Tens_HandleTypeDef* dev, uint8_t RequestingChanNum, structProgLine *curLine);
uint8_t TensProgramControlCommand(Tens_HandleTypeDef* dev, uint8_t chanNum, structProgLine *curLine);
void TensDisplayRequestCommand(Tens_HandleTypeDef* dev, uint8_t chanNum, structProgLine *curLine);
//bool TensNewDisplayRequest(Tens_HandleTypeDef* dev, uint8_t numChars);
#endif
