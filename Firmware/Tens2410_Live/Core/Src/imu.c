
#include "imu.h"
#include "math.h"













HAL_StatusTypeDef ImuInit(IMU_HandleTypeDef* dev, SPI_HandleTypeDef* Spi, GPIO_TypeDef* CS_Port, uint16_t CS_Pin, uint16_t Int1Pin, uint16_t Int2Pin){
    HAL_StatusTypeDef status;
    uint8_t regValue;

    dev->imuSpi = Spi;
    dev->CSPort = CS_Port;
    dev->CSPin = CS_Pin;

    dev->int1Pin = Int1Pin;
    dev->int2Pin = Int2Pin;

    dev->stepIntPin = 0;	//This will need to be set after the init if using this function.
    dev->gyroIntPin = 0;	//This will need to be set after the init if using this function.

    dev->gyroDataReady = false;
    dev->stepDataReady = false;

    dev->stepCountEnabled = false;
    dev->stepCount = 0;
    dev->stepCountLastDisplayed = 0;

    dev->accelData.x = 0;
    dev->accelData.y = 0;
    dev->accelData.z = 0;
    dev->gyroData.x = 0;
    dev->gyroData.y = 0;
    dev->gyroData.z = 0;

    ImuResetOrientation(dev);
    //dev->curOrientation.roll = 0;
    //dev->curOrientation.pitch = 0;
    //dev->curOrientation.yaw = 0;

    dev->upDirection = UP_Z_POS;  //picked random direction for starting out.
    dev->upDirectionLastDisplayed = UP_Z_POS;

    ImuResetGlobalZRotation(dev);
    dev->globalZRotationLastDisplayed = 0;

    // Step 1: Verify device ID
    status = ImuReadReg(dev, WHO_AM_I, &regValue, 1);
    if (status != HAL_OK || regValue != 0x6C) {
        return HAL_ERROR; // Failed to communicate or wrong device ID
    }

    // Step 2: Configure Accelerometer (ODR 104 Hz, ±2g full scale)
    status = ImuWriteReg(dev, CTRL1_XL, 0x48); // 104 Hz (0b0100), ±2g (0b00), enable				//orig 0x40
    if (status != HAL_OK) return HAL_ERROR;

    // Step 3: Configure Gyroscope (ODR 104 Hz, ±2000 dps full scale)
    status = ImuWriteReg(dev, CTRL2_G, 0x44); // 104 Hz (0b0100), ±2000 dps (0b11), enable			//orig 0x4C
    //status = ImuWriteReg(dev, CTRL2_G, 0x40); // 104 Hz (0b0100), ±2000 dps (0b11), enable
    if (status != HAL_OK) return HAL_ERROR;

    // Step 4: Enable block data update and data-ready interrupt
    status = ImuWriteReg(dev, CTRL3_C, 0x44); // BDU (block data update) + IF_INC (auto increment)
    if (status != HAL_OK) return HAL_ERROR;

    // Step 5: Configure Step Counter
    // Enable access to embedded functions
    status = ImuWriteReg(dev, FUNC_CFG_ACCESS, 0x80); // Enable embedded functions access
    if (status != HAL_OK) return HAL_ERROR;

    // Set step counter threshold and debounce (example values)
    status = ImuWriteReg(dev, PEDO_THS_REG, 0x10); // Threshold for step detection //original val: 0x20
    if (status != HAL_OK) return HAL_ERROR;
    status = ImuWriteReg(dev, PEDO_DEB_REG, 0x01); // Debounce time  //original val: 0x03
    if (status != HAL_OK) return HAL_ERROR;

    // Disable access to embedded functions
    status = ImuWriteReg(dev, FUNC_CFG_ACCESS, 0x00);
    if (status != HAL_OK) return HAL_ERROR;

    // Enable step counter in CTRL10_C
    status = ImuWriteReg(dev, CTRL10_C, 0x00); // Enable step counter (bit 2)
    if (status != HAL_OK) return HAL_ERROR;

    // Step 6: Configure Interrupts
    // INT1 for data-ready (accelerometer and gyroscope)
    status = ImuWriteReg(dev, INT1_CTRL, 0x03); // Enable INT1 for accelerometer (bit 1) and gyroscope (bit 0)
    if (status != HAL_OK) return HAL_ERROR;

    // INT2 for step detection
    status = ImuWriteReg(dev, INT2_CTRL, 0x40); // Enable INT2 for step detection (bit 6)
    if (status != HAL_OK) return HAL_ERROR;

    return HAL_OK; // Initialization successful
}




