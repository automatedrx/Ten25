#include "main.h"

#ifndef ENCODER_H
#define ENCODER_H

typedef enum {
	EncoderMode_minToMax	,
	EncoderMode_wrapAround	,
	EncoderMode_bounce
} enumEncoderMode;


typedef struct{
	TIM_HandleTypeDef* _htim;
	int16_t			   position;			//	This is the last position that was calculated by dividing TimerCounter val / _pulsesPerCount
	
	int16_t				minVal;
	int16_t				maxVal;
	int16_t				_lastVal;
	enumEncoderMode		countMode;
	uint8_t				_pulsesPerCount;	//	Used as a divider in case one "click" of a mechanical encoder might result in 4 timer increments.  Setting the
											//		PulsePerCount==4 would cause the counter to be divided by 4.
} Encoder_HandleTypeDef;


	void Encoder_Init(Encoder_HandleTypeDef* dev, TIM_HandleTypeDef* htim, int16_t StartVal, int16_t MinVal, int16_t MaxVal, enumEncoderMode CountMode, uint8_t PulsePerCount);
	void Encoder_ChangeDirection(Encoder_HandleTypeDef* dev, uint8_t newDirection);
	void Encoder_Update(Encoder_HandleTypeDef* dev);			//This checks the timer counter and adjusts it in case of wraparound or min/max.  Should be called often (main loop perhaps?)
	void Encoder_ChangePulsePerCount(Encoder_HandleTypeDef* dev, uint8_t newPulsePerCount);
	int16_t Encoder_GetCurVal(Encoder_HandleTypeDef* dev);
	void Encoder_SetCurVal(Encoder_HandleTypeDef* dev, int16_t NewVal);

#endif
