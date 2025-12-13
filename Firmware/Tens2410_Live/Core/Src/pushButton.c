#include "pushButton.h"



void PB_InitShort(Pushbutton_HandleTypeDef* dev, GPIO_TypeDef* Port, uint16_t Pin, uint8_t ActiveState){
	PB_Init(dev, Port, Pin, ActiveState, 10, 10, 0, 0, pbMode_Momentary);
}

void PB_Init(Pushbutton_HandleTypeDef* dev, GPIO_TypeDef* Port, uint16_t Pin, uint8_t ActiveState, uint32_t DebounceOnTime,
					uint32_t DebounceOffTime, uint32_t LongPressDelay, uint32_t LongPressRepeatInterval, enumPBMode PBMode){
	dev->_useDirectInput = true;
	dev->_useMatrix = false;

	dev->_port = Port;
	dev->_pin = Pin;
	dev->_activeState = ActiveState;
	dev->_debounceOnTime = DebounceOnTime;
	dev->_debounceOffTime = DebounceOffTime;
	dev->longPressDelay = LongPressDelay;
	dev->longPressRepeatInterval = LongPressRepeatInterval;

	dev->_curState = pbState_idle;
	dev->_lastInVal = 0;
	dev->_eventTime = 0;

	dev->_pbMode = PBMode;
	dev->_switchVal = false;
}

void PB_Init_Matrix(Pushbutton_HandleTypeDef* dev, Pushbutton_Matrix_Typedef* MatrixRef, uint8_t Row, uint8_t Col, uint8_t ActiveState, uint32_t DebounceOnTime,
		uint32_t DebounceOffTime, uint32_t LongPressDelay, uint32_t LongPressRepeatInterval, enumPBMode PBMode){
	//This sets up an individual button within a kepad matrix.  Remember to also init the matrix rows and columns with ports & pins.

	dev->_matrix = MatrixRef;

	dev->_useDirectInput = false;
	dev->_useMatrix = true;

	//dev->_port = Port;
	//dev->_pin = Pin;
	dev->_row = Row;
	dev->_col = Col;

	dev->_activeState = ActiveState;
	dev->_debounceOnTime = DebounceOnTime;
	dev->_debounceOffTime = DebounceOffTime;
	dev->longPressDelay = LongPressDelay;
	dev->longPressRepeatInterval = LongPressRepeatInterval;

	dev->_curState = pbState_idle;
	dev->_lastInVal = 0;
	dev->_eventTime = 0;

	dev->_pbMode = PBMode;
	dev->_switchVal = false;
}

