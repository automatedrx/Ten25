

#include "rfm69.h"
#include "rfm69registers.h"
//#include "utils.h"
/*
RFM69::RFM69(uint8_t slaveSelectPin, uint8_t interruptPin, bool isRFM69HW_HCW, SPIClass *spi) {
  _instance = this;
  _slaveSelectPin = slaveSelectPin;
  _interruptPin = interruptPin;
  _mode = RF69_MODE_STANDBY;
  _spyMode = false;
  _powerLevel = 31;
  _isRFM69HW = isRFM69HW_HCW;
  _spi = spi;

}
*/
bool RFM69Initialize(RFM69_HandleTypeDef* dev, SPI_HandleTypeDef* spi, GPIO_TypeDef* slaveSelectPort, uint16_t slaveSelectPin,
		bool isRFM69HW_HCW, uint8_t freqBand, uint16_t nodeID, uint8_t networkID){

	/*
#if defined(RF69_LISTENMODE_ENABLE)
  _isHighSpeed = true;
  _haveEncryptKey = false;
  uint32_t rxDuration = DEFAULT_LISTEN_RX_US;
  uint32_t idleDuration = DEFAULT_LISTEN_IDLE_US;
  listenModeSetDurations(rxDuration, idleDuration);
#endif
*/

	dev->slaveSelectPort 		= slaveSelectPort;
	dev->slaveSelectPin			= slaveSelectPin;
	dev->spi 					= spi;
	dev->mode					= RFM69_MODE_UNKNOWN;
	dev->spyModeEnabled			= false;
	dev->rxIrqFlag				= false;

	dev->radio.isRFM69HW_HCW	= isRFM69HW_HCW;
	dev->radio.freqBand			= freqBand;
	dev->radio.transmitLevelStep	= 1;
	dev->radio.useATC			= true;
	dev->radio.powerLevel		= 31;				//Initialized to max power. User should set the power level to the appropriate level after initialization.

	dev->comm._useEncryption	= false;
	dev->comm.gatewayId			= 1;
	dev->comm.myId				= nodeID;
	dev->comm.networkId			= networkID;

	dev->rxMsg.ackRequested		= false;
	dev->rxMsg.dataLen			= 0;
	dev->rxMsg.payloadLen		= 0;
	//dev->txMsg.ackRequested		= false;
	//dev->txMsg.dataLen			= 0;


	uint8_t CONFIG[][2] = {
		/* 0x01 */ { REG_OPMODE, RF_OPMODE_SEQUENCER_ON | RF_OPMODE_LISTEN_OFF | RF_OPMODE_STANDBY },
		/* 0x02 */ { REG_DATAMODUL, RF_DATAMODUL_DATAMODE_PACKET | RF_DATAMODUL_MODULATIONTYPE_FSK | RF_DATAMODUL_MODULATIONSHAPING_00 }, // no shaping
		/* 0x03 */ { REG_BITRATEMSB, RF_BITRATEMSB_55555}, // default: 4.8 KBPS
		/* 0x04 */ { REG_BITRATELSB, RF_BITRATELSB_55555},
		/* 0x05 */ { REG_FDEVMSB, RF_FDEVMSB_50000}, // default: 5KHz, (FDEV + BitRate / 2 <= 500KHz)
		/* 0x06 */ { REG_FDEVLSB, RF_FDEVLSB_50000},

		/* 0x07 */ { REG_FRFMSB, (uint8_t) (freqBand==RFM69_315MHZ ? RF_FRFMSB_315 : (freqBand==RFM69_433MHZ ? RF_FRFMSB_433 : (freqBand==RFM69_868MHZ ? RF_FRFMSB_868 : RF_FRFMSB_915))) },
		/* 0x08 */ { REG_FRFMID, (uint8_t) (freqBand==RFM69_315MHZ ? RF_FRFMID_315 : (freqBand==RFM69_433MHZ ? RF_FRFMID_433 : (freqBand==RFM69_868MHZ ? RF_FRFMID_868 : RF_FRFMID_915))) },
		/* 0x09 */ { REG_FRFLSB, (uint8_t) (freqBand==RFM69_315MHZ ? RF_FRFLSB_315 : (freqBand==RFM69_433MHZ ? RF_FRFLSB_433 : (freqBand==RFM69_868MHZ ? RF_FRFLSB_868 : RF_FRFLSB_915))) },

		// looks like PA1 and PA2 are not implemented on RFM69W/CW, hence the max output power is 13dBm
		// +17dBm and +20dBm are possible on RFM69HW
		// +13dBm formula: Pout = -18 + OutputPower (with PA0 or PA1**)
		// +17dBm formula: Pout = -14 + OutputPower (with PA1 and PA2)**
		// +20dBm formula: Pout = -11 + OutputPower (with PA1 and PA2)** and high power PA settings (section 3.3.7 in datasheet)
		///* 0x11 */ { REG_PALEVEL, RF_PALEVEL_PA0_ON | RF_PALEVEL_PA1_OFF | RF_PALEVEL_PA2_OFF | RF_PALEVEL_OUTPUTPOWER_11111},
		///* 0x13 */ { REG_OCP, RF_OCP_ON | RF_OCP_TRIM_95 }, // over current protection (default is 95mA)

		// RXBW defaults are { REG_RXBW, RF_RXBW_DCCFREQ_010 | RF_RXBW_MANT_24 | RF_RXBW_EXP_5} (RxBw: 10.4KHz)
		/* 0x19 */ { REG_RXBW, RF_RXBW_DCCFREQ_010 | RF_RXBW_MANT_16 | RF_RXBW_EXP_2 }, // (BitRate < 2 * RxBw)
		//for BR-19200: /* 0x19 */ { REG_RXBW, RF_RXBW_DCCFREQ_010 | RF_RXBW_MANT_24 | RF_RXBW_EXP_3 },
		/* 0x25 */ { REG_DIOMAPPING1, RF_DIOMAPPING1_DIO0_01 }, // DIO0 is the only IRQ we're using
		/* 0x26 */ { REG_DIOMAPPING2, RF_DIOMAPPING2_CLKOUT_OFF }, // DIO5 ClkOut disable for power saving
		/* 0x28 */ { REG_IRQFLAGS2, RF_IRQFLAGS2_FIFOOVERRUN }, // writing to this bit ensures that the FIFO & status flags are reset
		/* 0x29 */ { REG_RSSITHRESH, 220 }, // must be set to dBm = (-Sensitivity / 2), default is 0xE4 = 228 so -114dBm
		///* 0x2D */ { REG_PREAMBLELSB, RF_PREAMBLESIZE_LSB_VALUE } // default 3 preamble bytes 0xAAAAAA
		/* 0x2E */ { REG_SYNCCONFIG, RF_SYNC_ON | RF_SYNC_FIFOFILL_AUTO | RF_SYNC_SIZE_2 | RF_SYNC_TOL_0 },
		/* 0x2F */ { REG_SYNCVALUE1, 0x2D },      // attempt to make this compatible with sync1 byte of RFM12B lib
		/* 0x30 */ { REG_SYNCVALUE2, networkID }, // NETWORK ID
		//* 0x31 */ { REG_SYNCVALUE3, 0xAA },
		//* 0x31 */ { REG_SYNCVALUE4, 0xBB },
		/* 0x37 */ { REG_PACKETCONFIG1, RF_PACKET1_FORMAT_VARIABLE | RF_PACKET1_DCFREE_OFF | RF_PACKET1_CRC_ON | RF_PACKET1_CRCAUTOCLEAR_ON | RF_PACKET1_ADRSFILTERING_OFF },
		/* 0x38 */ { REG_PAYLOADLENGTH, 66 }, // in variable length mode: the max frame size, not used in TX
		///* 0x39 */ { REG_NODEADRS, nodeID }, // turned off because we're not using address filtering
		/* 0x3C */ { REG_FIFOTHRESH, RF_FIFOTHRESH_TXSTART_FIFONOTEMPTY | RF_FIFOTHRESH_VALUE }, // TX on FIFO not empty
		/* 0x3D */ { REG_PACKETCONFIG2, RF_PACKET2_RXRESTARTDELAY_2BITS | RF_PACKET2_AUTORXRESTART_OFF | RF_PACKET2_AES_OFF }, // RXRESTARTDELAY must match transmitter PA ramp-down time (bitrate dependent)
		//for BR-19200: /* 0x3D */ { REG_PACKETCONFIG2, RF_PACKET2_RXRESTARTDELAY_NONE | RF_PACKET2_AUTORXRESTART_ON | RF_PACKET2_AES_OFF }, // RXRESTARTDELAY must match transmitter PA ramp-down time (bitrate dependent)
		/* 0x6F */ { REG_TESTDAGC, RF_DAGC_IMPROVED_LOWBETA0 }, // run DAGC continuously in RX mode for Fading Margin Improvement, recommended default for AfcLowBetaOn=0
		{255, 0}
	};

	HAL_GPIO_WritePin(dev->slaveSelectPort, dev->slaveSelectPin, 1);

	uint32_t start = HAL_GetTick();
	uint8_t timeout = 50;
	do RFM69WriteReg(dev, REG_SYNCVALUE1, 0xAA); while (RFM69ReadReg(dev, REG_SYNCVALUE1) != 0xaa && HAL_GetTick()-start < timeout);
	start = HAL_GetTick();
	do RFM69WriteReg(dev, REG_SYNCVALUE1, 0x55); while (RFM69ReadReg(dev, REG_SYNCVALUE1) != 0x55 && HAL_GetTick()-start < timeout);

	for (uint8_t i = 0; CONFIG[i][0] != 255; i++){
		RFM69WriteReg(dev, CONFIG[i][0], CONFIG[i][1]);
	}

	// Encryption is persistent between resets and can trip you up during debugging.
	// Disable it during initialization so we always start from a known state.
	RFM69Encrypt(dev, 0);

	RFM69SetHighPower(dev, dev->radio.isRFM69HW_HCW); // called regardless if it's a RFM69W or RFM69HW (at this point _isRFM69HW may not be explicitly set by constructor and setHighPower() may not have been called yet (ie called after initialize() call)
	RFM69SetMode(dev, RFM69_MODE_STANDBY);	//default should be RFM69_MODE_STANDBY
	start = HAL_GetTick();
	while (((RFM69ReadReg(dev, REG_IRQFLAGS1) & RF_IRQFLAGS1_MODEREADY) == 0x00) && HAL_GetTick()-start < timeout); // wait for ModeReady
	if (HAL_GetTick()-start >= timeout){
		return false;
	}
//  attachInterrupt(_interruptNum, RFM69::isr0, RISING);

	return true;
}

