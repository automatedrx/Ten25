
#pragma once

#ifndef COMM915_H
#define COMM915_H

/*
//messaging protocol
//=======================

8 bytes(max) of data for CAN, 61(max) for radio.

Message format:  [Message Type] [Category] [Param Number] [opt. additional parameter] [data]
					1 byte		1 byte		1 byte			0 or 1 byte					57-58 bytes max

Byte[0]=  Message type
	? = request for data (parameter specified in next byte)
	@ = Data update broadcase and/or response to data request.  
	! = Command to set data.  parameter specified in next byte, data following in subsequent bytes.
	# = Comment, typically used for debugging purposes.
	
Byte[1]: Category	(0-9, A-Z, a-z) = 62 possible categories

Byte[2]: Parameter number (0-9, A-Z, a-z) = 62 possible parameters

Byte[3] to [x]: Data

*/

//Message Type:
typedef enum {
	commandType_Request 	= '?',
	commandType_Response 	= '@',
	commandType_SetData		= '!',
	commandType_Comment		= '#'
} commandTypeEnum;

//Categories: 
#define 	COMM_CATEGORY_INFORMATION		'0'
#define		COMM_CATEGORY_CANNEDMESSAGE		'1'
#define 	COMM_CATEGORY_OPERATION			'2'
#define		COMM_PARAMARRAY_MESSAGE			'3'
#define		COMM_CATEGORY_INPUTS			'4'
#define		COMM_CATEGORY_OUTPUTS			'5'
#define 	COMM_CATEGORY_POWERSETTINGS		'6'
#define 	COMM_CATEGORY_COMMUNICATIONS	'8'
#define		COMM_CATEGORY_CALIBRATIONDATA	'A'
//#define		COMM_CATEGORY_SENSORS			'A'


//Category: Device Information
#define		COMM_DEVINFO_SUMMARY			'0'	//A packet of identification info about the device: DeviceId, NetworkId, DeviceName, SleepMode, AutosendInterval, SerialNumber
#define		COMM_DEVINFO_DEVICEID			'1'	//BoardId or ProductId			
#define		COMM_DEVINFO_FWREV				'2'	//Firmware Rev			
#define		COMM_DEVINFO_PRODUCTNAME		'3'	//e.g. "Temp915H"		
#define		COMM_DEVINFO_SERIALNUMBER		'4'	//This is used to provide the serial number and also used to broadcast a device's id with serial number attached for identification and verification purposes.
												//When a device gets a new id, the device will then send out a commandType_Comment with this parameter to the broadcast address.  All devices on the network will then have this device's new id number.

#define		COMM_DEVINFO_CUSTOMNAME			'5'	//e.g. "Fam Room Temp"			
#define		COMM_DEVINFO_LOCATION			'6'	//e.g. "Zone 3" or "Garage"			

//Category: Device Operation ==================
#define		COMM_DEVOP_STATUS				'0'	//Current status: Idle, Running, Alarm, etc.
#define		COMM_DEVOP_ALARMCOUNT			'5'	//Total count of active alarms
#define		COMM_DEVOP_ALARMNUMBER			'6'	//* Get the id number of the alarm stored in the INDEX number following this byte
	//Next field must include the index #
#define		COMM_DEVOP_ALARMTEXT			'7'	//* Get the text/description of the alarm stored in the INDEX number following this byte
	//Next field must include the index #
#define		COMM_DEVOP_CLEARALARMS			'8'	//Reset pending alarms

//Category: ParamArray Message ================
  //This category allows a device to send multiple parameters within a single message.  
#define		COMM_PARRAY_DEVSPEC				'0'	//The inluded parameters will be specified according to the DeviceId of the sending device.  This is the "stock" param array message for each device type.
#define		COMM_PARRAY_CUSTOM				'2'

//Category: Device Input Data =================
#define		COMM_DEVOP_INPUTCOUNT			'0'	// Provides the total count of inputs.  This can be used as upper bounds for accessing INPUTPARAMNAME and INPUTPARAMNUM fields.
#define		COMM_DEVOP_INPUTPARAMLIST		'1'	//* The device has an array of all input parameters available, and that array is exposed here.  Get the parameter number of the input stored in the INDEX number following this byte  e.g. This device has 3 inputs: Param77, Param83, and Param104.  Accessing the following indexes here will result in:  index[0]=77, index[1]=83, index[2]=104.
	//Next field must include the index #
#define		COMM_DEVOP_INPUTPNAME			'3'	//* The next byte will be the Index for the associated input.  This is used to get/set the name for the input.
	//Next field must include the Index #
#define		COMM_DEVOP_INPUTVALUE			'5'	//* The next byte will be the Index for the associated input.  This is used to get/set the value for the input.
	//Next field must include the Index #