void ImuCsEnable(IMU_HandleTypeDef* dev){
	HAL_GPIO_WritePin(dev->CSPort, dev->CSPin, 0);
}

void ImuCsDisable(IMU_HandleTypeDef* dev){
	HAL_GPIO_WritePin(dev->CSPort, dev->CSPin, 1);
}

HAL_StatusTypeDef ImuWriteReg(IMU_HandleTypeDef* dev, uint8_t reg, uint8_t data) {
    uint8_t txData[2] = {reg & ~IMU_READ_BIT, data}; // Clear read bit for write
    ImuCsEnable(dev);
    HAL_StatusTypeDef status = HAL_SPI_Transmit(dev->imuSpi, txData, 2, HAL_MAX_DELAY);
    ImuCsDisable(dev);
    return status;
}

// Low-level SPI read function
HAL_StatusTypeDef ImuReadReg(IMU_HandleTypeDef* dev, uint8_t reg, uint8_t *data, uint16_t size) {
    uint8_t txData = reg | IMU_READ_BIT; // Set read bit
    ImuCsEnable(dev);
    HAL_StatusTypeDef status = HAL_SPI_Transmit(dev->imuSpi, &txData, 1, HAL_MAX_DELAY);
    if (status == HAL_OK) {
        status = HAL_SPI_Receive(dev->imuSpi, data, size, HAL_MAX_DELAY);

    }
    ImuCsDisable(dev);
    return status;
}


HAL_StatusTypeDef ImuReadStepCounter(IMU_HandleTypeDef* dev){
	uint8_t rxData[2];
	HAL_StatusTypeDef status;

	// Read low byte
	status = ImuReadReg(dev, 0x42, &rxData[0], 1); // STEP_COUNTER_L
	if (status != HAL_OK) return status;

	// Read high byte
	status = ImuReadReg(dev, 0x43, &rxData[1], 1); // STEP_COUNTER_H
	if (status != HAL_OK) return status;

	// Combine low and high bytes into 16-bit step count
	dev->stepCount += (uint16_t)(rxData[1] << 8) | rxData[0];

	// Clear the local interrupt flag indicating new data is ready to be read
	dev->stepDataReady = false;

	return HAL_OK;
}

// Function to reset the step counter
HAL_StatusTypeDef ImuResetStepCounter(IMU_HandleTypeDef* dev){
    HAL_StatusTypeDef status;

    // Step 1: Enable access to embedded functions
    status = ImuWriteReg(dev, FUNC_CFG_ACCESS, 0x80); // Enable embedded function access
    if (status != HAL_OK) return status;

    // Step 2: Write reset command to PEDO_CMD_REG
    status = ImuWriteReg(dev, 0x0B, 0x01); // 0x0B is the offset for PEDO_CMD_REG in embedded space
    if (status != HAL_OK) return status;

    // Step 3: Disable access to embedded functions
    status = ImuWriteReg(dev, FUNC_CFG_ACCESS, 0x00);
    if (status != HAL_OK) return status;

    return HAL_OK;
}


HAL_StatusTypeDef ImuReadAccelGyro(IMU_HandleTypeDef* dev){
    uint8_t rxData[12]; // 6 bytes for accel (3 axes) + 6 bytes for gyro (3 axes)
    HAL_StatusTypeDef status;

    // Read accelerometer data (auto-increment enabled in CTRL3_C during init)
    status = ImuReadReg(dev, 0x28 | 0x80, rxData, 6); // Start at OUTX_L_XL, read 6 bytes
    if (status != HAL_OK) return status;

    // Combine bytes into 16-bit values (little-endian)
    dev->accelData.x = (int16_t)((rxData[1] << 8) | rxData[0]); // OUTX_H_XL | OUTX_L_XL
    dev->accelData.y = (int16_t)((rxData[3] << 8) | rxData[2]); // OUTY_H_XL | OUTY_L_XL
    dev->accelData.z = (int16_t)((rxData[5] << 8) | rxData[4]); // OUTZ_H_XL | OUTZ_L_XL

    // Read gyroscope data (auto-increment enabled)
    status = ImuReadReg(dev, 0x22 | 0x80, &rxData[6], 6); // Start at OUTX_L_G, read 6 bytes
    if (status != HAL_OK) return status;

    // Combine bytes into 16-bit values
    dev->gyroData.x = (int16_t)((rxData[7] << 8) | rxData[6]);  // OUTX_H_G | OUTX_L_G
    dev->gyroData.y = (int16_t)((rxData[9] << 8) | rxData[8]);  // OUTY_H_G | OUTY_L_G
    dev->gyroData.z = (int16_t)((rxData[11] << 8) | rxData[10]); // OUTZ_H_G | OUTZ_L_G

    // Clear the local interrupt flag indicating new data is ready to be read
	dev->gyroDataReady = false;

    return HAL_OK;
}