void RFM69SelectSPI(RFM69_HandleTypeDef* dev){
	//possibly set up spi settings?  Only needed if some other device is sharing the spi bus and that other device requires different bus settings.
	HAL_GPIO_WritePin(dev->slaveSelectPort, dev->slaveSelectPin, 0);
}

void RFM69UnselectSPI(RFM69_HandleTypeDef* dev){
	//possibly set up spi settings?  Only needed if some other device is sharing the spi bus and that other device requires different bus settings.
	HAL_GPIO_WritePin(dev->slaveSelectPort, dev->slaveSelectPin, 1);
}

void RFM69WriteReg(RFM69_HandleTypeDef* dev, uint8_t addr, uint8_t value){
	RFM69SelectSPI(dev);
	addr |= 0x80;
	HAL_SPI_Transmit(dev->spi, &addr, 1, 50);
	HAL_SPI_Transmit(dev->spi, &value, 1, 50);
	RFM69UnselectSPI(dev);
}

uint8_t RFM69ReadReg(RFM69_HandleTypeDef* dev, uint8_t addr){
	RFM69SelectSPI(dev);
	HAL_SPI_Transmit(dev->spi, &addr, 1, 50);
	uint8_t rxVal = 0;
	addr = 0;
	HAL_SPI_TransmitReceive(dev->spi, &addr, &rxVal, 1, 50);

	RFM69UnselectSPI(dev);
	return rxVal;
}

