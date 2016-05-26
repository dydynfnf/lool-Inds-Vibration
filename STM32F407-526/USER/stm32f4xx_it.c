/**
  ******************************************************************************
  * @file    Project/STM32F4xx_StdPeriph_Templates/stm32f4xx_it.c 
  * @author  MCD Application Team
  * @version V1.4.0
  * @date    04-August-2014
  * @brief   Main Interrupt Service Routines.
  *          This file provides template for all exceptions handler and 
  *          peripherals interrupt service routine.
  ******************************************************************************
  * @attention
  *
  * <h2><center>&copy; COPYRIGHT 2014 STMicroelectronics</center></h2>
  *
  * Licensed under MCD-ST Liberty SW License Agreement V2, (the "License");
  * You may not use this file except in compliance with the License.
  * You may obtain a copy of the License at:
  *
  *        http://www.st.com/software_license_agreement_liberty_v2
  *
  * Unless required by applicable law or agreed to in writing, software 
  * distributed under the License is distributed on an "AS IS" BASIS, 
  * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  * See the License for the specific language governing permissions and
  * limitations under the License.
  *
  ******************************************************************************
  */

/* Includes ------------------------------------------------------------------*/
#include "stm32f4xx_it.h"
#include "ads1271.h"
#include "spi.h"

/** @addtogroup Template_Project
  * @{
  */

/* Private typedef -----------------------------------------------------------*/
/* Private define ------------------------------------------------------------*/
/* Private macro -------------------------------------------------------------*/
/* Private variables ---------------------------------------------------------*/
/* Private function prototypes -----------------------------------------------*/
/* Private functions ---------------------------------------------------------*/

/******************************************************************************/
/*            Cortex-M4 Processor Exceptions Handlers                         */
/******************************************************************************/

/**
  * @brief  This function handles NMI exception.
  * @param  None
  * @retval None
  */
void NMI_Handler(void)
{
}

/**
  * @brief  This function handles Hard Fault exception.
  * @param  None
  * @retval None
  */
void HardFault_Handler(void)
{
  /* Go to infinite loop when Hard Fault exception occurs */
  while (1)
  {
  }
}

/**
  * @brief  This function handles Memory Manage exception.
  * @param  None
  * @retval None
  */
void MemManage_Handler(void)
{
  /* Go to infinite loop when Memory Manage exception occurs */
  while (1)
  {
  }
}

/**
  * @brief  This function handles Bus Fault exception.
  * @param  None
  * @retval None
  */
void BusFault_Handler(void)
{
  /* Go to infinite loop when Bus Fault exception occurs */
  while (1)
  {
  }
}

/**
  * @brief  This function handles Usage Fault exception.
  * @param  None
  * @retval None
  */
void UsageFault_Handler(void)
{
  /* Go to infinite loop when Usage Fault exception occurs */
  while (1)
  {
  }
}

/**
  * @brief  This function handles SVCall exception.
  * @param  None
  * @retval None
  */
void SVC_Handler(void)
{
}

/**
  * @brief  This function handles Debug Monitor exception.
  * @param  None
  * @retval None
  */
void DebugMon_Handler(void)
{
}

/**
  * @brief  This function handles PendSVC exception.
  * @param  None
  * @retval None
  */
void PendSV_Handler(void)
{
}

/**
  * @brief  This function handles SysTick Handler.
  * @param  None
  * @retval None
  */
void SysTick_Handler(void)
{
 
}

/******************************************************************************/
/*                 STM32F4xx Peripherals Interrupt Handlers                   */
/*  Add here the Interrupt Handler for the used peripheral(s) (PPP), for the  */
/*  available peripheral interrupt handler's name please refer to the startup */
/*  file (startup_stm32f4xx.s).                                               */
/******************************************************************************/

/**
  * @brief  This function handles PPP interrupt request.
  * @param  None
  * @retval None
  */
/*void PPP_IRQHandler(void)
{
}*/

/**
  * @}
  */ 

/******************************************************************************/
/*                             外部中断4                                      */
/*                             ADC1中断                                       */
/******************************************************************************/
extern unsigned char spi1_rx[6];
extern short ad1,ad2;
int exti4_count=0;
void EXTI4_IRQHandler(void)
{
//	spi1_rx[0] = SPI1_ReadWriteByte(0xff);
//	spi1_rx[1] = SPI1_ReadWriteByte(0xff);
//	spi1_rx[2] = SPI1_ReadWriteByte(0xff);
//	spi1_rx[3] = SPI1_ReadWriteByte(0xff);
//	spi1_rx[4] = SPI1_ReadWriteByte(0xff);
//	spi1_rx[5] = SPI1_ReadWriteByte(0xff);
	
//	spi1_rx_dma_transfer_once();
//	spi1_tx_dma_transfer_once();
//	
//	ad1 = (spi1_rx[0]<<8) | (spi1_rx[1]);
//	ad2 = (spi1_rx[3]<<8) | (spi1_rx[4]);
//	
//	exti4_count++;
	
	EXTI_ClearITPendingBit(EXTI_Line4); //清除LINE4上的中断标志位 
}

/******************************************************************************/
/*                             外部中断12                                     */
/*                             ADC2中断                                       */
/******************************************************************************/
extern unsigned char spi2_rx[6];
extern short ad3,ad4;
void EXTI15_10_IRQHandler(void)
{
	spi2_rx[0] = SPI2_ReadWriteByte(0xff);
	spi2_rx[1] = SPI2_ReadWriteByte(0xff);
	spi2_rx[2] = SPI2_ReadWriteByte(0xff);
	spi2_rx[3] = SPI2_ReadWriteByte(0xff);
	spi2_rx[4] = SPI2_ReadWriteByte(0xff);
	spi2_rx[5] = SPI2_ReadWriteByte(0xff);
//	
	ad3 = (spi2_rx[0]<<8) | (spi2_rx[1]);
	ad4 = (spi2_rx[3]<<8) | (spi2_rx[4]);
	
	EXTI_ClearITPendingBit(EXTI_Line12); //清除LINE12上的中断标志位 
}

/******************************************************************************/
/*                             DMA2中断                                       */
/******************************************************************************/

int dma2_stream2_count=0;
int d_exti4_dma2_stream2=0;
void DMA2_Stream2_IRQHandler(void)
{
	dma2_stream2_count++;
	d_exti4_dma2_stream2 = exti4_count - dma2_stream2_count;
	DMA_ClearFlag(DMA2_Stream2,DMA_FLAG_TCIF2);
}

/************************ (C) COPYRIGHT STMicroelectronics *****END OF FILE****/
