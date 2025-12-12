/* USER CODE BEGIN Header */
/**
  ******************************************************************************
  * @file           : main.h
  * @brief          : Header for main.c file.
  *                   This file contains the common defines of the application.
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

/* Define to prevent recursive inclusion -------------------------------------*/
#ifndef __MAIN_H
#define __MAIN_H

#ifdef __cplusplus
extern "C" {
#endif

/* Includes ------------------------------------------------------------------*/
#include "stm32l4xx_hal.h"

/* Private includes ----------------------------------------------------------*/
/* USER CODE BEGIN Includes */

/* USER CODE END Includes */

/* Exported types ------------------------------------------------------------*/
/* USER CODE BEGIN ET */

/* USER CODE END ET */

/* Exported constants --------------------------------------------------------*/
/* USER CODE BEGIN EC */

/* USER CODE END EC */

/* Exported macro ------------------------------------------------------------*/
/* USER CODE BEGIN EM */

/* USER CODE END EM */

void HAL_TIM_MspPostInit(TIM_HandleTypeDef *htim);

/* Exported functions prototypes ---------------------------------------------*/
void Error_Handler(void);

/* USER CODE BEGIN EFP */
uint32_t getFlashPage(uint32_t Address);
/* USER CODE END EFP */

/* Private defines -----------------------------------------------------------*/
#define TENS_V_EN_Pin GPIO_PIN_2
#define TENS_V_EN_GPIO_Port GPIOE
#define TENS_POL_CH2_Pin GPIO_PIN_3
#define TENS_POL_CH2_GPIO_Port GPIOE
#define TENS_POL_CH3_Pin GPIO_PIN_4
#define TENS_POL_CH3_GPIO_Port GPIOE
#define LCD2_AO_Pin GPIO_PIN_5
#define LCD2_AO_GPIO_Port GPIOE
#define VBOOST_EN_Pin GPIO_PIN_6
#define VBOOST_EN_GPIO_Port GPIOE
#define TENS_POL_CH1_Pin GPIO_PIN_13
#define TENS_POL_CH1_GPIO_Port GPIOC
#define nPOWERON_ESP32_Pin GPIO_PIN_3
#define nPOWERON_ESP32_GPIO_Port GPIOC
#define LED_R_Pin GPIO_PIN_0
#define LED_R_GPIO_Port GPIOA
#define LED_G_Pin GPIO_PIN_1
#define LED_G_GPIO_Port GPIOA
#define LED_B_Pin GPIO_PIN_3
#define LED_B_GPIO_Port GPIOA
#define PB_COL1_Pin GPIO_PIN_6
#define PB_COL1_GPIO_Port GPIOA
#define PB_ROW1_Pin GPIO_PIN_7
#define PB_ROW1_GPIO_Port GPIOA
#define TENV_OUT_SENSE_Pin GPIO_PIN_5
#define TENV_OUT_SENSE_GPIO_Port GPIOC
#define BAT_V_SENSE_Pin GPIO_PIN_0
#define BAT_V_SENSE_GPIO_Port GPIOB
#define TENV_IN_SENSE_Pin GPIO_PIN_1
#define TENV_IN_SENSE_GPIO_Port GPIOB
#define SD_SW_Pin GPIO_PIN_2
#define SD_SW_GPIO_Port GPIOB
#define MOTOR1_DIR_Pin GPIO_PIN_8
#define MOTOR1_DIR_GPIO_Port GPIOE
#define IMU_CS_Pin GPIO_PIN_10
#define IMU_CS_GPIO_Port GPIOE
#define IMU_INT1_Pin GPIO_PIN_12
#define IMU_INT1_GPIO_Port GPIOE
#define IMU_INT1_EXTI_IRQn EXTI15_10_IRQn
#define IMU_CLK_Pin GPIO_PIN_13
#define IMU_CLK_GPIO_Port GPIOE
#define IMU_MISO_Pin GPIO_PIN_14
#define IMU_MISO_GPIO_Port GPIOE
#define IMU_MOSI_Pin GPIO_PIN_15
#define IMU_MOSI_GPIO_Port GPIOE
#define LCD_BL_Pin GPIO_PIN_10
#define LCD_BL_GPIO_Port GPIOB
#define IMU_INT2_Pin GPIO_PIN_11
#define IMU_INT2_GPIO_Port GPIOB
#define IMU_INT2_EXTI_IRQn EXTI15_10_IRQn
#define PB_POWER_Pin GPIO_PIN_12
#define PB_POWER_GPIO_Port GPIOB
#define RFM_SCK_Pin GPIO_PIN_13
#define RFM_SCK_GPIO_Port GPIOB
#define RFM_MISO_Pin GPIO_PIN_14
#define RFM_MISO_GPIO_Port GPIOB
#define RFM_MOSI_Pin GPIO_PIN_15
#define RFM_MOSI_GPIO_Port GPIOB
#define RFM_CS_Pin GPIO_PIN_8
#define RFM_CS_GPIO_Port GPIOD
#define GPS_PPS_Pin GPIO_PIN_9
#define GPS_PPS_GPIO_Port GPIOD
#define MOTOR1_MODE_Pin GPIO_PIN_10
#define MOTOR1_MODE_GPIO_Port GPIOD
#define POWER_ON_MAIN_Pin GPIO_PIN_11
#define POWER_ON_MAIN_GPIO_Port GPIOD
#define PB_ROW2_Pin GPIO_PIN_12
#define PB_ROW2_GPIO_Port GPIOD
#define PULSE_CH2_Pin GPIO_PIN_13
#define PULSE_CH2_GPIO_Port GPIOD
#define PB_COL2_Pin GPIO_PIN_14
#define PB_COL2_GPIO_Port GPIOD
#define PULSE_CH3_Pin GPIO_PIN_15
#define PULSE_CH3_GPIO_Port GPIOD
#define MOTOR1_PWM_Pin GPIO_PIN_6
#define MOTOR1_PWM_GPIO_Port GPIOC
#define MOTOR2_PWM_Pin GPIO_PIN_7
#define MOTOR2_PWM_GPIO_Port GPIOC
#define POWER_ON_IMU_GPS_Pin GPIO_PIN_8
#define POWER_ON_IMU_GPS_GPIO_Port GPIOA
#define GPS_TX_Pin GPIO_PIN_9
#define GPS_TX_GPIO_Port GPIOA
#define GPS_RX_Pin GPIO_PIN_10
#define GPS_RX_GPIO_Port GPIOA
#define RFM_DIO0_Pin GPIO_PIN_15
#define RFM_DIO0_GPIO_Port GPIOA
#define RFM_DIO0_EXTI_IRQn EXTI15_10_IRQn
#define LCD2_CS_Pin GPIO_PIN_1
#define LCD2_CS_GPIO_Port GPIOD
#define LCD_AO_Pin GPIO_PIN_3
#define LCD_AO_GPIO_Port GPIOD
#define LCD_CS_Pin GPIO_PIN_4
#define LCD_CS_GPIO_Port GPIOD
#define PB3_Pin GPIO_PIN_5
#define PB3_GPIO_Port GPIOD
#define LCD_RST_Pin GPIO_PIN_7
#define LCD_RST_GPIO_Port GPIOD
#define MOTOR2_DIR_Pin GPIO_PIN_4
#define MOTOR2_DIR_GPIO_Port GPIOB
#define PB_POWER2_Pin GPIO_PIN_5
#define PB_POWER2_GPIO_Port GPIOB
#define PB_COL3_Pin GPIO_PIN_6
#define PB_COL3_GPIO_Port GPIOB
#define PB_ROW3_Pin GPIO_PIN_7
#define PB_ROW3_GPIO_Port GPIOB
#define PB2_Pin GPIO_PIN_8
#define PB2_GPIO_Port GPIOB
#define MOTOR2_MODE_Pin GPIO_PIN_9
#define MOTOR2_MODE_GPIO_Port GPIOB
#define LCD2_BL_Pin GPIO_PIN_0
#define LCD2_BL_GPIO_Port GPIOE
#define PULSE_CH1_Pin GPIO_PIN_1
#define PULSE_CH1_GPIO_Port GPIOE

/* USER CODE BEGIN Private defines */
#define DEVICE_BOARD_E
/* USER CODE END Private defines */

#ifdef __cplusplus
}
#endif

#endif /* __MAIN_H */