void RFM69Encrypt(RFM69_HandleTypeDef* dev, const char* key) {
	// To enable encryption: radio.encrypt("ABCDEFGHIJKLMNOP");
	// To disable encryption: radio.encrypt(null) or radio.encrypt(0)
	// KEY HAS TO BE 16 bytes !!!

	RFM69SetMode(dev, RFM69_MODE_STANDBY);
	uint8_t validKey = key != 0 && strlen(key)!=0;
	if (validKey){
		memcpy(dev->comm._encryptionKey, key, 16);
		RFM69SelectSPI(dev);
		uint8_t txReg = (REG_AESKEY1 | 0x80);
		HAL_SPI_Transmit(dev->spi, &txReg, 1, 50);
		for (uint8_t i = 0; i < 16; i++){
			HAL_SPI_Transmit(dev->spi, &dev->comm._encryptionKey[i], 1, 50);
		}
		RFM69UnselectSPI(dev);
		dev->comm._useEncryption = true;
	}else{
		memset(dev->comm._encryptionKey, '0', 16);
		dev->comm._useEncryption = false;
	}
	RFM69WriteReg(dev, REG_PACKETCONFIG2, (RFM69ReadReg(dev, REG_PACKETCONFIG2) & 0xFE) | (validKey ? 1 : 0));
}

void RFM69SetHighPower(RFM69_HandleTypeDef* dev, bool _isRFM69HW_HCW) {
	// for RFM69 HW/HCW only: you must call setHighPower(true) after initialize() or else transmission won't work
	dev->radio.isRFM69HW_HCW = _isRFM69HW_HCW;
	RFM69WriteReg(dev, REG_OCP, _isRFM69HW_HCW ? RF_OCP_OFF : RF_OCP_ON); //disable OverCurrentProtection for HW/HCW
	RFM69SetPowerLevel(dev, dev->radio.powerLevel);
}