#define		COMM_DEVOP_INPUTUNITS			'7'	//* The next byte will be the Index for the associated input.  This is used to get/set the units for the input.
	//Next field must include the Index #
#define		COMM_DEVOP_INPUTDISPLAYUNIT		'9' //* The next byte will be the Index for the associated input.  This is used to get/set the display unit for the input.


//Category: Device Output Data ================
#define		COMM_DEVOP_OUTPUTCOUNT			'0'	// Provides the total count of outputs.  This can be used as upper bounds for accessing OUTPUTPARAMNAME and OUTPUTPARAMNUM fields.
#define		COMM_DEVOP_OUTPUTPARAMLIST		'1'	//* The device has an array of all output parameters available, and that array is exposed here.  Get the parameter number of the output stored in the INDEX number following this byte  e.g. This device has 3 outputs: Param147, Param24, and Param210.  Accessing the following indexes here will result in:  index[0]=147, index[1]=24, index[2]=210.
	//Next field must include the Index #
#define		COMM_DEVOP_OUTPUTPNAME			'3'	//* The next byte will be the Index for the associated output.  This is used to get/set the name for the output.
	//Next field must include the Index #
#define		COMM_DEVOP_OUTPUTVALUE			'5'	//* The next byte will be the Index for the associated output.  This is used to get/set the value for the output.
	//Next field must include the Index #
#define		COMM_DEVOP_OUTPUTUNITS			'7'	//* The next byte will be the Index for the associated output.  This is used to get/set the units for the output.
	//Next field must include the Index #
#define		COMM_DEVOP_OUTPUTDISPLAYUNIT	'9' //* The next byte will be the Index for the associated output.  This is used to get/set the display unit for the output.



//Category: Device Power Settings =============

#define		COMM_POWER_TYPE					'0'	//Type of power scheme for the device, e.g. Ext Power, Battery backup, battery, etc
#define		COMM_POWER_SLEEPMODE			'1'	//See sleepModeEnum
#define		COMM_POWER_SLEEPPERIOD			'2'	//Length of sleep period in ms
#define		COMM_POWER_BATTERYTYPE			'3' //See batteryTypeEnum
#define		COMM_POWER_ALLOWCHARGING		'4' //boolean
#define		COMM_POWER_BATTERYPERCENTAGE	'5'
#define		COMM_POWER_BATTERYVOLTAGE		'6'
//#define		COMM_POWER_EXTPOWERPRESENT		'7'
//#define		COMM_POWER_CHARGINGSTATUS		'8'
#define		COMM_POWER_POWERSTATUS			'7'


//Category: Device Communication Settings =====

#define		COMM_COMM_JOINNETWORKREQUEST	'0'	//Used for sending a request to join a network
#define		COMM_COMM_JOINNETWORKRESPONSE	'1'	//Includes all data needed for joining new network
#define		COMM_COMM_DEAUTH				'2'	//1. This is sent by the gateway to deauth a particular device.  The device will then go into network-join mode and request to (re)join the network.  
												//2. A device will send this message out to it's own address.  The sending device will not receive it since it is transmitting it.  If a device receives this address, it has the same ID as the device that transmitted it which means there's a conflict.
#define		COMM_COMM_GETFIRSTPENDINGMSG	'3' //A device is requesting the first pending message from the gateway.

#define		COMM_COMM_AUTOSENDINTERVAL		'5'
#define		COMM_COMM_AUTOSENDPARAMCOUNT	'6'
#define		COMM_COMM_AUTOSENDPARAMLIST		'7'
	//Next field must include the index #
#define		COMM_COMM_CLEARAUTOSENDLIST		'8'
#define		COMM_COMM_ADDAUTOSENDPARAM		'9'

#define   	COMM_COMM_CLEARDEVICELIST  		'C' //Instruction to clear and (re)initialize the device list
#define   	COMM_COMM_ADDNEWDEVICE  		'D' //Start 'add device' mode

#define		COMM_COMM_RADIOID				'F'
#define		COMM_COMM_RADIONETWORKID		'G'
#define		COMM_COMM_RADIOGATEWAYID		'H'
#define		COMM_COMM_RADIOBROADCASTID		'I'
#define		COMM_COMM_RADIOTXPOWERLEVEL		'J'
#define		COMM_COMM_RADIOUSEAUTOTXPOWER	'K'
#define		COMM_COMM_RADIORSSITARGET		'L'


//Category: Calibration Data ==================
#define		COMM_SENSORS_TEMPLOWERCALTARGET		'0'
#define		COMM_SENSORS_TEMPLOWERCALACTUAL		'1'
#define		COMM_SENSORS_TEMPUPPERCALTARGET		'2'
#define		COMM_SENSORS_TEMPUPPERCALACTUAL		'3'





