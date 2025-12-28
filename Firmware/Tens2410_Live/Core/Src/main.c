/* USER CODE BEGIN Header */
/**
  ******************************************************************************
  * @file           : main.c
  * @brief          : Main program body
  ******************************************************************************
  * @attention
  *
  * Copyright (c) 2024 STMicroelectronics.
  * All rights reserved.
  *
  * This software is licensed under terms that can be found in the LICENSE file
  * in the root directory of this software component.
  * If no LICENSE file comes with this software, it is provided AS-IS.
  *
  ******************************************************************************
  */
/* USER CODE END Header */
/* Includes ------------------------------------------------------------------*/
#include "main.h"
#include "fatfs.h"
#include "usb_device.h"

/* Private includes ----------------------------------------------------------*/
/* USER CODE BEGIN Includes */
#include <stdio.h>
#include <stdlib.h>
#include <rfm69.h>
#include "tens.h"
#include "pushButton.h"
#include "encoder.h"
#include "comDef.h"
#include "audio.h"
/* USER CODE END Includes */

/* Private typedef -----------------------------------------------------------*/
/* USER CODE BEGIN PTD */

/* USER CODE END PTD */

/* Private define ------------------------------------------------------------*/
/* USER CODE BEGIN PD */

#define USE_DISPLAY 1	//comment this to not use the onboard ST7735 display
//#define DEVICE_BOARD_E

/* USER CODE END PD */

/* Private macro -------------------------------------------------------------*/
/* USER CODE BEGIN PM */

/* USER CODE END PM */

/* Private variables ---------------------------------------------------------*/
ADC_HandleTypeDef hadc1;
DMA_HandleTypeDef hdma_adc1;

CRC_HandleTypeDef hcrc;

DAC_HandleTypeDef hdac1;

DFSDM_Filter_HandleTypeDef hdfsdm1_filter0;
DFSDM_Channel_HandleTypeDef hdfsdm1_channel1;
DFSDM_Channel_HandleTypeDef hdfsdm1_channel2;
DMA_HandleTypeDef hdma_dfsdm1_flt0;

UART_HandleTypeDef hlpuart1;
UART_HandleTypeDef huart1;
UART_HandleTypeDef huart2;

RTC_HandleTypeDef hrtc;

SD_HandleTypeDef hsd1;

SPI_HandleTypeDef hspi1;
SPI_HandleTypeDef hspi2;
DMA_HandleTypeDef hdma_spi2_tx;

TIM_HandleTypeDef htim1;
TIM_HandleTypeDef htim2;
TIM_HandleTypeDef htim3;
TIM_HandleTypeDef htim4;
TIM_HandleTypeDef htim6;
TIM_HandleTypeDef htim7;
TIM_HandleTypeDef htim16;
TIM_HandleTypeDef htim17;

/* USER CODE BEGIN PV */

//=== AUDIO ===//
Audio_HandleTypeDef haud;






//TIM_HandleTypeDef* phtimLed = NULL;
SPI_HandleTypeDef* phspiRadio = NULL;

#define RXBUFFSIZE	100

//============ USB ================
uint8_t		usbInitRxBuff[RXBUFFSIZE];
uint8_t		usbInitRxBuffIndex = 0;
uint8_t		usbInitRxOverflow = 0;			//Flag that is set by usbd_cdc_if.c when the RxBuff overflows.
uint8_t		usbNewBytesReceived = 0;  //This is set in usbd_cdc_if.c when new usb data is received and it indicates the number of bytes received.

char		usbRxBuff[RXBUFFSIZE];
uint8_t		usbRxBuffIndex = 0;



////============ IMU LSM6DSV16X ============
#include "imu.h"
IMU_HandleTypeDef	imu;
uint32_t	imuLastStepReadTime = 0;
uint32_t	imuLastGyroReadTime = 0;
//char chrDebug[100] = {0};

//============ ADC ================

#ifdef DEVICE_BOARD_B
///ADC Indexes
#define ADC_COUNT			2
#define ADC_INDEX_BATVSENSE	0
#define ADC_INDEX_CPUTEMP	1
//#define ADC_INDEX_		2
#endif
#ifdef DEVICE_BOARD_E
///ADC Indexes
#define ADC_COUNT			3
#define ADC_INDEX_BATVSENSE	2
//#define ADC_INDEX_CPUTEMP	1
#define ADC_INDEX_TENV_IN	0
#define ADC_INDEX_TENV_OUT	1
#endif


volatile uint16_t	adcVals[ADC_COUNT] = {0};
volatile uint8_t	adcConversionComplete = 0;
volatile uint8_t	adcConversionActive = 0;
uint32_t			adcReadInterval = 50;	//milliseconds between triggering an adc conversion
uint32_t			adcLastRead = 0;		//timestamp of last reading

//============ BATTERY ============
#define BATTLEVEL_PLUGGEDIN	105
#define BATTLEVEL_HIGH	90
#define BATTLEVEL_MED	40
#define BATTLEVEL_LOW	20
#define BATTLEVEL_DEAD	1

uint8_t		battPercent = 100;
uint8_t		battCurLevel = BATTLEVEL_HIGH;
uint8_t		battLastLevel = BATTLEVEL_HIGH;
uint32_t	battAtLevelTime = 0;
uint32_t	battAtLevelDebounceTime = 1000;  //Amount of time battery has to remain at a particular level to officially be considered at that level

uint32_t	battmVolt 	= 0;
uint32_t	battMinmV	= 3700;
uint32_t 	battMaxmV	= 4100;
#define		BATT_MAX_READINGS	30	//Total number of battery readings to average
uint16_t	battReadingsTaken	= 0;	//track the number of battery readings taken.  This allows the battery voltage reading to be accurate from startup until the MAX_READINGS have been taken.  This number is used as the divisor when calculating the running average.
uint16_t	battReadingIndex = 0;
//uint32_t	battmVoltReadings[BATT_MAX_READINGS] = {0};
uint16_t	battAdcReadings[BATT_MAX_READINGS] = {0};


//============ I/O ============
#ifdef DEVICE_BOARD_B
#define PB_COUNT	4	//powerPb (x1), PB2-4 (x3)
Pushbutton_HandleTypeDef pb[PB_COUNT];
bool	powerPbBypassed = true;		//As soon as the powerPb digital input reads "inactive", this will be set to false.  The power-off will only happen if this is false.
#endif
#ifdef DEVICE_BOARD_E
//#define PB_COUNT	9	//powerPb (x1), PB2-4 (x3)
//TODO: fix this!!
#define PB_COUNT	10

Pushbutton_HandleTypeDef pb[PB_COUNT];
Pushbutton_Matrix_Typedef pbMatrix;

//uint8_t tmpPB[10] = {0};

uint8_t curChanPbSelected = 1;

bool	powerPbBypassed = false; //true;		//As soon as the powerPb digital input reads "inactive", this will be set to false.  The power-off will only happen if this is false.
#endif
#define ENCODER_COUNT	1	//number of encoders
Encoder_HandleTypeDef	encoder[ENCODER_COUNT];
TIM_HandleTypeDef* encTim[] = {&htim1};
uint32_t			encoderReadInterval = 50;
uint32_t			encoderLastRead = 0;


//============ LED ============
TIM_HandleTypeDef* phtimLed = &htim2;
bool	curLedState = false;
uint8_t rgbVal[3] = {0};
#define LED_RCHAN	TIM_CHANNEL_1
#define LED_GCHAN	TIM_CHANNEL_2
#define LED_BCHAN	TIM_CHANNEL_4
typedef enum {
	led_Off				= 0,
	Led_OnSolid			,
	Led_Flash_Slow		,
	Led_Flash_Medium	,
	Led_Flash_Fast		,
	Led_On_1Sec			,
	Led_On_5Sec
} ledModeEnum;
ledModeEnum	ledMode = led_Off;
unsigned long ledModeLastChanged = 0;
unsigned long ledDelayPeriod = 500;		//How many milliseconds until the light turns off (LedOnBriefly) or until it toggles (LedFlash)
int32_t	ledFlashesRemaining = 0;
#define	LED_SLOW_FLASH_INTERVAL		1000
#define	LED_MEDIUM_FLASH_INTERVAL	250
#define	LED_FAST_FLASH_INTERVAL		50


//============ LCD ST7735 ===================
#ifdef USE_DISPLAY
#define DISPLAY_NAME	"TENS 2410"

#ifndef PROGMEM
#define PROGMEM
#endif

#include "ST7735Canvas.h"
#ifdef DEVICE_BOARD_B
TIM_HandleTypeDef* phtimLcdBl = &htim2;
#define LCD_BLCHAN	TIM_CHANNEL_3
#endif
#ifdef DEVICE_BOARD_E
TIM_HandleTypeDef* phtimLcdBl = &htim16;
#define LCD_BLCHAN	TIM_CHANNEL_1
#endif

LCD_HandleTypeDef lcd;

#include "Dialog10pNarRssi.h"
#include "Dialog24p.h"
const GFXfont *fontBig				= &Dialog_plain_24;
const GFXfont *fontStatusbar		= &Dialog_plain_10NarRssi;

uint8_t			_lastBattLevel = 0;
uint32_t		_battLastDisplayedTime = 0;
int16_t			_lastSigLevel = 0;

uint8_t*		_lastDisplayedIntensity = 0;
uint32_t		_IntensityLastDisplayedTime = 0;

//uint8_t			_lastChanState[NUM_CHANNELS] = {10};
uint16_t		_lastChanStateColor[NUM_CHANNELS] = {0};
uint32_t		_chanStateLastDisplayedTime = 0;

bool			_displayEqEnabled = true; //false;
uint8_t			_lastDisplayedAudioEq[NUM_FREQ_BANDS] = {0};
uint32_t		_audioEqLastDisplayedTime = 0;

LCD_CanvasHandleTypeDef canStatusBar;
LCD_CanvasHandleTypeDef canScreen;

bool			displayOn = true;
bool			displayAutoOff = true;
uint32_t		displayAutoOffTime = 10000;		//10 seconds until auto off
uint32_t		displayLastActivityTime = 0;	//timestamp of last activity that would cause display to be on. Used for displayAutoOff time

#endif

//============ Radio ===================
bool 		radioInstalled = true;
RFM69_HandleTypeDef	radio;
char 		radioRxBuff[RXBUFFSIZE];
uint8_t		radioRxBuffIndex = 0;
int16_t		lastRssiDisplayed = 100;





//============ SD Card ============
//FATFS _SDFatFS;  // File system object
//char _SDPath[4]; // SD card logical drive path








//============ Tens ============
Tens_HandleTypeDef	tens;
//DeviceType_t	deviceType = -1;
#ifdef DEVICE_BOARD_B
DeviceType_t deviceType = dtTens2410B1;
#endif
#ifdef DEVICE_BOARD_E
DeviceType_t deviceType = dtTens2410E1;
#endif

//TIM_HandleTypeDef	tensLoopTim;
uint8_t			tensMutex = 0;				//main loop sets this when it enters tensLoop and clears it after exiting.  Prevents the timer
								//ISR from firing and running the tensLoop if the main loop is currently running it.
uint32_t flashStartAddress_TensProgs = 0;


#define DEVICE_BOARD 1 //"TEN2410ADB"
//#include "deviceBoard.h"

//uint16_t minSpeed = 50; 	//10
//uint16_t maxSpeed = 500; 	//1000

int8_t speedMultiplier = 1;
uint16_t speedSlowOutMin = 10;
uint16_t speedSlowOutMax = 100;
uint16_t speedFastOutMin = 100;
uint16_t speedFastOutMax = 1000; //500;



// Temp storage for downloading Tens programs:
uint8_t				dlNumChans;
float				dlProgFileVersion;
uint16_t			dlNumProgFiles;
uint16_t			dlCurProg = 0;
//char				(*dlProgName)[TENS_FILENAME_LEN];
char				dlProgName[TENS_FILENAME_LEN];
//uint16_t*			pDLNumProgramLines;
uint16_t			dlNumProgramLines[TENS_PROGRAMS_MAX];
//uint32_t*			pdlProgStartAddr;
uint32_t			dlProgStartAddr[TENS_PROGRAMS_MAX];
//structProgLine		**dlPrograms;
//structProgLine*		dlProgramLines;
structProgLine		dlProgramLines[TENS_MAX_PROGRAM_LINES];
//============ Flash Data ==============
//Flash storage:
//  The "settings" structure will be stored at the very end of flash storage area.  As more modules need to store info (tens programs,
//		communications device info, etc.), that info will be stored on previous pages of flash.  So the settings starts on the last
//		page(s) and all subsequent flash data will be written on previous and previous pages.
uint32_t flashStartAddress_Settings = 0;
float SW_VER = 0.03; //This is used to see if the flash has been initialized.  If it hasn't then it will be initialized with default values.

typedef struct {
	float 		flashVersion;
/*
	int16_t		encMinVal[ENCODER_COUNT];
	int16_t		encMaxVal[ENCODER_COUNT];
	int16_t		encInitVal[ENCODER_COUNT];
	uint8_t		encCountMode[ENCODER_COUNT];
	uint8_t		encPulsePerCount[ENCODER_COUNT];

	int16_t		potMinVal[POT_COUNT];
	int16_t		potMaxVal[POT_COUNT];

	int16_t		joyXMinVal[POT_COUNT];
	int16_t		joyXMaxVal[POT_COUNT];
	int16_t		joyYMinVal[POT_COUNT];
	int16_t		joyYMaxVal[POT_COUNT];

	uint8_t		mainLcdBacklightLevel;
	uint8_t		mainLcdContrastLevel;		//valid range is 4-63.

	//uint32_t	serialSendPeriodic;			//bitmask of which channels to send data for during a periodic update
	//uint32_t	serialSendOnChange;			//bitmask of which channels to send data for when a change is detected
	uint32_t	serialPeriodicUpdateInterval;
	bool		serialSendPeriodic[CHANNEL_COUNT];
	bool		serialSendOnChange[CHANNEL_COUNT];
*/

	uint16_t 			gatewayId;			//uint16_t
	uint8_t 			networkId;			//uint8_t
	uint16_t 			nodeId;				//uint16_t
	char 				encryptKey[16];		//uint8_t[16]
	bool 				useHighPower;
	int16_t 			autoRssiPower;		//int16_t

	uint16_t			initialSystemProgNum;
	uint8_t				displayBrightness;


	uint32_t	spare[4];

} flashVals_t;
#define union_ByteSize  sizeof(flashVals_t)
#define union_WordSize (sizeof(flashVals_t) / 4) + ( (sizeof(flashVals_t) % 4) ? 1 : 0)
typedef union {
  flashVals_t vars;
  uint8_t byteArray[union_ByteSize];
  uint32_t wordArray[union_WordSize];
  uint64_t doubleWordArray[union_ByteSize / 8];
} unionFlash_t;
unionFlash_t settings;

bool unsavedChanges = false;
uint32_t settingLastChangedTime = 0;
uint32_t saveSettingsDelayMs = 1000;        //Allow a delay before saving settings to eeprom.  If many rapid changes are being made, such as holding down a button that rapid-fires, this will avoid writting to eeprom after each individual change.



/////// TESTING DATA //////////
#define MAX_INCOMING_CHANNELS	30
//int16_t testData[MAX_INCOMING_CHANNELS] = {0};
//int16_t testDataLast[MAX_INCOMING_CHANNELS] = {0};
int16_t paData[MAX_INCOMING_CHANNELS] = {0};
int16_t paDataLast[MAX_INCOMING_CHANNELS] = {0};

//int16_t tmpSpeed0 = 0;
//int16_t tmpSpeed1 = 0;

uint32_t audioTime = 0;
uint32_t audioDelta = 0;




/* USER CODE END PV */

/* Private function prototypes -----------------------------------------------*/
void SystemClock_Config(void);
void PeriphCommonClock_Config(void);
static void MX_GPIO_Init(void);
static void MX_DMA_Init(void);
static void MX_ADC1_Init(void);
static void MX_DAC1_Init(void);
static void MX_DFSDM1_Init(void);
static void MX_LPUART1_UART_Init(void);
static void MX_USART1_UART_Init(void);
static void MX_USART2_UART_Init(void);
static void MX_RTC_Init(void);
static void MX_SDMMC1_SD_Init(void);
static void MX_SPI1_Init(void);
static void MX_SPI2_Init(void);
static void MX_TIM1_Init(void);
static void MX_TIM2_Init(void);
static void MX_TIM3_Init(void);
static void MX_TIM4_Init(void);
static void MX_TIM16_Init(void);
static void MX_TIM17_Init(void);
static void MX_TIM6_Init(void);
static void MX_TIM7_Init(void);
static void MX_CRC_Init(void);
/* USER CODE BEGIN PFP */

void initAudio(void);
void enableAudio(bool Enabled);
void checkAudio(void);
void processAudioData(void);
void analyzeFrequencyBands(void);

//static uint32_t getFlashPage(uint32_t Address);	//moved to main.h
static uint32_t getFlashPageStartingAddress(uint16_t page);
void calculateFlashAddresses();
void loadSettings(void);
void loadSettingsFromDefaults(void);
uint32_t saveSettings(bool forceSave);
void settingsChanged(void);

void checkBattery(int32_t curMilliVolts);
void checkPower(void);
void powerOff(void);

void checkAnalog();
void readAnalog();


int16_t mapi16(int16_t x, int16_t inMin, int16_t inMax, int16_t outMin, int16_t outMax, bool clampLimits);


// ===IMU===
/*
void lsm6dsv16x_single_double_tap_handler(void);
void lsm6dsv16x_single_double_tap(void);
static int32_t platform_write(void *handle, uint8_t reg, const uint8_t *bufp,
                              uint16_t len);
static int32_t platform_read(void *handle, uint8_t reg, uint8_t *bufp,
                             uint16_t len);
static void tx_com( uint8_t *tx_buffer, uint16_t len );
static void platform_delay(uint32_t ms);
static void platform_init(void *handle);

*/
// === /imu ===

void changeLedMode(ledModeEnum newMode, uint8_t rVal, uint8_t gVal, uint8_t bVal, uint8_t numFlashes);
void checkLed();
void setRGBLight(uint8_t rLevel, uint8_t gLevel, uint8_t bLevel);

void usbReceive(uint8_t* Buf, uint32_t len);
void checkUsbRx();

void checkRadioRx(void);
uint16_t StringToI32Array(char *src, uint16_t srcLen, const char *delim, int32_t *dest, uint8_t maxFields);
void newMessageReceived(char* buff, uint8_t indx, comChanEnum comChannel, uint16_t sender);
void sendAck(comChanEnum comChannel, uint16_t sender);
void sendNack(comChanEnum comChannel, uint16_t sender);
void dataSend(char* buff, uint16_t len, comChanEnum channel, uint16_t target, bool requestAck);
extern uint8_t CDC_Transmit_FS(uint8_t* Buf, uint16_t Len);

bool setParameter(char* buff, uint8_t indx, comChanEnum comChannel, uint16_t sender);

bool setParameterArray(char* buff, uint8_t indx, comChanEnum comChannel, uint16_t sender);
bool SetParamArray1Data(int32_t *newData,  uint8_t dataCount);
bool SetParamArray_ProgLineData(int32_t *optParam, int32_t *lineData, uint8_t lineDataLen);

//bool sendParameter(char Command, char Param, uint8_t paramIndex, comChanEnum comChannel, uint16_t Sender);
bool sendParameter(char* buff, uint8_t indx, comChanEnum comChannel, uint16_t sender);
//bool sendParameterArray(char Command, char Param, comChanEnum comChannel, uint16_t sender);
bool sendParameterArray(char* buff, uint8_t indx, comChanEnum comChannel, uint16_t sender);

void DownloadProjectFromRemoteHost(comChanEnum comChannel, uint16_t sender, uint16_t numPrograms);

void initDisplay();
//void turnOffDisplay();
void turnDisplayOnOff(bool newState);

void checkDisplay(bool forceUpdate);
void updateDisplay(bool forceUpdate);
bool displayStatusBarNeedsUpdated();
void updateDisplayStatusBar();
bool displayIntensityNeedsUpdated();
void updateDisplayIntensity();
bool displayChanEnabledNeedsUpdated();
void updateDisplayChanEnabled();
bool displayAudioEqNeedsUpdated(void);
void updateDisplayAudioEq(void);

void initTens();

void initPushbuttons(void);
void checkPushbuttons(void);
void pushButtonEvent(uint8_t pbId, uint8_t eventId);

void initImu(void);
void checkImu(void);





/* USER CODE END PFP */

/* Private user code ---------------------------------------------------------*/
/* USER CODE BEGIN 0 */
//void initAudio(void){
//	//Start audio filtering system
//	// Initialize CMSIS FFT
//	arm_rfft_fast_init_f32(&fft_handler, FFT_SIZE);
//
//	// Start DFSDM with DMA for both filters
//	HAL_DFSDM_FilterRegularStart_DMA(&hdfsdm1_filter0, audio_buffer_left, AUDIO_BUFFER_SIZE);
//	//2nd channel is disabled because the LCD SPI is using the DMA channel that is needed by the 2nd DFSDM channel.
//	//HAL_DFSDM_FilterRegularStart_DMA(&hdfsdm1_filter1, audio_buffer_right, AUDIO_BUFFER_SIZE);
//
//	audioEnabled = true;
//}

//void enableAudio(bool Enabled){
//	audioEnabled = Enabled;
//	if(Enabled == true){
//		initAudio();
//	}else{
//		HAL_DFSDM_FilterRegularStop_DMA(&hdfsdm1_filter0);
//		audioBufferFull = 0;
//	}
//}
//
//void checkAudio(void){
//	if(audioBufferFull){
//		processAudioData();
//		analyzeFrequencyBands();
//		audioBufferFull = 0;
//	}
//}

//void processAudioData(void)
//{
//  // Process left channel (modify to combine channels if needed)
//  for (uint32_t i = 0; i < AUDIO_BUFFER_SIZE; i++)
//  {
//    fft_input[i] = (float32_t)audio_buffer_left[i] * 0.00003051757f; // Scale to [-1,1]
//    fft_input[i] *= (0.54f - 0.46f * arm_cos_f32(2 * PI * i / (AUDIO_BUFFER_SIZE - 1)));
//  }
//
//  arm_rfft_fast_f32(&fft_handler, fft_input, fft_output, 0);
//  arm_cmplx_mag_f32(fft_output, fft_magnitude, FFT_SIZE);
//}
//
//void analyzeFrequencyBands(void){
//	float32_t band_sums[NUM_FREQ_BANDS] = {0};
//	uint32_t bins_per_band = FFT_SIZE / NUM_FREQ_BANDS;
//	float32_t max_magnitude = 0;
//	float32_t avg_magnitude = 0;
//
//	// Calculate band sums and find max/avg magnitude
//	for (uint32_t i = 1; i < FFT_SIZE; i++){  // Skip DC component
//		uint32_t band = i / bins_per_band;
//	    if (band < NUM_FREQ_BANDS){
//	    	band_sums[band] += fft_magnitude[i];
//	    	avg_magnitude += fft_magnitude[i];
//	    	if (fft_magnitude[i] > max_magnitude)
//	    		max_magnitude = fft_magnitude[i];
//	    }
//	}
//	avg_magnitude /= (FFT_SIZE - 1);  // Average over non-DC bins
//
//	if (max_magnitude > 0){
//		// Calculate desired gain based on target level
//		float32_t current_level = 0;
//		for (uint8_t i = 0; i < NUM_FREQ_BANDS; i++){
//			float32_t normalized = band_sums[i] / (max_magnitude * bins_per_band) * 100.0f;
//			current_level += normalized;
//		}
//	    //current_level /= 5.0f;  // Average band level
//		current_level /= (NUM_FREQ_BANDS * 1.0f);  // Average band level
//
//	    // Update AGC gain with smoothing
//	    float32_t target_gain = (current_level > 0.1f) ? (TARGET_LEVEL / current_level) : MAX_GAIN;
//	    if (target_gain > MAX_GAIN) target_gain = MAX_GAIN;
//	    if (target_gain < 1.0f) target_gain = 1.0f;  // Don't attenuate below unity
//	    agc_gain = (AGC_ALPHA * target_gain) + ((1.0f - AGC_ALPHA) * agc_gain);
//
//	    // Apply gain and smoothing to bands
//	    for (uint8_t i = 0; i < NUM_FREQ_BANDS; i++){
//	    	float32_t normalized = band_sums[i] / (max_magnitude * bins_per_band);
//	    	float32_t current_value = normalized * 100.0f * agc_gain;
//
//	    	// Apply exponential moving average
//	    	smoothed_bands[i] = (SMOOTHING_ALPHA * current_value) + ((1.0f - SMOOTHING_ALPHA) * smoothed_bands[i]);
//
//	    	// Convert to uint8_t and clamp to 0-100
//	    	frequency_bands[i] = (uint8_t)smoothed_bands[i];
//	    	if (frequency_bands[i] > 100) frequency_bands[i] = 100;
//	    }
//
//	    tens.audioValLow = frequency_bands[0] + frequency_bands[1];
//	    if(tens.audioValLow > 100){
//	    	tens.audioValLow = 100;
//	    }
//	    tens.audioValMid = frequency_bands[2];
//	    tens.audioValHigh = frequency_bands[3];
//	    tens.audioValTotal = tens.audioValLow + tens.audioValMid + tens.audioValHigh;
//	    if(tens.audioValTotal > 100){
//	    	tens.audioValTotal = 100;
//	    }
//	}
//}




uint32_t getFlashPage(uint32_t Address){
	//The STM32L4's have dual bank memory; half of the memory is in the first bank and the other half in the second bank.
	//Bank 1 will ALWAYS have pages 0-255 and bank 2 will ALWAYS have pages 256-511.
	//A 1MB model of chip will have 512k in bank 1 (pages 0-255) and 512k in bank 2 (pages 256-511).
	//A 512KB model will have 256k in bank 1 (pages 0-127) and 256k in bank 2 (pages 256-383).
	//A 256KB model will have 128k in bank 1 (pages 0-63) and 128k in bank 2 (pages 256-319).


	uint16_t startingPage = (Address > (FLASH_SIZE / 2)) ? 256 : 0;
	uint32_t bankStartAddress = (Address > (FLASH_SIZE / 2)) ? (FLASH_BASE + (FLASH_SIZE / 2)) : FLASH_BASE;
	uint32_t targetPage = ( (Address - bankStartAddress) / FLASH_PAGE_SIZE) + startingPage;

	if(targetPage < 512){
		return targetPage;
	}else{
		return 0;
	}
}

static uint32_t getFlashPageStartingAddress(uint16_t page){
	if(page < 256){
		return FLASH_BASE + (FLASH_PAGE_SIZE * page);
	}else if(page < 512){
		return (FLASH_BASE + (FLASH_SIZE / 2)) + ( (page - 256) * FLASH_PAGE_SIZE);
	}else{
		return 0;
	}
}

void calculateFlashAddresses(){
	//The STM32L4's have dual bank memory; half of the memory is in the first bank and the other half in the second bank.
	//Bank 1 will ALWAYS have pages 0-255 and bank 2 will ALWAYS have pages 256-511.
	//A 1MB model of chip will have 512k in bank 1 (pages 0-255) and 512k in bank 2 (pages 256-511).
	//A 512KB model will have 256k in bank 1 (pages 0-127) and 256k in bank 2 (pages 256-383).
	//A 256KB model will have 128k in bank 1 (pages 0-63) and 128k in bank 2 (pages 256-319).

	//Bank 1's base address will always be = FLASH_BASE.
	//Bank 2's base address will be = (FLASH_BASE + (FLASH_SIZE / 2)).

	//uint32_t tmpBase1StartingAddr = FLASH_BASE;
	//uint32_t tmpBase2StartingAddr = (FLASH_BASE + (FLASH_SIZE / 2));

	//flash size:  FLASH_SIZE
	//flash start address: FLASH_BASE
	//end address (1 byte past end of flash): (FLASH_BASE + FLASH_SIZE)

	uint32_t tmpLastFlashAddress = FLASH_BASE + FLASH_SIZE - 1;
	uint32_t tmpSettingsSize = sizeof(settings);
	flashStartAddress_Settings = getFlashPageStartingAddress(getFlashPage(tmpLastFlashAddress - tmpSettingsSize));

	//Tens Program Files (new as of Tens2410-DB-B2-04)
	// There will be a fixed amount of storage for tens programs and this will store all programs (channel progs and sys progs).  The last page of the
	// storage will be the page prior to flashSettings page.
	//flashStartAddress_TensProgs = getFlashPageStartingAddress(getFlashPage(flashStartAddress_Settings - TENS_PROGRAM_STORAGE_NUMPAGES)); !!!!Parenthesis were in the wrong place. Only allowed for 1 page of flash storage for tens programs :(
	flashStartAddress_TensProgs = getFlashPageStartingAddress(getFlashPage(flashStartAddress_Settings) - TENS_PROGRAM_STORAGE_NUMPAGES);

	//uint32__t tmpXXXSize = sizeof(xxx);
		// flashStartAddress_xxx = getFlashPageStartingAddress(getFlashPage((flashStartAddress_Settings -1) - tmpXXXSize));

	//uint32__t tmpXXXSize = sizeof(xxx);
		// flashStartAddress_xxx = getFlashPageStartingAddress(getFlashPage((flashStartAddress_Settings -1) - tmpXXXSize));

	//uint32__t tmpXXXSize = sizeof(xxx);
		// flashStartAddress_xxx = getFlashPageStartingAddress(getFlashPage((flashStartAddress_Settings -1) - tmpXXXSize));



}