void RFM69SetPowerLevel(RFM69_HandleTypeDef* dev, uint8_t newPowerLevel) {
	// Control transmitter output power (this is NOT a dBm value!)
	// the power configurations are explained in the SX1231H datasheet (Table 10 on p21; RegPaLevel p66): http://www.semtech.com/images/datasheet/sx1231h.pdf
	// valid powerLevel parameter values are 0-31 and result in a directly proportional effect on the output/transmission power
	// this function implements 2 modes as follows:
	//   - for RFM69 W/CW the range is from 0-31 [-18dBm to 13dBm] (PA0 only on RFIO pin)
	//   - for RFM69 HW/HCW the range is from 0-22 [-2dBm to 20dBm]  (PA1 & PA2 on PA_BOOST pin & high Power PA settings - see section 3.3.7 in datasheet, p22)
	//   - the HW/HCW 0-24 range is split into 3 REG_PALEVEL parts:
	//     -  0-15 = REG_PALEVEL 16-31, ie [-2 to 13dBm] & PA1 only
	//     - 16-19 = REG_PALEVEL 26-29, ie [12 to 15dBm] & PA1+PA2
	//     - 20-23 = REG_PALEVEL 28-31, ie [17 to 20dBm] & PA1+PA2+HiPower (HiPower is only enabled before going in TX mode, ie by setMode(RF69_MODE_TX)
	// The HW/HCW range overlaps are to smooth out transitions between the 3 PA domains, based on actual current/RSSI measurements
	// Any changes to this function also demand changes in dependent function setPowerDBm()

	uint8_t PA_SETTING;
	if (dev->radio.isRFM69HW_HCW) {
		if (newPowerLevel>23){
			newPowerLevel = 23;
		}
		dev->radio.powerLevel =  newPowerLevel;

		//now set Pout value & active PAs based on _powerLevel range as outlined in summary above
		if (dev->radio.powerLevel < 16) {
			newPowerLevel += 16;
			PA_SETTING = RF_PALEVEL_PA1_ON; // enable PA1 only
		} else {
			if (dev->radio.powerLevel < 20){
				newPowerLevel += 10;
			}else{
				newPowerLevel += 8;
			}
			PA_SETTING = RF_PALEVEL_PA1_ON | RF_PALEVEL_PA2_ON; // enable PA1+PA2
		}
		RFM69SetHighPowerRegs(dev, true); //always call this in case we're crossing power boundaries in TX mode
	} else { //this is a W/CW, register value is the same as _powerLevel
		if (newPowerLevel>31){
			newPowerLevel = 31;
		}
		dev->radio.powerLevel =  newPowerLevel;
		PA_SETTING = RF_PALEVEL_PA0_ON; // enable PA0 only
	}

	//write value to REG_PALEVEL
	RFM69WriteReg(dev, REG_PALEVEL, PA_SETTING | newPowerLevel);
}

void RFM69SetHighPowerRegs(RFM69_HandleTypeDef* dev, bool enable) {
	// internal function - for HW/HCW only:
	// enables HiPower for 18-20dBm output
	// should only be used with PA1+PA2
	if (!dev->radio.isRFM69HW_HCW || dev->radio.powerLevel<20) enable=false;
	RFM69WriteReg(dev, REG_TESTPA1, enable ? 0x5D : 0x55);
	RFM69WriteReg(dev, REG_TESTPA2, enable ? 0x7C : 0x70);
}

void RFM69SetMode(RFM69_HandleTypeDef* dev, uint8_t newMode){
	if (newMode == dev->mode){
		return;
	}

	switch (newMode) {
    	case RFM69_MODE_TX:
    		RFM69WriteReg(dev, REG_OPMODE, (RFM69ReadReg(dev, REG_OPMODE) & 0xE3) | RF_OPMODE_TRANSMITTER);
    		if (dev->radio.isRFM69HW_HCW == true) RFM69SetHighPowerRegs(dev, true);
    		break;
    	case RFM69_MODE_RX:
    		RFM69WriteReg(dev, REG_OPMODE, (RFM69ReadReg(dev, REG_OPMODE) & 0xE3) | RF_OPMODE_RECEIVER);
    		if (dev->radio.isRFM69HW_HCW) RFM69SetHighPowerRegs(dev, false);
    		break;
    	case RFM69_MODE_SYNTH:
    		RFM69WriteReg(dev, REG_OPMODE, (RFM69ReadReg(dev, REG_OPMODE) & 0xE3) | RF_OPMODE_SYNTHESIZER);
    		break;
    	case RFM69_MODE_STANDBY:
    		RFM69WriteReg(dev, REG_OPMODE, (RFM69ReadReg(dev, REG_OPMODE) & 0xE3) | RF_OPMODE_STANDBY);
    		break;
    	case RFM69_MODE_SLEEP:
    		RFM69WriteReg(dev, REG_OPMODE, (RFM69ReadReg(dev, REG_OPMODE) & 0xE3) | RF_OPMODE_SLEEP);
    		break;
    	default:
    		return;
	}

	// we are using packet mode, so this check is not really needed
	// but waiting for mode ready is necessary when going from sleep because the FIFO may not be immediately available from previous mode
	while (dev->mode == RFM69_MODE_SLEEP && (RFM69ReadReg(dev, REG_IRQFLAGS1) & RF_IRQFLAGS1_MODEREADY) == 0x00); // wait for ModeReady

	dev->mode = newMode;
}