typedef enum {
	batteryTypeUnknown 	= 0,
	batteryTypeNiMH 	= 1,
	batteryTypeLiIon	= 2,
	batteryTypeLipo		= 3,
	batteryTypeAlkaline = 4
} batteryTypeEnum;

/*
typedef enum {
	chargingStatusNoCharger			= 0,
	chargingStatus_NotCharging		= 1,
	chargingStatus_SlowCharging		= 2,
	chargingStatus_FastCharging		= 3,
	chargingStatus_ChargeComplete	= 4
} chargingStatusEnum;
*/

typedef enum {
	tempDisplayUnit_None		= 0,
	tempDisplayUnit_C			= 1,
	tempDisplayUnit_F			= 2,
	tempDisplayUnit_Alternate	= 3
} tempDisplayUnitEnum;


typedef enum {						// default paramArray parameters for each device type:
	boardId_Unknown			= 0,
	boardId_Temp915A		= 1,
	boardId_Temp915B		= 2,
	boardId_Temp915C		= 3,
	boardId_Temp915D		= 4,
	boardId_Temp915E		= 5,
	boardId_Temp915F		= 6,
	boardId_Temp915G		= 7,
	boardId_Temp915H		= 8,
	boardId_Temp915TStatG	= 9,
	boardId_Temp915TStatGesp	= 10,
	boardId_Temp915I		= 11,	// Last8HexOfSerialNumber, TempC, Humidity, pressure, batteryPercentage, LastRssiFromGateway
	boardId_Temp915J		= 12,
	boardId_Temp915K		= 13
	
} boardIdEnum;

typedef enum {
	sleepMode_AlwaysOn = 0,
	sleepMode_DeepSleep = 1
} sleepModeEnum;

typedef enum {
	hvacMode_Unknown,
	hvacMode_Off,
	hvacMode_Cool,
	hvacMode_Heat,
	hvacMode_Auto
} enumHvacMode;

typedef enum {
	hvacState_Unknown,
	hvacState_Off,
	hvacState_Idle,
	hvacState_Cooling,
	hvacState_Heating
} enumHvacState;

typedef enum {
	fanMode_Unknown,
	fanMode_Off,
	fanMode_On,
	fanMode_Auto
} enumFanMode;

typedef enum {
	fanState_Unknown,
	fanState_Off,
	fanState_Idle,
	fanState_Low,
	fanState_High
} enumFanState;


//Canned Messages:

typedef enum {
  cannedMsg_Startup   = 0,
  cannedMsg_Wakeup    = 1,
  cannedMsg_GoingToSleep  = 2,
  cannedMsg_BatteryLow  = 3,
  cannedMsg_BatteryCritical = 4,
  cannedMsg_PoweringOff = 5
} cannedMsgEnum;
/*
#define CM_Startup  0
#define CM_Wakeup 1
#define CM_GoingToSleep  = 2,
#define CM_BatteryLow  = 3,
#define CM_PoweringOff = 4
*/
char cannedMsgStrings[][30] = {
	{"Startup"},
	{"Wakeup"},
	{"Sleeping"},
	{"Battery Low"},
	{"PoweringOff"},
};

typedef enum {
	comChan_radio,
	comChan_serial1,
	comChan_serial2,
	comChan_wifi
} comChannelEnum;


typedef enum {
	powerStatus_BatteryCritical,
	powerStatus_BatteryLow,
	powerStatus_BatteryOk,
	powerStatus_BatteryCharging,	//It's implied that it is also running on external power, which would be needed to charge the battery.
	powerStatus_ExtPower
} enumPowerStatus;




#endif

/*
 Operation:

 ==Device ID's==
   * On startup, the device will load it's id from flash memory settings.  If it hasn't yet been assigned an id
      then it will create a random id derived from its serial number.
   * Once a device has loaded it's deviceId, it will send out a broadcast message of:
   	   [commandType_Response] [COMM_CATEGORY_INFORMATION] [COMM_DEVINFO_SERIALNUMBER]
   	   This will allow all devices on the network to look up the serial number in their deviceLists and update
   	   the deviceList with the senderId attached to the broadcast message.
   * The serial number broadcast message will be sent out periodically so that any devices that miss the initial
   	   message will eventually get the updated information.
   	   *NOTE: When a gateway receives this serial number broadcast it will look up the serial number in it's deviceList.
   	   	   	  If the senderId of the message does not match what's stored in the deviceList, the gateway could
   	   	   	  optionally send a deauth message to the old deviceId that is stored in the deviceList.




 */