void loadSettings(){
	//volatile uint32_t* addr = (__IO uint32_t *)StartPageAddress;
	//volatile uint8_t* addr = (__IO uint8_t *)StartPageAddress;
	volatile uint8_t* addr = (__IO uint8_t *)flashStartAddress_Settings;



	//Read flash data into settings structure:
	for(uint8_t n=0; n<(union_ByteSize); n++){
		settings.byteArray[n] = *addr;
		addr++;
	}

	if(settings.vars.flashVersion != SW_VER){
		loadSettingsFromDefaults();
		saveSettings(true);
	}
}

void loadSettingsFromDefaults(){
	settings.vars.flashVersion 	= SW_VER;
/*
	for(uint8_t n=0; n< ENCODER_COUNT; n++){
		settings.vars.encMinVal[n] = -10;
		settings.vars.encMaxVal[n] = 10;
		settings.vars.encInitVal[n] = 0;
		settings.vars.encCountMode[n] = EncoderMode_minToMax;
		settings.vars.encPulsePerCount[n] = 4;
	}


	for(uint8_t n=0; n< POT_COUNT; n++){
		settings.vars.potMinVal[n] = 0;
		settings.vars.potMaxVal[n] = 100;
	}

	for(uint8_t n=0; n< JOY_COUNT; n++){
		settings.vars.joyXMinVal[n] = -100;
		settings.vars.joyXMaxVal[n] = 100;
		settings.vars.joyYMinVal[n] = -100;
		settings.vars.joyYMaxVal[n] = 100;
	}

	settings.vars.mainLcdBacklightLevel = 75;
	settings.vars.mainLcdContrastLevel = 7;

	settings.vars.serialPeriodicUpdateInterval = 1000;
	//settings.vars.serialSendPeriodic = 0b00000000001111111111111111111111;	//bit mask of which channels to send periodically
	//settings.vars.serialSendOnChange = 0b00000000001111111111111111111111;	//bit mask of which channels to send periodically
	for(uint8_t n=0; n<CHANNEL_COUNT; n++){
		settings.vars.serialSendPeriodic[n] = false;
		settings.vars.serialSendOnChange[n] = true;
	}
*/

	//Radio:
	//settings.vars.broadcastAddress = 0;
	settings.vars.gatewayId = 1;
	settings.vars.networkId = 25;

	//Use part of the device's serial number (UID) to generate an id.
	volatile uint16_t* tmpAddr = (__IO uint16_t *)0x1FFF7592;  //XY coordinates of chip on wafer
	settings.vars.nodeId = *tmpAddr;

	sprintf(settings.vars.encryptKey, "sampleEncryptKe");
	settings.vars.encryptKey[15] = 'y';
	settings.vars.useHighPower = true;	//only set this for rfm69HW_HCW
	settings.vars.autoRssiPower = -80;

	settings.vars.initialSystemProgNum = 0;

	settings.vars.displayBrightness = 50;
}

uint32_t saveSettings(bool forceSave){
	if((forceSave == true) || ((unsavedChanges == true) && (HAL_GetTick() - settingLastChangedTime > saveSettingsDelayMs))){
		//uint32_t memAddress = StartPageAddress;
		uint32_t memAddress = flashStartAddress_Settings;
		static FLASH_EraseInitTypeDef EraseInitStruct;
		uint32_t PAGEError;
		//int sofar=0;

		HAL_FLASH_Unlock();  // Unlock the Flash to enable the flash control register access

		// Erase the user Flash area
		uint32_t StartPage = getFlashPage(memAddress);

		/* Fill EraseInit structure*/
		EraseInitStruct.TypeErase   = FLASH_TYPEERASE_PAGES;
		EraseInitStruct.Page 	   = StartPage;
		EraseInitStruct.NbPages     = 1;

		if (HAL_FLASHEx_Erase(&EraseInitStruct, &PAGEError) != HAL_OK){
			//Error occurred while page erase
			return HAL_FLASH_GetError ();
		}

		//Program the user Flash area
		for(uint8_t n=0; n<(union_ByteSize / 8); n++){
			if (HAL_FLASH_Program(FLASH_TYPEPROGRAM_DOUBLEWORD, memAddress, settings.doubleWordArray[n]) != HAL_OK){
				// Error occurred while writing data in Flash memory
				return HAL_FLASH_GetError ();
			}
			memAddress += 8;
		}

		HAL_FLASH_Lock();	// Lock the Flash to disable the flash control register access
		unsavedChanges = false;
	}
	return 0;
}

void settingsChanged(){
	unsavedChanges = true;
	settingLastChangedTime = HAL_GetTick();
}


void checkBattery(int32_t curMilliVolts){
	//int32_t tmpPct = (battmVolt - battMinmV) * (100 - 0) / (battMaxmV - battMinmV) + 0;
	int32_t tmpPct = 0;
	if(battmVolt > battMinmV){
		tmpPct = (battmVolt - battMinmV) * (100 - 0) / (battMaxmV - battMinmV) + 0;
	}
	//battPercent = (tmpPct < 0) ? 0 : (tmpPct > 100) ? 100 : (uint8_t)(tmpPct);
	battPercent = (tmpPct < 0) ? 0 : (uint8_t)(tmpPct);

	//Get the battery level according to the most recent voltage measurement:
	uint8_t tmpBattLevel = BATTLEVEL_HIGH;
	if(battPercent >= BATTLEVEL_PLUGGEDIN){
		tmpBattLevel = BATTLEVEL_PLUGGEDIN;
		displayLastActivityTime = HAL_GetTick();	//keep the display active if plugged in.
	}else if(battPercent >= BATTLEVEL_HIGH){
		tmpBattLevel = BATTLEVEL_HIGH;
	}else if(battPercent >= BATTLEVEL_MED){
		tmpBattLevel = BATTLEVEL_MED;
	}else if(battPercent >= BATTLEVEL_LOW){
		tmpBattLevel = BATTLEVEL_LOW;
	}else{
		tmpBattLevel = BATTLEVEL_DEAD;
	}

	//Does the battery level agree with the settled-upon battCurLevel?
	if(tmpBattLevel == battCurLevel){
		//Yes.  All is good.
	}else{
		//No, battery is reading at a different level than the settled-upon level.
		//Is this the first time we're seeing this new level?
		if(tmpBattLevel != battLastLevel){
			//Yes, first time.  Capture the time for debouncing:
			battAtLevelTime = HAL_GetTick();
		}else{
			//No.  But have we been at this new level long enough to consider the newly measured level to be the "official" new level?
			if(HAL_GetTick() - battAtLevelTime > battAtLevelDebounceTime){
				//We are now officially at the new level.
				battCurLevel = tmpBattLevel;
			}
		}
	}

	battLastLevel = tmpBattLevel;
}

void checkPower(void){
	//Is the powerPb bypassed?
#ifdef DEVICE_BOARD_B
	if(HAL_GPIO_ReadPin(PB_POWER_GPIO_Port, PB_POWER_Pin) == GPIO_PIN_SET){
		powerPbBypassed = false;
	}
#endif
#ifdef DEVICE_BOARD_E
	if(HAL_GPIO_ReadPin(PB_POWER_GPIO_Port, PB_POWER_Pin) == GPIO_PIN_RESET){
	//if(HAL_GPIO_ReadPin(PB_POWER_GPIO_Port, PB_POWER_Pin) == GPIO_PIN_SET){
		powerPbBypassed = false;
	}
#endif

	if(powerPbBypassed)	return;

	//Is there a pushbutton request for power off?
	if(PB_CurState(&pb[0]) == pbState_longpress_Pressed){
		powerOff();
		return;
	}
	//Check battery level:
	if(battCurLevel == BATTLEVEL_DEAD){
		powerOff();
		return;
	}

//TODO: add functionality for idle time power-off.  Say, if no user input has occurred for at least 5-10 minutes.

}

void powerOff(void){
	//Stop all program activity, turn off display, turn off power latch, wait for user to release power button.
	TensStop(&tens, 0, progState_Stopped);
	HAL_GPIO_WritePin(POWER_ON_MAIN_GPIO_Port, POWER_ON_MAIN_Pin, GPIO_PIN_RESET);
	//turnOffDisplay();
	turnDisplayOnOff(false);
	while(1);
}

void HAL_ADC_ConvCpltCallback(ADC_HandleTypeDef *hadc)
{
	adcConversionComplete = 1;
}

void checkAnalog(){
	if(adcConversionComplete == 1){
		adcConversionActive = 0;
		adcConversionComplete = 0;		//Clear the flag.  It will be set in the ISR Callback for ADC_Conversion complete.
		readAnalog();
		adcLastRead = HAL_GetTick();
	}else if( (adcConversionActive == 0) && (HAL_GetTick() - adcLastRead > adcReadInterval) ){
		adcConversionActive = 1;
		HAL_ADC_Start_DMA(&hadc1, (uint32_t *)adcVals, ADC_COUNT);
	}
}

void readAnalog(){
//	uint32_t maxAdcValue = 4096; //12 bit
//	uint32_t tmpAdcBuffer = 120; //4096 * 0.03

	//Battery Voltage:
	battAdcReadings[battReadingIndex] = adcVals[ADC_INDEX_BATVSENSE];
	battReadingsTaken = ((battReadingsTaken < BATT_MAX_READINGS) ? battReadingsTaken + 1 : BATT_MAX_READINGS);
	battReadingIndex = ((battReadingIndex >= (BATT_MAX_READINGS - 1) ) ? 0 : battReadingIndex + 1);
	//Calculate the average:
	uint32_t battAdcAvg = 0;
	for(uint16_t n=0; n< battReadingsTaken; n++){
		battAdcAvg += battAdcReadings[n];
	}
	battAdcAvg /= battReadingsTaken;
	battmVolt = mapi16(battAdcAvg, 2168, 3042, 3600, 5000, false);  //fromMin & fromMax values obtained via testing on prototype board

	checkBattery(battmVolt);
	//uint32_t tmpPct = (battmVolt - battMinmV) * (100 - 0) / (battMaxmV - battMinmV) + 0;
	//battPercent = (tmpPct < 0) ? 0 : (tmpPct > 100) ? 100 : (uint8_t)(tmpPct); - Already performed in checkBattery() function.

	////Pots
	////for(uint8_t n=0; n<POT_COUNT; n++){
	////	chanVal[POT1_CHAN_INDEX + n] = mapi16(adcVals[potAdcIndex[n]], 0 + tmpAdcBuffer, maxAdcValue - tmpAdcBuffer, settings.vars.potMinVal[n], settings.vars.potMaxVal[n], true);
	////}
	//for(uint8_t n=0; n<CHANNEL_COUNT; n++){
	//	if(channel[n].chanType == chanType_Pot){
	//		channel[n].curVal = mapi16(adcVals[channel[n].adcIndex], tmpAdcBuffer, maxAdcValue - tmpAdcBuffer,
	//									channel[n].minVal, channel[n].maxVal, true);
	//	}
	//}

}