/*
uint8_t PB_CheckForEvent(Pushbutton_HandleTypeDef* dev){
	//This reads the digital input and determines whether a pushbutton event has occurred.   The event (if any)
	//will be returned.
	uint8_t retVal = pbEvent_none;
	uint8_t curInVal = (HAL_GPIO_ReadPin(dev->_port, dev->_pin) == dev->_activeState);
	//curState now represents the logic state of the input; 0 = inactive, 1=active.

	switch(dev->_curState){
	case pbState_idle:
		if(curInVal == 1){
			  //Since the overall state is IDLE that means the input was not active last time through the loop.  Start the debounce.
			  dev->_eventTime = HAL_GetTick();
			  dev->_curState = pbState_debounce_On;
		}
		break;
	case pbState_debounce_On:
		if(curInVal == dev->_lastInVal){
			//No input state change since last loop.  Check to see if we've maintained this state long enough to move to a different state:
			if(HAL_GetTick() - dev->_eventTime > dev->_debounceOnTime){
				//Time delay has been satisfied.  Change to the destination state.
				if(curInVal == 1){
					dev->_curState = pbState_shortpress_Pressed;
					dev->_eventTime = HAL_GetTick();	//event time is reset so that we can later track how long the button is held for a long-press.
//Todo:  		  Possibly fire off a callback function here?  Event: Short Press
					retVal = pbEvent_shortpress_Pressed;
				}else{
					dev->_curState = pbState_idle;
					//No need for further action here.  The button was on for less than the minimum debounce time, then went back off.
				}
			}
		}else{
			//Input has changed state since last time through the loop.
			if(curInVal == 1){
				//(re)Start the debounce timer.
				dev->_eventTime = HAL_GetTick();
			}
		}
		break;
	case pbState_shortpress_Pressed:
		if(curInVal == 0){
			//Input is currently inactive.  Start debouncing for turn-off
			dev->_curState = pbState_shortpress_Debounce_Off;
			dev->_eventTime = HAL_GetTick();
		}else{
			//Input is still on.  Has it been held long enough to count as a long-press?
			if( (HAL_GetTick() - dev->_eventTime > dev->longPressDelay) && (dev->longPressDelay > 0) ){
				dev->_curState = pbState_longpress_Pressed;
				dev->_eventTime = HAL_GetTick();
//Todo:  	  Possibly fire off a callback function here?  Event: LongPress
				retVal = pbEvent_longpress_Pressed;
			}
		}
		break;
	case pbState_shortpress_Debounce_Off:
		if(curInVal == 1){
			//Input is back on.  Return to the pressed state.
			dev->_eventTime = HAL_GetTick();
			dev->_curState = pbState_shortpress_Pressed;
		}else{
			//Input is off.  Is this the first time through the loop that it's been off, or has it been off for a while?
			if(curInVal != dev->_lastInVal){
				//First time through.
				dev->_eventTime = HAL_GetTick();
			}else if(HAL_GetTick() - dev->_eventTime > dev->_debounceOffTime){
				//Button is now officially "released".
				dev->_curState = pbState_idle;
				dev->_eventTime = HAL_GetTick();
//Todo:  	  Possibly fire off a callback function here?  Event: Short Press Released
				retVal = pbEvent_shortpress_Released;
			}
		}
		break;
	case pbState_longpress_Pressed:
		if(curInVal == 0){
			//Input is currently inactive.  Start debouncing for turn-off
			dev->_curState = pbState_longpress_debounce_off;
			dev->_eventTime = HAL_GetTick();
		}else{
			//Input is still on.  Has it been held long enough to count as a long-press repeat?
			if( (HAL_GetTick() - dev->_eventTime > dev->longPressRepeatInterval) && (dev->longPressRepeatInterval > 0) ){
				//A LongPressRepeat event has just occurred.  Restart the event timer and fire off an event.
				dev->_eventTime = HAL_GetTick();
//Todo:  	  Possibly fire off a callback function here?  Event: LongPress Repeat
				retVal = pbEvent_longpress_Repeat;
			}
		}
		break;
	case pbState_longpress_debounce_off:
		if(curInVal == 1){
			//Input is back on.  Return to the pressed state.
			dev->_eventTime = HAL_GetTick();
			dev->_curState = pbState_longpress_Pressed;
		}else{
			//Input is off.  Is this the first time through the loop that it's been off, or has it been off for a while?
			if(curInVal != dev->_lastInVal){
				//First time through.
				dev->_eventTime = HAL_GetTick();
			}else if(HAL_GetTick() - dev->_eventTime > dev->_debounceOffTime){
				//Button is now officially "released".
				dev->_curState = pbState_idle;
				dev->_eventTime = HAL_GetTick();
//Todo:  	  Possibly fire off a callback function here?  Event: Long Press Released
				retVal = pbEvent_longpress_Released;
			}
		}
		break;
	default:
		dev->_curState = pbState_idle;
		dev->_eventTime = HAL_GetTick();
		break;
	}
	dev->_lastInVal = curInVal;
	return retVal;
}
*/
uint8_t PB_CheckForEvent(Pushbutton_HandleTypeDef* dev){
	//This reads the digital input and determines whether a pushbutton event has occurred.   The event (if any)
	//will be returned.
	uint8_t retVal = pbEvent_none;

	uint8_t curInVal = 0;
	if(dev->_useDirectInput == true){
		curInVal = (HAL_GPIO_ReadPin(dev->_port, dev->_pin) == dev->_activeState);
	}else if(dev->_useMatrix == true){
		uint8_t r = dev->_row;
		uint8_t c = dev->_col;
		uint8_t tmpCurVal = dev->_matrix->inData[r] & (1 << c);
		tmpCurVal = (tmpCurVal > 0 ? 1 : 0);
		curInVal = (tmpCurVal == dev->_activeState);
	}
	//curState now represents the logic state of the input; 0 = inactive, 1=active.

	switch(dev->_curState){
	case pbState_idle:
		if(curInVal == 1){
			  //Since the overall state is IDLE that means the input was not active last time through the loop.  Start the debounce.
			  dev->_eventTime = HAL_GetTick();
			  dev->_curState = pbState_debounce_On;
		}
		break;
	case pbState_debounce_On:
		if(curInVal == dev->_lastInVal){
			//No input state change since last loop.  Check to see if we've maintained this state long enough to move to a different state:
			if(HAL_GetTick() - dev->_eventTime > dev->_debounceOnTime){
				//Time delay has been satisfied.  Change to the destination state.
				if(curInVal == 1){
					dev->_curState = pbState_shortpress_Pressed;
					dev->_eventTime = HAL_GetTick();	//event time is reset so that we can later track how long the button is held for a long-press.
//Todo:  		  Possibly fire off a callback function here?  Event: Short Press
					if(dev->_pbMode == pbMode_Momentary){
						retVal = pbEvent_shortpress_Pressed;
					}else if(dev->_pbMode == pbMode_MomentaryWithLatch){
						//is the switch being turned on, or turned off?
						if(dev->_switchVal == false){
							//This is the initial press.  If it's held then later it will be latched on.
							retVal = pbEvent_shortpress_Pressed;
						}else{
							//Time to turn off the switch.
							retVal = pbEvent_Switch_Turned_Off;
							dev->_switchVal = false;
						}
					}else if(dev->_pbMode == pbMode_Switch){
						//toggle the switch value.
						if(dev->_switchVal == true){
							retVal = pbEvent_Switch_Turned_Off;
							dev->_switchVal = false;
						}else{
							retVal = pbEvent_Switch_Turned_On;
							dev->_switchVal = true;
						}
					}

				}else{
					dev->_curState = pbState_idle;
					//No need for further action here.  The button was on for less than the minimum debounce time, then went back off.
				}
			}
		}else{
			//Input has changed state since last time through the loop.
			if(curInVal == 1){
				//(re)Start the debounce timer.
				dev->_eventTime = HAL_GetTick();
			}
		}
		break;
	case pbState_shortpress_Pressed:
		if(curInVal == 0){
			//Input is currently inactive.  Start debouncing for turn-off
			dev->_curState = pbState_shortpress_Debounce_Off;
			dev->_eventTime = HAL_GetTick();
		}else{
			//Input is still on.  Has it been held long enough to count as a long-press?
			if( (HAL_GetTick() - dev->_eventTime > dev->longPressDelay) && (dev->longPressDelay > 0) ){
				dev->_curState = pbState_longpress_Pressed;
				dev->_eventTime = HAL_GetTick();
//Todo:  	  Possibly fire off a callback function here?  Event: LongPress
				if(dev->_pbMode == pbMode_Momentary){
					retVal = pbEvent_longpress_Pressed;
				}else if(dev->_pbMode == pbMode_MomentaryWithLatch){
					//Latch the switch on.
					dev->_switchVal = true;
					retVal = pbEvent_Switch_Turned_On;
				}//nothing to do here if we're in pure switch mode.
			}
		}
		break;
	case pbState_shortpress_Debounce_Off:
		if(curInVal == 1){
			//Input is back on.  Return to the pressed state.
			dev->_eventTime = HAL_GetTick();
			dev->_curState = pbState_shortpress_Pressed;
		}else{
			//Input is off.  Is this the first time through the loop that it's been off, or has it been off for a while?
			if(curInVal != dev->_lastInVal){
				//First time through.
				dev->_eventTime = HAL_GetTick();
			}else if(HAL_GetTick() - dev->_eventTime > dev->_debounceOffTime){
				//Button is now officially "released".
				dev->_curState = pbState_idle;
				dev->_eventTime = HAL_GetTick();
//				if(dev->_pbMode == pbMode_Momentary){
//Todo:  	  		Possibly fire off a callback function here?  Event: Short Press Released
					retVal = pbEvent_shortpress_Released;
//				}//nothing to do here for other pb modes.
			}
		}
		break;
	case pbState_longpress_Pressed:
		if(curInVal == 0){
			//Input is currently inactive.  Start debouncing for turn-off
			dev->_curState = pbState_longpress_debounce_off;
			dev->_eventTime = HAL_GetTick();
		}else{
			//Input is still on.  Has it been held long enough to count as a long-press repeat?
			if( (HAL_GetTick() - dev->_eventTime > dev->longPressRepeatInterval) && (dev->longPressRepeatInterval > 0) ){
				//A LongPressRepeat event has just occurred.  Restart the event timer and fire off an event.
				dev->_eventTime = HAL_GetTick();
//Todo:  	  Possibly fire off a callback function here?  Event: LongPress Repeat
				retVal = pbEvent_longpress_Repeat;
			}
		}
		break;
	case pbState_longpress_debounce_off:
		if(curInVal == 1){
			//Input is back on.  Return to the pressed state.
			dev->_eventTime = HAL_GetTick();
			dev->_curState = pbState_longpress_Pressed;
		}else{
			//Input is off.  Is this the first time through the loop that it's been off, or has it been off for a while?
			if(curInVal != dev->_lastInVal){
				//First time through.
				dev->_eventTime = HAL_GetTick();
			}else if(HAL_GetTick() - dev->_eventTime > dev->_debounceOffTime){
				//Button is now officially "released".
				dev->_curState = pbState_idle;
				dev->_eventTime = HAL_GetTick();
//Todo:  	  Possibly fire off a callback function here?  Event: Long Press Released
				retVal = pbEvent_longpress_Released;
			}
		}
		break;
	default:
		dev->_curState = pbState_idle;
		dev->_eventTime = HAL_GetTick();
		break;
	}
	dev->_lastInVal = curInVal;
	return retVal;
}

