#include "main.h"

#ifndef PUSHBUTTON_H
#define PUSHBUTTON_H

#include <stdbool.h>

/*
//States for _curState								//Return value for curState():
#define PBSTATE_IDLE 						0		// 0
#define PBSTATE_DEBOUNCE_ON					1		// 0
#define PBSTATE_SHORTPRESS					2		// 1
#define PBSTATE_SHORTPRESS_DEBOUNCE_OFF		3		// 1
#define PBSTATE_SHORTPRESS_RELEASED			4		// 0
#define PBSTATE_LONGPRESS					5		// 1
#define PBSTATE_LONGPRESS_REPEAT			6		// 1
#define PBSTATE_LONGPRESS_DEBOUNCE_OFF		7		// 1
#define PBSTATE_LONGPRESS_RELEASED			8		// 0

#define PBEVENT_NONE				0
#define PBEVENT_SHORTPRESS			1
#define PBEVENT_SHORTPRESS_RELEASED	2
#define PBEVENT_LONGPRESS			3
#define PBEVENT_LONGPRESS_REPEAT	4
#define PBEVENT_LONGPRESS_RELEASED	5
*/


typedef enum {
	pbState_idle					,
	pbState_debounce_On				,
	pbState_shortpress_Pressed		,
	pbState_shortpress_Debounce_Off	,
	pbState_shortpress_Released		,
	pbState_longpress_Pressed		,
	pbState_longpress_Repeat		,
	pbState_longpress_debounce_off	,
	pbState_longpress_Released
} enumPBState;

typedef enum {
	pbEvent_none					,
	pbEvent_shortpress_Pressed		,
	pbEvent_shortpress_Released		,
	pbEvent_longpress_Pressed		,
	pbEvent_longpress_Repeat		,
	pbEvent_longpress_Released		,
	pbEvent_Switch_Turned_On		,
	pbEvent_Switch_Turned_Off
} enumPBEvent;

typedef enum {
	pbMode_Momentary,						//Acts as a momentary pushbutton
	pbMode_MomentaryWithLatch,				//Acts as a momentary pushbutton, but if held for longPress time it will latch On.
	pbMode_Switch							//Acts as a toggle switch
} enumPBMode;

typedef struct{
	uint8_t			numRows;
	uint8_t			numCols;

	GPIO_TypeDef*	_rowPort[8];
	GPIO_TypeDef*	_rowPort2;
	uint16_t		_rowPin[8];
	GPIO_TypeDef*	_colPort[8];
	uint16_t		_colPin[8];

	uint8_t			inData[8];
}Pushbutton_Matrix_Typedef;

typedef struct{
	bool			_useDirectInput;	//set to true if this is directly connected to a gpio
	bool			_useMatrix;			//set to true if this is part of a keypad matrix

	GPIO_TypeDef*	_port;				//only for DirectInput
	uint16_t		_pin;				//only for DirectInput

	uint8_t			_row;				//only for Matrix.  This is the output
	uint8_t			_col;				//only for Matrix.  This is the input

	uint8_t			_activeState;
	
	//uint8_t 		_curState;
	enumPBState 	_curState;
	uint8_t 		_lastInVal;
	uint32_t		_eventTime;
	uint32_t 		_debounceOnTime;
	uint32_t 		_debounceOffTime;
	
	uint32_t 		longPressRepeatInterval;
	uint32_t 		longPressDelay;

	enumPBMode		_pbMode;
	bool			_switchVal;					//tracks the current state if in latch or switch mode.

	//The following is for keypad matrixes
	Pushbutton_Matrix_Typedef* _matrix;


} Pushbutton_HandleTypeDef;


void PB_InitShort(Pushbutton_HandleTypeDef* dev, GPIO_TypeDef* Port, uint16_t Pin, uint8_t ActiveState);
void PB_Init(Pushbutton_HandleTypeDef* dev, GPIO_TypeDef* Port, uint16_t Pin, uint8_t ActiveState, uint32_t DebounceOnTime,
		uint32_t DebounceOffTime, uint32_t LongPressDelay, uint32_t LongPressRepeatInterval, enumPBMode PBMode);
void PB_Init_Matrix(Pushbutton_HandleTypeDef* dev, Pushbutton_Matrix_Typedef* MatrixRef, uint8_t Row, uint8_t Col, uint8_t ActiveState, uint32_t DebounceOnTime,
		uint32_t DebounceOffTime, uint32_t LongPressDelay, uint32_t LongPressRepeatInterval, enumPBMode PBMode);

uint8_t PB_CheckForEvent(Pushbutton_HandleTypeDef* dev);//void update();			//This checks the current state of the io input.  Should be called often (main loop perhaps?)

uint8_t PB_CurState(Pushbutton_HandleTypeDef* dev);
uint8_t PB_CurLogicVal(Pushbutton_HandleTypeDef* dev);
uint32_t PB_HeldDuration(Pushbutton_HandleTypeDef* dev);

//Matrix related:
//Pushbutton_Matrix_Typedef* PB_CreateMatrixObject(uint8_t NumRows, uint8_t NumCols);
void PB_ReadMatrix(Pushbutton_Matrix_Typedef* mRef);
#endif