//int16_t mapi16(int16_t x, int16_t inMin, int16_t inMax, int16_t outMin, int16_t outMax){
//	return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
//}
int16_t mapi16(int16_t x, int16_t inMin, int16_t inMax, int16_t outMin, int16_t outMax, bool clampLimits){
	int16_t retVal = (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
	if(clampLimits == true){
		retVal = (retVal < outMin ? outMin : (retVal > outMax ? outMax : retVal));
	}
	return retVal;
}


/*
void lsm6dsv16x_single_double_tap_handler(void){
  lsm6dsv16x_all_sources_t status;

  // Read output only if new xl value is available //
  lsm6dsv16x_all_sources_get(&dev_ctx, &status);

  if (status.single_tap) {
    stap_event_catched = 1;
  }
  if (status.double_tap) {
    dtap_event_catched = 1;
  }
}

void lsm6dsv16x_single_double_tap(void){
  lsm6dsv16x_pin_int_route_t pin_int;
  lsm6dsv16x_reset_t rst;

  // Initialize mems driver interface //
  dev_ctx.write_reg = platform_write;
  dev_ctx.read_reg = platform_read;
  dev_ctx.mdelay = platform_delay;
  dev_ctx.handle = &SENSOR_BUS;

  // Init test platform //
  platform_init(dev_ctx.handle);
  // Wait sensor boot time //
  platform_delay(BOOT_TIME);
  // Check device ID //
  lsm6dsv16x_device_id_get(&dev_ctx, &whoamI);

  if (whoamI != LSM6DSV16X_ID)
    while (1);

  // Restore default configuration //
  lsm6dsv16x_reset_set(&dev_ctx, LSM6DSV16X_RESTORE_CTRL_REGS);
  do {
    lsm6dsv16x_reset_get(&dev_ctx, &rst);
  } while (rst != LSM6DSV16X_READY);

  // Enable Block Data Update //
  lsm6dsv16x_block_data_update_set(&dev_ctx, PROPERTY_ENABLE);

#if defined(NUCLEO_H503RB)
  // if I3C is used then INT pin must be explicitly enabled //
  lsm6dsv16x_i3c_int_en_set(&dev_ctx, 1);
#endif

  pin_int.double_tap = PROPERTY_ENABLE;
  pin_int.single_tap = PROPERTY_ENABLE;
  lsm6dsv16x_pin_int1_route_set(&dev_ctx, &pin_int);
  //lsm6dsv16x_pin_int2_route_set(&dev_ctx, &pin_int);

  irq.enable = 1;
  irq.lir = 0;
  lsm6dsv16x_interrupt_enable_set(&dev_ctx, irq);

  tap.tap_z_en = 1;
  lsm6dsv16x_tap_detection_set(&dev_ctx, tap);

  tap_ths.z = 3;
  lsm6dsv16x_tap_thresholds_set(&dev_ctx, tap_ths);

  tap_win.tap_gap = 7;
  tap_win.shock = 3;
  tap_win.quiet = 3;
  lsm6dsv16x_tap_time_windows_set(&dev_ctx, tap_win);

  lsm6dsv16x_tap_mode_set(&dev_ctx, LSM6DSV16X_BOTH_SINGLE_DOUBLE);

  // Set Output Data Rate.//
  lsm6dsv16x_xl_data_rate_set(&dev_ctx, LSM6DSV16X_ODR_AT_480Hz);
  // Set full scale //
  lsm6dsv16x_xl_full_scale_set(&dev_ctx, LSM6DSV16X_8g);

  // wait forever (6D event handle in irq handler) //
  while (1) {
    if (stap_event_catched) {
        stap_event_catched = 0;
        snprintf((char *)lsmTx_buffer, sizeof(lsmTx_buffer), "Single TAP\r\n");
        tx_com(lsmTx_buffer, strlen((char const *)lsmTx_buffer));
    }
    if (dtap_event_catched) {
        dtap_event_catched = 0;
        snprintf((char *)lsmTx_buffer, sizeof(lsmTx_buffer), "Double TAP\r\n");
        tx_com(lsmTx_buffer, strlen((char const *)lsmTx_buffer));
    }
  }
}

static int32_t platform_write(void *handle, uint8_t reg, const uint8_t *bufp, uint16_t len){
	HAL_GPIO_WritePin(IMU_CS_GPIO_Port, IMU_CS_Pin, GPIO_PIN_RESET);
	HAL_SPI_Transmit(handle, &reg, 1, 1000);
	HAL_SPI_Transmit(handle, (uint8_t*) bufp, len, 1000);
	HAL_GPIO_WritePin(IMU_CS_GPIO_Port, IMU_CS_Pin, GPIO_PIN_SET);
	return 0;
}

static int32_t platform_read(void *handle, uint8_t reg, uint8_t *bufp, uint16_t len){
	reg |= 0x80;
	HAL_GPIO_WritePin(IMU_CS_GPIO_Port, IMU_CS_Pin, GPIO_PIN_RESET);
	HAL_SPI_Transmit(handle, &reg, 1, 1000);
	HAL_SPI_Receive(handle, bufp, len, 1000);
	HAL_GPIO_WritePin(IMU_CS_GPIO_Port, IMU_CS_Pin, GPIO_PIN_SET);
	return 0;
}

int _write(int file, char *ptr, int len)
{
  (void)file;
  int DataIdx;

  for (DataIdx = 0; DataIdx < len; DataIdx++)
  {
    //__io_putchar(*ptr++);
	  ITM_SendChar(*ptr++);
  }
  return len;
}

static void tx_com(uint8_t *tx_buffer, uint16_t len){
	HAL_UART_Transmit(&huart1, tx_buffer, len, 1000);
	//printf("Hello world \n");
	int DataIdx;
	for(DataIdx=0; DataIdx < len; DataIdx++){
		ITM_SendChar(tx_buffer[DataIdx]);
	}
	ITM_SendChar('\n');
}

static void platform_delay(uint32_t ms){
	HAL_Delay(ms);
}

static void platform_init(void *handle){
	__NOP();
}
*/

/* void changeLedMode(ledModeEnum newMode, uint8_t rVal, uint8_t gVal, uint8_t bVal, uint8_t numFlashes) */
void changeLedMode(ledModeEnum newMode, uint8_t rVal, uint8_t gVal, uint8_t bVal, uint8_t numFlashes){
	rgbVal[0] = rVal, rgbVal[1] = gVal, rgbVal[2] = bVal;

	switch(newMode){
	case led_Off:
	case Led_OnSolid:
		break;
	case Led_Flash_Slow:
		ledDelayPeriod = LED_SLOW_FLASH_INTERVAL;
		break;
	case Led_Flash_Medium:
		ledDelayPeriod = LED_MEDIUM_FLASH_INTERVAL;
		break;
	case Led_Flash_Fast:
		ledDelayPeriod = LED_FAST_FLASH_INTERVAL;
		break;
	case Led_On_1Sec:
		ledDelayPeriod = 1000;
		break;
	case Led_On_5Sec:
		ledDelayPeriod = 5000;
		break;
	}

	ledFlashesRemaining = numFlashes;
	if(newMode <= Led_OnSolid){
		ledModeLastChanged = HAL_GetTick();
	}else{
		ledModeLastChanged = (HAL_GetTick() - ledDelayPeriod - 1);	//This will allow the checkLed function to immediately start the new sequence.
	}

	displayLastActivityTime = HAL_GetTick();
	ledMode = newMode;
}

void checkLed(){
	//This sub handles turning the led on/off at delayed intervals

	if(ledMode == led_Off){
		if(curLedState != false){
			curLedState = false;
			setRGBLight(0, 0, 0);
		}
	}else if(ledMode == Led_OnSolid){
		setRGBLight(rgbVal[0], rgbVal[1], rgbVal[2]);
		if(curLedState != true){
			curLedState = true;
			//setRGBLight(rgbVal[0], rgbVal[1], rgbVal[2]);
		}
	}else if(ledMode > Led_OnSolid){
		if(HAL_GetTick() - ledModeLastChanged >= ledDelayPeriod){
			if(curLedState == true){
				curLedState = false;
				setRGBLight(0, 0, 0);
				//if(ledFlashesRemaining == 0){
				//	ledMode = led_Off;
				//}
			}else{
				if(ledFlashesRemaining == 0){  //if flashesRemaining <0 then it will continuously flash.
					ledMode = led_Off;
				}else{
					//More flashes remaining.  (Either a specific count higher than zero, or a negative count which causes continuous flashes.)
					curLedState = true;
					setRGBLight(rgbVal[0], rgbVal[1], rgbVal[2]);
					if(ledFlashesRemaining > 0){
						ledFlashesRemaining--;
					}
				}
			}
			ledModeLastChanged = HAL_GetTick();
		}
	}
}

void setRGBLight(uint8_t rLevel, uint8_t gLevel, uint8_t bLevel){
	rLevel = (rLevel <= 100 ? rLevel : 100);
	gLevel = (gLevel <= 100 ? gLevel : 100);
	bLevel = (bLevel <= 100 ? bLevel : 100);
	__HAL_TIM_SET_COMPARE(phtimLed, LED_RCHAN, rLevel);
	__HAL_TIM_SET_COMPARE(phtimLed, LED_GCHAN, gLevel);
	__HAL_TIM_SET_COMPARE(phtimLed, LED_BCHAN, bLevel);
}

void usbReceive(uint8_t* Buf, uint32_t len){
	//This accepts incoming data from the USB Rx (usbd_cdc_if.c) and copies it to an incoming buffer.

	//be careful to not overflow the rx data buffer!
	if( (usbInitRxBuffIndex + len) < RXBUFFSIZE){
		memcpy(&usbInitRxBuff[usbInitRxBuffIndex], Buf, len);
		usbInitRxBuffIndex += len;
	}else{
		//Not enough room in the buffer for the entire incoming data.
		if(usbInitRxBuffIndex < (RXBUFFSIZE - 1)){
			//copy whatever data we can until the buffer is completely full.
			uint8_t tmpShortLen = RXBUFFSIZE - usbInitRxBuffIndex;
			memcpy(&usbInitRxBuff[usbInitRxBuffIndex], Buf, tmpShortLen);
			usbInitRxBuffIndex += tmpShortLen;
		}
		//set the overflow flag
		usbInitRxOverflow++;
	}
//	usbNewBytesReceived += len;
}

void checkUsbRx(){
	if(usbInitRxOverflow){
		//usb buffer overflowed.  uh oh.
		__NOP();
		usbInitRxBuffIndex = 0;
		usbInitRxOverflow = 0;
	}
	if(usbInitRxBuffIndex > 0){
		//Incoming usb data.  Parse the data and see if we have a complete messsage yet.
		char tmpChar;
		//for (uint8_t i = 0; i < radio.rxMsg.dataLen; i++){
		for (uint8_t i = 0; i < usbInitRxBuffIndex; i++){
			//tmpChar = (char)radio.rxMsg.dataBuff[i];
			tmpChar = (char)usbInitRxBuff[i];
			if(usbRxBuffIndex == 0){
				//this is the command byte.
				usbRxBuff[usbRxBuffIndex] = tmpChar;
				usbRxBuffIndex++;
			}else if( (tmpChar == '\n') || (tmpChar == '\r') ){
				newMessageReceived(usbRxBuff, usbRxBuffIndex, comChan_Usb, 0);
				usbRxBuffIndex = 0;
			}else if((tmpChar > 31) && (tmpChar < 127)){
				usbRxBuff[usbRxBuffIndex] = tmpChar;
				usbRxBuffIndex++;
				if(usbRxBuffIndex >= RXBUFFSIZE){
					usbRxBuffIndex = 0;
				}
			}
		}
		usbInitRxBuffIndex = 0;
	}
}

void checkRadioRx(){

	//Don't send an ACK here, wait until we've verified the message to be valid, THEN send an ACK or NAK.
	//if(radio.ACK_REQUESTED == true){
	//	radio.sendACK();
	//}

	//if(RFM69ReceiveDone(&radio)){
	if(RFM69ReceiveDone(&radio)){
		//uint16_t idTemp = radio.rxMsg.senderId;
		char tmpChar;
		//for (uint8_t i = 0; i < radio.rxMsg.dataLen; i++){
		for (uint8_t i = 0; i < radio.rxMsg.dataLen; i++){
			//tmpChar = (char)radio.rxMsg.dataBuff[i];
			tmpChar = (char)radio.rxMsg.dataBuff[i];
		    if(radioRxBuffIndex == 0){
		    	//this is the command byte.
		    	radioRxBuff[radioRxBuffIndex] = tmpChar;
		    	radioRxBuffIndex++;
		    }else if(tmpChar == '\n'){
		    	//Serial.print("   [RX_RSSI:");Serial.print(radio.RSSI);Serial.print("]");
		    	newMessageReceived(radioRxBuff, radioRxBuffIndex, comChan_Radio, radio.rxMsg.senderId);
		    	radioRxBuffIndex = 0;
		    }else if((tmpChar > 31) && (tmpChar < 127)){
		    	radioRxBuff[radioRxBuffIndex] = tmpChar;
		    	radioRxBuffIndex++;
		    	if(radioRxBuffIndex > 99){
		    		radioRxBuffIndex = 0;
		        }
		    	//Serial.print(tmpChar);
		    }
		}
	}
}

/*
void newMessageReceived(char* buff, uint8_t indx, comChanEnum comChannel, uint16_t sender){
	bool ackRequested = false;
	switch(comChannel){
	case comChan_radio:
		changeLedMode(Led_Flash_Fast, 0, 0, 10, 1);
		break;
	case comChan_Usb:
		changeLedMode(Led_Flash_Fast, 0, 10, 0, 1);
		break;
	case comChan_EspBt:
	case comChan_EspWifi:
	default:
		changeLedMode(Led_Flash_Fast, 0, 5, 5, 1);

	}


//	if(radio.rxMsg.ackRequested){
//		RFM69SendACK(&radio, 0, 0);
//	}



	//extract the fields of data:
	if(indx < 4) return;

	char tmpInData[70] = {0};
	memcpy(tmpInData, &buff[0],indx);	//copy buff[3]-buff[buffLen] into tmpInData[0 - end]
	char tmpData[MAX_INCOMING_CHANNELS][10];
	uint8_t tmpFieldCount = 0;
	char * pch;
	pch = strtok (tmpInData,",");
	while ((pch != NULL) && (tmpFieldCount < MAX_INCOMING_CHANNELS)){
		char tmp[10];
		sprintf(tmp,"%s",pch);
		strcpy(tmpData[tmpFieldCount], tmp);
		pch = strtok (NULL, ",");
		tmpFieldCount++;
	}

	if(tmpFieldCount == 21){ //if a different number of fields was recovered then the format was not as we were expecting.
		for(uint8_t t=0; t<tmpFieldCount; t++){
			testData[t] = atoi(tmpData[t]);
		}

		uint8_t	tmpMaxLED = 30;
		int16_t tmpRGB[3] = {0};

		for(uint8_t l=0; l<3; l++){
			tmpRGB[l] = testData[l]; //tmpRGB[l] = atoi(tmpData[l]);
			tmpRGB[l] = (tmpRGB[l] < 0) ? tmpRGB[l] / -3 : tmpRGB[l] / 3;
			tmpRGB[l] = (tmpRGB[l] > tmpMaxLED) ? tmpMaxLED : tmpRGB[l];
		}
		setRGBLight(tmpRGB[0], tmpRGB[1], tmpRGB[2]);

//TODO: Maybe move this to a header somewhere?
#define MSG_JS1X	0
#define MSG_JS1Y	1
#define MSG_JS2X	2
#define MSG_JS2Y	3
#define MSG_POT1	4
#define MSG_POT2	5
#define MSG_PB2		6
#define MSG_PB3		7
#define MSG_PB4		8
#define MSG_PB5		9
#define MSG_PB6		10
#define MSG_PB7		11
#define MSG_PB8		12
#define MSG_PB9		13
#define MSG_PB11	14
#define MSG_PB12	15
#define MSG_PB13	16
#define MSG_PB14	17
#define MSG_ENC1	18
#define MSG_ENC2	19
#define MSG_ENC4	20


		int16_t curTestVal = 0;

		//ENC4
		// Speed Multiplier
		if(testData[MSG_ENC4] != testDataLast[MSG_ENC4]){
			testDataLast[MSG_ENC4] = testData[MSG_ENC4];
			curTestVal = testData[MSG_ENC4];

			speedMultiplier = curTestVal;

			if(speedMultiplier < 0){
				speedMultiplier *= -1;
			}else if(speedMultiplier < 1){
				speedMultiplier = 1;
			}

			speedSlowOutMin = 100 / (uint16_t)(speedMultiplier);
			speedSlowOutMax = 100;
			speedFastOutMin = 100;
			speedFastOutMax = 100 * (uint16_t)(speedMultiplier);

			testDataLast[MSG_JS1X] = 0;
			testDataLast[MSG_JS1Y] = 0;
			testDataLast[MSG_JS2X] = 0;
			testDataLast[MSG_JS2Y] = 0;
		}

//		//JS1 X
//		if(testData[MSG_JS1X] != testDataLast[MSG_JS1X]){
//			testDataLast[MSG_JS1X] = testData[MSG_JS1X];
//			curTestVal = testData[MSG_JS1X];
//			if(curTestVal < 0){
//				curTestVal = (uint16_t)mapi16(curTestVal, -100, 0, speedSlowOutMin, speedSlowOutMax, true);
//			}else if(curTestVal == 0){
//				curTestVal = 100;
//			}else{
//				curTestVal = (uint16_t)mapi16(curTestVal, 0, 100, speedFastOutMin, speedFastOutMax, true);
//			}
//			TensSetSpeed(&tens, MOT_INDEX_START + 0, curTestVal);
//		}

//		//JS1 Y
//		if(testData[MSG_JS1Y] != testDataLast[MSG_JS1Y]){
//			testDataLast[MSG_JS1Y] = testData[MSG_JS1Y];
//			curTestVal = testData[MSG_JS1Y];
//			if(curTestVal < 0){
//				curTestVal = (uint16_t)mapi16(curTestVal, -100, 0, speedSlowOutMin, speedSlowOutMax, true);
//			}else if(curTestVal == 0){
//				curTestVal = 100;
//			}else{
//				curTestVal = (uint16_t)mapi16(curTestVal, 0, 100, speedFastOutMin, speedFastOutMax, true);
//			}
//			TensSetSpeed(&tens, MOT_INDEX_START + 1, curTestVal);
//		}


		//ENC1
		if(testData[MSG_ENC1] != testDataLast[MSG_ENC1]){
			testDataLast[MSG_ENC1] = testData[MSG_ENC1];
			curTestVal = testData[MSG_ENC1];

			testDataLast[MSG_JS1X] = testData[MSG_JS1X] +1;
			testDataLast[MSG_JS1Y] = testData[MSG_JS1Y] +1;
			testDataLast[MSG_JS2X] = testData[MSG_JS2X] +1;
			testDataLast[MSG_JS2Y] = testData[MSG_JS2Y] +1;
		}

		//JS1 X
		if(testData[MSG_JS1X] != testDataLast[MSG_JS1X]){
			testDataLast[MSG_JS1X] = testData[MSG_JS1X];
			curTestVal = testData[MSG_JS1X];

			switch(testData[MSG_ENC1]){
			case 0:  //motor speed
				if(curTestVal < 0){
					curTestVal = (uint16_t)mapi16(curTestVal, -100, 0, speedSlowOutMin, speedSlowOutMax, true);
				}else if(curTestVal == 0){
					curTestVal = 100;
				}else{
					curTestVal = (uint16_t)mapi16(curTestVal, 0, 100, speedFastOutMin, speedFastOutMax, true);
				}
				TensSetSpeed(&tens, MOT_INDEX_START + 0, curTestVal);
				break;
			case 1:		//motor volume
				break;
			case 2:		//tens 1 speed
				if(curTestVal < 0){
					curTestVal = (uint16_t)mapi16(curTestVal, -100, 0, minSpeed, 100, true);
				}else if(curTestVal == 0){
					curTestVal = 100;
				}else{
					curTestVal = (uint16_t)mapi16(curTestVal, 0, 100, 100, maxSpeed, true);
				}
				TensSetSpeed(&tens, TENS_INDEX_START + 0, curTestVal);
				break;
			case 3:		//tens 1 voltage
				if(curTestVal < 0){
					curTestVal = (100 + curTestVal) * testData[MSG_POT1] / 100;
				}else if(curTestVal == 0){
					curTestVal = testData[MSG_POT1];
				}else{
					curTestVal = (100 + curTestVal) * testData[MSG_POT1] / 100;
				}
				TensSetDacVal(&tens.tensChan[TENS_INDEX_START + 0], curTestVal);
				break;
			case 4:
			case 5:
			default:
				break;
			}
		}

		//JS1 Y
		if(testData[MSG_JS1Y] != testDataLast[MSG_JS1Y]){
			testDataLast[MSG_JS1Y] = testData[MSG_JS1Y];
			curTestVal = testData[MSG_JS1Y];
			switch(testData[MSG_ENC1]){
			case 0:  //motor speed
				if(curTestVal < 0){
					curTestVal = (uint16_t)mapi16(curTestVal, -100, 0, speedSlowOutMin, speedSlowOutMax, true);
				}else if(curTestVal == 0){
					curTestVal = 100;
				}else{
					curTestVal = (uint16_t)mapi16(curTestVal, 0, 100, speedFastOutMin, speedFastOutMax, true);
				}
				TensSetSpeed(&tens, MOT_INDEX_START + 1, curTestVal);
				break;
			case 1:		//motor volume
				break;
			case 2:		//tens 1 speed
				if(curTestVal < 0){
					curTestVal = (uint16_t)mapi16(curTestVal, -100, 0, minSpeed, 100, true);
				}else if(curTestVal == 0){
					curTestVal = 100;
				}else{
					curTestVal = (uint16_t)mapi16(curTestVal, 0, 100, 100, maxSpeed, true);
				}
				TensSetSpeed(&tens, TENS_INDEX_START + 1, curTestVal);
				break;
			case 3:		//tens 1 voltage
				if(curTestVal < 0){
					curTestVal = (100 + curTestVal) * testData[MSG_POT2] / 100;
				}else if(curTestVal == 0){
					curTestVal = testData[MSG_POT2];
				}else{
					curTestVal = (100 + curTestVal) * testData[MSG_POT2] / 100;
				}
				TensSetDacVal(&tens.tensChan[TENS_INDEX_START + 1], curTestVal);
				break;
			case 4:
			case 5:
			default:
				break;
			}
		}

		//JS2 X
		if(testData[MSG_JS2X] != testDataLast[MSG_JS2X]){
			testDataLast[MSG_JS2X] = testData[MSG_JS2X];
			curTestVal = testData[MSG_JS2X];
			if(testData[MSG_ENC1] != 2){
				if(curTestVal < 0){
					curTestVal = (uint16_t)mapi16(curTestVal, -100, 0, minSpeed, 100, true);
				}else if(curTestVal == 0){
					curTestVal = 100;
				}else{
					curTestVal = (uint16_t)mapi16(curTestVal, 0, 100, 100, maxSpeed, true);
				}
				TensSetSpeed(&tens, TENS_INDEX_START + 0, curTestVal);
			}
		}

		//JS2 Y
		if(testData[MSG_JS2Y] != testDataLast[MSG_JS2Y]){
			testDataLast[MSG_JS2Y] = testData[MSG_JS2Y];
			curTestVal = testData[MSG_JS2Y];
			if(testData[MSG_ENC1] != 2){
				if(curTestVal < 0){
					curTestVal = (uint16_t)mapi16(curTestVal, -100, 0, minSpeed, 100, true);
				}else if(curTestVal == 0){
					curTestVal = 100;
				}else{
					curTestVal = (uint16_t)mapi16(curTestVal, 0, 100, 100, maxSpeed, true);
				}
				TensSetSpeed(&tens, TENS_INDEX_START + 1, curTestVal);
			}
		}


		//POT1
		if(testData[MSG_POT1] != testDataLast[MSG_POT1]){
			//testDataLast[MSG_POT1] = testData[MSG_POT1];
			curTestVal = testData[MSG_POT1];
			//if(curTestVal < 50){
			//	curTestVal = (uint16_t)mapi16(curTestVal, 0, 50, 10, 100, true);
			//}else if(curTestVal == 50){
			//	curTestVal = 100;
			//}else{
			//	curTestVal = (uint16_t)mapi16(curTestVal, 50, 100, 100, 1000, true);
			//}
			//TensSetSpeed(&tens, NUM_CHANNELS, curTestVal);
			if( (testData[MSG_ENC1] != 3) ||
					((testData[MSG_ENC1] == 3) && (testData[MSG_JS1X] == 0)) ){
				testDataLast[MSG_POT1] = testData[MSG_POT1];
				TensSetDacVal(&tens.tensChan[2], curTestVal);
			}
		}

		//POT2
		if(testData[MSG_POT2] != testDataLast[MSG_POT2]){
			//testDataLast[MSG_POT2] = testData[MSG_POT2];
			curTestVal = testData[MSG_POT2];

			//Tens Voltage
			//TensSetDacVal(&tens.tensChan[2], curTestVal);
			if( (testData[MSG_ENC1] != 3) ||
								((testData[MSG_ENC1] == 3) && (testData[MSG_JS1Y] == 0)) ){
				testDataLast[MSG_POT2] = testData[MSG_POT2];
				TensSetDacVal(&tens.tensChan[3], curTestVal);
			}
		}


		//PB2
		if(testData[MSG_PB2] != testDataLast[MSG_PB2]){
			testDataLast[MSG_PB2] = testData[MSG_PB2];
			curTestVal = testData[MSG_PB2];

			// Enable chan 0
			if(curTestVal == 1){
				TensSetChannelEnable(&tens.tensChan[0], true);
			}else if(curTestVal == 0){
				TensSetChannelEnable(&tens.tensChan[0], false);
			}

		}


		//PB3
		if(testData[MSG_PB3] != testDataLast[MSG_PB3]){
			testDataLast[MSG_PB3] = testData[MSG_PB3];
			curTestVal = testData[MSG_PB3];

			// Enable chan 1
			if(curTestVal == 1){
				TensSetChannelEnable(&tens.tensChan[1], true);
			}else if(curTestVal == 0){
				TensSetChannelEnable(&tens.tensChan[1], false);
			}
		}

		//PB4
		if(testData[MSG_PB4] != testDataLast[MSG_PB4]){
			testDataLast[MSG_PB4] = testData[MSG_PB4];
			curTestVal = testData[MSG_PB4];

			// Enable chan 2
			if(curTestVal == 1){
				TensSetChannelEnable(&tens.tensChan[2], true);
			}else if(curTestVal == 0){
				TensSetChannelEnable(&tens.tensChan[2], false);
			}
		}

		//PB5
		if(testData[MSG_PB5] != testDataLast[MSG_PB5]){
			testDataLast[MSG_PB5] = testData[MSG_PB5];
			curTestVal = testData[MSG_PB5];

			// Enable chan 3
			if(curTestVal == 1){
				TensSetChannelEnable(&tens.tensChan[3], true);
			}else if(curTestVal == 0){
				TensSetChannelEnable(&tens.tensChan[3], false);
			}
		}

		//PB6
		if(testData[MSG_PB6] != testDataLast[MSG_PB6]){
			testDataLast[MSG_PB6] = testData[MSG_PB6];
			curTestVal = testData[MSG_PB6];
			TensvBoostEnable(&tens, (curTestVal == 1) ? true : false);

			//Start or stop programs
			if(curTestVal == 1){
				TensStart(&tens, 0);
				TensStart(&tens, 1);
				TensStart(&tens, 2);
				TensStart(&tens, 3);
			}else if(curTestVal == 0){
				TensStop(&tens, NUM_CHANNELS, progState_Paused);
			}
		}


		//PB7
		if(testData[MSG_PB7] != testDataLast[MSG_PB7]){
			testDataLast[MSG_PB7] = testData[MSG_PB7];
			curTestVal = testData[MSG_PB7];

		}

		//PB8
		if(testData[MSG_PB8] != testDataLast[MSG_PB8]){
			testDataLast[MSG_PB8] = testData[MSG_PB8];
			curTestVal = testData[MSG_PB8];

		}

		//PB9
		if(testData[MSG_PB9] != testDataLast[MSG_PB9]){
			testDataLast[MSG_PB9] = testData[MSG_PB9];
			curTestVal = testData[MSG_PB9];

		}

//		//PB10
//		if(testData[MSG_PB10] != testDataLast[MSG_PB10]){
//			testDataLast[MSG_PB10] = testData[MSG_PB10];
//			curTestVal = testData[MSG_PB10];
//
//		}

		//PB11
		if(testData[MSG_PB11] != testDataLast[MSG_PB11]){
			testDataLast[MSG_PB11] = testData[MSG_PB11];
			curTestVal = testData[MSG_PB11];

		}

		//PB12
		if(testData[MSG_PB12] != testDataLast[MSG_PB12]){
			testDataLast[MSG_PB12] = testData[MSG_PB12];
			curTestVal = testData[MSG_PB12];

		}

		//PB13
		if(testData[MSG_PB13] != testDataLast[MSG_PB13]){
			testDataLast[MSG_PB13] = testData[MSG_PB13];
			curTestVal = testData[MSG_PB13];

		}

		//PB14
		if(testData[MSG_PB14] != testDataLast[MSG_PB14]){
			testDataLast[MSG_PB14] = testData[MSG_PB14];
			curTestVal = testData[MSG_PB14];

		}


//		//ENC1
//		if(testData[MSG_ENC1] != testDataLast[MSG_ENC1]){
//			testDataLast[MSG_ENC1] = testData[MSG_ENC1];
//			curTestVal = testData[MSG_ENC1];
//
//		}

		//ENC2
		if(testData[MSG_ENC2] != testDataLast[MSG_ENC2]){
			testDataLast[MSG_ENC2] = testData[MSG_ENC2];
			curTestVal = testData[MSG_ENC2];

		}

//		//ENC3
//		if(testData[MSG_ENC3] != testDataLast[MSG_ENC3]){
//			testDataLast[MSG_ENC3] = testData[MSG_ENC3];
//			curTestVal = testData[MSG_ENC3];
//
//		}



		//ENC5
//		if(testData[MSG_ENC5] != testDataLast[MSG_ENC5]){
//			testDataLast[MSG_ENC5] = testData[MSG_ENC5];
//			curTestVal = testData[MSG_ENC5];
//
//		}


	}
}
*/

uint16_t StringToI32Array(char *src, uint16_t srcLen, const char *delim, int32_t *dest, uint8_t maxFields){
	//accept a string, split it by the delim, then convert it to an array of int32 values.
	//Return the number of fields extracted.
	if(maxFields == 0) maxFields = 50;

	char tmpInData[200] = {0};
	//char *tmpInData = (char*)calloc(srcLen+1, sizeof(char));
	memcpy(tmpInData, src, srcLen);
	char tmpData[50][10];
	//char (*tmpData)[10];
	//tmpData = calloc(maxFields, sizeof(*tmpData));

	uint8_t tmpFieldCount = 0;
	char * pch;
	//pch = strtok (tmpInData,",");
	pch = strtok(tmpInData, delim);
	while ((pch != NULL) && (tmpFieldCount < maxFields)){
		char tmp[10];
		sprintf(tmp,"%s",pch);
		strcpy(tmpData[tmpFieldCount], tmp);
		pch = strtok(NULL, ",");
		dest[tmpFieldCount] = atoi(tmpData[tmpFieldCount]);
		tmpFieldCount++;
	}

	//free(tmpInData);
	//free(tmpData);

	return tmpFieldCount;
}


void newMessageReceived(char* buff, uint8_t indx, comChanEnum comChannel, uint16_t sender){
	//Msg format: [cmd char][main param char][opt Index num 1 (ascii number string)] [,opt Index num 2] [,...] [=] [value array]
#define MAX_MSG_PARAMS 10
#define MAX_DATA_PARAMS 20

	char tmpCommand = buff[0];

	bool msgValid = false;
	bool ackRequested = ( (tmpCommand >= 'B') && (tmpCommand <= 'Q') );

	if( (comChannel == comChan_Radio) && (radio.rxMsg.ackRequested) ){
		RFM69SendACK(&radio, 0, 0);
		ackRequested = false;
	}

	switch(tmpCommand){
	case commandType_GetSingleParam:
		//msgValid = sendParameter(tmpCommand, tmpParam, comChannel, sender);
		msgValid = sendParameter(buff, indx, comChannel, sender);
		break;
	case commandType_GetParamArray:
		//msgValid = sendParameterArray(tmpCommand, tmpParam, comChannel, sender);
		msgValid = sendParameterArray(buff, indx, comChannel, sender);
		break;
	case commandType_SetParamArray:
		msgValid = setParameterArray(buff, indx, comChannel, sender);
		break;
	case commandType_SetSingleParam:
		msgValid = setParameter(buff, indx, comChannel, sender);
		break;
	case commandType_GetFileData:
//TODO: finish this
		msgValid = false;
		break;
	case commandType_SetFileData:
//TODO: finish this
		msgValid = false;
		break;
	case commandType_Reset:
		if( (buff[0]='=') && (buff[1]='Y') && (buff[2]='E') && (buff[3]='S') ){
			NVIC_SystemReset();
		}
		break;
	case commandType_DeleteTensProg:
		if( (buff[0]='=') && (buff[1]='Y') && (buff[2]='E') && (buff[3]='S') ){
			TensPrepFlashForDownload(&tens);
			TensWriteFileAllocationTable(&tens, 0, 0);
			TensReadFileAllocationTable(&tens);
		}
		break;
	case commandType_DebugPause:
		tens.debugMode = 1;
		tens.debugPendingStep = 0;
		msgValid = true; //SendACK();
		break;

	case commandType_DebugResume:
		tens.debugMode = 0;
		tens.debugPendingStep = 0;
		msgValid = true; //SendACK();
		break;

	case commandType_DebugStep:
		if (tens.debugMode) {
			tens.debugPendingStep = 1;  // Will execute one line on next tick
		}
		msgValid = true; //SendACK();
		break;

	case commandType_DebugSetBreakpoint:
		break;
	case commandType_DebugClearBreakpoint:
		break;
	default:
		msgValid = false;
	}

	if(msgValid){
		switch(comChannel){
		case comChan_Radio:
			changeLedMode(Led_Flash_Fast, 0, 0, 10, 1);
			break;
		case comChan_Usb:
			changeLedMode(Led_Flash_Fast, 0, 10, 0, 1);
			break;
		case comChan_EspBt:
		case comChan_EspWifi:
		default:
			changeLedMode(Led_Flash_Fast, 0, 5, 5, 1);
		}
		if(ackRequested){
			sendAck(comChannel, sender);
		}
	}else{
		changeLedMode(Led_Flash_Fast, 10, 0, 0, 3);
		if(ackRequested){
			sendNack(comChannel, sender);
		}
	}

}

void sendAck(comChanEnum comChannel, uint16_t sender){
	char dataBuff[1] = {0};
	uint8_t len = 0;
	dataBuff[0] = commandType_ACK;
	dataBuff[1] = '\n';
	dataSend(dataBuff, len+2, comChannel, sender, false);
}

void sendNack(comChanEnum comChannel, uint16_t sender){
	char dataBuff[1] = {0};
	uint8_t len = 0;
	dataBuff[0] = commandType_NAK;
	dataBuff[1] = '\n';
	dataSend(dataBuff, len+2, comChannel, sender, false);
}

void dataSend(char* buff, uint16_t len, comChanEnum channel, uint16_t target, bool requestAck){
	switch(channel){
	case comChan_Radio:
		if(requestAck){
			RFM69SendWithRetry(&radio, target, buff, len, 1, RFM69_ACK_TIMEOUT);
		}else{
			RFM69Send(&radio, target, buff, len, false);
		}
		break;
	case comChan_Usb:
		CDC_Transmit_FS((uint8_t *)buff, len);
		break;
	case comChan_EspBt:
		break;
	case comChan_EspWifi:
		break;
	default:
		break;
	}
}

bool setParameter(char* buff, uint8_t indx, comChanEnum comChannel, uint16_t sender){
	//char tmpCommand = buff[0];
	char tmpParam = buff[1];
	bool retVal = false;
	//char tmpChanIndex = buff[2];
	//int8_t chanIndex = -1; //buff[2]; //leave this as a char, just in case it's already less than ascii '0' (48)
	//if( (tmpChanIndex >= '0') && (tmpChanIndex <= ((NUM_CHANNELS + NUM_SYS_PROGS) + '0' )) ){
	//	chanIndex = tmpChanIndex - '0';
	//}

	uint8_t valStartIndex = 0;
	for(uint8_t e=2; e<indx; e++){
		if(buff[e] == '='){
			valStartIndex = e+1;	//valStartIndex is the index of the first char of the value section
			break;
		}
	}

	int16_t chanIndex = -1;
	if(valStartIndex > 2){
		char tmpChanIndex[10] = {0};
		for(uint8_t n=2; n<valStartIndex; n++){
			tmpChanIndex[n-2] = buff[n];
		}
		chanIndex = atoi(tmpChanIndex);
	}
	char tmpVal[20] = {0};
	for(uint8_t n =valStartIndex; n<indx; n++){
		tmpVal[n-valStartIndex] = buff[n];
	}
	int32_t newVal = atoi(tmpVal);

	switch(tmpParam){
	case pStat_ProgNumber:

		break;
	case pStat_ChanEnabled:
		//if( ((chanIndex >= '0') && (chanIndex <= (NUM_CHANNELS + 1)+ '0' )) && (buff[3] == '=') ){
		//	chanIndex -= '0';
		if( (chanIndex >= 0) && (chanIndex < NUM_CHANNELS) ){
			bool tmpEnVal = (buff[4] == '1') ? true : false;
			if(chanIndex == 0){
				TensvBoostEnable(&tens, tmpEnVal);
			}
			TensEnableChannel(&tens, chanIndex, tmpEnVal);
			retVal = true;
		}
		break;
	case pStat_CurLineNum:

		break;
	case pStat_ChanCurPWidthPct:

		break;
	case pStat_ChanCurIntensityPct:
		if( (chanIndex >= 0) && (chanIndex < NUM_CHANNELS) ){
			if(tens.tensChan[chanIndex].chanType == chanType_Tens){
				if( (newVal >= 0) && (newVal <=100) ){
					//TensSetDacVal(&tens.tensChan[chanIndex], (uint16_t)newVal);
					//TensSetDacVal(&tens, chanIndex, (uint16_t)newVal);
					TensSetTensIntensity(&tens, chanIndex, (uint16_t)newVal);
				}
			}
		}
		break;
	case pStat_CurSpeed:
		if( (chanIndex >= 0) && (chanIndex <= NUM_CHANNELS) ){
			if(comChannel == comChan_Radio){
				//value will range from -100 (slowest) to 100 (fastest) with 0 being normal speed.
				if(newVal < 0){
					newVal = (uint16_t)mapi16(newVal, -100, 0, speedSlowOutMin, speedSlowOutMax, true);
				}else if(newVal == 0){
					newVal = 100;
				}else{
					newVal = (uint16_t)mapi16(newVal, 0, 100, speedFastOutMin, speedFastOutMax, true);
				}
				TensSetSpeed(&tens, chanIndex, newVal);
			}else if(comChannel == comChan_Usb){
				//value already formatted, ranges from minSpeed to maxSpeed.
				TensSetSpeed(&tens, chanIndex, newVal);
			}

		}
		break;
	case pStat_MinIntensity:
		if( (chanIndex >= 0) && (chanIndex < NUM_CHANNELS) ){
			if( (newVal >= 0) && (newVal <=100) ){
				tens.tensChan[chanIndex].intensityMin = (uint8_t)newVal;
			}
		}
		break;
	case pStat_MaxIntensity:
		if( (chanIndex >= 0) && (chanIndex < NUM_CHANNELS) ){
			if( (newVal >= 0) && (newVal <=100) ){
				tens.tensChan[chanIndex].intensityMax = (uint8_t)newVal;
			}
		}
		break;
	case pStat_SwapPolarity:
		if( (chanIndex >= 0) && (chanIndex < NUM_CHANNELS) ){
			tens.tensChan[chanIndex].polaritySwapped = ((uint8_t)newVal == 0 ? false : true);
			TensSetPolarity(&tens.tensChan[chanIndex], tens.tensChan[chanIndex].polarity);
		}
		break;
	case pStat_MaxOutputPulsewidthPct:
		if( (chanIndex >= 0) && (chanIndex < NUM_CHANNELS) ){
			if( (newVal >= 0) && (newVal <=100) ){
				tens.tensChan[chanIndex].outputMaxPct = (uint8_t)newVal;
			}
		}
		break;
	case pStat_NumPrograms:
		//A remote device is initiating a program download to this device.
		//Stop all channels and prepare for the download.

		//Overview of downloading a program to this device:
		// The remote device (Host) controls the process.  It initiates the download and sends all the data to us.
		//    Once this device (client) receives data, it responds with an ACK or NAK.
		// 1.	The host sends us a single parameter msg (pStat_NumPrograms) that includes the number of programs it'll be sending to us.
		//			When this device receives that message, we end up at this point right here.  We will clear & size the dlNumProgLines array
		//			for the incoming data, then send an ACK.
		// 	--Start of Loop 1--
		// 2.	The host will send a single param msg (pStat_ProgramLength) with a program number as the param index.  We will clear & size the
		//			dlProgLines array to accept the lines that will be sent in later messages.
		// 2a.	If this msg is for the first program (prog 0) then we will also clear the flash area for program storage.
		// 2b.	Loop for all remaining programs.
		//	--End of Loop 1--
		//	--Start of Prog Download Loop--
		// 3. 	The host will send a single param msg (pStat_ProgramName) with a program number as the param index.  We will store this in
		//			the dlProgName array.
		//	--Start of Prog Line Data Loop--
		// 4a. 	The host will then send a paramArray msg (pStat_ProgLineData) for each line in the current program.
		// 4b.	When the last line of the current program is received, we will write that program to flash.
		//	--End of Prog Line Data Loop--
		// 4. 	When the last line of the last program has been written to flash, we will write the FAT table to flash.
		//	--End of Prog Download Loop--
		// 5. 	After the FAT table has been written, we will read the FAT and load the default program into the master channel (0).


		if(newVal < 1){
			sendNack(comChannel, sender);
		}else{
			TensStop(&tens, 0, progState_Stopped);
			//DownloadProjectFromRemoteHost
			dlNumProgFiles = (uint16_t)newVal;

			//Create enough storage for the number of programs specified:
			//free(pDLNumProgramLines);
			memset(&dlNumProgramLines, 0, TENS_PROGRAMS_MAX * sizeof(dlNumProgramLines[0]));
			//pDLNumProgramLines = (uint16_t*)calloc( dlNumProgFiles, sizeof(dlNumProgFiles) );
			//free(dlProgStartAddr);
			//dlProgStartAddr = (uint32_t*)calloc(dlNumProgFiles, sizeof(uint32_t));
			memset(&dlProgStartAddr, 0, TENS_PROGRAMS_MAX * sizeof(dlProgStartAddr[0]));

//			free(dlProgName);
//			dlProgName = calloc(dlNumProgFiles, sizeof(*dlProgName) );
//			if(chanIndex == 0){
			//Clear the flash storage for the program download:
			TensPrepFlashForDownload(&tens);
//			}

			sendAck(comChannel, sender);  //To cancel the operation, send a NAK instead of an ACK
		}
		break;

	case pStat_ProgramLength:
		//The remote device will send a separate setParam message for each program line, each message containing
		//the length of one particular program.
		if( (chanIndex >= dlNumProgFiles) || (newVal < 0) ){
			sendNack(comChannel, sender);
		}else{
			//grab the number of lines for the current program (chanIndex)
			dlNumProgramLines[chanIndex] = newVal;

			//calculate the program start address:
			if(chanIndex == 0){
				dlProgStartAddr[0] = tens.flashStartAddress_Progs + TensCalculateFatSize(dlNumProgFiles);
			}else{
				//This is not the first program, so we can expect to have already received data for the previous program.
				dlProgStartAddr[chanIndex] = dlProgStartAddr[chanIndex-1] + TensCalculateProgramSize(dlNumProgramLines[chanIndex-1]);
			}

			////allocate enough memory for all the program lines for this program, and grab a pointer to the memory location
			//structProgLine	*tmpProgLines = calloc(newVal, sizeof(structProgLine));
			//dlPrograms[chanIndex] = tmpProgLines;

//			//If this message contains the length of the last program in the list then prep the progLine storage array
//			if(chanIndex == dlNumProgFiles - 1){
//				//calculate the total number of lines expected.
//				uint16_t totLines = 0;
//				for(uint16_t n=0; n<dlNumProgFiles; n++){
//					totLines += pDLNumProgramLines[n];
//				}
//				dlProgramLines = (structProgLine*)calloc(totLines, sizeof(structProgLine));

//			}
			sendAck(comChannel, sender);
		}
		break;
	case pStat_ProgramName:
		//The remote device will send a separate setParam message for each program name, each message containing
		//the name of progNum(chanIndex).
		if(chanIndex >= dlNumProgFiles){
			sendNack(comChannel, sender);
		}else{
			//store the program name
			memset(dlProgName, 0, TENS_FILENAME_LEN);
			for(uint8_t n=0; n<TENS_FILENAME_LEN; n++){
//				dlProgName[chanIndex][n] = buff[n+valStartIndex];
				dlProgName[n] = buff[n+valStartIndex];
				if(buff[n+valStartIndex] == 0){
					break;
				}
			}

			//Immediately following this message, we can expect the remote Host to start sending us program line data messages.
			//Prep the storage for this program.
			//free(dlProgramLines);
			//dlProgramLines = (structProgLine*)calloc(pDLNumProgramLines[chanIndex], sizeof(structProgLine));
			memset(&dlProgramLines, 0, TENS_MAX_PROGRAM_LINES * sizeof(dlProgramLines[0]));
			dlCurProg = chanIndex;

			sendAck(comChannel, sender);
		}
		break;

	default:
		break;
	}


	return retVal;
}


bool setParameterArray(char* buff, uint8_t indx, comChanEnum comChannel, uint16_t sender){
	if(buff[0] != commandType_SetParamArray){
		return false;
	}

	//The buffer for this message will be:
	//  [0: commandType_SetParamArray] [1: pArray_X] [opt param1][opt ',' & param2] = [val 0] , [val 1] , ... [val Z]

	//Find the '='
	uint8_t valStartIndex = 0;
	for(uint8_t e=2; e<indx; e++){
		if(buff[e] == '='){
			valStartIndex = e+1;	//valStartIndex is the index of the first char of the value section
			break;
		}
	}
	if( (valStartIndex < 2) || (valStartIndex >= indx) ) return false;
	uint8_t paramArrayNum = buff[1];	//0;

	//See if there are any optional params included after the pArray num:
	int32_t optParam[10] = {0};
	uint16_t optParamCount = 0;
	if(valStartIndex > 3){
		char optParamString[20] = {0};
		uint16_t optParamStringLen = 0;
		for(uint8_t n=2; n<valStartIndex-1; n++){
			optParamString[n-2] = buff[n];
			optParamStringLen++;
		}
		optParamCount = StringToI32Array(optParamString, optParamStringLen, "," , optParam, 20);
	}

	//Extract the paramArray data from the right side of the "="
	int32_t i32Data[MAX_INCOMING_CHANNELS] = {0};
	uint16_t dataCount = StringToI32Array(&buff[valStartIndex], indx - valStartIndex, ",", i32Data,  MAX_INCOMING_CHANNELS);


	switch(paramArrayNum){
	case pArray_Unknown:

		break;
	case pArray_DeviceInfo:
		return SetParamArray1Data(i32Data, dataCount);
		break;
	case pArray_ChanMinMax:

		break;
	case pArray_ChanStats:

		break;
	//case pArray_SysStats:
//
	//	break;
	case pArray_ProgLineData:
		if(optParamCount == 2){
			if(SetParamArray_ProgLineData(optParam, i32Data, dataCount) == true){
				sendAck(comChannel, sender);
			}else{
				sendNack(comChannel, sender);
			}
		}else{
			return false;
		}
		break;
	default:
		break;
	}

	return false;
}

bool SetParamArray1Data(int32_t *newData,  uint8_t dataCount){
	int16_t curTestVal = 0;
	//if( (paramArrayNum == 1) && (tmpFieldCount != 21) ){ //if a different number of fields was recovered then the format was not as we were expecting.
	if(dataCount != 21){ //if a different number of fields was recovered then the format was not as we were expecting.
		return false;
	}

	for(uint8_t t=0; t<dataCount; t++){
		//paData[t] = atoi(tmpData[t]);
		paData[t] = (int16_t)newData[t];
	}


	//ENC4
	// Speed Multiplier
	if(paData[PA1_ENC4] != paDataLast[PA1_ENC4]){
		paDataLast[PA1_ENC4] = paData[PA1_ENC4];
		curTestVal = paData[PA1_ENC4];

		speedMultiplier = curTestVal;

		if(speedMultiplier < 0){
			speedMultiplier *= -1;
		}else if(speedMultiplier < 1){
			speedMultiplier = 1;
		}

		speedSlowOutMin = 100 / (uint16_t)(speedMultiplier);
		speedSlowOutMax = 100;
		speedFastOutMin = 100;
		speedFastOutMax = 100 * (uint16_t)(speedMultiplier);

		paDataLast[PA1_JS1X] = 0;
		paDataLast[PA1_JS1Y] = 0;
		paDataLast[PA1_JS2X] = 0;
		paDataLast[PA1_JS2Y] = 0;
	}

//		//JS1 X
//		if(testData[MSG_JS1X] != testDataLast[MSG_JS1X]){
//			testDataLast[MSG_JS1X] = testData[MSG_JS1X];
//			curTestVal = testData[MSG_JS1X];
//			if(curTestVal < 0){
//				curTestVal = (uint16_t)mapi16(curTestVal, -100, 0, speedSlowOutMin, speedSlowOutMax, true);
//			}else if(curTestVal == 0){
//				curTestVal = 100;
//			}else{
//				curTestVal = (uint16_t)mapi16(curTestVal, 0, 100, speedFastOutMin, speedFastOutMax, true);
//			}
//			TensSetSpeed(&tens, MOT_INDEX_START + 0, curTestVal);
//		}

//		//JS1 Y
//		if(testData[MSG_JS1Y] != testDataLast[MSG_JS1Y]){
//			testDataLast[MSG_JS1Y] = testData[MSG_JS1Y];
//			curTestVal = testData[MSG_JS1Y];
//			if(curTestVal < 0){
//				curTestVal = (uint16_t)mapi16(curTestVal, -100, 0, speedSlowOutMin, speedSlowOutMax, true);
//			}else if(curTestVal == 0){
//				curTestVal = 100;
//			}else{
//				curTestVal = (uint16_t)mapi16(curTestVal, 0, 100, speedFastOutMin, speedFastOutMax, true);
//			}
//			TensSetSpeed(&tens, MOT_INDEX_START + 1, curTestVal);
//		}


	//ENC1
	if(paData[PA1_ENC1] != paDataLast[PA1_ENC1]){
		paDataLast[PA1_ENC1] = paData[PA1_ENC1];
		curTestVal = paData[PA1_ENC1];

		paDataLast[PA1_JS1X] = paData[PA1_JS1X] +1;
		paDataLast[PA1_JS1Y] = paData[PA1_JS1Y] +1;
		paDataLast[PA1_JS2X] = paData[PA1_JS2X] +1;
		paDataLast[PA1_JS2Y] = paData[PA1_JS2Y] +1;
	}

	//JS1 X
	if(paData[PA1_JS1X] != paDataLast[PA1_JS1X]){
		paDataLast[PA1_JS1X] = paData[PA1_JS1X];
		curTestVal = paData[PA1_JS1X];

		switch(paData[PA1_ENC1]){
		case 0:  //motor speed
			if(curTestVal < 0){
				curTestVal = (uint16_t)mapi16(curTestVal, -100, 0, speedSlowOutMin, speedSlowOutMax, true);
			}else if(curTestVal == 0){
				curTestVal = 100;
			}else{
				curTestVal = (uint16_t)mapi16(curTestVal, 0, 100, speedFastOutMin, speedFastOutMax, true);
			}
			TensSetSpeed(&tens, MOT_INDEX_START + 0, curTestVal);
			break;
		case 1:		//motor volume
			break;
		case 2:		//tens 1 speed
			if(curTestVal < 0){
				curTestVal = (uint16_t)mapi16(curTestVal, -100, 0, tens.tensChan[TENS_INDEX_START + 0].chanMinSpeed, 100, true); //curTestVal = (uint16_t)mapi16(curTestVal, -100, 0, minSpeed, 100, true);
			}else if(curTestVal == 0){
				curTestVal = 100;
			}else{
				curTestVal = (uint16_t)mapi16(curTestVal, 0, 100, 100, tens.tensChan[TENS_INDEX_START + 0].chanMaxSpeed, true);
			}
			TensSetSpeed(&tens, TENS_INDEX_START + 0, curTestVal);
			break;
		case 3:		//tens 1 voltage
			if(curTestVal < 0){
				curTestVal = (100 + curTestVal) * paData[PA1_POT1] / 100;
			}else if(curTestVal == 0){
				curTestVal = paData[PA1_POT1];
			}else{
				curTestVal = (100 + curTestVal) * paData[PA1_POT1] / 100;
			}
			//TensSetDacVal(&tens.tensChan[TENS_INDEX_START + 0], curTestVal);
			//TensSetDacVal(&tens, (TENS_INDEX_START + 0), curTestVal);
			TensSetTensIntensity(&tens, (TENS_INDEX_START + 0), curTestVal);
			break;
		case 4:
		case 5:
		default:
			break;
		}
	}

	//JS1 Y
	if(paData[PA1_JS1Y] != paDataLast[PA1_JS1Y]){
		paDataLast[PA1_JS1Y] = paData[PA1_JS1Y];
		curTestVal = paData[PA1_JS1Y];
		switch(paData[PA1_ENC1]){
		case 0:  //motor speed
			if(curTestVal < 0){
				curTestVal = (uint16_t)mapi16(curTestVal, -100, 0, speedSlowOutMin, speedSlowOutMax, true);
			}else if(curTestVal == 0){
				curTestVal = 100;
			}else{
				curTestVal = (uint16_t)mapi16(curTestVal, 0, 100, speedFastOutMin, speedFastOutMax, true);
			}
			TensSetSpeed(&tens, MOT_INDEX_START + 1, curTestVal);
			break;
		case 1:		//motor volume
			break;
		case 2:		//tens 1 speed
			if(curTestVal < 0){
				curTestVal = (uint16_t)mapi16(curTestVal, -100, 0, tens.tensChan[TENS_INDEX_START + 1].chanMinSpeed, 100, true);
			}else if(curTestVal == 0){
				curTestVal = 100;
			}else{
				curTestVal = (uint16_t)mapi16(curTestVal, 0, 100, 100, tens.tensChan[TENS_INDEX_START + 1].chanMaxSpeed, true);
			}
			TensSetSpeed(&tens, TENS_INDEX_START + 1, curTestVal);
			break;
		case 3:		//tens 1 voltage
			if(curTestVal < 0){
				curTestVal = (100 + curTestVal) * paData[PA1_POT2] / 100;
			}else if(curTestVal == 0){
				curTestVal = paData[PA1_POT2];
			}else{
				curTestVal = (100 + curTestVal) * paData[PA1_POT2] / 100;
			}
			//TensSetDacVal(&tens.tensChan[TENS_INDEX_START + 1], curTestVal);
			//TensSetDacVal(&tens, (TENS_INDEX_START + 1), curTestVal);
			TensSetTensIntensity(&tens, (TENS_INDEX_START + 1), curTestVal);
			break;
		case 4:
		case 5:
		default:
			break;
		}
	}

	//JS2 X
	if(paData[PA1_JS2X] != paDataLast[PA1_JS2X]){
		paDataLast[PA1_JS2X] = paData[PA1_JS2X];
		curTestVal = paData[PA1_JS2X];
		if(paData[PA1_ENC1] != 2){
			if(curTestVal < 0){
				curTestVal = (uint16_t)mapi16(curTestVal, -100, 0, tens.tensChan[TENS_INDEX_START + 0].chanMinSpeed, 100, true);
			}else if(curTestVal == 0){
				curTestVal = 100;
			}else{
				curTestVal = (uint16_t)mapi16(curTestVal, 0, 100, 100, tens.tensChan[TENS_INDEX_START + 0].chanMaxSpeed, true);
			}
			TensSetSpeed(&tens, TENS_INDEX_START + 0, curTestVal);
		}
	}

	//JS2 Y
	if(paData[PA1_JS2Y] != paDataLast[PA1_JS2Y]){
		paDataLast[PA1_JS2Y] = paData[PA1_JS2Y];
		curTestVal = paData[PA1_JS2Y];
		if(paData[PA1_ENC1] != 2){
			if(curTestVal < 0){
				curTestVal = (uint16_t)mapi16(curTestVal, -100, 0, tens.tensChan[TENS_INDEX_START + 1].chanMinSpeed, 100, true);
			}else if(curTestVal == 0){
				curTestVal = 100;
			}else{
				curTestVal = (uint16_t)mapi16(curTestVal, 0, 100, 100, tens.tensChan[TENS_INDEX_START + 1].chanMaxSpeed, true);
			}
			TensSetSpeed(&tens, TENS_INDEX_START + 1, curTestVal);
		}
	}


	//POT1
	if(paData[PA1_POT1] != paDataLast[PA1_POT1]){
		//testDataLast[MSG_POT1] = testData[MSG_POT1];
		curTestVal = paData[PA1_POT1];
		//if(curTestVal < 50){
		//	curTestVal = (uint16_t)mapi16(curTestVal, 0, 50, 10, 100, true);
		//}else if(curTestVal == 50){
		//	curTestVal = 100;
		//}else{
		//	curTestVal = (uint16_t)mapi16(curTestVal, 50, 100, 100, 1000, true);
		//}
		//TensSetSpeed(&tens, NUM_CHANNELS, curTestVal);
		if( (paData[PA1_ENC1] != 3) ||
				((paData[PA1_ENC1] == 3) && (paData[PA1_JS1X] == 0)) ){
			paDataLast[PA1_POT1] = paData[PA1_POT1];
			//TensSetDacVal(&tens.tensChan[2], curTestVal);
			//TensSetDacVal(&ten, 3, curTestVal);
		}
	}

	//POT2
	if(paData[PA1_POT2] != paDataLast[PA1_POT2]){
		//testDataLast[MSG_POT2] = testData[MSG_POT2];
		curTestVal = paData[PA1_POT2];

		//Tens Voltage
		//TensSetDacVal(&tens.tensChan[2], curTestVal);
		if( (paData[PA1_ENC1] != 3) ||
							((paData[PA1_ENC1] == 3) && (paData[PA1_JS1Y] == 0)) ){
			paDataLast[PA1_POT2] = paData[PA1_POT2];
			//TensSetDacVal(&tens.tensChan[3], curTestVal);
		}
	}


	//PB2
	if(paData[PA1_PB2] != paDataLast[PA1_PB2]){
		paDataLast[PA1_PB2] = paData[PA1_PB2];
		curTestVal = paData[PA1_PB2];

		// Enable chan 1
		if(curTestVal == 1){
			TensEnableChannel(&tens, 1, true);
		}else if(curTestVal == 0){
			TensEnableChannel(&tens, 1, false);
		}

	}


	//PB3
	if(paData[PA1_PB3] != paDataLast[PA1_PB3]){
		paDataLast[PA1_PB3] = paData[PA1_PB3];
		curTestVal = paData[PA1_PB3];

		// Enable chan 2
		if(curTestVal == 1){
			TensEnableChannel(&tens, 2, true);
		}else if(curTestVal == 0){
			TensEnableChannel(&tens, 2, false);
		}
	}

	//PB4
	if(paData[PA1_PB4] != paDataLast[PA1_PB4]){
		paDataLast[PA1_PB4] = paData[PA1_PB4];
		curTestVal = paData[PA1_PB4];

		// Enable chan 3
		if(curTestVal == 1){
			TensEnableChannel(&tens, 3, true);
		}else if(curTestVal == 0){
			TensEnableChannel(&tens, 3, false);
		}
	}

	//PB5
	if(paData[PA1_PB5] != paDataLast[PA1_PB5]){
		paDataLast[PA1_PB5] = paData[PA1_PB5];
		curTestVal = paData[PA1_PB5];

		// Enable chan 4
		if(curTestVal == 1){
			TensEnableChannel(&tens, 4, true);
		}else if(curTestVal == 0){
			TensEnableChannel(&tens, 4, false);
		}
	}

	//PB6
	if(paData[PA1_PB6] != paDataLast[PA1_PB6]){
		paDataLast[PA1_PB6] = paData[PA1_PB6];
		curTestVal = paData[PA1_PB6];
		TensvBoostEnable(&tens, (curTestVal == 1) ? true : false);

		//Start or stop programs
		if(curTestVal == 1){
			TensStart(&tens, 0);
		}else if(curTestVal == 0){
			TensStop(&tens, 0, progState_Paused);
		}
	}


	//PB7
	if(paData[PA1_PB7] != paDataLast[PA1_PB7]){
		paDataLast[PA1_PB7] = paData[PA1_PB7];
		curTestVal = paData[PA1_PB7];

	}

	//PB8
	if(paData[PA1_PB8] != paDataLast[PA1_PB8]){
		paDataLast[PA1_PB8] = paData[PA1_PB8];
		curTestVal = paData[PA1_PB8];

	}

	//PB9
	if(paData[PA1_PB9] != paDataLast[PA1_PB9]){
		paDataLast[PA1_PB9] = paData[PA1_PB9];
		curTestVal = paData[PA1_PB9];

	}

//	//PB10
//	if(testData[MSG_PB10] != testDataLast[MSG_PB10]){
//		testDataLast[MSG_PB10] = testData[MSG_PB10];
//		curTestVal = testData[MSG_PB10];
//
//	}

	//PB11
	if(paData[PA1_PB11] != paDataLast[PA1_PB11]){
		paDataLast[PA1_PB11] = paData[PA1_PB11];
		curTestVal = paData[PA1_PB11];

	}

	//PB12
	if(paData[PA1_PB12] != paDataLast[PA1_PB12]){
		paDataLast[PA1_PB12] = paData[PA1_PB12];
		curTestVal = paData[PA1_PB12];

	}

	//PB13
	if(paData[PA1_PB13] != paDataLast[PA1_PB13]){
		paDataLast[PA1_PB13] = paData[PA1_PB13];
		curTestVal = paData[PA1_PB13];

	}

	//PB14
	if(paData[PA1_PB14] != paDataLast[PA1_PB14]){
		paDataLast[PA1_PB14] = paData[PA1_PB14];
		curTestVal = paData[PA1_PB14];

	}


//	//ENC1
//	if(testData[MSG_ENC1] != testDataLast[MSG_ENC1]){
//		testDataLast[MSG_ENC1] = testData[MSG_ENC1];
//		curTestVal = testData[MSG_ENC1];
//
//	}

	//ENC2
	if(paData[PA1_ENC2] != paDataLast[PA1_ENC2]){
		paDataLast[PA1_ENC2] = paData[PA1_ENC2];
		curTestVal = paData[PA1_ENC2];

	}

//	//ENC3
//	if(testData[MSG_ENC3] != testDataLast[MSG_ENC3]){
//		testDataLast[MSG_ENC3] = testData[MSG_ENC3];
//		curTestVal = testData[MSG_ENC3];
//
//	}



	//ENC5
//	if(testData[MSG_ENC5] != testDataLast[MSG_ENC5]){
//		testDataLast[MSG_ENC5] = testData[MSG_ENC5];
//		curTestVal = testData[MSG_ENC5];
//
//	}


	return true;

}

bool SetParamArray_ProgLineData(int32_t *optParam, int32_t *lineData, uint8_t lineDataLen){
	bool retVal = false;
	//if(lineDataLen != 14) return false;
	if(lineDataLen != DATAFIELD_LENGTH+1) return false;

	uint16_t progNum = optParam[0];
	uint16_t lineNum = optParam[1];
	if(progNum >= dlNumProgFiles) return false;
	//if(lineNum >= pDLNumProgramLines[progNum]) return false;
	if(lineNum >= dlNumProgramLines[progNum]) return false;
	if(progNum != dlCurProg) return false;

	if(lineNum != lineData[0]){
		return false;
	}

//	//Calculate the index of this new line in the progLine array:
//	uint16_t i=0;
//	for(uint16_t n=0; n<progNum; n++){
//		i += pDLNumProgramLines[n];
//	}
//	i += lineNum;
	uint16_t i=lineNum;

	//Save the incoming data to the download progLine array
	dlProgramLines[i].command = lineData[DFCOMMAND +1];
	dlProgramLines[i].channel = lineData[DFCHANNEL +1];
	dlProgramLines[i].gotoTrue = lineData[DFGTT +1];
	dlProgramLines[i].gotoFalse = lineData[DFGTF +1];
	dlProgramLines[i].pi81S = lineData[DF81S +1];
	dlProgramLines[i].pi81V1 = lineData[DF81V1 +1];
	dlProgramLines[i].pi81V2 = lineData[DF81V2 +1];
	dlProgramLines[i].pi82S = lineData[DF82S +1];
	dlProgramLines[i].pi82V1 = lineData[DF82V1 +1];
	dlProgramLines[i].pi82V2 = lineData[DF82V2 +1];
	dlProgramLines[i].polarity = lineData[DFPOLARITY +1];
	dlProgramLines[i].pi321S = lineData[DF321S +1];
	dlProgramLines[i].pi321V1 = lineData[DF321V1 +1];
	dlProgramLines[i].pi321V2 = lineData[DF321V2 +1];
	dlProgramLines[i].pi322S = lineData[DF322S +1];
	dlProgramLines[i].pi322V1 = lineData[DF322V1 +1];
	dlProgramLines[i].pi322V2 = lineData[DF322V1 +1];
	dlProgramLines[i].pi323S = lineData[DF323S +1];
	dlProgramLines[i].pi323V1 = lineData[DF323V1 +1];
	dlProgramLines[i].pi323V2 = lineData[DF323V2 +1];
	//dlProgramLines[i].numRepeats = lineData[DFREPEATS];
	retVal = true;

	//If this is the final line of the current program of the download then save the data to flash.
//	if( (progNum == dlNumProgFiles -1) && (lineNum == pDLNumProgramLines[progNum] - 1) ){
	if(lineNum == dlNumProgramLines[progNum] - 1){
		//Save to flash.
		if(TensSaveProgramToFlash(&tens, progNum, dlProgStartAddr, dlProgName, dlNumProgramLines[dlCurProg], dlProgramLines) != HAL_OK){
			retVal = false;
		}
	}

	//If this is the final line of the final program of the download then write the FAT table to flash memory.
	if( (progNum == dlNumProgFiles -1) && (lineNum == dlNumProgramLines[progNum] - 1) ){
		if(TensWriteFileAllocationTable(&tens, dlNumProgFiles, dlNumProgramLines) == HAL_OK){
			//Load the new program into memory:
			TensReadFileAllocationTable(&tens);
			TensLoadFile(&tens, 0, 0);
		}else{
			//sendparam flash error!
			retVal = false;
		}
		//Free the memory used from the download:
		dlNumChans = 0;
		dlNumProgFiles = 0;
		dlCurProg = 0;
		//free(dlProgName);
		//free(dlProgramLines);
		//free(pDLNumProgramLines);
	}

	return retVal;
}

//bool sendParameter(char Command, char Param, uint8_t paramIndex, comChanEnum comChannel, uint16_t Sender){
bool sendParameter(char* buff, uint8_t indx, comChanEnum comChannel, uint16_t sender){
	char Param = buff[1];
	uint8_t paramIndex = buff[2] - '0';  //paramIndex is the index of a Tens/Motor channel.  All indexes are based on motA=0, motB=1, tensA=2, tensB=3, etc.  Only used for some parameters, not all.
	char dataBuff[20] = {0};
	uint8_t len = 0;
	dataBuff[0] = commandType_SetSingleParam;
	dataBuff[1] = Param;
	//dataBuff[2] = '=';
	uint8_t tmpStart = 2;
	uint8_t tmpVal = 0;

	switch(Param){
	case pStat_BattLevel:
		len = sprintf(&dataBuff[tmpStart], "=%d\n", battPercent);
		break;
	case pStat_Charging:
		tmpVal = 0; // tmpVal = (battCharging == true) ? 1 : 0;
		len = sprintf(&dataBuff[tmpStart], "=%d\n", tmpVal);
		break;
	case pStat_Charged:
		tmpVal = 0; // tmpVal = (battCharged == true) ? 1 : 0;
		len = sprintf(&dataBuff[tmpStart], "=%d\n", tmpVal);
		break;
	case pStat_NumMotorChannels:
		len = sprintf(&dataBuff[tmpStart], "=%d\n", NUM_MOTOR_CHANNELS);
		break;
	case pStat_NumTensChannels:
		len = sprintf(&dataBuff[tmpStart], "=%d\n", NUM_TENS_CHANNELS);
		break;
	case pStat_NumChannels:
		len = sprintf(&dataBuff[tmpStart], "=%d\n", NUM_CHANNELS);
		break;
	case pStat_NumInputs:
		len = sprintf(&dataBuff[tmpStart], "=%d\n", NUM_DINPUTS);
		break;
	case pStat_NumOutputs:
		len = sprintf(&dataBuff[tmpStart], "=%d\n", NUM_DOUTPUTS);
		break;
	case pStat_MotIndexStart:
		len = sprintf(&dataBuff[tmpStart], "=%d\n", MOT_INDEX_START);
		break;
	case pStat_TensIndexStart:
		len = sprintf(&dataBuff[tmpStart], "=%d\n", TENS_INDEX_START);
		break;
//	case pStat_ChanProglinesMax:
//		len = sprintf(&dataBuff[tmpStart], "=%d\n", CHAN_PROGLINES_MAX);
//		break;
//	case pStat_SysProgLinesMax:
//		len = sprintf(&dataBuff[tmpStart], "=%d\n", SYS_PROGLINES_MAX);
//		break;
	case pStat_TensProgCurVer:
		//len = sprintf(&dataBuff[tmpStart], "=TENS2502\n");
		uint16_t tmpVer = TENS_PROGRAM_CUR_VER;
		len = sprintf(&dataBuff[tmpStart], "=TENS%d\n", tmpVer);
		break;
	case pStat_TensProgMinVer:
		//len = sprintf(&dataBuff[tmpStart], "=TENS2502\n");
		//break;
	case pStat_ChanType:
		//tmpVal = (tens.tensChan[paramIndex].chanType == chanType_Motor) ? 0 : 1;
		tmpVal = tens.tensChan[paramIndex].chanType;
		len = sprintf(&dataBuff[tmpStart], "%d=%d\n", paramIndex, tmpVal);
		break;
	case pStat_ChanEnabled:
		if( (paramIndex > 0) && (paramIndex < NUM_CHANNELS) ){
			tmpVal = (tens.tensChan[paramIndex].chanEnabled == true) ? 1 : 0;
		}else{
			tmpVal = -1;
		}
		len = sprintf(&dataBuff[tmpStart], "%d=%d\n", paramIndex, tmpVal);
		break;
	case pStat_ProgNumber:
//TODO: finish this
		break;
	case pStat_ProgState:
		if(paramIndex < NUM_CHANNELS){
			tmpVal = tens.curProgStatus[paramIndex].progState;
		}else{
			tmpVal = progState_Empty;
		}
		len = sprintf(&dataBuff[tmpStart], "%d=%d\n", paramIndex, tmpVal);
		break;
	case pStat_CurLineNum:
		if(paramIndex < NUM_CHANNELS){
			tmpVal = tens.curProgStatus[paramIndex].curLineNum;
		}else{
			tmpVal = -1;
		}
		len = sprintf(&dataBuff[tmpStart], "%d=%d\n", paramIndex, tmpVal);
		break;
	case pStat_ChanCurPWidthPct:
		if(paramIndex < NUM_CHANNELS){
			//tmpVal = tens.tensChan[paramIndex].curVal;
			tmpVal = tens.tensChan[paramIndex].mappedCurVal;
		}else{
			tmpVal = 0;
		}
		len = sprintf(&dataBuff[tmpStart], "%d=%d\n", paramIndex, tmpVal);
		break;
	case pStat_ChanCurIntensityPct:
		//if( (paramIndex >= TENS_INDEX_START) && (paramIndex <= TENS_INDEX_START + NUM_TENS_CHANNELS) ){
		if(paramIndex < NUM_CHANNELS){
			tmpVal = tens.tensChan[paramIndex].curIntensityPct;
			//tmpVal = TensGetDacVal(&tens.tensChan[paramIndex]);
		}else{
			tmpVal = 0;
		}
		len = sprintf(&dataBuff[tmpStart], "%d=%d\n", Param, tmpVal);
		break;
	case pStat_CurSpeed:
		if(paramIndex < NUM_CHANNELS){
			tmpVal = tens.tensChan[paramIndex].chanSpeed;
		}
		len = sprintf(&dataBuff[tmpStart], "%d=%d\n", paramIndex, tmpVal);
		break;
	case pStat_MinIntensity:
		if(paramIndex < NUM_CHANNELS){
			tmpVal = tens.tensChan[paramIndex].intensityMin;
		}
		len = sprintf(&dataBuff[tmpStart], "%d=%d\n", paramIndex, tmpVal);
		break;
	case pStat_MaxIntensity:
		if(paramIndex < NUM_CHANNELS){
			tmpVal = tens.tensChan[paramIndex].intensityMax;
		}
		len = sprintf(&dataBuff[tmpStart], "%d=%d\n", paramIndex, tmpVal);
		break;
	case pStat_MaxOutputPulsewidthPct:
		if(paramIndex < NUM_CHANNELS){
			tmpVal = tens.tensChan[paramIndex].outputMaxPct;
		}
		len = sprintf(&dataBuff[tmpStart], "%d=%d\n", paramIndex, tmpVal);
		break;
	case pStat_NumPrograms:
		len = sprintf(&dataBuff[tmpStart], "=%d\n", tens.numProgFiles);
		break;
	case pStat_ProgramLength:
		uint16_t tmpProgLen = 0;
		if(paramIndex < tens.numProgFiles){
			tmpProgLen = tens.numProgramLines[paramIndex];
		}
		len = sprintf(&dataBuff[tmpStart], "%d=%d\n", paramIndex, tmpProgLen);
		break;
	case pStat_ProgramName:
		char tmpProgName[TENS_FILENAME_LEN+1];
		memset(tmpProgName,0,TENS_FILENAME_LEN+1);
		if(paramIndex < tens.numProgFiles){
			//memcpy(tmpProgName, tens.curProgName[paramIndex], TENS_FILENAME_LEN);
			memcpy(tmpProgName, (char*)tens.progFileAddress[paramIndex], TENS_FILENAME_LEN);
		}
		len = sprintf(&dataBuff[tmpStart], "%d=%s\n", paramIndex, tmpProgName);
		break;

	default:
		return false;
		break;
	}


	dataSend(dataBuff, len+tmpStart, comChannel, sender, false);
	return true;
}

//bool sendParameterArray(char Command, char Param, comChanEnum comChannel, uint16_t sender){
bool sendParameterArray(char* buff, uint8_t indx, comChanEnum comChannel, uint16_t sender){
	bool retVal = false;
	char Param = buff[1];
	char dataBuff[512] = {0}; //char dataBuff[156] = {0}; 12/28/25 had to increase the buffer to hold debug data
	dataBuff[0] = commandType_SetParamArray;
	dataBuff[1] = Param;
	uint16_t len = 2;
	//uint8_t tmpStart = 2;
#define MAX_MSG_INDEXES 5
//	char tmpIndexString[MAX_MSG_INDEXES * 4] = {0};

//	int8_t paramIndex = -1;
	int32_t msgIndex[MAX_MSG_INDEXES] = {0};
	uint16_t msgIndexCount = 0;

	//Check if there are any index numbers following the param
	if(indx > 2){
		//Extract all incoming indexes located after the main param and before any equal sign
		char msgIndexString[MAX_MSG_PARAMS * 3] = {0};
		//char optParamString[20] = {0};
		uint16_t msgIndexStringLen = 0;
		for(uint8_t n=2; n<indx; n++){
			msgIndexString[n-2] = buff[n];
			msgIndexStringLen++;
		}
		msgIndexCount = StringToI32Array(msgIndexString, msgIndexStringLen, "," , msgIndex, MAX_MSG_INDEXES);
	}


	switch(Param){
	case pArray_DeviceInfo:
		// DeviceType, RadioId, NumberOfPrograms, DeviceName, SerialNumber
		char tmpName[] = {"Proto E"};
		char tmpSerial[] = {"12345678987654321"};
		len += sprintf(&dataBuff[len], "=%d,%d,%d,%s,%s", (uint8_t)deviceType, radio.comm.myId, tens.numProgFiles, tmpName, tmpSerial);
		dataBuff[len] = 10;
		len++;
		retVal = true;
		break;
	case pArray_ChanMinMax:
		// sysMaster + chan1-4,   min/max values
		// minSpeed, maxSpeed, minIntensity, maxIntensity
		if( (msgIndex[0] >= 0) && (msgIndex[0] < NUM_CHANNELS) ){
			//channel min/max
			uint16_t tmpMin = (tens.tensChan[msgIndex[0]].intensityMin <= 100 ? tens.tensChan[msgIndex[0]].intensityMin : 0);
			uint16_t tmpMax = (tens.tensChan[msgIndex[0]].intensityMax <= 100 ? tens.tensChan[msgIndex[0]].intensityMax : 0);


			len += sprintf(&dataBuff[len], "%d=%d,%d,%u,%u\n", (int16_t)msgIndex[0],
					(int16_t)tens.tensChan[msgIndex[0]].chanMinSpeed, (int16_t)tens.tensChan[msgIndex[0]].chanMaxSpeed,	tmpMin, tmpMax);
		}else{
			//error
			return false;
		}
		retVal = true;
		break;
	case pArray_ChanStats:
		// chan0-3
		// 0 chanEnabled, 1 chanSpeed, 2 curLineNum, 3 startVal, 4 endVal, 5 modDuration,
		// 6 percentComplete, 7 RepeatsRemaining, 8 chanCurVal, 9 chanCurIntensity,
		// 10 progState, 11 polaritySwapped, 12 polarity, 13 curProgNumber, 14 minIntensity, 15 maxIntensity, 16 maxOutputPulsewidthPct
		if( (msgIndex[0] < 0) || (msgIndex[0] >= NUM_CHANNELS) ) return false;
		TensProgramStatus_HandleTypeDef* pCurStatus = &tens.curProgStatus[msgIndex[0]];
		structProgLine* pCurLine = &tens.curProgLine[msgIndex[0]];

		uint16_t tmpMin = (tens.tensChan[msgIndex[0]].intensityMin <= 100 ? tens.tensChan[msgIndex[0]].intensityMin : 0);
		uint16_t tmpMax = (tens.tensChan[msgIndex[0]].intensityMax <= 100 ? tens.tensChan[msgIndex[0]].intensityMax : 0);

		uint32_t tmpTotalDuration = pCurStatus->modOutputDuration + pCurStatus->modDelayDuration;
		float tmpTotalPctComplete = tens.tensChan[msgIndex[0]].pctComplete;
		if(pCurLine->command == tensCommand_TenMotOutput){
			//Calculate the stats for output channels by combining the output time with the postDelay time and report on the progLine as a whole.
			if( (tens.tensChan[msgIndex[0]].activeState == Active) && (pCurStatus->modDelayDuration > 0) ){
				//We're in the output state of a TenMot output line and there's a postDelay with this line.
				//The tmpTotalPctComplete we obtained is just for the output portion of the line.  Calculate a new pctComplete considering the postDelay portion also.
				tmpTotalPctComplete = (tmpTotalPctComplete * pCurStatus->modOutputDuration / 100);  // = ms completed of output
				tmpTotalPctComplete = (tmpTotalPctComplete + pCurStatus->modDelayDuration) / tmpTotalDuration * 100; // = % complete of entire line
			}else if( (tens.tensChan[msgIndex[0]].activeState == PostDelay) && (pCurStatus->modDelayDuration > 0) ){
				//We're in the output state of a TenMot output line and there's a postDelay with this line.
				//The tmpTotalPctComplete we obtained is just for the postDelay portion of the line.  Calculate a new pctComplete considering the output portion also.
				tmpTotalPctComplete = (tmpTotalPctComplete * pCurStatus->modDelayDuration / 100);  // = ms completed of postDelay
				tmpTotalPctComplete = (tmpTotalPctComplete + pCurStatus->modOutputDuration) / tmpTotalDuration * 100; // = % complete of entire line
			}
		}

		len += sprintf(&dataBuff[len], "%d=%d,%d,%d,%d,%d,%lu,%d,%d,%d,%d,%d,%d,%d,%d,%u,%u,%u\n", (int16_t)msgIndex[0],
					(tens.tensChan[msgIndex[0]].chanEnabled == true ? 1 : 0), tens.tensChan[msgIndex[0]].chanSpeed,
					pCurStatus->curLineNum, tens.tensChan[msgIndex[0]].startVal,
					tens.tensChan[msgIndex[0]].endVal, tmpTotalDuration,
					(uint8_t)floor(tmpTotalPctComplete), tens.tensChan[msgIndex[0]].repeatCounter,
					(uint8_t)tens.tensChan[msgIndex[0]].mappedCurVal, tens.tensChan[msgIndex[0]].curIntensityPct,
					pCurStatus->progState, (tens.tensChan[msgIndex[0]].polaritySwapped == false ? 0 : 1),
					pCurLine->polarity, tens.curProgNum[msgIndex[0]], tmpMin, tmpMax, tens.tensChan[msgIndex[0]].outputMaxPct);
		retVal = true;
		break;

	case pArray_ProgLineData:
//TODO: finish this.
		retVal = false;
		if(msgIndexCount >=2){
			if(msgIndex[0] == 7){
				__NOP();
			}
			if(msgIndex[0] < tens.numProgFiles){
				if(msgIndex[1] < tens.numProgramLines[msgIndex[0]]){
					structProgLine* tmpLineObj = (structProgLine *)TensGetProgLineAddressFromFlash(&tens, msgIndex[0], msgIndex[1]);
					if((uint32_t)tmpLineObj > tens.progFileAddress[0]){
						len += sprintf(&dataBuff[len], "%d,%d=%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%d,%ld,%ld,%d,%ld,%ld,%d,%ld,%ld\n", (int16_t)msgIndex[0], (int16_t)msgIndex[1],
							(int16_t)msgIndex[1], (uint8_t)tmpLineObj->command, (uint8_t)tmpLineObj->channel, (uint16_t)tmpLineObj->gotoTrue,
							(uint16_t)tmpLineObj->gotoFalse, (uint8_t)tmpLineObj->pi81S, (uint8_t)tmpLineObj->pi81V1, (uint8_t)tmpLineObj->pi81V2,
							(uint8_t)tmpLineObj->pi82S, (uint8_t)tmpLineObj->pi82V1, (uint8_t)tmpLineObj->pi82V1,
							(uint8_t)tmpLineObj->polarity,
							(uint8_t)tmpLineObj->pi321S, tmpLineObj->pi321V1, tmpLineObj->pi321V2,
							(uint8_t)tmpLineObj->pi322S, tmpLineObj->pi322V1, tmpLineObj->pi322V2,
							(uint8_t)tmpLineObj->pi323S, tmpLineObj->pi323V1, tmpLineObj->pi323V2);
						retVal = true;
					}
				}
			}
		}
		break;
	case pArray_ChanDebugData:
		// 0 curProgNum, 1 curLineNum, 2 progState, 3 number of program variables, 4 number of program timers,
		// 5 variable values,  5 + (number of program variables * 4) start of timer values
		if( (msgIndex[0] < 0) || (msgIndex[0] >= NUM_CHANNELS) ) return false;
		TensProgramStatus_HandleTypeDef* pCurStatusDebug = &tens.curProgStatus[msgIndex[0]];

		len += sprintf(&dataBuff[len], "%d=%d,%d,%d,%d,%d", (int16_t)msgIndex[0],
				tens.curProgNum[msgIndex[0]], pCurStatusDebug->curLineNum, pCurStatusDebug->progState, NUM_VARIABLES, NUM_TIMERS);

		// Append variables
		for (int v = 0; v < NUM_VARIABLES; v++) {
			len += snprintf(&dataBuff[len], sizeof(dataBuff)-(len+1), ",%ld", pCurStatusDebug->variable[v]);
		}
		// Append timer values
		for (int t = 0; t < NUM_TIMERS; t++) {
			len += snprintf(&dataBuff[len], sizeof(dataBuff)-(len+1), ",%ld", pCurStatusDebug->timer[t]);
		}
		retVal = true;
		break;
	default:
		retVal = false;
		break;
	}

	if(retVal == true){
		dataSend(dataBuff, len, comChannel, sender, false);
	}
	return retVal;
}

void DownloadProjectFromRemoteHost(comChanEnum comChannel, uint16_t sender, uint16_t numPrograms){

}

void initDisplay(){
#ifdef USE_DISPLAY

#ifdef DEVICE_BOARD_B
	ST7735_Init(&lcd, 80, 160, &hspi2, LCD_CS_GPIO_Port, LCD_CS_Pin, LCD_AO_GPIO_Port, LCD_AO_Pin, LCD_RST_GPIO_Port, LCD_RST_Pin, 1, INITR_MINI160x80_PLUGIN, true);
	ST7735_SetRotation(&lcd, 1);
	#endif
#ifdef DEVICE_BOARD_E
	ST7735_Init(&lcd, 80, 160, &hspi2, LCD2_CS_GPIO_Port, LCD2_CS_Pin, LCD2_AO_GPIO_Port, LCD2_AO_Pin, LCD_RST_GPIO_Port, LCD_RST_Pin, 1, INITR_MINI160x80_PLUGIN, true);
	ST7735_SetRotation(&lcd, 3);
#endif


	HAL_TIM_PWM_Start(phtimLcdBl, LCD_BLCHAN);

	//__HAL_TIM_SET_COMPARE(phtimLcdBl, LCD_BLCHAN, 50);
	__HAL_TIM_SET_COMPARE(phtimLcdBl, LCD_BLCHAN, settings.vars.displayBrightness);

	ST7735_FillScreen(&lcd, BLACK);
	ST7735_SetFont(&lcd, fontBig);
	ST7735_SetTextColor(&lcd, WHITE, BLACK);
	//char tmpName[] = {"Hi :)"};
	//uint16_t tmpW = ST7735_GetTextWidth(&lcd, tmpName);
	//uint16_t tmpH = ST7735_GetTextHeight(&lcd, tmpName, false);
	//ST7735_SetCursor(&lcd, (160-tmpW)/2 , 5 + tmpH);
	//ST7735_Print(&lcd, tmpName);

	uint16_t tmpW = ST7735_GetTextWidth(&lcd, DISPLAY_NAME);
	uint16_t tmpH = ST7735_GetTextHeight(&lcd, DISPLAY_NAME, false);
	ST7735_SetCursor(&lcd, (160-tmpW)/2 , 20 + tmpH);
	ST7735_Print(&lcd, DISPLAY_NAME);



	ST7735_SetFont(&lcd, fontStatusbar);
	ST7735_SetTextColor(&lcd, WHITE, BLACK);
	tmpW = ST7735_GetTextWidth(&lcd, PROJECTNAME);
	tmpH = ST7735_GetTextHeight(&lcd, PROJECTNAME, false);
	ST7735_SetCursor(&lcd, (160-tmpW)/2 , 60 + tmpH);
	ST7735_Print(&lcd, PROJECTNAME);


	//char chrSWV[30] = {0};
	char chrSWV[25] = {"ver 0.02"};
	//char* chrSWV = calloc(20, sizeof(char));
	//float tmpVer = settings.vars.flashVersion;
	//sprintf(chrSWV, "ver %f", tmpVer);
	//sprintf(chrSWV, "ver %.03f", 5.3);
	tmpW = ST7735_GetTextWidth(&lcd, chrSWV);
	tmpH = ST7735_GetTextHeight(&lcd, chrSWV, false);
	ST7735_SetCursor(&lcd, (160-tmpW)/2 , 79);
	ST7735_Print(&lcd, chrSWV);

	HAL_Delay(2000);


	ST7735_InitCanvas(&lcd, &canStatusBar, lcd.width, 20);
	ST7735_InitCanvas(&lcd, &canScreen, lcd.width, lcd.height);






	ST7735_FillScreen(&lcd, BLACK);

#endif
}

//void turnOffDisplay(){
//	//ST7735_FillScreen(&lcd, BLACK);
//	__HAL_TIM_SET_COMPARE(phtimLcdBl, LCD_BLCHAN, 0);
//	ST7735_DisplayOff(&lcd);
//}
void turnDisplayOnOff(bool newState){
	if(newState == true){
		//turn display on
		__HAL_TIM_SET_COMPARE(phtimLcdBl, LCD_BLCHAN, settings.vars.displayBrightness);
		ST7735_DisplayOn(&lcd);
	}else{
		//turn display off
		__HAL_TIM_SET_COMPARE(phtimLcdBl, LCD_BLCHAN, 0);
		ST7735_DisplayOff(&lcd);
	}
	displayOn = newState;
}

//void updateDisplay(bool forceUpdate){
//	updateDisplayStatusBar(forceUpdate);
//	updateDisplayIntensity(forceUpdate);
//	updateDisplayChanEnabled(forceUpdate);
//}
//
//void updateDisplayStatusBar(bool forceUpdate){
//
//	//if( (HAL_GetTick() - _battLastDisplayedTime > 1000) || (forceUpdate == true) ){
//	if(HAL_GetTick() - _battLastDisplayedTime > 1000){
//		uint8_t battDiff = (battCurLevel < _lastBattLevel) ? (_lastBattLevel - battCurLevel) : (battCurLevel - _lastBattLevel);
//		if((battDiff >= 5) || (forceUpdate == true) ){
//			uint16_t tmpX, tmpY;
//			tmpX = 135, tmpY = 2;
//
//			//redraw the status bar.
//			uint16_t bgColor = BLACK;
//			uint16_t fgColor = WHITE;
//			ST7735_FillRect(&lcd, 0, 0, lcd.width, 10, bgColor);
//			//ST7735_SetCursor(&lcd, tmpX, tmpY+7);
//			ST7735_SetFont(&lcd, fontStatusbar);
//			ST7735_SetTextColor(&lcd, fgColor, bgColor);
//
//
//			//Print the step count:
//			ST7735_SetCursor(&lcd, 5, tmpY+7);
//			char chrSteps[10] = {0};
//			uint8_t tmpLen = sprintf(chrSteps, "%ld", imu.stepCount);
//			ST7735_Write(&lcd, (uint8_t *)chrSteps, tmpLen);
//			imu.stepCountLastDisplayed = imu.stepCount;
//
//			//Print the Orientation Indicator:
//			ST7735_SetCursor(&lcd, 45, tmpY+7);
//			switch(imu.upDirection){
//			case UP_X_POS:  	// +X face up
//				ST7735_Print(&lcd, "X+");
//				break;
//			case UP_X_NEG:  	// -X face up
//				ST7735_Print(&lcd, "X-");
//				break;
//			case UP_Y_POS:  	// +Y face up
//				ST7735_Print(&lcd, "Y+");
//				break;
//			case UP_Y_NEG:  	// -Y face up
//				ST7735_Print(&lcd, "Y-");
//				break;
//			case UP_Z_POS:  	// +Z face up
//				ST7735_Print(&lcd, "Z+");
//				break;
//			case UP_Z_NEG:		// -Z face up
//				ST7735_Print(&lcd, "Z-");
//				break;
//			default:
//				break;
//			}
//			imu.upDirectionLastDisplayed = imu.upDirection;
//
//			//Print the GlobalZ Rotation:
//			ST7735_SetCursor(&lcd, 85, tmpY+7);
//			char chrZRot[10] = {0};
//			tmpLen = sprintf(chrZRot, "%.0f", imu.globalZRotation);
//			ST7735_Write(&lcd, (uint8_t *)chrZRot, tmpLen);
//			imu.globalZRotationLastDisplayed = imu.globalZRotation;
//
//			//Draw the battery Icon:
//			ST7735_SetCursor(&lcd, tmpX, tmpY+7);
//			//determine what icon to draw:
//			if(battCurLevel == BATTLEVEL_PLUGGEDIN){
//				ST7735_WriteChar(&lcd, (char) 0x24);  //Charging symbol
//			}else{
//				//Draw the battery icon
//				ST7735_WriteChar(&lcd, (char) 0x26);	//battery symbol
//				//Fill in the icon based on the battery level:
//				uint8_t numBattLines = (battCurLevel / 10) + 1;
//				numBattLines = (numBattLines > 10) ? 10 : numBattLines;
//				for(uint8_t n=0; n<numBattLines; n++){
//					ST7735_DrawFastVLine(&lcd, tmpX + n, tmpY+1, 6, fgColor);
//				}
//			}
//			_lastBattLevel = battCurLevel;
//			_battLastDisplayedTime = HAL_GetTick();
//		}
//	}
//}
//
//void updateDisplayIntensity(bool forceUpdate){
//	bool needsUpdated = false;
//	for(uint8_t n=0; n<NUM_TENS_CHANNELS; n++){
//		if(_lastDisplayedIntensity[n] != tens.tensChan[n + TENS_INDEX_START].curIntensityPct){
//			needsUpdated = true;
//		}
//	}
//
//	if(forceUpdate || (needsUpdated && (HAL_GetTick() - _IntensityLastDisplayedTime > 250)) ){
//		uint16_t tmpBuff=4, tmpX=5, tmpY=30, tmpW=150, tmpH=4;
//		uint16_t bgColor=DARKGRAY, fgColor = WHITE, fgColorOk = WHITE, fgColorCaution = YELLOW, fgColorDanger = RED;
//		uint16_t tmpActiveX = 0;
//		for(uint8_t n=0; n<NUM_TENS_CHANNELS; n++){
//			uint16_t tmpIntensity = tens.tensChan[n + TENS_INDEX_START].curIntensityPct;
//			_lastDisplayedIntensity[n] = tmpIntensity;
//			if(tmpIntensity < 51){
//				fgColor = fgColorOk;
//			}else if(tmpIntensity < 75){
//				fgColor = fgColorCaution;
//			}else{
//				fgColor = fgColorDanger;
//			}
//			tmpActiveX = (uint16_t)mapi16(tmpIntensity, 0, 100, 0, tmpW, true);
//			//Fill the foreground color (active)
//			ST7735_FillRect(&lcd, tmpX, tmpY, tmpActiveX, tmpH, fgColor);
//			//Fill the background color (inactive)
//			ST7735_FillRect(&lcd, tmpX + tmpActiveX, tmpY, tmpW - tmpActiveX, tmpH, bgColor);
//			//Advance Y for the next bar
//			tmpY += tmpH + tmpBuff;
//		}
//		_IntensityLastDisplayedTime = HAL_GetTick();
//	}
//}
//
//void updateDisplayChanEnabled(bool forceUpdate){
//	bool needsUpdated = false;
//	uint16_t colorOutline=DARKGRAY, colorActive=GREEN, colorStopped=RED, colorPaused=YELLOW;
//	uint16_t newChanColor[NUM_CHANNELS] = {0};
//
//
//	for(uint8_t n=0; n<NUM_CHANNELS; n++){
////		if(_lastChanState[n] != tens.curProgStatus[n].progState){
////			needsUpdated = true;
////		}
//
//		switch(tens.curProgStatus[n].progState){
//		case progState_Unknown:
//		case progState_Empty:
//		case progState_Stopped:
//			newChanColor[n] = colorStopped;
//			break;
//		case progState_Paused:
//			newChanColor[n] = colorPaused;
//			break;
//		case progState_Running:
//		case progState_LineComplete:
//			newChanColor[n] = colorActive;
//			break;
//		case progState_End:
//			newChanColor[n] = colorStopped;
//			break;
//		}
//
//		if(_lastChanStateColor[n] != newChanColor[n]){
//			needsUpdated = true;
//		}
//	}
//
//	if(forceUpdate || (needsUpdated && (HAL_GetTick() - _chanStateLastDisplayedTime > 250)) ){
//		uint16_t tmpXStart=4, tmpRad=3, tmpY=14;
//		uint16_t tmpXGrpInterval=20, tmpXChanInterval=(tmpRad*2)+2;
//		uint16_t xc=tmpXStart+tmpRad, yc=tmpY+tmpRad;
////
////		for(uint8_t n=0; n<NUM_CHANNELS; n++){
////			_lastChanState[n] = tens.curProgStatus[n].progState;
////
////			switch(_lastChanState[n]){
////			case progState_Unknown:
////			case progState_Empty:
////			case progState_Stopped:
////				colorBG = colorStopped;
////				break;
////			case progState_Paused:
////				colorBG = colorPaused;
////				break;
////			case progState_Running:
////			case progState_LineComplete:
////				colorBG = colorActive;
////				break;
////			case progState_End:
////				colorBG = colorStopped;
////				break;
////			}
//
//		for(uint8_t n=0; n<NUM_CHANNELS; n++){
//			ST7735_FillCircle(&lcd, xc, yc, tmpRad, newChanColor[n]);
//			ST7735_DrawCircle(&lcd, xc, yc, tmpRad, colorOutline);
//			_lastChanStateColor[n] = newChanColor[n];
//
//			if( (n==MOT_INDEX_START-1) || (n==TENS_INDEX_START-1) || (n==AUX_INDEX_START-1)){
//				xc+= tmpXGrpInterval;
//			}else{
//				xc += tmpXChanInterval;
//			}
//
//		}
//		_chanStateLastDisplayedTime = HAL_GetTick();
//	}
//}
void checkDisplay(bool forceUpdate){
	if(forceUpdate == true){
		displayLastActivityTime = HAL_GetTick();
	}

	//Check on/off state
	if(displayAutoOff == true){
		if(displayOn == true){
			// Diplay is currently on.  Is it time to turn off the display?
			if(HAL_GetTick() - displayLastActivityTime > displayAutoOffTime){
				//display just timed out.
				turnDisplayOnOff(false);
				//return;
			}else{
				//Leave it on and update it.
				updateDisplay(forceUpdate);
			}
		}else{
			// Display is currently off.  Do we need to turn it on?
			if(HAL_GetTick() - displayLastActivityTime < displayAutoOffTime){
				// Display needs to be turned on.
				turnDisplayOnOff(true);
				updateDisplay(true);
			}else{
				// Leave it off.
				return;
			}
		}
	}else{
		// Not using AutoOff.  If the diplay is currently on then update it.  If not then skip.
		if(displayOn == true){
			updateDisplay(forceUpdate);
		}
	}
}

void updateDisplay(bool forceUpdate){

	if(forceUpdate ||
			displayStatusBarNeedsUpdated() ||
			(displayIntensityNeedsUpdated() && (HAL_GetTick() - _IntensityLastDisplayedTime > 250) ) ||
			(displayChanEnabledNeedsUpdated() && (HAL_GetTick() - _chanStateLastDisplayedTime > 250) )  ||
			displayAudioEqNeedsUpdated() ){

		if(lcd.dmaWriteActive == 0){
			ST7735_FillScreenCanvas(&canScreen, BLACK);
			updateDisplayStatusBar();
			updateDisplayIntensity();
			updateDisplayChanEnabled();
			updateDisplayAudioEq();

			ST7735_WriteCanvasDMA(&canScreen);
		}

	}
}

bool displayStatusBarNeedsUpdated(){
	uint8_t battDiff = (battCurLevel < _lastBattLevel) ? (_lastBattLevel - battCurLevel) : (battCurLevel - _lastBattLevel);
	if(battDiff >= 5) return true;
	if(imu.upDirection != imu.upDirectionLastDisplayed) return true;
	if(imu.globalZRotation != imu.globalZRotationLastDisplayed) return true;
	return false;
}
void updateDisplayStatusBar(){

	uint16_t tmpX, tmpY;
	tmpX = 135, tmpY = 2;

	//redraw the status bar.
	uint16_t bgColor = BLACK;
	uint16_t fgColor = WHITE;
	//ST7735_FillRect(&lcd, 0, 0, lcd.width, 10, bgColor);
	ST7735_FillRectCanvas(&canScreen, 0, 0, lcd.width, 10, bgColor);
	//ST7735_SetFont(&lcd, fontStatusbar);
	ST7735_SetFontCanvas(&canScreen, fontStatusbar);
	//ST7735_SetTextColor(&lcd, fgColor, bgColor);
	ST7735_SetTextColorCanvas(&canScreen, fgColor, bgColor);

	//Print the step count:
	//ST7735_SetCursor(&lcd, 5, tmpY+7);
	ST7735_SetCursorCanvas(&canScreen, 5, tmpY+7);
	char chrSteps[10] = {0};
	uint8_t tmpLen = sprintf(chrSteps, "%ld", imu.stepCount);
	//ST7735_Write(&lcd, (uint8_t *)chrSteps, tmpLen);
	ST7735_WriteTextCanvas(&canScreen, (uint8_t *)chrSteps, tmpLen);
	imu.stepCountLastDisplayed = imu.stepCount;

	//Print the Orientation Indicator:
	ST7735_SetCursorCanvas(&canScreen, 45, tmpY+7);
	switch(imu.upDirection){
	case UP_X_POS:  	// +X face up
		ST7735_PrintTextCanvas(&canScreen, "X+");
		break;
	case UP_X_NEG:  	// -X face up
		ST7735_PrintTextCanvas(&canScreen, "X-");
		break;
	case UP_Y_POS:  	// +Y face up
		ST7735_PrintTextCanvas(&canScreen, "Y+");
		break;
	case UP_Y_NEG:  	// -Y face up
		ST7735_PrintTextCanvas(&canScreen, "Y-");
		break;
	case UP_Z_POS:  	// +Z face up
		ST7735_PrintTextCanvas(&canScreen, "Z+");
		break;
	case UP_Z_NEG:		// -Z face up
		ST7735_PrintTextCanvas(&canScreen, "Z-");
		break;
	default:
		break;
	}
	imu.upDirectionLastDisplayed = imu.upDirection;

	//Print the GlobalZ Rotation:
	ST7735_SetCursorCanvas(&canScreen, 85, tmpY+7);
//	//char *chrZRot;// = {0};
//	//chrZRot = calloc(20, sizeof(char));
//	//tmpLen = sprintf(chrZRot, "%0.1f", imu.globalZRotation);
//	uint8_t chrZRot[20];
//	memset(&chrZRot, " ", sizeof(chrZRot));
//	tmpLen = sprintf((char *)chrZRot, "%0.1f", imu.globalZRotation);

	char chrZRot[10] = {0};
	int16_t iZRot = (int16_t)(floor(imu.globalZRotation));
	tmpLen = sprintf(chrZRot, "%d", iZRot);

	ST7735_WriteTextCanvas(&canScreen, (uint8_t *)chrZRot, tmpLen);
	imu.globalZRotationLastDisplayed = imu.globalZRotation;

	//Draw the battery Icon:
	ST7735_SetCursorCanvas(&canScreen, tmpX, tmpY+7);
	//determine what icon to draw:
	if(battCurLevel == BATTLEVEL_PLUGGEDIN){
		ST7735_WriteCharCanvas(&canScreen, (char) 0x24);  //Charging symbol
	}else{
		//Draw the battery icon
		ST7735_WriteCharCanvas(&canScreen, (char) 0x26);	//battery symbol
		//Fill in the icon based on the battery level:
		uint8_t numBattLines = (battCurLevel / 10) + 1;
		numBattLines = (numBattLines > 10) ? 10 : numBattLines;
		for(uint8_t n=0; n<numBattLines; n++){
			ST7735_DrawFastVLineCanvas(&canScreen, tmpX + n, tmpY+1, 6, fgColor);
		}
	}
	_lastBattLevel = battCurLevel;
	_battLastDisplayedTime = HAL_GetTick();


}

bool displayIntensityNeedsUpdated(){
	for(uint8_t n=0; n<NUM_TENS_CHANNELS; n++){
		if(_lastDisplayedIntensity[n] != tens.tensChan[n + TENS_INDEX_START].curIntensityPct){
			return true;
		}
	}
	return false;
}
void updateDisplayIntensity(){
	uint16_t tmpBuff=4, tmpX=5, tmpY=30, tmpW=150, tmpH=4;
	uint16_t bgColor=DARKGRAY, fgColor = WHITE, fgColorOk = WHITE, fgColorCaution = YELLOW, fgColorDanger = RED;
	uint16_t tmpActiveX = 0;
	for(uint8_t n=0; n<NUM_TENS_CHANNELS; n++){
		uint16_t tmpIntensity = tens.tensChan[n + TENS_INDEX_START].curIntensityPct;
		_lastDisplayedIntensity[n] = tmpIntensity;
		if(tmpIntensity < 51){
			fgColor = fgColorOk;
		}else if(tmpIntensity < 75){
			fgColor = fgColorCaution;
		}else{
			fgColor = fgColorDanger;
		}
		tmpActiveX = (uint16_t)mapi16(tmpIntensity, 0, 100, 0, tmpW, true);
		//Fill the foreground color (active)
		//ST7735_FillRect(&lcd, tmpX, tmpY, tmpActiveX, tmpH, fgColor);
		ST7735_FillRectCanvas(&canScreen, tmpX, tmpY, tmpActiveX, tmpH, fgColor);
		//Fill the background color (inactive)
		ST7735_FillRectCanvas(&canScreen, tmpX + tmpActiveX, tmpY, tmpW - tmpActiveX, tmpH, bgColor);
		//Advance Y for the next bar
		tmpY += tmpH + tmpBuff;
	}
	_IntensityLastDisplayedTime = HAL_GetTick();
}

bool displayChanEnabledNeedsUpdated(){
	uint16_t colorActive=GREEN, colorStopped=RED, colorPaused=YELLOW;
	uint16_t newChanColor = 0;

	for(uint8_t n=0; n<NUM_CHANNELS; n++){
		switch(tens.curProgStatus[n].progState){
		case progState_Unknown:
		case progState_Empty:
		case progState_Stopped:
			newChanColor = colorStopped;
			break;
		case progState_Paused:
			newChanColor = colorPaused;
			break;
		case progState_Running:
		case progState_LineComplete:
			newChanColor = colorActive;
			break;
		case progState_End:
			newChanColor = colorStopped;
			break;
		}

		if(_lastChanStateColor[n] != newChanColor){
			return true;
		}
	}
	return false;
}

void updateDisplayChanEnabled(){
	uint16_t tmpXStart=4, tmpRad=3, tmpY=14;
	uint16_t tmpXGrpInterval=20, tmpXChanInterval=(tmpRad*2)+2;
	uint16_t xc=tmpXStart+tmpRad, yc=tmpY+tmpRad;
	uint16_t colorPolNormal=DARKGRAY, colorPolReversed=WHITE;
	uint16_t colorOutline=colorPolNormal;
	uint16_t colorActive=GREEN, colorStopped=RED, colorPaused=YELLOW;
	uint16_t newChanColor = colorOutline;

	for(uint8_t n=0; n<NUM_CHANNELS; n++){

		switch(tens.curProgStatus[n].progState){
		case progState_Unknown:
		case progState_Empty:
		case progState_Stopped:
			newChanColor = colorStopped;
			break;
		case progState_Paused:
			newChanColor = colorPaused;
			break;
		case progState_Running:
		case progState_LineComplete:
			newChanColor = colorActive;
			break;
		case progState_End:
			newChanColor = colorStopped;
			break;
		}

		colorOutline = (tens.tensChan[n].polaritySwapped == false ? colorPolNormal : colorPolReversed);

		ST7735_FillCircleCanvas(&canScreen, xc, yc, tmpRad, newChanColor);
		ST7735_DrawCircleCanvas(&canScreen, xc, yc, tmpRad, colorOutline);
		_lastChanStateColor[n] = newChanColor;

		if( (n==MOT_INDEX_START-1) || (n==TENS_INDEX_START-1) || (n==AUX_INDEX_START-1)){
			xc+= tmpXGrpInterval;
		}else{
			xc += tmpXChanInterval;
		}
	}
	_chanStateLastDisplayedTime = HAL_GetTick();
}

bool displayAudioEqNeedsUpdated(void){
	if(_displayEqEnabled == false){
		return false;
	}

	for(uint8_t n=0; n<NUM_FREQ_BANDS; n++){
		if(_lastDisplayedAudioEq[n] != tens.audioRef->freqBandVal[n])
			return true;
	}
	return false;
}

void updateDisplayAudioEq(void){
	if(_displayEqEnabled == false){
		return;
	}

	uint16_t tmpBarH=20, tmpY=79-tmpBarH, tmpBarW=(160 / (NUM_FREQ_BANDS+2) ); //Add 1 spot for total and a blank spacer.
	uint16_t colorInactive=GRAY;
	uint16_t colorActive = GREEN;

	uint16_t curX = 0;
	uint16_t activeH = 0;
	uint16_t inActiveH = 0;
	for(uint8_t n=0; n<(NUM_FREQ_BANDS+2); n++){
		curX = n * tmpBarW;
		if(n<NUM_FREQ_BANDS){
			//EQ display for freq band
			activeH = tens.audioRef->freqBandVal[n] / (100 / tmpBarH);
			_lastDisplayedAudioEq[n] = tens.audioRef->freqBandVal[n];
		}else if(n == NUM_FREQ_BANDS){
			//Empty space
			activeH = 0;
		}else{
			//Total audio level
			activeH = tens.audioRef->audioValTotal / (100 / tmpBarH);
		}
		activeH = (activeH <= 20 ? activeH : 20);
		inActiveH = 20 - activeH;
		inActiveH = (inActiveH <= 20 ? inActiveH : 20);
		ST7735_FillRectCanvas(&canScreen, curX, tmpY, tmpBarW, inActiveH, colorInactive);
		ST7735_FillRectCanvas(&canScreen, curX, tmpY + inActiveH, tmpBarW, activeH, colorActive);
	}
}

void initTens(){

#if DEVICE_BOARD==1 //"TEN2410ADB"

	//Init the top-level object
#ifdef DEVICE_BOARD_B
	TensInit(&tens, &htim7, VBOOST_EN_GPIO_Port, VBOOST_EN_Pin, 1, flashStartAddress_TensProgs, speedSlowOutMin, speedFastOutMax * 2);
#endif
#ifdef DEVICE_BOARD_E
	TensInitWithVEnable(&tens, &htim7, VBOOST_EN_GPIO_Port, VBOOST_EN_Pin, 1, TENS_V_EN_GPIO_Port, TENS_V_EN_Pin, 1, flashStartAddress_TensProgs, speedSlowOutMin, speedFastOutMax * 2);
#endif

	//init the tens channels
	HAL_DAC_Start(&hdac1, DAC_CHANNEL_1);
	HAL_DAC_Start(&hdac1, DAC_CHANNEL_2);

	//Set the pwm output channels to pwm output mode (PH/EN)
	HAL_GPIO_WritePin(MOTOR1_MODE_GPIO_Port, MOTOR1_MODE_Pin, GPIO_PIN_SET);
	HAL_GPIO_WritePin(MOTOR2_MODE_GPIO_Port, MOTOR2_MODE_Pin, GPIO_PIN_SET);

	//==
	////init the motor channels
	//tens.numChans += 2;
	//TensInitMotorChannel(&tens.tensChan[1], MOTOR1_DIR_GPIO_Port, MOTOR1_DIR_Pin, &htim3, &TIM3->CCR1, TIM_CHANNEL_1, 10, 100, 10, 1000);
	//TensInitMotorChannel(&tens.tensChan[2], MOTOR2_DIR_GPIO_Port, MOTOR2_DIR_Pin, &htim3, &TIM3->CCR2, TIM_CHANNEL_2, 10, 100, 10, 1000);
	//
	////init the tens channels
	//tens.numChans += 2;
	//TensInitTensChannel(&tens.tensChan[3], PORT_E4_GPIO_Port, PORT_E4_Pin, &htim4, &TIM4->CCR2, TIM_CHANNEL_2, &hdac1, DAC_CHANNEL_1, 830, 1190, 300, 10, 1000);
	//TensInitTensChannel(&tens.tensChan[4], PORT_E3_GPIO_Port, PORT_E3_Pin, &htim4, &TIM4->CCR4, TIM_CHANNEL_4, &hdac1, DAC_CHANNEL_2, 830, 1190, 300, 10, 1000);
	//
	////init the Aux channels
	//tens.numChans += NUM_AUX_CHANNELS;
	//TensInitAuxChannel(&tens.tensChan[5], 5, 2000);
	//TensInitAuxChannel(&tens.tensChan[6], 5, 2000);
	//==


	//init the motor channels
	TensInitMotorChannel(&tens.tensChan[tens.numChans++], MOTOR1_DIR_GPIO_Port, MOTOR1_DIR_Pin, &htim3, &TIM3->CCR1, TIM_CHANNEL_1, 10, 100, speedSlowOutMin, speedFastOutMax);
	TensInitMotorChannel(&tens.tensChan[tens.numChans++], MOTOR2_DIR_GPIO_Port, MOTOR2_DIR_Pin, &htim3, &TIM3->CCR2, TIM_CHANNEL_2, 10, 100, speedSlowOutMin, speedFastOutMax);

	//init the tens channels
#ifdef DEVICE_BOARD_B
	TensInitTensChannel(&tens, tens.numChans++, TENS_POL_CH2_GPIO_Port, TENS_POL_CH2_Pin, &htim4, &TIM4->CCR2, TIM_CHANNEL_2, &hdac1, DAC_CHANNEL_1, 830, 1190, 300, speedSlowOutMin, speedFastOutMax);
	TensInitTensChannel(&tens, tens.numChans++, TENS_POL_CH3_GPIO_Port, TENS_POL_CH3_Pin, &htim4, &TIM4->CCR4, TIM_CHANNEL_4, &hdac1, DAC_CHANNEL_2, 830, 1190, 300, speedSlowOutMin, speedFastOutMax);
#endif
#ifdef DEVICE_BOARD_E
//	TensInitTensChannel(&tens, tens.numChans++, TENS_POL_CH1_GPIO_Port, TENS_POL_CH1_Pin, &htim17, &TIM17->CCR1, TIM_CHANNEL_1, &hdac1, DAC_CHANNEL_1, 800, 1200, 300, speedSlowOutMin, speedFastOutMax);
//	TensInitTensChannel(&tens, tens.numChans++, TENS_POL_CH2_GPIO_Port, TENS_POL_CH2_Pin, &htim4, &TIM4->CCR2, TIM_CHANNEL_2, &hdac1, DAC_CHANNEL_2, 800, 1200, 300, speedSlowOutMin, speedFastOutMax);
//	TensInitTensChannel(&tens, tens.numChans++, TENS_POL_CH3_GPIO_Port, TENS_POL_CH3_Pin, &htim4, &TIM4->CCR4, TIM_CHANNEL_4, &hdac1, DAC_CHANNEL_2, 800, 1200, 300, speedSlowOutMin, speedFastOutMax);

	//Original channel order:
//	TensInitTensChannel(&tens, tens.numChans++, TENS_POL_CH1_GPIO_Port, TENS_POL_CH1_Pin, &htim17, &TIM17->CCR1, TIM_CHANNEL_1, &hdac1, DAC_CHANNEL_1, 800, 1300, 300, speedSlowOutMin, speedFastOutMax); //800, 1400, 300
//	TensInitTensChannel(&tens, tens.numChans++, TENS_POL_CH2_GPIO_Port, TENS_POL_CH2_Pin, &htim4, &TIM4->CCR2, TIM_CHANNEL_2, &hdac1, DAC_CHANNEL_2, 800, 1300, 300, speedSlowOutMin, speedFastOutMax);
//	TensInitTensChannel(&tens, tens.numChans++, TENS_POL_CH3_GPIO_Port, TENS_POL_CH3_Pin, &htim4, &TIM4->CCR4, TIM_CHANNEL_4, &hdac1, DAC_CHANNEL_2, 800, 1300, 300, speedSlowOutMin, speedFastOutMax);

	//Reverse Channel order (chan 1, 2, 3 left-to-right when facing the front of unit)
	TensInitTensChannel(&tens, tens.numChans++, TENS_POL_CH3_GPIO_Port, TENS_POL_CH3_Pin, &htim4, &TIM4->CCR4, TIM_CHANNEL_4, &hdac1, DAC_CHANNEL_2, 800, 1300, 300, speedSlowOutMin, speedFastOutMax);
	TensInitTensChannel(&tens, tens.numChans++, TENS_POL_CH2_GPIO_Port, TENS_POL_CH2_Pin, &htim4, &TIM4->CCR2, TIM_CHANNEL_2, &hdac1, DAC_CHANNEL_2, 800, 1300, 300, speedSlowOutMin, speedFastOutMax);
	TensInitTensChannel(&tens, tens.numChans++, TENS_POL_CH1_GPIO_Port, TENS_POL_CH1_Pin, &htim17, &TIM17->CCR1, TIM_CHANNEL_1, &hdac1, DAC_CHANNEL_1, 800, 1300, 300, speedSlowOutMin, speedFastOutMax); //800, 1400, 300


#endif
	//init the Aux channels
	for(uint8_t n = 0; n<NUM_AUX_CHANNELS; n++){
		//TensInitAuxChannel(&tens.tensChan[tens.numChans++], 5, 2000);
		TensInitAuxChannel(&tens.tensChan[tens.numChans++], speedSlowOutMin / 2, speedFastOutMax * 2);
	}


	//Load the default startup system program:
	if(TensFileExists(&tens, settings.vars.initialSystemProgNum)){
		TensLoadFile(&tens, 0, settings.vars.initialSystemProgNum);
	}

	//Misc
	free(_lastDisplayedIntensity);
	_lastDisplayedIntensity = (uint8_t*)calloc(NUM_TENS_CHANNELS, sizeof(uint8_t));

#else
	//Some text here.
#endif
}

void initPushbuttons(void){
#ifdef DEVICE_BOARD_B
	//powerPb (x1), PB2-14 (x13), encoders (x5), joysticks (x2) = 21
	PB_Init(&pb[0], PB_POWER_GPIO_Port, PB_POWER_Pin, 0, 10, 10, 1000, 0, pbMode_Momentary);
	PB_Init(&pb[1], PB2_GPIO_Port, PB2_Pin, 0, 10, 10, 1000, 250, pbMode_Momentary);
	PB_Init(&pb[2], PB3_GPIO_Port, PB3_Pin, 0, 10, 10, 1000, 250, pbMode_Momentary);
	PB_Init(&pb[3], PB4_GPIO_Port, PB4_Pin, 0, 10, 10, 1000, 250, pbMode_Momentary);
#endif
#ifdef DEVICE_BOARD_E
	//powerPb (x1), PB1-10 (x9)
	PB_Init(&pb[0], PB_POWER2_GPIO_Port, PB_POWER2_Pin, 0, 10, 10, 1000, 0, pbMode_Momentary);
	//PB2-10

	pbMatrix.numRows = 3;
	pbMatrix._rowPort[0] = PB_ROW1_GPIO_Port;
	pbMatrix._rowPort[1] = PB_ROW2_GPIO_Port;
	pbMatrix._rowPort[2] = PB_ROW3_GPIO_Port;
	pbMatrix._rowPin[0] = PB_ROW1_Pin;
	pbMatrix._rowPin[1] = PB_ROW2_Pin;
	pbMatrix._rowPin[2] = PB_ROW3_Pin;

	pbMatrix.numCols = 3;
	pbMatrix._colPort[0] = PB_COL1_GPIO_Port;
	pbMatrix._colPort[1] = PB_COL2_GPIO_Port;
	pbMatrix._colPort[2] = PB_COL3_GPIO_Port;
	pbMatrix._colPin[0] = PB_COL1_Pin;
	pbMatrix._colPin[1] = PB_COL2_Pin;
	pbMatrix._colPin[2] = PB_COL3_Pin;

	PB_Init_Matrix(&pb[1], &pbMatrix, 0, 0, 0, 10, 10, 1000, 250, pbMode_Momentary);
	PB_Init_Matrix(&pb[2], &pbMatrix, 1, 0, 0, 10, 10, 1000, 250, pbMode_Momentary);
	PB_Init_Matrix(&pb[3], &pbMatrix, 2, 0, 0, 10, 10, 1000, 250, pbMode_Momentary);
	PB_Init_Matrix(&pb[4], &pbMatrix, 0, 1, 0, 10, 10, 1000, 250, pbMode_Momentary);
	PB_Init_Matrix(&pb[5], &pbMatrix, 1, 1, 0, 10, 10, 1000, 250, pbMode_Momentary);
	PB_Init_Matrix(&pb[6], &pbMatrix, 2, 1, 0, 10, 10, 1000, 250, pbMode_Momentary);
	PB_Init_Matrix(&pb[7], &pbMatrix, 0, 2, 0, 10, 10, 1000, 250, pbMode_Momentary);
	PB_Init_Matrix(&pb[8], &pbMatrix, 1, 2, 0, 10, 10, 1000, 250, pbMode_Momentary);
	PB_Init_Matrix(&pb[9], &pbMatrix, 2, 2, 0, 10, 10, 1000, 250, pbMode_Momentary);

#endif
}

void checkPushbuttons(void){

	PB_ReadMatrix(&pbMatrix);

//	for(uint8_t n=0; n<3; n++){
//		tmpPB[(n*3) + 0] = pbMatrix.inData[n] & (1 << 0);
//		tmpPB[(n*3) + 1] = pbMatrix.inData[n] & (1 << 1);
//		tmpPB[(n*3) + 2] = pbMatrix.inData[n] & (1 << 2);
//	}


	for(uint8_t n=0; n< PB_COUNT; n++){
		uint8_t pbEvent = PB_CheckForEvent(&pb[n]);

		if(pbEvent != pbEvent_none){
			pushButtonEvent(n, pbEvent);
		}
	}
}

void pushButtonEvent(uint8_t pbId, uint8_t eventId){
	displayLastActivityTime = HAL_GetTick();

	if(pbId == 0){ //PB1 --> Power Pb
//		checkPower();  //Let the checkPower function handle it.
		return;
	}

#ifdef DEVICE_BOARD_B
	tmpBoard = "B";

	if(pbId == 1)	//Board B, PB2
		//turn down intensity
		bool doUpdate = false;
		uint8_t stepVal = 1;
		uint8_t newVal[NUM_TENS_CHANNELS] = {0};
		switch(eventId){
		case pbEvent_longpress_Pressed:
		case pbEvent_longpress_Repeat:
			stepVal = 10;//20;
		case pbEvent_shortpress_Pressed:
			doUpdate = true;
			for(uint8_t n=0; n<NUM_TENS_CHANNELS; n++){
				newVal[n] = tens.tensChan[n + TENS_INDEX_START].curIntensityPct;
				newVal[n] = (newVal[n] >= stepVal ? newVal[n] - stepVal : 0);
			}
			break;
		default:
			break;
		}
		if(doUpdate == true){
			changeLedMode(Led_Flash_Medium, 0, 0, 10, 1);
			for(uint8_t n=0; n<NUM_TENS_CHANNELS; n++){
				//TensSetDacVal(&tens.tensChan[n + TENS_INDEX_START], newVal[n]);
				TensSetDacVal(&tens, (n + TENS_INDEX_START), newVal[n]);
			}
		}
		return;
	}

	if(pbId == 2){  //PB3
		//turn up intensity
		bool doUpdate = false;
		uint8_t newVal[NUM_TENS_CHANNELS] = {0};
		uint8_t stepVal = 1;
		uint8_t maxVal = 100;
		switch(eventId){
		case pbEvent_longpress_Pressed:
		case pbEvent_longpress_Repeat:
			stepVal = 10;
		case pbEvent_shortpress_Pressed:
			doUpdate = true;
			for(uint8_t n=0; n<NUM_TENS_CHANNELS; n++){
				newVal[n] = tens.tensChan[n + TENS_INDEX_START].curIntensityPct;
				newVal[n] = (newVal[n] <= (maxVal - stepVal) ? newVal[n] + stepVal : maxVal);
			}
			break;
		default:
			break;
		}
		if(doUpdate == true){
			changeLedMode(Led_Flash_Medium, 10, 0, 0, 1);
			for(uint8_t n=0; n<NUM_TENS_CHANNELS; n++){
				//TensSetDacVal(&tens.tensChan[n + TENS_INDEX_START], newVal[n]);
				TensSetDacVal(&tens, (n + TENS_INDEX_START), newVal[n]);
			}
		}
		return;
	}



	if(pbId == 3){	//PB4
		bool tmpChangeEnable = false;
		bool tmpEnVal = false;

		switch(eventId){
		case pbEvent_shortpress_Pressed:
			tmpChangeEnable = true;
			tmpEnVal = !(tens.tensChan[0].chanEnabled);
			break;
		case pbEvent_longpress_Pressed:
			//changeLedMode(Led_Flash_Fast, 0, 0, 10, 1);
			tmpChangeEnable = true;
			tmpEnVal = false;
			break;
		case pbEvent_shortpress_Released:
			//changeLedMode(Led_Flash_Medium, 10, 0, 0, 1);
			break;
		case pbEvent_longpress_Released:
			//changeLedMode(Led_Flash_Medium, 0, 10, 0, 1);
			break;
		default:
		}

		if(tmpChangeEnable == true){
			TensvBoostEnable(&tens, tmpEnVal);
			TensEnableChannel(&tens, 0, tmpEnVal);
			if(tmpEnVal == true){
				changeLedMode(Led_Flash_Fast, 0, 10, 0, 10);
			}else{
				changeLedMode(Led_On_5Sec, 10, 0, 0, 0);
			}
		}
		return;
	}

#endif
#ifdef DEVICE_BOARD_E

	if( (pbId == 2) || (pbId == 3) ){ 	//Board E, PB3 or PB4
		//Change the master speed.
		uint16_t tmpCurSpeed = tens.tensChan[0].chanSpeed;
		uint16_t pctChange = 0;
		switch(eventId){
		case pbEvent_longpress_Pressed:
		case pbEvent_longpress_Repeat:
		case pbEvent_shortpress_Pressed:
			pctChange = 10;
			break;
		default:
			break;
		}

		if(pctChange > 0){
			//The pushbuttons will change the value by 10% per press.  We need to convert this % to a value we can send to
			//  TensSetSpeed, which expects a value between speedMin (~10) and speedMax (~500).  So we need to calculate how much
			//  10% of the speed value is, depending on the current speedMin/speedMax settings, and depending on if the curSpeed
			//  is currently above 100 or below 100.

			uint8_t	 slowButtonId = 3;
			uint16_t stepVal = 0;
//			uint16_t slowChange = ( (100 - speedSlowOutMin) / (10-1));
//			uint16_t fastChange = ( (speedFastOutMax - 100) / (10-1));
			uint16_t slowChange = ( (100 - tens.tensChan[0].chanMinSpeed) / (10-1));
			uint16_t fastChange = ( (tens.tensChan[0].chanMaxSpeed - 100) / (10-1));
			uint16_t newSpeed = 0;

			if(tmpCurSpeed < 100){
				stepVal = slowChange;
			}else if(tmpCurSpeed == 100){
				stepVal = (pbId == slowButtonId ? slowChange : fastChange);
			}else{
				stepVal = fastChange;
			}

			if(pbId == slowButtonId){
				//Request to slow down.
				if(tmpCurSpeed > 100){
					if(tmpCurSpeed - stepVal < 100){
						newSpeed = 100;
					}else{
						newSpeed = tmpCurSpeed - stepVal;
					}
				}else{
					if(speedSlowOutMin + stepVal < tmpCurSpeed){
						newSpeed = tmpCurSpeed - stepVal;
					}else{
						newSpeed = speedSlowOutMin;
					}
				}
			}else{
				//Request to speed up.
				if(tmpCurSpeed < 100){
					if(tmpCurSpeed + stepVal > 100){
						newSpeed = 100;
					}else{
						newSpeed = tmpCurSpeed + stepVal;
					}
				}else{
					if(speedFastOutMax - stepVal > tmpCurSpeed){
						newSpeed = tmpCurSpeed + stepVal;
					}else{
						newSpeed = speedFastOutMax;
					}
				}
			}

			if(newSpeed > 0){
				TensSetSpeed(&tens, 0, newSpeed);
			}

		}



	}

	if( (pbId == 6) || (pbId == 9) ){ 	//Board E, PB7 or PB10
		//turn down intensity for one of the two Tens DAC's
		uint8_t tmpChan = (pbId == 6 ? TENS_INDEX_START : TENS_INDEX_START + 2);

		bool doUpdate = false;
		uint8_t stepVal = 1;
		uint8_t newVal = 0;
		switch(eventId){
		case pbEvent_longpress_Pressed:
		case pbEvent_longpress_Repeat:
			stepVal = 10;//20;
		case pbEvent_shortpress_Pressed:
			doUpdate = true;
			newVal = tens.tensChan[tmpChan].curIntensityPct;
			newVal = (newVal >= stepVal ? newVal - stepVal : 0);
			break;
		default:
			break;
		}
		if(doUpdate == true){
			changeLedMode(Led_Flash_Medium, 0, 0, 10, 1);
			TensSetTensIntensity(&tens, tmpChan, newVal);
		}
		return;
	}

	if( (pbId == 5) || (pbId == 8) ){ 	//Board E, PB6 or PB9
		//turn up intensity for one of the two Tens DAC's
		uint8_t tmpChan = (pbId == 5 ? TENS_INDEX_START : TENS_INDEX_START + 2);

		bool doUpdate = false;
		uint8_t newVal = 0;
		uint8_t stepVal = 1;
		uint8_t maxVal = 100;
		switch(eventId){
		case pbEvent_longpress_Pressed:
		case pbEvent_longpress_Repeat:
			stepVal = 10;
		case pbEvent_shortpress_Pressed:
			doUpdate = true;
			newVal = tens.tensChan[tmpChan].curIntensityPct;
			newVal = (newVal <= (maxVal - stepVal) ? newVal + stepVal : maxVal);
			break;
		default:
			break;
		}
		if(doUpdate == true){
			changeLedMode(Led_Flash_Medium, 10, 0, 0, 1);
			TensSetTensIntensity(&tens, tmpChan, newVal);
		}
		return;
	}



	if(pbId == 4){	//PB5
		bool tmpChangeEnable = false;
		bool tmpEnVal = false;

		switch(eventId){
		case pbEvent_shortpress_Pressed:
			tmpChangeEnable = true;
			tmpEnVal = !(tens.tensChan[0].chanEnabled);
			break;
		case pbEvent_longpress_Pressed:
			//changeLedMode(Led_Flash_Fast, 0, 0, 10, 1);
			tmpChangeEnable = true;
			tmpEnVal = false;
			break;
		case pbEvent_shortpress_Released:
			//changeLedMode(Led_Flash_Medium, 10, 0, 0, 1);
			break;
		case pbEvent_longpress_Released:
			//changeLedMode(Led_Flash_Medium, 0, 10, 0, 1);
			break;
		default:
		}

		if(tmpChangeEnable == true){
			TensvBoostEnable(&tens, tmpEnVal);
			TensEnableChannel(&tens, 0, tmpEnVal);
			if(tmpEnVal == true){
				changeLedMode(Led_Flash_Fast, 0, 10, 0, 10);
			}else{
				changeLedMode(Led_On_5Sec, 10, 0, 0, 0);
			}
		}
		return;
	}

	if(pbId == 1){	//PB2
		//Current channel selected
		switch(eventId){
		case pbEvent_shortpress_Pressed:
			break;
		case pbEvent_longpress_Pressed:
			break;
		case pbEvent_shortpress_Released:
			curChanPbSelected = (curChanPbSelected < (TENS_INDEX_START + NUM_TENS_CHANNELS) ? curChanPbSelected + 1 : 1);
			changeLedMode(Led_Flash_Fast, 0, 10, 0, 2);
			break;
		case pbEvent_longpress_Released:
			break;
		default:
		}
		return;
	}

	if(pbId == 7){	//PB8
		switch(eventId){
		case pbEvent_shortpress_Pressed:
			if( (curChanPbSelected >= TENS_INDEX_START) && (curChanPbSelected < (TENS_INDEX_START + NUM_TENS_CHANNELS)) ){
				bool tmpCurPol = tens.tensChan[curChanPbSelected].polaritySwapped;
				TensSwapPolarity(&tens.tensChan[curChanPbSelected], (tmpCurPol == true ? false : true));
				changeLedMode(Led_Flash_Fast, 0, 10, 0, 2);
			}
			break;
		case pbEvent_longpress_Pressed:
			break;
		case pbEvent_shortpress_Released:
			break;
		case pbEvent_longpress_Released:
			break;
		default:
		}
		return;
	}

#endif




}