/*
void RFM69SpyMode(RFM69_HandleTypeDef* dev, bool onOff) {
	// true = disable ID filtering to capture all packets on network, regardless of TARGETID
	// false (default) = enable node/broadcast ID filtering to capture only frames sent to this/broadcast address
	dev->spyModeEnabled = onOff;
  //writeReg(REG_PACKETCONFIG1, (readReg(REG_PACKETCONFIG1) & 0xF9) | (onOff ? RF_PACKET1_ADRSFILTERING_OFF : RF_PACKET1_ADRSFILTERING_NODEBROADCAST));
}
*/

void RFM69Sleep(RFM69_HandleTypeDef* dev) {
	//put transceiver in sleep mode to save battery - to wake or resume receiving just call RFM69ReceiveDone()
	RFM69SetMode(dev, RFM69_MODE_SLEEP);
}


bool RFM69ReceiveDone(RFM69_HandleTypeDef* dev) {
	// checks if a packet was received and/or puts transceiver in receive (ie RX or listen) mode
	if (dev->rxIrqFlag == true) {
		dev->rxIrqFlag = false;
		RFM69InterruptHandler(dev);
	}
	if(dev->mode == RFM69_MODE_RX && dev->rxMsg.payloadLen > 0){
		RFM69SetMode(dev, RFM69_MODE_STANDBY); // enables interrupts
		return true;
	}else if(dev->mode == RFM69_MODE_RX){ // already in RX no payload yet
		return false;
	}
	RFM69ReceiveBegin(dev);
	return false;
}

void RFM69ReceiveBegin(RFM69_HandleTypeDef* dev) {
	// internal function
	dev->rxMsg.ackRssi = 0;
	dev->rxMsg.ackRssiRequested = false;
	dev->rxMsg.dataLen = 0;
	dev->rxMsg.senderId = 0;
	dev->rxMsg.targetId = 0;
	dev->rxMsg.payloadLen = 0;
	dev->rxMsg.ackRequested = false;
	dev->rxMsg.ackReceived = false;

//#if defined(RF69_LISTENMODE_ENABLE)
//  RF69_LISTEN_BURST_REMAINING_MS = 0;
//#endif
	dev->rxMsg.rssi = 0;
	dev->rxMsg.rssiRequested = false;
	if (RFM69ReadReg(dev, REG_IRQFLAGS2) & RF_IRQFLAGS2_PAYLOADREADY){
		RFM69WriteReg(dev, REG_PACKETCONFIG2, (RFM69ReadReg(dev, REG_PACKETCONFIG2) & 0xFB) | RF_PACKET2_RXRESTART); // avoid RX deadlocks
	}
	RFM69WriteReg(dev, REG_DIOMAPPING1, RF_DIOMAPPING1_DIO0_01); // set DIO0 to "PAYLOADREADY" in receive mode
	RFM69SetMode(dev, RFM69_MODE_RX);
}


