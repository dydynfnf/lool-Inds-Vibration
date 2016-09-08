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
#include "dma.h"
#include "sram.h"
#include "flash.h"
#include "iwgd.h"

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
/*                             外部中断2                                      */
/*                             复位按键中断                                   */
/******************************************************************************/

extern struct Device_Config device_config;
void EXTI2_IRQHandler(void)
{
	//设置ip
	device_config.sip[0]=192;
	device_config.sip[1]=168;
	device_config.sip[2]=1;
	device_config.sip[3]=16;

	//设置子网掩码
	device_config.sub[0]=255;
	device_config.sub[1]=255;
	device_config.sub[2]=255;
	device_config.sub[3]=0;

	//设置网关
	device_config.ga[0]=192;
	device_config.ga[1]=168;
	device_config.ga[2]=1;
	device_config.ga[3]=1;
	
	//设置零漂系数
	device_config.drift1 = 0;
	device_config.drift2 = 0;
	device_config.drift3 = 0;
	device_config.drift4 = 0;
	
	//设置分频系数
	device_config.div = 0;
	
	save_device_config();//保存配置到flash
	
	EXTI_ClearITPendingBit(EXTI_Line2);//清除LINE4上的中断标志位
	
	sys_restart();//复位
}

/******************************************************************************/
/*                             缓存深度                                       */
/******************************************************************************/

extern unsigned int dw;

/******************************************************************************/
/*                             外部中断4                                      */
/*                             ADC1中断                                       */
/******************************************************************************/

extern unsigned char ad1_buffer[Max_Buffer],ad2_buffer[Max_Buffer];
unsigned int ad_i=0;
extern unsigned char spi1_rx[6];
//int exti4_count=0;
void EXTI4_IRQHandler(void)
{
	spi1_rx_dma_transfer_once();//开启一次dma接收
	spi1_tx_dma_transfer_once();//开启一次dma发送
	
	ad1_buffer[ad_i] = spi1_rx[3];//偶数位为高字节
	ad1_buffer[ad_i+1] = spi1_rx[4];//奇数位为低字节
	ad2_buffer[ad_i] = spi1_rx[0];//偶数位为高字节
	ad2_buffer[ad_i+1] = spi1_rx[1];//奇数位为低字节
	
	ad_i+=2;
	if(ad_i>=Max_Buffer)//判断写入数据是否超过最大缓存
	{
		ad_i=0;
	}
	
	dw+=2;//缓存深度
	
	if(dw >=  Max_Buffer)//缓存深度最多等于缓存长度
	{
		dw = Max_Buffer;
	}
	
//	exti4_count++;
	
	EXTI_ClearITPendingBit(EXTI_Line4);//清除LINE4上的中断标志位
}

/******************************************************************************/
/*                             外部中断12                                     */
/*                             ADC2中断                                       */
/******************************************************************************/

extern unsigned char ad3_buffer[Max_Buffer],ad4_buffer[Max_Buffer];
unsigned int ad_j=0;
extern unsigned char spi2_rx[6];
//int exti5_count=0;
void EXTI15_10_IRQHandler(void)
{	
	spi2_rx_dma_transfer_once();//开启一次dma接收
	spi2_tx_dma_transfer_once();//开启一次dma发送
	
	ad3_buffer[ad_j] = spi2_rx[3];//偶数位为高字节
	ad3_buffer[ad_j+1] = spi2_rx[4];//奇数位为低字节
	ad4_buffer[ad_j] = spi2_rx[0];//偶数位为高字节
	ad4_buffer[ad_j+1] = spi2_rx[1];//奇数位为低字节
	
	ad_j+=2;
	if(ad_j>=Max_Buffer)//判断写入数据是否超过最大缓存
	{
		ad_j=0;
	}
	
//	exti5_count++;
	
	EXTI_ClearITPendingBit(EXTI_Line12);//清除LINE12上的中断标志位
	
}

/******************************************************************************/
/*                             DMA2中断                                       */
/******************************************************************************/

//int dma2_count=0;
//int d_exti4_dma2=0;
void DMA2_Stream2_IRQHandler(void)
{
//	dma2_count++;
//	d_exti4_dma2 = exti4_count - dma2_count;
	DMA_ClearFlag(DMA2_Stream2,DMA_FLAG_TCIF2);
}

/******************************************************************************/
/*                             DMA1中断                                       */
/******************************************************************************/

//int dma1_count=0;
//int d_exti5_dma1=0;
void DMA1_Stream3_IRQHandler(void)
{
//	dma1_count++;
//	d_exti5_dma1 = exti5_count - dma1_count;
	DMA_ClearFlag(DMA1_Stream3,DMA_FLAG_TCIF3);
}

/************************ (C) COPYRIGHT STMicroelectronics *****END OF FILE****/