void initImu(void){
	ImuInit(&imu, &hspi1, IMU_CS_GPIO_Port, IMU_CS_Pin, IMU_INT1_Pin, IMU_INT2_Pin);
	imu.stepIntPin = imu.int2Pin;
	imu.gyroIntPin = imu.int1Pin;
	imu.stepCountEnabled = true;

}

void checkImu(void){
	bool displayUpdateNeeded = false;
	bool forceReadSteps = (HAL_GetTick() - imuLastStepReadTime > 10000);
	bool forceReadGyro = (HAL_GetTick() - imuLastGyroReadTime > 1000);

	if( (imu.stepCountEnabled == true) && ( (imu.stepDataReady == true) || (forceReadSteps == true) ) ){
		ImuReadStepCounter(&imu);
		imuLastStepReadTime = HAL_GetTick();
		if(imu.stepCount != imu.stepCountLastDisplayed){
			displayUpdateNeeded = true;
		}
	}


	if( (imu.gyroDataReady == true) || (forceReadGyro == true) ){
		ImuGet6DOrientation(&imu);
		ImuDetermineUpDirection(&imu);
		imuLastGyroReadTime = HAL_GetTick();
		if(imu.upDirection != imu.upDirectionLastDisplayed){
			displayUpdateNeeded = true;
		}
		if(imu.upDirection == UP_Z_POS){
			displayLastActivityTime = HAL_GetTick();
		}
		ImuGetGlobalZRotation(&imu);
		if(floor(imu.globalZRotation) != floor(imu.globalZRotationLastDisplayed)) displayUpdateNeeded = true;
	}

	if(displayUpdateNeeded == true){
		updateDisplayStatusBar(true);
	}

}