void RFM69InterruptHandler(RFM69_HandleTypeDef* dev) {
	//if (dev->mode == RFM69_MODE_RX && (RFM69ReadReg(dev, REG_IRQFLAGS2) & RF_IRQFLAGS2_PAYLOADREADY)){
	if (((dev->mode == RFM69_MODE_RX) || (dev->mode == RFM69_MODE_STANDBY)) && (RFM69ReadReg(dev, REG_IRQFLAGS2) & RF_IRQFLAGS2_PAYLOADREADY)){
		RFM69SetMode(dev, RFM69_MODE_STANDBY);
		RFM69SelectSPI(dev);
		uint8_t rxByte = 0;
		uint8_t txByte = (REG_FIFO & 0x7F);
		HAL_SPI_Transmit(dev->spi, &txByte, 1, 50);
		txByte = 0;
		HAL_SPI_TransmitReceive(dev->spi, &txByte, &dev->rxMsg.payloadLen, 1, 50);
		dev->rxMsg.payloadLen = (dev->rxMsg.payloadLen > 66 ? 66 : dev->rxMsg.payloadLen); // precaution
		HAL_SPI_TransmitReceive(dev->spi, &txByte, &rxByte, 1, 50);
		dev->rxMsg.targetId = (int16_t)rxByte;
		HAL_SPI_TransmitReceive(dev->spi, &txByte, &rxByte, 1, 50);
		dev->rxMsg.senderId = (int16_t)rxByte;
		uint8_t CTLbyte = 0;
		HAL_SPI_TransmitReceive(dev->spi, &txByte, &CTLbyte, 1, 50);
		dev->rxMsg.targetId |= ((uint16_t)(CTLbyte) & 0x0C) << 6; //10 bit address (most significant 2 bits stored in bits(2,3) of CTL byte
		dev->rxMsg.senderId |= ((uint16_t)(CTLbyte) & 0x03) << 8; //10 bit address (most sifnigicant 2 bits stored in bits(0,1) of CTL byte
		if( (!( (dev->spyModeEnabled) || (dev->rxMsg.targetId == dev->comm.myId) || (dev->rxMsg.targetId == RFM69_BROADCAST_ADDR) )) // match this node's address, or broadcast address or anything in spy mode
				|| (dev->rxMsg.payloadLen < 3) ){ // address situation could receive packets that are malformed and don't fit this libraries extra fields
			dev->rxMsg.payloadLen = 0;
			RFM69UnselectSPI(dev);
			RFM69ReceiveBegin(dev);
			return;
		}

		dev->rxMsg.dataLen = dev->rxMsg.payloadLen - 3;
		dev->rxMsg.ackReceived = CTLbyte & RFM69_CTL_SENDACK; // extract ACK-received flag
		dev->rxMsg.ackRequested = CTLbyte & RFM69_CTL_REQACK; // extract ACK-requested flag
		uint8_t _pl = dev->radio.powerLevel; //interruptHook() can change _powerLevel so remember it
		RFM69InterruptHook(dev, CTLbyte);    // TWS: hook to derived class interrupt function

		for (uint8_t i = 0; i < dev->rxMsg.dataLen; i++){
			HAL_SPI_TransmitReceive(dev->spi, &txByte, &dev->rxMsg.dataBuff[i], 1, 50);
		}

		dev->rxMsg.dataBuff[dev->rxMsg.dataLen] = 0; // add null at end of string

		//If this message was an ack, find out if the other device has more messages waiting for this device:
		if((dev->rxMsg.ackReceived == true) && (dev->rxMsg.dataLen > 0) && (dev->rxMsg.dataBuff[0] == '1') ){
			dev->rxMsg.messagesPending = true;
		}else{
			dev->rxMsg.messagesPending = false;
		}

		RFM69UnselectSPI(dev);
		RFM69SetMode(dev, RFM69_MODE_RX);
		if (_pl != dev->radio.powerLevel){
			RFM69SetPowerLevel(dev, dev->radio.powerLevel); //set new _powerLevel if changed
		}
	}
	dev->rxMsg.rssi = RFM69ReadRSSI(dev, false); // readRSSI();
}


int16_t RFM69ReadRSSI(RFM69_HandleTypeDef* dev, bool forceTrigger) {
	// get the received signal strength indicator (RSSI)
	int16_t rssi = 0;
	if (forceTrigger){
		// RSSI trigger not needed if DAGC is in continuous mode
		RFM69WriteReg(dev, REG_RSSICONFIG, RF_RSSI_START);
		while ((RFM69ReadReg(dev, REG_RSSICONFIG) & RF_RSSI_DONE) == 0x00); // wait for RSSI_Ready
	}
	rssi = -RFM69ReadReg(dev, REG_RSSIVALUE);
	rssi >>= 1;
	return rssi;
}

void RFM69InterruptHook(RFM69_HandleTypeDef* dev, uint8_t CTLbyte) {
	dev->rxMsg.ackRssiRequested = CTLbyte & RFM69_CTL_RESERVE1; // TomWS1: extract the ACK RSSI request bit (could potentially merge with ACK_REQUESTED)
	// TomWS1: now see if this was an ACK with an ACK_RSSI response
	if (dev->rxMsg.ackReceived && dev->rxMsg.ackRssiRequested) {
    // the next two bytes contain the ACK_RSSI (assuming the datalength is valid)
		if (dev->rxMsg.dataLen >= 1) {
			uint8_t txByte = 0;
			uint8_t tmpRssi = 0;
			HAL_SPI_TransmitReceive(dev->spi, &txByte, &tmpRssi, 1, 50);
			dev->rxMsg.ackRssi = (int16_t)(tmpRssi * -1);	//rssi was sent as single byte positive value, get the real value by * -1
			dev->rxMsg.dataLen -= 1;   // and compensate data length accordingly
			// TomWS1: Now dither transmitLevel value (register update occurs later when transmitting);
			if (dev->radio.targetRssi != 0) {
				uint8_t maxLevel = (dev->radio.isRFM69HW_HCW ? 23 : 31);
				if (dev->rxMsg.ackRssi < dev->radio.targetRssi && dev->radio.powerLevel < maxLevel) {
					dev->radio.powerLevel += dev->radio.transmitLevelStep;
				}else if (dev->rxMsg.ackRssi > dev->radio.targetRssi && dev->radio.powerLevel > 0){
					dev->radio.powerLevel--;
				}
			}
		}
	}
}

