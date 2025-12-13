


#ifndef RFM69_h
#define RFM69_h

#include <stdbool.h>
#include "stm32l4xx_hal.h"
#include <string.h>

#define RFM69_MAX_DATA_LEN		61 		// to take advantage of the built in AES/CRC we want to limit the frame size to the internal FIFO size (66 bytes - 3 bytes overhead - 2 bytes crc)
#define CSMA_LIMIT				-90 	// upper RX signal sensitivity threshold in dBm for carrier sense access


//#define null                  0
#define COURSE_TEMP_COEF    	-90 	// puts the temperature reading in the ballpark, user can fine tune the returned value
#define RFM69_BROADCAST_ADDR   	0
#define RFM69_CSMA_LIMIT_MS 		1000
#define RFM69_TX_LIMIT_MS   		1000
#define RFM69_FSTEP  			61.03515625 // == FXOSC / 2^19 = 32MHz / 2^19 (p13 in datasheet)

// TWS: define CTLbyte bits
#define RFM69_CTL_SENDACK   0x80
#define RFM69_CTL_REQACK    0x40
#define RFM69_CTL_RESERVE1  0x20

#define RFM69_ACK_TIMEOUT   30  // 30ms roundtrip req for 61byte packets

typedef enum {
	RFM69_MODE_SLEEP		= 0,	// XTAL OFF
	RFM69_MODE_STANDBY		= 1,	// XTAL ON
	RFM69_MODE_SYNTH		= 2,	// PLL ON
	RFM69_MODE_RX			= 3,	// RX MODE
	RFM69_MODE_TX			= 4,	// TX MODE
	RFM69_MODE_UNKNOWN		= 5		// used for initialization phase
} RFM69_Mode;

typedef enum {
	// available frequency bands
	RFM69_315MHZ			= 31, // non trivial values to avoid misconfiguration
	RFM69_433MHZ			= 43,
	RFM69_868MHZ			= 86,
	RFM69_915MHZ			= 91
} RFM69_FrequencyBand;

//typedef enum {
//	RFM69_SLEEP_15MS	= ,
//
//} RFM69_SleepTime;




typedef struct{
	bool				isRFM69HW_HCW;		//tracks whether the device is *capable* of using high power
	//bool				useHighPower;		//tracks whether or not high power is *enabled*
	uint8_t				powerLevel;
	RFM69_FrequencyBand	freqBand;
	bool				useATC;
	uint8_t				transmitLevelStep;
	int8_t				targetRssi;
} RFM69Radio;

typedef struct{
	bool				_useEncryption;
	uint8_t				_encryptionKey[17];
	uint16_t			gatewayId;
	uint16_t			myId;
	uint8_t				networkId;
} RFM69Comm;

typedef struct{
	uint8_t				dataBuff[100];
	uint8_t				dataLen;
	uint8_t				payloadLen;
	uint16_t			senderId;
	uint16_t			targetId;
	bool				ackRequested;
	bool				ackRssiRequested;
	bool				ackReceived;
	int16_t				ackRssi;			//This was the remote unit's RSSI of this unit's last transmission, being reported back for automatic level control calculations.
	bool				rssiRequested;
	int16_t				rssi;				//RSSI value of incoming message from remote sender
	bool				messagesPending;	//Used when sending/receiving an Ack.  This let's the receiver know if the sender has more messages pending for the receiver, therefore letting the receiver know to not go to sleep immediately.
} RFM69Message;

typedef enum {
	networkMembershipUnknown,
	networkMemberhsipOffline,
	networkMemberhsipInitReady,
	networkMemberhsipInitAwaitingResponse,
	networkMemberhsipReconnecting,
	networkMemberhsipActive,
	networkMemberhsipGateway
} networkMemberhsipEnum;

typedef struct{
	bool				rxIrqFlag;
	RFM69_Mode			mode;
	RFM69Message		rxMsg;
	SPI_HandleTypeDef* 	spi;
	GPIO_TypeDef*		slaveSelectPort;
	uint16_t			slaveSelectPin;
	RFM69Comm			comm;
	RFM69Radio			radio;
	bool				spyModeEnabled;
	uint32_t			lastAckReceivedTime;
	networkMemberhsipEnum	networkMembershipMode;
	int8_t				networkMembershipRequestsRemaining;
	uint32_t			lastNetworkRequestSent;
	bool				canBeGateway;
} RFM69_HandleTypeDef;







bool RFM69Initialize(RFM69_HandleTypeDef* dev, SPI_HandleTypeDef* spi, GPIO_TypeDef* slaveSelectPort, uint16_t slaveSelectPin,
		bool isRFM69HW_HCW, uint8_t freqBand, uint16_t nodeID, uint8_t networkID);
void RFM69SelectSPI(RFM69_HandleTypeDef* dev);
void RFM69UnselectSPI(RFM69_HandleTypeDef* dev);
void RFM69WriteReg(RFM69_HandleTypeDef* dev, uint8_t addr, uint8_t value);
uint8_t RFM69ReadReg(RFM69_HandleTypeDef* dev, uint8_t addr);
void RFM69Encrypt(RFM69_HandleTypeDef* dev, const char* key);
void RFM69SetHighPower(RFM69_HandleTypeDef* dev, bool _isRFM69HW_HCW);
void RFM69SetPowerLevel(RFM69_HandleTypeDef* dev, uint8_t powerLevel);
void RFM69SetHighPowerRegs(RFM69_HandleTypeDef* dev, bool enable);
void RFM69SetMode(RFM69_HandleTypeDef* dev, uint8_t newMode);


//void RFM69SpyMode(RFM69_HandleTypeDef* dev, bool onOff);
void RFM69Sleep(RFM69_HandleTypeDef* dev);

bool RFM69ReceiveDone(RFM69_HandleTypeDef* dev);
void RFM69ReceiveBegin(RFM69_HandleTypeDef* dev);
void RFM69InterruptHandler(RFM69_HandleTypeDef* dev);
int16_t RFM69ReadRSSI(RFM69_HandleTypeDef* dev, bool forceTrigger);
void RFM69InterruptHook(RFM69_HandleTypeDef* dev, uint8_t CTLbyte);

bool RFM69SendWithRetry(RFM69_HandleTypeDef* dev, uint16_t toAddress, const void* buffer, uint8_t bufferSize, uint8_t retries, uint32_t retryWaitTime);
bool RFM69AckReceived(RFM69_HandleTypeDef* dev, uint16_t fromNodeID);
void RFM69Send(RFM69_HandleTypeDef* dev, uint16_t toAddress, const void* buffer, uint8_t bufferSize, bool requestACK);
bool RFM69CanSend(RFM69_HandleTypeDef* dev);
void RFM69SendFrame(RFM69_HandleTypeDef* dev, uint16_t toAddress, const void* buffer, uint8_t bufferSize, bool requestACK, bool sendACK, bool sendRSSI, int16_t lastRSSI);

void RFM69SendACK(RFM69_HandleTypeDef* dev, const void* buffer, uint8_t bufferSize);

void RFM69SetAddress(RFM69_HandleTypeDef* dev, uint16_t addr);
void RFM69SetNetwork(RFM69_HandleTypeDef* dev, uint8_t networkID);


#endif