/* USER CODE END 0 */

/**
  * @brief  The application entry point.
  * @retval int
  */
int main(void)
{

  /* USER CODE BEGIN 1 */

  /* USER CODE END 1 */

  /* MCU Configuration--------------------------------------------------------*/

  /* Reset of all peripherals, Initializes the Flash interface and the Systick. */
  HAL_Init();

  /* USER CODE BEGIN Init */

  /* USER CODE END Init */

  /* Configure the system clock */
  SystemClock_Config();

  /* Configure the peripherals common clocks */
  PeriphCommonClock_Config();

  /* USER CODE BEGIN SysInit */

  /* USER CODE END SysInit */

  /* Initialize all configured peripherals */
  MX_GPIO_Init();
  MX_DMA_Init();
  MX_ADC1_Init();
  MX_DAC1_Init();
  MX_DFSDM1_Init();
  MX_LPUART1_UART_Init();
  MX_USART1_UART_Init();
  MX_USART2_UART_Init();
  MX_RTC_Init();
  MX_SDMMC1_SD_Init();
  MX_SPI1_Init();
  MX_SPI2_Init();
  MX_TIM1_Init();
  MX_TIM2_Init();
  MX_TIM3_Init();
  MX_TIM4_Init();
  MX_TIM16_Init();
  MX_TIM17_Init();
  MX_TIM6_Init();
  MX_TIM7_Init();
  MX_CRC_Init();
  MX_USB_DEVICE_Init();
  MX_FATFS_Init();
  /* USER CODE BEGIN 2 */


  // Mount
  FRESULT res = f_mount(&SDFatFS, (TCHAR const*)SDPath, 1);
  if (res != FR_OK) {
	  volatile FRESULT debug_res = res;  // Should be 1
	  volatile uint32_t sd_error = hsd1.ErrorCode;
	  //Error_Handler();
	  //while(1);
  }










  calculateFlashAddresses();
  loadSettings();
  initTens();
  initPushbuttons();
//  initEncoders();


  //init audio
  //initAudio();
  AudioInit(&haud, &hdfsdm1_filter0);
  tens.audioRef = &haud;


  //phtimLed = &htim2;
  phspiRadio = &hspi2;

  //init the RGB LED
  HAL_TIM_PWM_Start(phtimLed, LED_RCHAN);
  HAL_TIM_PWM_Start(phtimLed, LED_GCHAN);
  HAL_TIM_PWM_Start(phtimLed, LED_BCHAN);

  //init the onboard LCD
  initDisplay();


  ////lsm6dsv16x_single_double_tap();
  initImu();






  //Testing only!
  settings.vars.nodeId = 3;
  saveSettings(true);

  //Testing audio
  //HAL_TIM_Base_Start_IT(&htim6);



  if( RFM69Initialize(&radio, phspiRadio, RFM_CS_GPIO_Port, RFM_CS_Pin, true, RFM69_915MHZ, 3, 25) == true){
	  changeLedMode(Led_Flash_Fast, 0, 10, 0, 5);
	  //changeLedMode(Led_OnSolid, 0, 10, 0, 5);
  }else{
	  changeLedMode(Led_Flash_Fast, 25, 0, 0, 50);
  }


  updateDisplayIntensity(true);

  tens.imuRef = &imu;


  /* USER CODE END 2 */

  /* Infinite loop */
  /* USER CODE BEGIN WHILE */
  while (1)
  {
    /* USER CODE END WHILE */

    /* USER CODE BEGIN 3 */
	  AudioCheck(&haud);

	  checkAnalog();
	  checkPower();
	  checkPushbuttons();
////	  readEncoders();
	  checkLed();
	  checkUsbRx();
	  checkRadioRx();
////	  uint8_t tmpRssiDisplay = radio.rxMsg.rssi;
	  //updateDisplay(false);
	  checkDisplay(false);


	  checkImu();

	  tensMutex = 1;
	  TensLoop(&tens);
	  tensMutex = 0;


//	  //=====
//	  static uint32_t debugTimer = 0;
//	  if (HAL_GetTick() - debugTimer > 1000) {  // Every second
//	      debugTimer = HAL_GetTick();
//
//	      bool anyNonZero = false;
//	      for (uint32_t i = 0; i < 32; i++) {  // Check first 32 samples
//	          if (haud.audio_buffer_interleaved[i] != 0) {
//	              anyNonZero = true;
//	              break;
//	          }
//	      }
//
//	      // Send via USB CDC (or UART if you have one)
//	      char dbg[128];
//	      sprintf(dbg, "Audio buffer non-zero: %d | First sample: %ld | Total: %d\r\n",
//	              anyNonZero, haud.audio_buffer_interleaved[0], haud.audioValTotal);
//	      //CDC_Transmit_FS((uint8_t*)dbg, strlen(dbg));
//	  }
//	  //=====

  }
  /* USER CODE END 3 */
}