bool RFM69SendWithRetry(RFM69_HandleTypeDef* dev, uint16_t toAddress, const void* buffer, uint8_t bufferSize, uint8_t retries, uint32_t retryWaitTime) {
	//=============================================================================
	//  sendWithRetry() - overrides the base to allow increasing power when repeated ACK requests fail
	//=============================================================================
	uint32_t sentTime;
	for (uint8_t i = 0; i <= retries; i++) {
		RFM69Send(dev, toAddress, buffer, bufferSize, true);
		sentTime = HAL_GetTick();
		uint8_t maxLevel = dev->radio.isRFM69HW_HCW ? 23 : 31;
		while (HAL_GetTick() - sentTime < retryWaitTime){
			if(RFM69AckReceived(dev, toAddress)){
				return true;
			}
		}
		if ((dev->radio.useATC == true) && (dev->radio.powerLevel < maxLevel)) {
			RFM69SetPowerLevel(dev, dev->radio.powerLevel + dev->radio.transmitLevelStep);
		}
	}
	return false;
}

// should be polled immediately after sending a packet with ACK request
bool RFM69AckReceived(RFM69_HandleTypeDef* dev, uint16_t fromNodeID) {
	if (RFM69ReceiveDone(dev)){
		dev->lastAckReceivedTime = HAL_GetTick();
		return (( (dev->rxMsg.senderId == fromNodeID) || (fromNodeID == RFM69_BROADCAST_ADDR) ) && dev->rxMsg.ackReceived);
	}
	return false;
}

void RFM69Send(RFM69_HandleTypeDef* dev, uint16_t toAddress, const void* buffer, uint8_t bufferSize, bool requestACK){
	RFM69WriteReg(dev, REG_PACKETCONFIG2, (RFM69ReadReg(dev, REG_PACKETCONFIG2) & 0xFB) | RF_PACKET2_RXRESTART); // avoid RX deadlocks
	uint32_t now = HAL_GetTick();
	while (!RFM69CanSend(dev) && HAL_GetTick() - now < RFM69_CSMA_LIMIT_MS){
		RFM69ReceiveDone(dev);
	}
	RFM69SendFrame(dev, toAddress, buffer, bufferSize, requestACK, false, false, 0);
}

bool RFM69CanSend(RFM69_HandleTypeDef* dev){
	if (dev->mode == RFM69_MODE_RX && dev->rxMsg.payloadLen == 0 && RFM69ReadRSSI(dev, false) < CSMA_LIMIT){ // if signal stronger than -100dBm is detected assume channel activity{
		RFM69SetMode(dev, RFM69_MODE_STANDBY);
    	return true;
	}
	return false;
}

