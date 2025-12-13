

#ifndef _IMU_H_
#define _IMU_H_

#include "main.h"
//#include <stdio.h>
//#include <string.h>
#include <stdbool.h>



// LSM6DSOXTR Register Addresses (from datasheet)
#define WHO_AM_I          0x0F // Device ID (should return 0x6C)
#define CTRL1_XL          0x10 // Accelerometer control
#define CTRL2_G           0x11 // Gyroscope control
#define CTRL3_C           0x12 // General control (e.g., data-ready)
#define CTRL10_C          0x19 // Step counter enable
#define INT1_CTRL         0x0D // INT1 interrupt control
#define INT2_CTRL         0x0E // INT2 interrupt control
#define FUNC_CFG_ACCESS   0x01 // Access embedded functions for step counter
#define PEDO_THS_REG      0x0F // Step counter threshold (embedded function)
#define PEDO_DEB_REG      0x14 // Step counter debounce (embedded function)
#define PEDO_CMD_REG      0x11 // Embedded function register for pedometer commands

// Register read/write bit
#define IMU_READ_BIT          0x80 // Set MSB to 1 for read, 0 for write



typedef struct {
	int16_t 	x; // X-axis acceleration (mg)
	int16_t 	y; // Y-axis acceleration (mg)
	int16_t 	z; // Z-axis acceleration (mg)
} AccelerometerData_t;

typedef struct {
	int16_t 	x; // X-axis angular rate (mdps)
	int16_t 	y; // Y-axis angular rate (mdps)
	int16_t 	z; // Z-axis angular rate (mdps)
} GyroData_t;

typedef struct {
	int16_t 	roll; // X-axis angular rate (mdps)
	int16_t 	pitch; // Y-axis angular rate (mdps)
	int16_t 	yaw; // Z-axis angular rate (mdps)
} Orientation_t;

// Enum for the six discrete upward directions
typedef enum {
    UP_X_POS = 0,    // +X face up
    UP_X_NEG,    // -X face up
    UP_Y_POS,    // +Y face up
    UP_Y_NEG,    // -Y face up
    UP_Z_POS,    // +Z face up
    UP_Z_NEG     // -Z face up
} UpDirection_t;


typedef struct {
	SPI_HandleTypeDef*	imuSpi;
	GPIO_TypeDef*		CSPort;
	uint16_t			CSPin;
	uint16_t			int1Pin;	//Exti 12
	uint16_t			int2Pin;	//Exti 11

	uint16_t			stepIntPin;		//set this equal to either int1Pin or int2Pin if using this interrupt.
	uint16_t			gyroIntPin;		//set this equal to either int1Pin or int2Pin if using this interrupt.

	bool				gyroDataReady;
	bool				stepDataReady;

	bool				stepCountEnabled;
	int32_t				stepCount;
	int32_t				stepCountLastDisplayed;

	AccelerometerData_t accelData;
	GyroData_t			gyroData;

	Orientation_t		curOrientation;
	float				lastOrientTime;	//used during orientation calculation

	UpDirection_t		upDirection;
	UpDirection_t		upDirectionLastDisplayed;
	float				globalZRotation;	//used to track rotation about a world Z Axis
	float				globalZRotationLastDisplayed;
	float				globalZLastTime;

} IMU_HandleTypeDef;


HAL_StatusTypeDef ImuInit(IMU_HandleTypeDef* dev, SPI_HandleTypeDef* Spi, GPIO_TypeDef* CS_Port, uint16_t CS_Pin, uint16_t Int1Pin, uint16_t Int2Pin);
void ImuCsEnable(IMU_HandleTypeDef* dev);
void ImuCsDisable(IMU_HandleTypeDef* dev);
HAL_StatusTypeDef ImuWriteReg(IMU_HandleTypeDef* dev, uint8_t reg, uint8_t data);
HAL_StatusTypeDef ImuReadReg(IMU_HandleTypeDef* dev, uint8_t reg, uint8_t *data, uint16_t size);

HAL_StatusTypeDef ImuReadStepCounter(IMU_HandleTypeDef* dev);
HAL_StatusTypeDef ImuResetStepCounter(IMU_HandleTypeDef* dev);

HAL_StatusTypeDef ImuReadAccelGyro(IMU_HandleTypeDef* dev);

HAL_StatusTypeDef ImuClearInterrupts(IMU_HandleTypeDef* dev);

float ImuGetCurrentOrientTime(void);
HAL_StatusTypeDef ImuGet6DOrientation(IMU_HandleTypeDef* dev);
void ImuResetOrientation(IMU_HandleTypeDef* dev);

UpDirection_t ImuDetermineUpDirection(IMU_HandleTypeDef* dev);
float ImuGetGlobalZRotation(IMU_HandleTypeDef *dev);
void ImuResetGlobalZRotation(IMU_HandleTypeDef *dev);
void ImuSetGlobalZRotation(IMU_HandleTypeDef *dev, float newVal);

#endif