/**
  * @brief System Clock Configuration
  * @retval None
  */
void SystemClock_Config(void)
{
  RCC_OscInitTypeDef RCC_OscInitStruct = {0};
  RCC_ClkInitTypeDef RCC_ClkInitStruct = {0};

  /** Configure the main internal regulator output voltage
  */
  if (HAL_PWREx_ControlVoltageScaling(PWR_REGULATOR_VOLTAGE_SCALE1) != HAL_OK)
  {
    Error_Handler();
  }

  /** Configure LSE Drive Capability
  */
  HAL_PWR_EnableBkUpAccess();
  __HAL_RCC_LSEDRIVE_CONFIG(RCC_LSEDRIVE_LOW);

  /** Initializes the RCC Oscillators according to the specified parameters
  * in the RCC_OscInitTypeDef structure.
  */
  RCC_OscInitStruct.OscillatorType = RCC_OSCILLATORTYPE_HSE|RCC_OSCILLATORTYPE_LSE;
  RCC_OscInitStruct.HSEState = RCC_HSE_ON;
  RCC_OscInitStruct.LSEState = RCC_LSE_ON;
  RCC_OscInitStruct.PLL.PLLState = RCC_PLL_ON;
  RCC_OscInitStruct.PLL.PLLSource = RCC_PLLSOURCE_HSE;
  RCC_OscInitStruct.PLL.PLLM = 2;
  RCC_OscInitStruct.PLL.PLLN = 10;
  RCC_OscInitStruct.PLL.PLLP = RCC_PLLP_DIV7;
  RCC_OscInitStruct.PLL.PLLQ = RCC_PLLQ_DIV2;
  RCC_OscInitStruct.PLL.PLLR = RCC_PLLR_DIV2;
  if (HAL_RCC_OscConfig(&RCC_OscInitStruct) != HAL_OK)
  {
    Error_Handler();
  }

  /** Initializes the CPU, AHB and APB buses clocks
  */
  RCC_ClkInitStruct.ClockType = RCC_CLOCKTYPE_HCLK|RCC_CLOCKTYPE_SYSCLK
                              |RCC_CLOCKTYPE_PCLK1|RCC_CLOCKTYPE_PCLK2;
  RCC_ClkInitStruct.SYSCLKSource = RCC_SYSCLKSOURCE_PLLCLK;
  RCC_ClkInitStruct.AHBCLKDivider = RCC_SYSCLK_DIV1;
  RCC_ClkInitStruct.APB1CLKDivider = RCC_HCLK_DIV1;
  RCC_ClkInitStruct.APB2CLKDivider = RCC_HCLK_DIV1;

  if (HAL_RCC_ClockConfig(&RCC_ClkInitStruct, FLASH_LATENCY_4) != HAL_OK)
  {
    Error_Handler();
  }
}