HAL_StatusTypeDef ImuClearInterrupts(IMU_HandleTypeDef* dev){
	HAL_StatusTypeDef status;
	uint8_t regValue = 0;

	// Clear Gyroscope Data-Ready Interrupt (INT1)
	// Read STATUS_REG to acknowledge data-ready
	status = ImuReadReg(dev, 0x1E, &regValue, 1); // STATUS_REG
	if (status != HAL_OK) return status;
	// Optionally, read gyroscope data to ensure the condition is resolved
	// (This is done automatically in data read functions, so optional here)

	// Clear Step Counter Interrupt (INT2)
	// Read FUNC_SRC to acknowledge step detection
	status = ImuReadReg(dev, 0x53, &regValue, 1); // FUNC_SRC
	if (status != HAL_OK) return status;
	// Optionally, read step counter data to reset the condition
	// (This can be done in the step counter read function)

	return HAL_OK;
}

float ImuGetCurrentOrientTime(void){
    return (float)HAL_GetTick() / 1000.0f; // Convert ms to seconds
}

HAL_StatusTypeDef ImuGet6DOrientation(IMU_HandleTypeDef* dev){//, Orientation6D_t *orientation) {

    //AccelerometerData_t accelData;
    //GyroscopeData_t gyroData;
    HAL_StatusTypeDef status;

    // Step 1: Read accelerometer and gyroscope data
    status = ImuReadAccelGyro(dev);// &accelData, &gyroData);
    if (status != HAL_OK) return status;

    // Step 2: Convert raw data to physical units
    // Accelerometer: ±2g range, sensitivity 0.061 mg/LSB = 0.000061 g/LSB
    float ax = dev->accelData.x * 0.000061f; // g
    float ay = dev->accelData.y * 0.000061f; // g
    float az = dev->accelData.z * 0.000061f; // g

    // Gyroscope: ±2000 dps range, sensitivity 70 mdps/LSB = 0.070 dps/LSB
    float gx = dev->gyroData.x * 0.070f; // degrees per second
    float gy = dev->gyroData.y * 0.070f; // degrees per second
    float gz = dev->gyroData.z * 0.070f; // degrees per second

    // Step 3: Calculate time delta
    float currentTime = ImuGetCurrentOrientTime();
    float dt = (dev->lastOrientTime > 0.0f) ? (currentTime - dev->lastOrientTime) : 0.01f; // Default dt = 10ms if first run
    dev->lastOrientTime = currentTime;

    // Step 4: Calculate orientation from accelerometer (static)
    float accelRoll = atan2f(ay, az) * 180.0f / 3.14159265359f;  // Roll from Y/Z
    float accelPitch = -atan2f(ax, sqrtf(ay * ay + az * az)) * 180.0f / 3.14159265359f; // Pitch from X

    // Step 5: Integrate gyroscope data (dynamic)
    float gyroRoll = dev->curOrientation.roll + gx * dt;
    float gyroPitch = dev->curOrientation.pitch + gy * dt;
    float gyroYaw = dev->curOrientation.yaw + gz * dt;

    // Step 6: Apply complementary filter (α = 0.98 for gyroscope dominance)
    float alpha = 0.98f;
    dev->curOrientation.roll = alpha * gyroRoll + (1.0f - alpha) * accelRoll;
    dev->curOrientation.pitch = alpha * gyroPitch + (1.0f - alpha) * accelPitch;
    dev->curOrientation.yaw = gyroYaw; // Yaw relies on gyro integration (drifts without magnetometer)

    // Step 7: Copy to output
    //orientation->roll = currentOrientation.roll;
    //orientation->pitch = currentOrientation.pitch;
    //orientation->yaw = currentOrientation.yaw;

    return HAL_OK;
}