uint8_t PB_CurState(Pushbutton_HandleTypeDef* dev){
	//return dev->_curState;
//	if(dev->_pbMode == pbMode_Momentary){
		return dev->_curState;
//	}else{

//	}
}

/*
uint8_t PB_CurLogicVal(Pushbutton_HandleTypeDef* dev){
	//This returns whether the pushbutton is currently "pressed" or "not pressed" (momentary mode)
	// or "on" or "off" (switch mode).

	uint8_t retVal = 0;
	switch(dev->_curState){
	case pbState_idle:
		retVal = 0;
		break;
	case pbState_debounce_On:
		retVal = 0;
		break;
	case pbState_shortpress_Pressed:
	case pbState_shortpress_Debounce_Off:
		retVal = 1;
		break;
	case pbState_shortpress_Released:
		retVal = 0;
		break;
	case pbState_longpress_Pressed:
	case pbState_longpress_Repeat:
	case pbState_longpress_debounce_off:
		retVal = 1;
		break;
	case pbState_longpress_Released:
		retVal = 0;
		break;
	}
	return retVal;
}
*/
uint8_t PB_CurLogicVal(Pushbutton_HandleTypeDef* dev){
	//This returns whether the pushbutton is currently "pressed" or "not pressed" (momentary mode)
	// or "on" or "off" (switch mode).

	uint8_t retVal = 0;
	if(dev->_pbMode == pbMode_Momentary){
		switch(dev->_curState){
		case pbState_idle:
			retVal = 0;
			break;
		case pbState_debounce_On:
			retVal = 0;
			break;
		case pbState_shortpress_Pressed:
		case pbState_shortpress_Debounce_Off:
			retVal = 1;
			break;
		case pbState_shortpress_Released:
			retVal = 0;
			break;
		case pbState_longpress_Pressed:
		case pbState_longpress_Repeat:
		case pbState_longpress_debounce_off:
			retVal = 1;
			break;
		case pbState_longpress_Released:
			retVal = 0;
			break;
		}
	}else{
		retVal = (dev->_switchVal == true) ? 1 : 0;
	}
	return retVal;
}




//=========== Matrix functions
//Pushbutton_Matrix_Typedef* PB_CreateMatrixObject(uint8_t NumRows, uint8_t NumCols){
//	Pushbutton_Matrix_Typedef tmpMatrix;
//	tmpMatrix.numRows = NumRows;
//	tmpMatrix.numCols = NumCols;
//	return &tmpMatrix;
//}
void PB_ReadMatrix(Pushbutton_Matrix_Typedef* mRef){
	//Rows are outputs, Cols are inputs.
	for(uint8_t r=0; r< mRef->numRows; r++){
		HAL_GPIO_WritePin(mRef->_rowPort[r], mRef->_rowPin[r], 1);
	}

	for(uint8_t r=0; r< mRef->numRows; r++){
		mRef->inData[r] = 0;

		HAL_GPIO_WritePin(mRef->_rowPort[r], mRef->_rowPin[r], 0);
		for(uint8_t c=0; c< mRef->numCols; c++){
			uint8_t x = HAL_GPIO_ReadPin(mRef->_colPort[c], mRef->_colPin[c]);
			mRef->inData[r] |= (x << c);
		}
		HAL_GPIO_WritePin(mRef->_rowPort[r], mRef->_rowPin[r], 1);
	}
}