/**
  * @brief Peripherals Common Clock Configuration
  * @retval None
  */
void PeriphCommonClock_Config(void)
{
  RCC_PeriphCLKInitTypeDef PeriphClkInit = {0};

  /** Initializes the peripherals clock
  */
  PeriphClkInit.PeriphClockSelection = RCC_PERIPHCLK_SAI1|RCC_PERIPHCLK_USB
                              |RCC_PERIPHCLK_SDMMC1;
  PeriphClkInit.Sai1ClockSelection = RCC_SAI1CLKSOURCE_PLLSAI1;
  PeriphClkInit.UsbClockSelection = RCC_USBCLKSOURCE_PLLSAI1;
  PeriphClkInit.Sdmmc1ClockSelection = RCC_SDMMC1CLKSOURCE_PLLSAI1;
  PeriphClkInit.PLLSAI1.PLLSAI1Source = RCC_PLLSOURCE_HSE;
  PeriphClkInit.PLLSAI1.PLLSAI1M = 2;
  PeriphClkInit.PLLSAI1.PLLSAI1N = 12;
  PeriphClkInit.PLLSAI1.PLLSAI1P = RCC_PLLP_DIV7;
  PeriphClkInit.PLLSAI1.PLLSAI1Q = RCC_PLLQ_DIV4;
  PeriphClkInit.PLLSAI1.PLLSAI1R = RCC_PLLR_DIV4;
  PeriphClkInit.PLLSAI1.PLLSAI1ClockOut = RCC_PLLSAI1_SAI1CLK|RCC_PLLSAI1_48M2CLK;
  if (HAL_RCCEx_PeriphCLKConfig(&PeriphClkInit) != HAL_OK)
  {
    Error_Handler();
  }
}

/**
  * @brief ADC1 Initialization Function
  * @param None
  * @retval None
  */
static void MX_ADC1_Init(void)
{

  /* USER CODE BEGIN ADC1_Init 0 */

  /* USER CODE END ADC1_Init 0 */

  ADC_MultiModeTypeDef multimode = {0};
  ADC_ChannelConfTypeDef sConfig = {0};

  /* USER CODE BEGIN ADC1_Init 1 */

  /* USER CODE END ADC1_Init 1 */

  /** Common config
  */
  hadc1.Instance = ADC1;
  hadc1.Init.ClockPrescaler = ADC_CLOCK_SYNC_PCLK_DIV2;
  hadc1.Init.Resolution = ADC_RESOLUTION_12B;
  hadc1.Init.DataAlign = ADC_DATAALIGN_RIGHT;
  hadc1.Init.ScanConvMode = ADC_SCAN_ENABLE;
  hadc1.Init.EOCSelection = ADC_EOC_SINGLE_CONV;
  hadc1.Init.LowPowerAutoWait = DISABLE;
  hadc1.Init.ContinuousConvMode = DISABLE;
  hadc1.Init.NbrOfConversion = 3;
  hadc1.Init.DiscontinuousConvMode = DISABLE;
  hadc1.Init.ExternalTrigConv = ADC_SOFTWARE_START;
  hadc1.Init.ExternalTrigConvEdge = ADC_EXTERNALTRIGCONVEDGE_NONE;
  hadc1.Init.DMAContinuousRequests = DISABLE;
  hadc1.Init.Overrun = ADC_OVR_DATA_PRESERVED;
  hadc1.Init.OversamplingMode = DISABLE;
  if (HAL_ADC_Init(&hadc1) != HAL_OK)
  {
    Error_Handler();
  }

  /** Configure the ADC multi-mode
  */
  multimode.Mode = ADC_MODE_INDEPENDENT;
  if (HAL_ADCEx_MultiModeConfigChannel(&hadc1, &multimode) != HAL_OK)
  {
    Error_Handler();
  }

  /** Configure Regular Channel
  */
  sConfig.Channel = ADC_CHANNEL_16;
  sConfig.Rank = ADC_REGULAR_RANK_1;
  sConfig.SamplingTime = ADC_SAMPLETIME_640CYCLES_5;
  sConfig.SingleDiff = ADC_SINGLE_ENDED;
  sConfig.OffsetNumber = ADC_OFFSET_NONE;
  sConfig.Offset = 0;
  if (HAL_ADC_ConfigChannel(&hadc1, &sConfig) != HAL_OK)
  {
    Error_Handler();
  }

  /** Configure Regular Channel
  */
  sConfig.Channel = ADC_CHANNEL_14;
  sConfig.Rank = ADC_REGULAR_RANK_2;
  if (HAL_ADC_ConfigChannel(&hadc1, &sConfig) != HAL_OK)
  {
    Error_Handler();
  }

  /** Configure Regular Channel
  */
  sConfig.Channel = ADC_CHANNEL_15;
  sConfig.Rank = ADC_REGULAR_RANK_3;
  if (HAL_ADC_ConfigChannel(&hadc1, &sConfig) != HAL_OK)
  {
    Error_Handler();
  }
  /* USER CODE BEGIN ADC1_Init 2 */

  /* USER CODE END ADC1_Init 2 */

}

/**
  * @brief CRC Initialization Function
  * @param None
  * @retval None
  */
static void MX_CRC_Init(void)
{

  /* USER CODE BEGIN CRC_Init 0 */

  /* USER CODE END CRC_Init 0 */

  /* USER CODE BEGIN CRC_Init 1 */

  /* USER CODE END CRC_Init 1 */
  hcrc.Instance = CRC;
  hcrc.Init.DefaultPolynomialUse = DEFAULT_POLYNOMIAL_ENABLE;
  hcrc.Init.DefaultInitValueUse = DEFAULT_INIT_VALUE_ENABLE;
  hcrc.Init.InputDataInversionMode = CRC_INPUTDATA_INVERSION_NONE;
  hcrc.Init.OutputDataInversionMode = CRC_OUTPUTDATA_INVERSION_DISABLE;
  hcrc.InputDataFormat = CRC_INPUTDATA_FORMAT_BYTES;
  if (HAL_CRC_Init(&hcrc) != HAL_OK)
  {
    Error_Handler();
  }
  /* USER CODE BEGIN CRC_Init 2 */

  /* USER CODE END CRC_Init 2 */

}

/**
  * @brief DAC1 Initialization Function
  * @param None
  * @retval None
  */
static void MX_DAC1_Init(void)
{

  /* USER CODE BEGIN DAC1_Init 0 */

  /* USER CODE END DAC1_Init 0 */

  DAC_ChannelConfTypeDef sConfig = {0};

  /* USER CODE BEGIN DAC1_Init 1 */

  /* USER CODE END DAC1_Init 1 */

  /** DAC Initialization
  */
  hdac1.Instance = DAC1;
  if (HAL_DAC_Init(&hdac1) != HAL_OK)
  {
    Error_Handler();
  }

  /** DAC channel OUT1 config
  */
  sConfig.DAC_SampleAndHold = DAC_SAMPLEANDHOLD_DISABLE;
  sConfig.DAC_Trigger = DAC_TRIGGER_NONE;
  sConfig.DAC_OutputBuffer = DAC_OUTPUTBUFFER_ENABLE;
  sConfig.DAC_ConnectOnChipPeripheral = DAC_CHIPCONNECT_DISABLE;
  sConfig.DAC_UserTrimming = DAC_TRIMMING_FACTORY;
  if (HAL_DAC_ConfigChannel(&hdac1, &sConfig, DAC_CHANNEL_1) != HAL_OK)
  {
    Error_Handler();
  }

  /** DAC channel OUT2 config
  */
  if (HAL_DAC_ConfigChannel(&hdac1, &sConfig, DAC_CHANNEL_2) != HAL_OK)
  {
    Error_Handler();
  }
  /* USER CODE BEGIN DAC1_Init 2 */

  /* USER CODE END DAC1_Init 2 */

}

/**
  * @brief DFSDM1 Initialization Function
  * @param None
  * @retval None
  */
static void MX_DFSDM1_Init(void)
{

  /* USER CODE BEGIN DFSDM1_Init 0 */

  /* USER CODE END DFSDM1_Init 0 */

  /* USER CODE BEGIN DFSDM1_Init 1 */

  /* USER CODE END DFSDM1_Init 1 */
  hdfsdm1_filter0.Instance = DFSDM1_Filter0;
  hdfsdm1_filter0.Init.RegularParam.Trigger = DFSDM_FILTER_SW_TRIGGER;
  hdfsdm1_filter0.Init.RegularParam.FastMode = ENABLE;
  hdfsdm1_filter0.Init.RegularParam.DmaMode = ENABLE;
  hdfsdm1_filter0.Init.FilterParam.SincOrder = DFSDM_FILTER_SINC4_ORDER;
  hdfsdm1_filter0.Init.FilterParam.Oversampling = 64;
  hdfsdm1_filter0.Init.FilterParam.IntOversampling = 1;
  if (HAL_DFSDM_FilterInit(&hdfsdm1_filter0) != HAL_OK)
  {
    Error_Handler();
  }
  hdfsdm1_channel1.Instance = DFSDM1_Channel1;
  hdfsdm1_channel1.Init.OutputClock.Activation = ENABLE;
  hdfsdm1_channel1.Init.OutputClock.Selection = DFSDM_CHANNEL_OUTPUT_CLOCK_AUDIO;
  hdfsdm1_channel1.Init.OutputClock.Divider = 5;
  hdfsdm1_channel1.Init.Input.Multiplexer = DFSDM_CHANNEL_EXTERNAL_INPUTS;
  hdfsdm1_channel1.Init.Input.DataPacking = DFSDM_CHANNEL_STANDARD_MODE;
  hdfsdm1_channel1.Init.Input.Pins = DFSDM_CHANNEL_FOLLOWING_CHANNEL_PINS;
  hdfsdm1_channel1.Init.SerialInterface.Type = DFSDM_CHANNEL_SPI_FALLING;
  hdfsdm1_channel1.Init.SerialInterface.SpiClock = DFSDM_CHANNEL_SPI_CLOCK_INTERNAL;
  hdfsdm1_channel1.Init.Awd.FilterOrder = DFSDM_CHANNEL_FASTSINC_ORDER;
  hdfsdm1_channel1.Init.Awd.Oversampling = 1;
  hdfsdm1_channel1.Init.Offset = 0;
  hdfsdm1_channel1.Init.RightBitShift = 0x08;
  if (HAL_DFSDM_ChannelInit(&hdfsdm1_channel1) != HAL_OK)
  {
    Error_Handler();
  }
  hdfsdm1_channel2.Instance = DFSDM1_Channel2;
  hdfsdm1_channel2.Init.OutputClock.Activation = ENABLE;
  hdfsdm1_channel2.Init.OutputClock.Selection = DFSDM_CHANNEL_OUTPUT_CLOCK_AUDIO;
  hdfsdm1_channel2.Init.OutputClock.Divider = 5;
  hdfsdm1_channel2.Init.Input.Multiplexer = DFSDM_CHANNEL_EXTERNAL_INPUTS;
  hdfsdm1_channel2.Init.Input.DataPacking = DFSDM_CHANNEL_STANDARD_MODE;
  hdfsdm1_channel2.Init.Input.Pins = DFSDM_CHANNEL_SAME_CHANNEL_PINS;
  hdfsdm1_channel2.Init.SerialInterface.Type = DFSDM_CHANNEL_SPI_RISING;
  hdfsdm1_channel2.Init.SerialInterface.SpiClock = DFSDM_CHANNEL_SPI_CLOCK_INTERNAL;
  hdfsdm1_channel2.Init.Awd.FilterOrder = DFSDM_CHANNEL_FASTSINC_ORDER;
  hdfsdm1_channel2.Init.Awd.Oversampling = 1;
  hdfsdm1_channel2.Init.Offset = 1;
  hdfsdm1_channel2.Init.RightBitShift = 0x08;
  if (HAL_DFSDM_ChannelInit(&hdfsdm1_channel2) != HAL_OK)
  {
    Error_Handler();
  }
  if (HAL_DFSDM_FilterConfigRegChannel(&hdfsdm1_filter0, DFSDM_CHANNEL_1, DFSDM_CONTINUOUS_CONV_ON) != HAL_OK)
  {
    Error_Handler();
  }
  /* USER CODE BEGIN DFSDM1_Init 2 */

  /* USER CODE END DFSDM1_Init 2 */

}

/**
  * @brief LPUART1 Initialization Function
  * @param None
  * @retval None
  */
static void MX_LPUART1_UART_Init(void)
{

  /* USER CODE BEGIN LPUART1_Init 0 */

  /* USER CODE END LPUART1_Init 0 */

  /* USER CODE BEGIN LPUART1_Init 1 */

  /* USER CODE END LPUART1_Init 1 */
  hlpuart1.Instance = LPUART1;
  hlpuart1.Init.BaudRate = 209700;
  hlpuart1.Init.WordLength = UART_WORDLENGTH_7B;
  hlpuart1.Init.StopBits = UART_STOPBITS_1;
  hlpuart1.Init.Parity = UART_PARITY_NONE;
  hlpuart1.Init.Mode = UART_MODE_TX_RX;
  hlpuart1.Init.HwFlowCtl = UART_HWCONTROL_NONE;
  hlpuart1.Init.OneBitSampling = UART_ONE_BIT_SAMPLE_DISABLE;
  hlpuart1.AdvancedInit.AdvFeatureInit = UART_ADVFEATURE_NO_INIT;
  if (HAL_UART_Init(&hlpuart1) != HAL_OK)
  {
    Error_Handler();
  }
  /* USER CODE BEGIN LPUART1_Init 2 */

  /* USER CODE END LPUART1_Init 2 */

}

/**
  * @brief USART1 Initialization Function
  * @param None
  * @retval None
  */
static void MX_USART1_UART_Init(void)
{

  /* USER CODE BEGIN USART1_Init 0 */

  /* USER CODE END USART1_Init 0 */

  /* USER CODE BEGIN USART1_Init 1 */

  /* USER CODE END USART1_Init 1 */
  huart1.Instance = USART1;
  huart1.Init.BaudRate = 115200;
  huart1.Init.WordLength = UART_WORDLENGTH_8B;
  huart1.Init.StopBits = UART_STOPBITS_1;
  huart1.Init.Parity = UART_PARITY_NONE;
  huart1.Init.Mode = UART_MODE_TX_RX;
  huart1.Init.HwFlowCtl = UART_HWCONTROL_NONE;
  huart1.Init.OverSampling = UART_OVERSAMPLING_16;
  huart1.Init.OneBitSampling = UART_ONE_BIT_SAMPLE_DISABLE;
  huart1.AdvancedInit.AdvFeatureInit = UART_ADVFEATURE_NO_INIT;
  if (HAL_UART_Init(&huart1) != HAL_OK)
  {
    Error_Handler();
  }
  /* USER CODE BEGIN USART1_Init 2 */

  /* USER CODE END USART1_Init 2 */

}

/**
  * @brief USART2 Initialization Function
  * @param None
  * @retval None
  */
static void MX_USART2_UART_Init(void)
{

  /* USER CODE BEGIN USART2_Init 0 */

  /* USER CODE END USART2_Init 0 */

  /* USER CODE BEGIN USART2_Init 1 */

  /* USER CODE END USART2_Init 1 */
  huart2.Instance = USART2;
  huart2.Init.BaudRate = 115200;
  huart2.Init.WordLength = UART_WORDLENGTH_8B;
  huart2.Init.StopBits = UART_STOPBITS_1;
  huart2.Init.Parity = UART_PARITY_NONE;
  huart2.Init.Mode = UART_MODE_TX_RX;
  huart2.Init.HwFlowCtl = UART_HWCONTROL_NONE;
  huart2.Init.OverSampling = UART_OVERSAMPLING_16;
  huart2.Init.OneBitSampling = UART_ONE_BIT_SAMPLE_DISABLE;
  huart2.AdvancedInit.AdvFeatureInit = UART_ADVFEATURE_NO_INIT;
  if (HAL_UART_Init(&huart2) != HAL_OK)
  {
    Error_Handler();
  }
  /* USER CODE BEGIN USART2_Init 2 */

  /* USER CODE END USART2_Init 2 */

}

/**
  * @brief RTC Initialization Function
  * @param None
  * @retval None
  */
static void MX_RTC_Init(void)
{

  /* USER CODE BEGIN RTC_Init 0 */

  /* USER CODE END RTC_Init 0 */

  /* USER CODE BEGIN RTC_Init 1 */

  /* USER CODE END RTC_Init 1 */

  /** Initialize RTC Only
  */
  hrtc.Instance = RTC;
  hrtc.Init.HourFormat = RTC_HOURFORMAT_24;
  hrtc.Init.AsynchPrediv = 127;
  hrtc.Init.SynchPrediv = 255;
  hrtc.Init.OutPut = RTC_OUTPUT_DISABLE;
  hrtc.Init.OutPutRemap = RTC_OUTPUT_REMAP_NONE;
  hrtc.Init.OutPutPolarity = RTC_OUTPUT_POLARITY_HIGH;
  hrtc.Init.OutPutType = RTC_OUTPUT_TYPE_OPENDRAIN;
  if (HAL_RTC_Init(&hrtc) != HAL_OK)
  {
    Error_Handler();
  }
  /* USER CODE BEGIN RTC_Init 2 */

  /* USER CODE END RTC_Init 2 */

}

/**
  * @brief SDMMC1 Initialization Function
  * @param None
  * @retval None
  */
static void MX_SDMMC1_SD_Init(void)
{

  /* USER CODE BEGIN SDMMC1_Init 0 */

  /* USER CODE END SDMMC1_Init 0 */

  /* USER CODE BEGIN SDMMC1_Init 1 */
  __HAL_RCC_SDMMC1_CLK_ENABLE();  // Ensure clock is enabled
  /* USER CODE END SDMMC1_Init 1 */
  hsd1.Instance = SDMMC1;
  hsd1.Init.ClockEdge = SDMMC_CLOCK_EDGE_RISING;
  hsd1.Init.ClockBypass = SDMMC_CLOCK_BYPASS_DISABLE;
  hsd1.Init.ClockPowerSave = SDMMC_CLOCK_POWER_SAVE_DISABLE;
  hsd1.Init.BusWide = SDMMC_BUS_WIDE_4B;
  hsd1.Init.HardwareFlowControl = SDMMC_HARDWARE_FLOW_CONTROL_DISABLE;
  hsd1.Init.ClockDiv = 7;
  /* USER CODE BEGIN SDMMC1_Init 2 */

//  if (HAL_SD_Init(&hsd1) != HAL_OK) {
//	  Error_Handler();
//  }
  HAL_SD_Init(&hsd1);
  /* USER CODE END SDMMC1_Init 2 */

}

/**
  * @brief SPI1 Initialization Function
  * @param None
  * @retval None
  */
static void MX_SPI1_Init(void)
{

  /* USER CODE BEGIN SPI1_Init 0 */

  /* USER CODE END SPI1_Init 0 */

  /* USER CODE BEGIN SPI1_Init 1 */

  /* USER CODE END SPI1_Init 1 */
  /* SPI1 parameter configuration*/
  hspi1.Instance = SPI1;
  hspi1.Init.Mode = SPI_MODE_MASTER;
  hspi1.Init.Direction = SPI_DIRECTION_2LINES;
  hspi1.Init.DataSize = SPI_DATASIZE_8BIT;
  hspi1.Init.CLKPolarity = SPI_POLARITY_LOW;
  hspi1.Init.CLKPhase = SPI_PHASE_1EDGE;
  hspi1.Init.NSS = SPI_NSS_SOFT;
  hspi1.Init.BaudRatePrescaler = SPI_BAUDRATEPRESCALER_16;
  hspi1.Init.FirstBit = SPI_FIRSTBIT_MSB;
  hspi1.Init.TIMode = SPI_TIMODE_DISABLE;
  hspi1.Init.CRCCalculation = SPI_CRCCALCULATION_DISABLE;
  hspi1.Init.CRCPolynomial = 7;
  hspi1.Init.CRCLength = SPI_CRC_LENGTH_DATASIZE;
  hspi1.Init.NSSPMode = SPI_NSS_PULSE_ENABLE;
  if (HAL_SPI_Init(&hspi1) != HAL_OK)
  {
    Error_Handler();
  }
  /* USER CODE BEGIN SPI1_Init 2 */

  /* USER CODE END SPI1_Init 2 */

}

/**
  * @brief SPI2 Initialization Function
  * @param None
  * @retval None
  */
static void MX_SPI2_Init(void)
{

  /* USER CODE BEGIN SPI2_Init 0 */

  /* USER CODE END SPI2_Init 0 */

  /* USER CODE BEGIN SPI2_Init 1 */

  /* USER CODE END SPI2_Init 1 */
  /* SPI2 parameter configuration*/
  hspi2.Instance = SPI2;
  hspi2.Init.Mode = SPI_MODE_MASTER;
  hspi2.Init.Direction = SPI_DIRECTION_2LINES;
  hspi2.Init.DataSize = SPI_DATASIZE_8BIT;
  hspi2.Init.CLKPolarity = SPI_POLARITY_LOW;
  hspi2.Init.CLKPhase = SPI_PHASE_1EDGE;
  hspi2.Init.NSS = SPI_NSS_SOFT;
  hspi2.Init.BaudRatePrescaler = SPI_BAUDRATEPRESCALER_16;
  hspi2.Init.FirstBit = SPI_FIRSTBIT_MSB;
  hspi2.Init.TIMode = SPI_TIMODE_DISABLE;
  hspi2.Init.CRCCalculation = SPI_CRCCALCULATION_DISABLE;
  hspi2.Init.CRCPolynomial = 7;
  hspi2.Init.CRCLength = SPI_CRC_LENGTH_DATASIZE;
  hspi2.Init.NSSPMode = SPI_NSS_PULSE_ENABLE;
  if (HAL_SPI_Init(&hspi2) != HAL_OK)
  {
    Error_Handler();
  }
  /* USER CODE BEGIN SPI2_Init 2 */

  /* USER CODE END SPI2_Init 2 */

}

/**
  * @brief TIM1 Initialization Function
  * @param None
  * @retval None
  */
static void MX_TIM1_Init(void)
{

  /* USER CODE BEGIN TIM1_Init 0 */

  /* USER CODE END TIM1_Init 0 */

  TIM_Encoder_InitTypeDef sConfig = {0};
  TIM_MasterConfigTypeDef sMasterConfig = {0};

  /* USER CODE BEGIN TIM1_Init 1 */

  /* USER CODE END TIM1_Init 1 */
  htim1.Instance = TIM1;
  htim1.Init.Prescaler = 0;
  htim1.Init.CounterMode = TIM_COUNTERMODE_UP;
  htim1.Init.Period = 65535;
  htim1.Init.ClockDivision = TIM_CLOCKDIVISION_DIV1;
  htim1.Init.RepetitionCounter = 0;
  htim1.Init.AutoReloadPreload = TIM_AUTORELOAD_PRELOAD_DISABLE;
  sConfig.EncoderMode = TIM_ENCODERMODE_TI1;
  sConfig.IC1Polarity = TIM_ICPOLARITY_RISING;
  sConfig.IC1Selection = TIM_ICSELECTION_DIRECTTI;
  sConfig.IC1Prescaler = TIM_ICPSC_DIV1;
  sConfig.IC1Filter = 0;
  sConfig.IC2Polarity = TIM_ICPOLARITY_RISING;
  sConfig.IC2Selection = TIM_ICSELECTION_DIRECTTI;
  sConfig.IC2Prescaler = TIM_ICPSC_DIV1;
  sConfig.IC2Filter = 0;
  if (HAL_TIM_Encoder_Init(&htim1, &sConfig) != HAL_OK)
  {
    Error_Handler();
  }
  sMasterConfig.MasterOutputTrigger = TIM_TRGO_RESET;
  sMasterConfig.MasterOutputTrigger2 = TIM_TRGO2_RESET;
  sMasterConfig.MasterSlaveMode = TIM_MASTERSLAVEMODE_DISABLE;
  if (HAL_TIMEx_MasterConfigSynchronization(&htim1, &sMasterConfig) != HAL_OK)
  {
    Error_Handler();
  }
  /* USER CODE BEGIN TIM1_Init 2 */

  /* USER CODE END TIM1_Init 2 */

}

