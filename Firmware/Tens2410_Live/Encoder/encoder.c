#include "encoder.h"



void Encoder_Init(Encoder_HandleTypeDef* dev, TIM_HandleTypeDef* htim, int16_t StartVal, int16_t MinVal, int16_t MaxVal, enumEncoderMode CountMode, uint8_t PulsePerCount){
	dev->_htim = htim;
	HAL_TIM_Encoder_Start(dev->_htim, TIM_CHANNEL_ALL);
	__HAL_TIM_SET_COUNTER(dev->_htim, (uint16_t)StartVal);
	dev->minVal = MinVal;
	dev->maxVal = MaxVal;
	dev->countMode = CountMode;
	dev->_pulsesPerCount = PulsePerCount;
}

void Encoder_ChangeDirection(Encoder_HandleTypeDef* dev, uint8_t newDirection){
	//newDirection:  0 = reverse, 1 = forward, 2 = toggle
	uint32_t tmpReg = dev->_htim->Instance->CCER;

	if(newDirection == 0){			//reverse direction
		tmpReg &= 0xFFFFFFFD;
	}else if(newDirection == 1){	//forward direction
		tmpReg |= 0x00000002;
	}if(newDirection == 2){			//toggle direction
		if(tmpReg & 0x00000002){
			tmpReg &= 0xFFFFFFFD;
		}else{
			tmpReg |= 0x00000002;
		}
	}
	dev->_htim->Instance->CCER = tmpReg;
}

void Encoder_Update(Encoder_HandleTypeDef* dev){
	int16_t tmpPos = (int16_t)(__HAL_TIM_GET_COUNTER(dev->_htim)) / dev->_pulsesPerCount;
	if(tmpPos > dev->maxVal){
		if(dev->countMode == EncoderMode_minToMax){
			tmpPos = dev->maxVal;
		}else if(dev->countMode == EncoderMode_wrapAround){
			tmpPos = dev->minVal;
		}else if(dev->countMode == EncoderMode_bounce){
			tmpPos = dev->maxVal - 1;
			//change direction
			Encoder_ChangeDirection(dev, 2);
		}
		__HAL_TIM_SET_COUNTER(dev->_htim, (uint16_t)(tmpPos * dev->_pulsesPerCount));
	}else if(tmpPos < dev->minVal){
		if(dev->countMode == EncoderMode_minToMax){
			tmpPos = dev->minVal;
		}else if(dev->countMode == EncoderMode_wrapAround){
			tmpPos = dev->maxVal;
		}else if(dev->countMode == EncoderMode_bounce){
			tmpPos = dev->minVal + 1;
			//change direction
			Encoder_ChangeDirection(dev, 2);
		}
		__HAL_TIM_SET_COUNTER(dev->_htim, (uint16_t)(tmpPos * dev->_pulsesPerCount));
	}
	dev->position = tmpPos;
}

void Encoder_ChangePulsePerCount(Encoder_HandleTypeDef* dev, uint8_t newPulsePerCount){
	//capture the current Position, change the pulsesPerCount, set timer value based on current position and new pulsesPerCount
	int16_t tmpPos = dev->position;
	dev->_pulsesPerCount = newPulsePerCount;
	Encoder_SetCurVal(dev, tmpPos);
}

int16_t Encoder_GetCurVal(Encoder_HandleTypeDef* dev){
	Encoder_Update(dev);
	return dev->position;
}

void Encoder_SetCurVal(Encoder_HandleTypeDef* dev, int16_t NewVal){
	__HAL_TIM_SET_COUNTER(dev->_htim, (uint16_t)(NewVal) * dev->_pulsesPerCount);
}