void RFM69SendFrame(RFM69_HandleTypeDef* dev, uint16_t toAddress, const void* buffer, uint8_t bufferSize, bool requestACK, bool sendACK, bool sendRSSI, int16_t lastRSSI) {
	RFM69SetMode(dev, RFM69_MODE_STANDBY); // turn off receiver to prevent reception while filling fifo
	while ((RFM69ReadReg(dev, REG_IRQFLAGS1) & RF_IRQFLAGS1_MODEREADY) == 0x00){
		// wait for ModeReady
	}
	//writeReg(REG_DIOMAPPING1, RF_DIOMAPPING1_DIO0_00); // DIO0 is "Packet Sent"

	bufferSize += (sendACK && sendRSSI)?1:0;  // if sending ACK_RSSI then increase data size by 1
	if (bufferSize > RFM69_MAX_DATA_LEN){
		bufferSize = RFM69_MAX_DATA_LEN;
	}

	// write to FIFO
	RFM69SelectSPI(dev);
	uint8_t txData = (REG_FIFO | 0x80);
	HAL_SPI_Transmit(dev->spi, &txData, 1, 50);
	txData = (bufferSize + 3);
	HAL_SPI_Transmit(dev->spi, &txData, 1, 50);
	txData = (uint8_t)toAddress;				//lower 8 bits
	HAL_SPI_Transmit(dev->spi, &txData, 1, 50);
	txData = (uint8_t)dev->comm.myId;			//lower 8 bits
	HAL_SPI_Transmit(dev->spi, &txData, 1, 50);


	// CTL (control byte)
	uint8_t CTLbyte=0x0;
	if (toAddress > 0xFF){
		CTLbyte |= (toAddress & 0x300) >> 6; //assign last 2 bits of address if > 255
	}
	if (dev->comm.myId > 0xFF){
		CTLbyte |= (dev->comm.myId & 0x300) >> 8;   //assign last 2 bits of address if > 255
	}

	if (sendACK) {                   // TomWS1: adding logic to return ACK_RSSI if requested
		//_spi->transfer(CTLbyte | RFM69_CTL_SENDACK | (sendRSSI?RFM69_CTL_RESERVE1:0));  // TomWS1  TODO: Replace with EXT1
		txData = (CTLbyte | RFM69_CTL_SENDACK | (sendRSSI?RFM69_CTL_RESERVE1:0));
		HAL_SPI_Transmit(dev->spi, &txData, 1, 50);
		if (sendRSSI) {
			//_spi->transfer(abs(lastRSSI)); //RSSI dBm is negative expected between [-100 .. -20], convert to positive and pass along as single extra header byte
			txData = (lastRSSI > 0 ? lastRSSI : -lastRSSI);
			HAL_SPI_Transmit(dev->spi, &txData, 1, 50);
			bufferSize -=1;              // account for the extra ACK-RSSI 'data' byte
		}
	}else if (requestACK) {  // TODO: add logic to request ackRSSI with ACK - this is when both ends of a transmission would dial power down. May not work well for gateways in multi node networks
//start here
		//_spi->transfer(CTLbyte | (_targetRSSI ? RFM69_CTL_REQACK | RFM69_CTL_RESERVE1 : RFM69_CTL_REQACK));
		txData = (CTLbyte | (dev->radio.targetRssi ? RFM69_CTL_REQACK | RFM69_CTL_RESERVE1 : RFM69_CTL_REQACK));
		HAL_SPI_Transmit(dev->spi, &txData, 1, 50);
	}else{
		//_spi->transfer(CTLbyte);
		txData = CTLbyte;
		HAL_SPI_Transmit(dev->spi, &txData, 1, 50);
	}

	for (uint8_t i = 0; i < bufferSize; i++){
		//_spi->transfer(((uint8_t*) buffer)[i]);
		txData = ((uint8_t*) buffer)[i];
		HAL_SPI_Transmit(dev->spi, &txData, 1, 50);
	}
	RFM69UnselectSPI(dev);

	// no need to wait for transmit mode to be ready since its handled by the radio
	RFM69SetMode(dev, RFM69_MODE_TX);
	//uint32_t txStart = HAL_GetTick();
	//while (digitalRead(_interruptPin) == 0 && HAL_GetTick() - txStart < RF69_TX_LIMIT_MS); // wait for DIO0 to turn HIGH signalling transmission finish
	while ((RFM69ReadReg(dev, REG_IRQFLAGS2) & RF_IRQFLAGS2_PACKETSENT) == 0x00){
		// wait for PacketSent
	}
	RFM69SetMode(dev, RFM69_MODE_STANDBY);
}

void RFM69SendACK(RFM69_HandleTypeDef* dev, const void* buffer, uint8_t bufferSize) {
	// should be called immediately after reception in case sender wants ACK
	dev->rxMsg.ackRequested = 0;   // TomWS1 added to make sure we don't end up in a timing race and infinite loop sending Acks
	uint16_t sender = dev->rxMsg.senderId;
	int16_t _RSSI = dev->rxMsg.rssi; // save payload received RSSI value
	bool sendRSSI = dev->rxMsg.ackRssiRequested;
	RFM69WriteReg(dev, REG_PACKETCONFIG2, (RFM69ReadReg(dev, REG_PACKETCONFIG2) & 0xFB) | RF_PACKET2_RXRESTART); // avoid RX deadlocks
	uint32_t now = HAL_GetTick();
	while (!RFM69CanSend(dev) && HAL_GetTick() - now < RFM69_CSMA_LIMIT_MS){
		RFM69ReceiveDone(dev);
	}
	dev->rxMsg.senderId = sender;    // TomWS1: Restore SenderID after it gets wiped out by receiveDone()
	RFM69SendFrame(dev, sender, buffer, bufferSize, false, true, sendRSSI, _RSSI);   // TomWS1: Special override on sendFrame with extra params
	dev->rxMsg.rssi = _RSSI; // restore payload RSSI
}


void RFM69SetAddress(RFM69_HandleTypeDef* dev, uint16_t addr){
	//set this node's address
	dev->comm.myId = addr;
	RFM69WriteReg(dev, REG_NODEADRS, dev->comm.myId); //unused in packet mode
}


void RFM69SetNetwork(RFM69_HandleTypeDef* dev, uint8_t networkID){
	//set this node's network id
	dev->comm.networkId = networkID;
	RFM69WriteReg(dev, REG_SYNCVALUE2, networkID);
}