/**
  * @brief TIM2 Initialization Function
  * @param None
  * @retval None
  */
static void MX_TIM2_Init(void)
{

  /* USER CODE BEGIN TIM2_Init 0 */

  /* USER CODE END TIM2_Init 0 */

  TIM_ClockConfigTypeDef sClockSourceConfig = {0};
  TIM_MasterConfigTypeDef sMasterConfig = {0};
  TIM_OC_InitTypeDef sConfigOC = {0};

  /* USER CODE BEGIN TIM2_Init 1 */

  /* USER CODE END TIM2_Init 1 */
  htim2.Instance = TIM2;
  htim2.Init.Prescaler = 800-1;
  htim2.Init.CounterMode = TIM_COUNTERMODE_UP;
  htim2.Init.Period = 100;
  htim2.Init.ClockDivision = TIM_CLOCKDIVISION_DIV1;
  htim2.Init.AutoReloadPreload = TIM_AUTORELOAD_PRELOAD_ENABLE;
  if (HAL_TIM_Base_Init(&htim2) != HAL_OK)
  {
    Error_Handler();
  }
  sClockSourceConfig.ClockSource = TIM_CLOCKSOURCE_INTERNAL;
  if (HAL_TIM_ConfigClockSource(&htim2, &sClockSourceConfig) != HAL_OK)
  {
    Error_Handler();
  }
  if (HAL_TIM_PWM_Init(&htim2) != HAL_OK)
  {
    Error_Handler();
  }
  sMasterConfig.MasterOutputTrigger = TIM_TRGO_RESET;
  sMasterConfig.MasterSlaveMode = TIM_MASTERSLAVEMODE_DISABLE;
  if (HAL_TIMEx_MasterConfigSynchronization(&htim2, &sMasterConfig) != HAL_OK)
  {
    Error_Handler();
  }
  sConfigOC.OCMode = TIM_OCMODE_PWM1;
  sConfigOC.Pulse = 0;
  sConfigOC.OCPolarity = TIM_OCPOLARITY_LOW;
  sConfigOC.OCFastMode = TIM_OCFAST_DISABLE;
  if (HAL_TIM_PWM_ConfigChannel(&htim2, &sConfigOC, TIM_CHANNEL_1) != HAL_OK)
  {
    Error_Handler();
  }
  if (HAL_TIM_PWM_ConfigChannel(&htim2, &sConfigOC, TIM_CHANNEL_2) != HAL_OK)
  {
    Error_Handler();
  }
  sConfigOC.OCPolarity = TIM_OCPOLARITY_HIGH;
  if (HAL_TIM_PWM_ConfigChannel(&htim2, &sConfigOC, TIM_CHANNEL_3) != HAL_OK)
  {
    Error_Handler();
  }
  sConfigOC.OCPolarity = TIM_OCPOLARITY_LOW;
  if (HAL_TIM_PWM_ConfigChannel(&htim2, &sConfigOC, TIM_CHANNEL_4) != HAL_OK)
  {
    Error_Handler();
  }
  /* USER CODE BEGIN TIM2_Init 2 */

  /* USER CODE END TIM2_Init 2 */
  HAL_TIM_MspPostInit(&htim2);

}

/**
  * @brief TIM3 Initialization Function
  * @param None
  * @retval None
  */
static void MX_TIM3_Init(void)
{

  /* USER CODE BEGIN TIM3_Init 0 */

  /* USER CODE END TIM3_Init 0 */

  TIM_ClockConfigTypeDef sClockSourceConfig = {0};
  TIM_MasterConfigTypeDef sMasterConfig = {0};
  TIM_OC_InitTypeDef sConfigOC = {0};

  /* USER CODE BEGIN TIM3_Init 1 */

  /* USER CODE END TIM3_Init 1 */
  htim3.Instance = TIM3;
  htim3.Init.Prescaler = 800-1;
  htim3.Init.CounterMode = TIM_COUNTERMODE_UP;
  htim3.Init.Period = 100-1;
  htim3.Init.ClockDivision = TIM_CLOCKDIVISION_DIV1;
  htim3.Init.AutoReloadPreload = TIM_AUTORELOAD_PRELOAD_DISABLE;
  if (HAL_TIM_Base_Init(&htim3) != HAL_OK)
  {
    Error_Handler();
  }
  sClockSourceConfig.ClockSource = TIM_CLOCKSOURCE_INTERNAL;
  if (HAL_TIM_ConfigClockSource(&htim3, &sClockSourceConfig) != HAL_OK)
  {
    Error_Handler();
  }
  if (HAL_TIM_PWM_Init(&htim3) != HAL_OK)
  {
    Error_Handler();
  }
  sMasterConfig.MasterOutputTrigger = TIM_TRGO_RESET;
  sMasterConfig.MasterSlaveMode = TIM_MASTERSLAVEMODE_DISABLE;
  if (HAL_TIMEx_MasterConfigSynchronization(&htim3, &sMasterConfig) != HAL_OK)
  {
    Error_Handler();
  }
  sConfigOC.OCMode = TIM_OCMODE_PWM1;
  sConfigOC.Pulse = 0;
  sConfigOC.OCPolarity = TIM_OCPOLARITY_HIGH;
  sConfigOC.OCFastMode = TIM_OCFAST_DISABLE;
  if (HAL_TIM_PWM_ConfigChannel(&htim3, &sConfigOC, TIM_CHANNEL_1) != HAL_OK)
  {
    Error_Handler();
  }
  if (HAL_TIM_PWM_ConfigChannel(&htim3, &sConfigOC, TIM_CHANNEL_2) != HAL_OK)
  {
    Error_Handler();
  }
  /* USER CODE BEGIN TIM3_Init 2 */

  /* USER CODE END TIM3_Init 2 */
  HAL_TIM_MspPostInit(&htim3);

}

/**
  * @brief TIM4 Initialization Function
  * @param None
  * @retval None
  */
static void MX_TIM4_Init(void)
{

  /* USER CODE BEGIN TIM4_Init 0 */

  /* USER CODE END TIM4_Init 0 */

  TIM_ClockConfigTypeDef sClockSourceConfig = {0};
  TIM_MasterConfigTypeDef sMasterConfig = {0};
  TIM_OC_InitTypeDef sConfigOC = {0};

  /* USER CODE BEGIN TIM4_Init 1 */

  /* USER CODE END TIM4_Init 1 */
  htim4.Instance = TIM4;
  htim4.Init.Prescaler = 125;
  htim4.Init.CounterMode = TIM_COUNTERMODE_UP;
  htim4.Init.Period = 10000-1;
  htim4.Init.ClockDivision = TIM_CLOCKDIVISION_DIV1;
  htim4.Init.AutoReloadPreload = TIM_AUTORELOAD_PRELOAD_DISABLE;
  if (HAL_TIM_Base_Init(&htim4) != HAL_OK)
  {
    Error_Handler();
  }
  sClockSourceConfig.ClockSource = TIM_CLOCKSOURCE_INTERNAL;
  if (HAL_TIM_ConfigClockSource(&htim4, &sClockSourceConfig) != HAL_OK)
  {
    Error_Handler();
  }
  if (HAL_TIM_PWM_Init(&htim4) != HAL_OK)
  {
    Error_Handler();
  }
  sMasterConfig.MasterOutputTrigger = TIM_TRGO_RESET;
  sMasterConfig.MasterSlaveMode = TIM_MASTERSLAVEMODE_DISABLE;
  if (HAL_TIMEx_MasterConfigSynchronization(&htim4, &sMasterConfig) != HAL_OK)
  {
    Error_Handler();
  }
  sConfigOC.OCMode = TIM_OCMODE_PWM1;
  sConfigOC.Pulse = 0;
  sConfigOC.OCPolarity = TIM_OCPOLARITY_HIGH;
  sConfigOC.OCFastMode = TIM_OCFAST_DISABLE;
  if (HAL_TIM_PWM_ConfigChannel(&htim4, &sConfigOC, TIM_CHANNEL_2) != HAL_OK)
  {
    Error_Handler();
  }
  if (HAL_TIM_PWM_ConfigChannel(&htim4, &sConfigOC, TIM_CHANNEL_4) != HAL_OK)
  {
    Error_Handler();
  }
  /* USER CODE BEGIN TIM4_Init 2 */

  /* USER CODE END TIM4_Init 2 */
  HAL_TIM_MspPostInit(&htim4);

}

/**
  * @brief TIM6 Initialization Function
  * @param None
  * @retval None
  */
static void MX_TIM6_Init(void)
{

  /* USER CODE BEGIN TIM6_Init 0 */

  /* USER CODE END TIM6_Init 0 */

  TIM_MasterConfigTypeDef sMasterConfig = {0};

  /* USER CODE BEGIN TIM6_Init 1 */

  /* USER CODE END TIM6_Init 1 */
  htim6.Instance = TIM6;
  htim6.Init.Prescaler = 20;
  htim6.Init.CounterMode = TIM_COUNTERMODE_UP;
  htim6.Init.Period = 1000-1;
  htim6.Init.AutoReloadPreload = TIM_AUTORELOAD_PRELOAD_DISABLE;
  if (HAL_TIM_Base_Init(&htim6) != HAL_OK)
  {
    Error_Handler();
  }
  sMasterConfig.MasterOutputTrigger = TIM_TRGO_RESET;
  sMasterConfig.MasterSlaveMode = TIM_MASTERSLAVEMODE_DISABLE;
  if (HAL_TIMEx_MasterConfigSynchronization(&htim6, &sMasterConfig) != HAL_OK)
  {
    Error_Handler();
  }
  /* USER CODE BEGIN TIM6_Init 2 */

  /* USER CODE END TIM6_Init 2 */

}

/**
  * @brief TIM7 Initialization Function
  * @param None
  * @retval None
  */
static void MX_TIM7_Init(void)
{

  /* USER CODE BEGIN TIM7_Init 0 */

  /* USER CODE END TIM7_Init 0 */

  TIM_MasterConfigTypeDef sMasterConfig = {0};

  /* USER CODE BEGIN TIM7_Init 1 */

  /* USER CODE END TIM7_Init 1 */
  htim7.Instance = TIM7;
  htim7.Init.Prescaler = 8000-1;
  htim7.Init.CounterMode = TIM_COUNTERMODE_UP;
  htim7.Init.Period = 4;
  htim7.Init.AutoReloadPreload = TIM_AUTORELOAD_PRELOAD_DISABLE;
  if (HAL_TIM_Base_Init(&htim7) != HAL_OK)
  {
    Error_Handler();
  }
  sMasterConfig.MasterOutputTrigger = TIM_TRGO_RESET;
  sMasterConfig.MasterSlaveMode = TIM_MASTERSLAVEMODE_DISABLE;
  if (HAL_TIMEx_MasterConfigSynchronization(&htim7, &sMasterConfig) != HAL_OK)
  {
    Error_Handler();
  }
  /* USER CODE BEGIN TIM7_Init 2 */

  /* USER CODE END TIM7_Init 2 */

}

/**
  * @brief TIM16 Initialization Function
  * @param None
  * @retval None
  */
static void MX_TIM16_Init(void)
{

  /* USER CODE BEGIN TIM16_Init 0 */

  /* USER CODE END TIM16_Init 0 */

  TIM_OC_InitTypeDef sConfigOC = {0};
  TIM_BreakDeadTimeConfigTypeDef sBreakDeadTimeConfig = {0};

  /* USER CODE BEGIN TIM16_Init 1 */

  /* USER CODE END TIM16_Init 1 */
  htim16.Instance = TIM16;
  htim16.Init.Prescaler = 800-1;
  htim16.Init.CounterMode = TIM_COUNTERMODE_UP;
  htim16.Init.Period = 100;
  htim16.Init.ClockDivision = TIM_CLOCKDIVISION_DIV1;
  htim16.Init.RepetitionCounter = 0;
  htim16.Init.AutoReloadPreload = TIM_AUTORELOAD_PRELOAD_DISABLE;
  if (HAL_TIM_Base_Init(&htim16) != HAL_OK)
  {
    Error_Handler();
  }
  if (HAL_TIM_PWM_Init(&htim16) != HAL_OK)
  {
    Error_Handler();
  }
  sConfigOC.OCMode = TIM_OCMODE_PWM1;
  sConfigOC.Pulse = 0;
  sConfigOC.OCPolarity = TIM_OCPOLARITY_LOW;
  sConfigOC.OCNPolarity = TIM_OCNPOLARITY_HIGH;
  sConfigOC.OCFastMode = TIM_OCFAST_DISABLE;
  sConfigOC.OCIdleState = TIM_OCIDLESTATE_RESET;
  sConfigOC.OCNIdleState = TIM_OCNIDLESTATE_RESET;
  if (HAL_TIM_PWM_ConfigChannel(&htim16, &sConfigOC, TIM_CHANNEL_1) != HAL_OK)
  {
    Error_Handler();
  }
  sBreakDeadTimeConfig.OffStateRunMode = TIM_OSSR_DISABLE;
  sBreakDeadTimeConfig.OffStateIDLEMode = TIM_OSSI_DISABLE;
  sBreakDeadTimeConfig.LockLevel = TIM_LOCKLEVEL_OFF;
  sBreakDeadTimeConfig.DeadTime = 0;
  sBreakDeadTimeConfig.BreakState = TIM_BREAK_DISABLE;
  sBreakDeadTimeConfig.BreakPolarity = TIM_BREAKPOLARITY_HIGH;
  sBreakDeadTimeConfig.AutomaticOutput = TIM_AUTOMATICOUTPUT_DISABLE;
  if (HAL_TIMEx_ConfigBreakDeadTime(&htim16, &sBreakDeadTimeConfig) != HAL_OK)
  {
    Error_Handler();
  }
  /* USER CODE BEGIN TIM16_Init 2 */

  /* USER CODE END TIM16_Init 2 */
  HAL_TIM_MspPostInit(&htim16);

}

/**
  * @brief TIM17 Initialization Function
  * @param None
  * @retval None
  */
static void MX_TIM17_Init(void)
{

  /* USER CODE BEGIN TIM17_Init 0 */

  /* USER CODE END TIM17_Init 0 */

  TIM_OC_InitTypeDef sConfigOC = {0};
  TIM_BreakDeadTimeConfigTypeDef sBreakDeadTimeConfig = {0};

  /* USER CODE BEGIN TIM17_Init 1 */

  /* USER CODE END TIM17_Init 1 */
  htim17.Instance = TIM17;
  htim17.Init.Prescaler = 125;
  htim17.Init.CounterMode = TIM_COUNTERMODE_UP;
  htim17.Init.Period = 10000-1;
  htim17.Init.ClockDivision = TIM_CLOCKDIVISION_DIV1;
  htim17.Init.RepetitionCounter = 0;
  htim17.Init.AutoReloadPreload = TIM_AUTORELOAD_PRELOAD_DISABLE;
  if (HAL_TIM_Base_Init(&htim17) != HAL_OK)
  {
    Error_Handler();
  }
  if (HAL_TIM_PWM_Init(&htim17) != HAL_OK)
  {
    Error_Handler();
  }
  sConfigOC.OCMode = TIM_OCMODE_PWM1;
  sConfigOC.Pulse = 0;
  sConfigOC.OCPolarity = TIM_OCPOLARITY_HIGH;
  sConfigOC.OCNPolarity = TIM_OCNPOLARITY_HIGH;
  sConfigOC.OCFastMode = TIM_OCFAST_DISABLE;
  sConfigOC.OCIdleState = TIM_OCIDLESTATE_RESET;
  sConfigOC.OCNIdleState = TIM_OCNIDLESTATE_RESET;
  if (HAL_TIM_PWM_ConfigChannel(&htim17, &sConfigOC, TIM_CHANNEL_1) != HAL_OK)
  {
    Error_Handler();
  }
  sBreakDeadTimeConfig.OffStateRunMode = TIM_OSSR_DISABLE;
  sBreakDeadTimeConfig.OffStateIDLEMode = TIM_OSSI_DISABLE;
  sBreakDeadTimeConfig.LockLevel = TIM_LOCKLEVEL_OFF;
  sBreakDeadTimeConfig.DeadTime = 0;
  sBreakDeadTimeConfig.BreakState = TIM_BREAK_DISABLE;
  sBreakDeadTimeConfig.BreakPolarity = TIM_BREAKPOLARITY_HIGH;
  sBreakDeadTimeConfig.AutomaticOutput = TIM_AUTOMATICOUTPUT_DISABLE;
  if (HAL_TIMEx_ConfigBreakDeadTime(&htim17, &sBreakDeadTimeConfig) != HAL_OK)
  {
    Error_Handler();
  }
  /* USER CODE BEGIN TIM17_Init 2 */

  /* USER CODE END TIM17_Init 2 */
  HAL_TIM_MspPostInit(&htim17);

}

/**
  * Enable DMA controller clock
  */
static void MX_DMA_Init(void)
{

  /* DMA controller clock enable */
  __HAL_RCC_DMA1_CLK_ENABLE();

  /* DMA interrupt init */
  /* DMA1_Channel1_IRQn interrupt configuration */
  HAL_NVIC_SetPriority(DMA1_Channel1_IRQn, 0, 0);
  HAL_NVIC_EnableIRQ(DMA1_Channel1_IRQn);
  /* DMA1_Channel4_IRQn interrupt configuration */
  HAL_NVIC_SetPriority(DMA1_Channel4_IRQn, 0, 0);
  HAL_NVIC_EnableIRQ(DMA1_Channel4_IRQn);
  /* DMA1_Channel5_IRQn interrupt configuration */
  HAL_NVIC_SetPriority(DMA1_Channel5_IRQn, 0, 0);
  HAL_NVIC_EnableIRQ(DMA1_Channel5_IRQn);

}

/**
  * @brief GPIO Initialization Function
  * @param None
  * @retval None
  */
static void MX_GPIO_Init(void)
{
  GPIO_InitTypeDef GPIO_InitStruct = {0};
  /* USER CODE BEGIN MX_GPIO_Init_1 */
  /* USER CODE END MX_GPIO_Init_1 */

  /* GPIO Ports Clock Enable */
  __HAL_RCC_GPIOE_CLK_ENABLE();
  __HAL_RCC_GPIOC_CLK_ENABLE();
  __HAL_RCC_GPIOH_CLK_ENABLE();
  __HAL_RCC_GPIOA_CLK_ENABLE();
  __HAL_RCC_GPIOB_CLK_ENABLE();
  __HAL_RCC_GPIOD_CLK_ENABLE();

  /*Configure GPIO pin Output Level */
  HAL_GPIO_WritePin(GPIOE, TENS_V_EN_Pin|TENS_POL_CH2_Pin|TENS_POL_CH3_Pin|LCD2_AO_Pin
                          |VBOOST_EN_Pin|MOTOR1_DIR_Pin, GPIO_PIN_RESET);

  /*Configure GPIO pin Output Level */
  HAL_GPIO_WritePin(TENS_POL_CH1_GPIO_Port, TENS_POL_CH1_Pin, GPIO_PIN_RESET);

  /*Configure GPIO pin Output Level */
  HAL_GPIO_WritePin(nPOWERON_ESP32_GPIO_Port, nPOWERON_ESP32_Pin, GPIO_PIN_SET);

  /*Configure GPIO pin Output Level */
  HAL_GPIO_WritePin(GPIOA, PB_ROW1_Pin|POWER_ON_IMU_GPS_Pin, GPIO_PIN_SET);

  /*Configure GPIO pin Output Level */
  HAL_GPIO_WritePin(IMU_CS_GPIO_Port, IMU_CS_Pin, GPIO_PIN_SET);

  /*Configure GPIO pin Output Level */
  HAL_GPIO_WritePin(GPIOD, RFM_CS_Pin|POWER_ON_MAIN_Pin|PB_ROW2_Pin|LCD2_CS_Pin
                          |LCD_CS_Pin|LCD_RST_Pin, GPIO_PIN_SET);

  /*Configure GPIO pin Output Level */
  HAL_GPIO_WritePin(GPIOD, GPS_PPS_Pin|MOTOR1_MODE_Pin|LCD_AO_Pin, GPIO_PIN_RESET);

  /*Configure GPIO pin Output Level */
  HAL_GPIO_WritePin(GPIOB, MOTOR2_DIR_Pin|MOTOR2_MODE_Pin, GPIO_PIN_RESET);

  /*Configure GPIO pin Output Level */
  HAL_GPIO_WritePin(PB_ROW3_GPIO_Port, PB_ROW3_Pin, GPIO_PIN_SET);

  /*Configure GPIO pins : TENS_V_EN_Pin TENS_POL_CH2_Pin TENS_POL_CH3_Pin LCD2_AO_Pin
                           VBOOST_EN_Pin MOTOR1_DIR_Pin IMU_CS_Pin */
  GPIO_InitStruct.Pin = TENS_V_EN_Pin|TENS_POL_CH2_Pin|TENS_POL_CH3_Pin|LCD2_AO_Pin
                          |VBOOST_EN_Pin|MOTOR1_DIR_Pin|IMU_CS_Pin;
  GPIO_InitStruct.Mode = GPIO_MODE_OUTPUT_PP;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  GPIO_InitStruct.Speed = GPIO_SPEED_FREQ_LOW;
  HAL_GPIO_Init(GPIOE, &GPIO_InitStruct);

  /*Configure GPIO pins : TENS_POL_CH1_Pin nPOWERON_ESP32_Pin */
  GPIO_InitStruct.Pin = TENS_POL_CH1_Pin|nPOWERON_ESP32_Pin;
  GPIO_InitStruct.Mode = GPIO_MODE_OUTPUT_PP;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  GPIO_InitStruct.Speed = GPIO_SPEED_FREQ_LOW;
  HAL_GPIO_Init(GPIOC, &GPIO_InitStruct);

  /*Configure GPIO pin : PB_COL1_Pin */
  GPIO_InitStruct.Pin = PB_COL1_Pin;
  GPIO_InitStruct.Mode = GPIO_MODE_INPUT;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  HAL_GPIO_Init(PB_COL1_GPIO_Port, &GPIO_InitStruct);

  /*Configure GPIO pins : PB_ROW1_Pin POWER_ON_IMU_GPS_Pin */
  GPIO_InitStruct.Pin = PB_ROW1_Pin|POWER_ON_IMU_GPS_Pin;
  GPIO_InitStruct.Mode = GPIO_MODE_OUTPUT_PP;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  GPIO_InitStruct.Speed = GPIO_SPEED_FREQ_LOW;
  HAL_GPIO_Init(GPIOA, &GPIO_InitStruct);

  /*Configure GPIO pins : SD_SW_Pin PB_COL3_Pin */
  GPIO_InitStruct.Pin = SD_SW_Pin|PB_COL3_Pin;
  GPIO_InitStruct.Mode = GPIO_MODE_INPUT;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  HAL_GPIO_Init(GPIOB, &GPIO_InitStruct);

  /*Configure GPIO pin : IMU_INT1_Pin */
  GPIO_InitStruct.Pin = IMU_INT1_Pin;
  GPIO_InitStruct.Mode = GPIO_MODE_IT_RISING;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  HAL_GPIO_Init(IMU_INT1_GPIO_Port, &GPIO_InitStruct);

  /*Configure GPIO pin : IMU_INT2_Pin */
  GPIO_InitStruct.Pin = IMU_INT2_Pin;
  GPIO_InitStruct.Mode = GPIO_MODE_IT_RISING;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  HAL_GPIO_Init(IMU_INT2_GPIO_Port, &GPIO_InitStruct);

  /*Configure GPIO pins : PB_POWER_Pin PB_POWER2_Pin PB2_Pin */
  GPIO_InitStruct.Pin = PB_POWER_Pin|PB_POWER2_Pin|PB2_Pin;
  GPIO_InitStruct.Mode = GPIO_MODE_INPUT;
  GPIO_InitStruct.Pull = GPIO_PULLUP;
  HAL_GPIO_Init(GPIOB, &GPIO_InitStruct);

  /*Configure GPIO pins : RFM_CS_Pin GPS_PPS_Pin MOTOR1_MODE_Pin POWER_ON_MAIN_Pin
                           PB_ROW2_Pin LCD2_CS_Pin LCD_AO_Pin LCD_CS_Pin
                           LCD_RST_Pin */
  GPIO_InitStruct.Pin = RFM_CS_Pin|GPS_PPS_Pin|MOTOR1_MODE_Pin|POWER_ON_MAIN_Pin
                          |PB_ROW2_Pin|LCD2_CS_Pin|LCD_AO_Pin|LCD_CS_Pin
                          |LCD_RST_Pin;
  GPIO_InitStruct.Mode = GPIO_MODE_OUTPUT_PP;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  GPIO_InitStruct.Speed = GPIO_SPEED_FREQ_LOW;
  HAL_GPIO_Init(GPIOD, &GPIO_InitStruct);

  /*Configure GPIO pin : PB_COL2_Pin */
  GPIO_InitStruct.Pin = PB_COL2_Pin;
  GPIO_InitStruct.Mode = GPIO_MODE_INPUT;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  HAL_GPIO_Init(PB_COL2_GPIO_Port, &GPIO_InitStruct);

  /*Configure GPIO pin : RFM_DIO0_Pin */
  GPIO_InitStruct.Pin = RFM_DIO0_Pin;
  GPIO_InitStruct.Mode = GPIO_MODE_IT_RISING;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  HAL_GPIO_Init(RFM_DIO0_GPIO_Port, &GPIO_InitStruct);

  /*Configure GPIO pin : PB3_Pin */
  GPIO_InitStruct.Pin = PB3_Pin;
  GPIO_InitStruct.Mode = GPIO_MODE_INPUT;
  GPIO_InitStruct.Pull = GPIO_PULLUP;
  HAL_GPIO_Init(PB3_GPIO_Port, &GPIO_InitStruct);

  /*Configure GPIO pins : MOTOR2_DIR_Pin PB_ROW3_Pin MOTOR2_MODE_Pin */
  GPIO_InitStruct.Pin = MOTOR2_DIR_Pin|PB_ROW3_Pin|MOTOR2_MODE_Pin;
  GPIO_InitStruct.Mode = GPIO_MODE_OUTPUT_PP;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  GPIO_InitStruct.Speed = GPIO_SPEED_FREQ_LOW;
  HAL_GPIO_Init(GPIOB, &GPIO_InitStruct);

  /* EXTI interrupt init*/
  HAL_NVIC_SetPriority(EXTI15_10_IRQn, 0, 0);
  HAL_NVIC_EnableIRQ(EXTI15_10_IRQn);

  /* USER CODE BEGIN MX_GPIO_Init_2 */
  /* USER CODE END MX_GPIO_Init_2 */
}

/* USER CODE BEGIN 4 */

//void HAL_DFSDM_FilterRegConvCpltCallback(DFSDM_Filter_HandleTypeDef *hdfsdm_filter){
//	if (hdfsdm_filter == &hdfsdm1_filter0){  // Trigger on left channel complete
//		audioBufferFull = 1;
//	}
//}
void HAL_DFSDM_FilterRegConvCpltCallback(DFSDM_Filter_HandleTypeDef *hdfsdm_filter) {
    if (hdfsdm_filter == &hdfsdm1_filter0) {
        haud.bufferFull = true;  // haud is your global Audio_HandleTypeDef in main.c
    }
}



void HAL_GPIO_EXTI_Callback(uint16_t GPIO_Pin){
	if(GPIO_Pin == RFM_DIO0_Pin){
		radio.rxIrqFlag = true;
	}else if(GPIO_Pin == imu.stepIntPin){
		imu.stepDataReady = true;
	}else if(GPIO_Pin == imu.gyroIntPin){
		imu.gyroDataReady = true;
	}else{
		__NOP();
	}
}

void HAL_TIM_PeriodElapsedCallback(TIM_HandleTypeDef *htim){
	if(htim->Instance == tens.loopTimer->Instance){// tensLoopTim.Instance){
		if(tensMutex == 0){
			TensLoop(&tens);
		}
	}else if(htim->Instance == TIM6){

	}else{
		for(uint8_t n=0; n<tens.numChans; n++){
			if(tens.tensChan[n].timerHandle->Instance == htim->Instance){
				TensTimerPeriodElapsed(&tens, n);
				//return;
			}
		}
	}
}

void HAL_SPI_TxCpltCallback(SPI_HandleTypeDef *hspi){
	lcd.dmaWriteActive = 0;
	ST7735_Unselect(&lcd);
}

/* USER CODE END 4 */

/**
  * @brief  This function is executed in case of error occurrence.
  * @retval None
  */
void Error_Handler(void)
{
  /* USER CODE BEGIN Error_Handler_Debug */
  /* User can add his own implementation to report the HAL error return state */
  __disable_irq();
  while (1)
  {
  }
  /* USER CODE END Error_Handler_Debug */
}
#ifdef USE_FULL_ASSERT
/**
  * @brief  Reports the name of the source file and the source line number
  *         where the assert_param error has occurred.
  * @param  file: pointer to the source file name
  * @param  line: assert_param error line source number
  * @retval None
  */
void assert_failed(uint8_t *file, uint32_t line)
{
  /* USER CODE BEGIN 6 */
  /* User can add his own implementation to report the file name and line number,
     ex: printf("Wrong parameters value: file %s on line %d\r\n", file, line) */
  /* USER CODE END 6 */
}
#endif /* USE_FULL_ASSERT */