// Reset orientation (optional, call if needed)
void ImuResetOrientation(IMU_HandleTypeDef* dev){
	dev->curOrientation.roll = 0.0f;
	dev->curOrientation.pitch = 0.0f;
	dev->curOrientation.yaw = 0.0f;
    dev->lastOrientTime = 0.0f;
}


// Function to determine which direction is facing upward
UpDirection_t ImuDetermineUpDirection(IMU_HandleTypeDef* dev){
    //Call ImuGetCurrentOrientTime prior to get current data from the IMU soon before calling this function.

    // Convert raw accelerometer data to g-forces (sensitivity 0.122 mg/LSB for ±4g)
    float ax = dev->accelData.x * 0.122f / 1000.0f; // Convert mg to g
    float ay = dev->accelData.y * 0.122f / 1000.0f; // Convert mg to g
    float az = dev->accelData.z * 0.122f / 1000.0f; // Convert mg to g

    // Find the axis with the largest negative acceleration (closest to -1g)
    float absX = fabsf(ax);
    float absY = fabsf(ay);
    float absZ = fabsf(az);

    if (absX >= absY && absX >= absZ) {
        // X-axis has the largest magnitude
    	dev->upDirection = (ax < 0) ? UP_X_NEG : UP_X_POS;
    } else if (absY >= absX && absY >= absZ) {
        // Y-axis has the largest magnitude
    	dev->upDirection = (ay < 0) ? UP_Y_NEG : UP_Y_POS;
    } else {
        // Z-axis has the largest magnitude
    	dev->upDirection = (az < 0) ? UP_Z_NEG : UP_Z_POS;
    }

    return dev->upDirection;
}



float ImuGetGlobalZRotation(IMU_HandleTypeDef *dev){
	//Call ImuGetCurrentOrientTime prior to get current data from the IMU soon before calling this function.
	//Call ImuDetermineUpDirection prior to get current data from the IMU soon before calling this function.

	// Convert raw gyroscope data to degrees per second (sensitivity 17.50 mdps/LSB for ±500 dps)
	float gx = dev->gyroData.x * 0.0175f; // degrees per second
	float gy = dev->gyroData.y * 0.0175f; // degrees per second
	float gz = dev->gyroData.z * 0.0175f; // degrees per second

	//// Determine the current upward direction
	//UpDirection_t upDirection = LSM6DSOXTR_DetermineUpDirection();

	// Calculate time delta
	float currentTime = ImuGetCurrentOrientTime();
	float dt = (dev->globalZLastTime > 0.0f) ? (currentTime - dev->globalZLastTime) : 0.01f; // Default 10ms if first run
	dev->globalZLastTime = currentTime;

	// Select the appropriate gyroscope axis based on the upward direction
	float angularRate = 0.0f;
	switch (dev->upDirection) {
		case UP_X_POS: // +X up, rotate about x-axis (positive direction)
		case UP_X_NEG: // -X up, rotate about x-axis (negative direction inverted)
			angularRate = gx;
			if (dev->upDirection == UP_X_NEG) angularRate = -angularRate; // Invert for -X up
			break;
		case UP_Y_POS: // +Y up, rotate about y-axis (positive direction)
		case UP_Y_NEG: // -Y up, rotate about y-axis (negative direction inverted)
			angularRate = gy;
			if (dev->upDirection == UP_Y_NEG) angularRate = -angularRate; // Invert for -Y up
			break;
		case UP_Z_POS: // +Z up, rotate about z-axis (positive direction)
		case UP_Z_NEG: // -Z up, rotate about z-axis (negative direction inverted)
			angularRate = gz;
			if (dev->upDirection == UP_Z_NEG) angularRate = -angularRate; // Invert for -Z up
			break;
	}

	// Integrate rotation (angularRate * dt)
	dev->globalZRotation += angularRate * dt;
	while(dev->globalZRotation > 360.0){
		dev->globalZRotation -= 360;
	}
	while(dev->globalZRotation < 0.0){
		dev->globalZRotation += 360.0;
	}

	return dev->globalZRotation;
}


void ImuResetGlobalZRotation(IMU_HandleTypeDef *dev){
	dev->globalZRotation = 0.0;
	dev->globalZLastTime = 0.0;
}

void ImuSetGlobalZRotation(IMU_HandleTypeDef *dev, float newVal){
	dev->globalZRotation = newVal;
	dev->globalZLastTime = 0.0;
}










