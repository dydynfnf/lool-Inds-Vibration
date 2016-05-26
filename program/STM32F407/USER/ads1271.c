#include "ads1271.h"
#include "pwm.h"
#include "delay.h"
#include "spi.h"
#include "dma.h"

/******************************************************************************/
/*                             ADS1271初始化                                  */
/******************************************************************************/

void ads1271_init(void)
{
	pwm_init(1,4);//时钟频率 84/1/4 = 21MHz
	sync_config();//sync
	pdwn_calibration();//自校准
  sync();//同步
	spi1_init();//spi1初始化
	spi2_init();//spi2初始化
	dma_init();//dma初始化
}

/******************************************************************************/
/*                               SYNC配置                                     */
/******************************************************************************/

void sync_config(void)
{
	GPIO_InitTypeDef  GPIO_InitStructure;
	
	RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_GPIOB, ENABLE);//使能GPIOB时钟

  GPIO_InitStructure.GPIO_Pin = GPIO_Pin_15;//GPIOB15
  GPIO_InitStructure.GPIO_Mode = GPIO_Mode_OUT;//普通输出模式
  GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;//推挽输出
  GPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz;//100MHz
  GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_UP;//上拉
  GPIO_Init(GPIOB, &GPIO_InitStructure);//初始化GPIO
}

/******************************************************************************/
/*                             低功耗 自校准                                  */
/******************************************************************************/

void pdwn_calibration(void)
{
	GPIO_SetBits(GPIOB,GPIO_Pin_15);
	delay_ms(20);
	GPIO_ResetBits(GPIOB,GPIO_Pin_15);
	delay_ms(20);
	GPIO_SetBits(GPIOB,GPIO_Pin_15);
}

/******************************************************************************/
/*                                 同步                                       */
/******************************************************************************/

void sync(void)
{
	GPIO_SetBits(GPIOB,GPIO_Pin_15);
	delay_ms(20);
	GPIO_ResetBits(GPIOB,GPIO_Pin_15);
	delay_us(1);
	GPIO_SetBits(GPIOB,GPIO_Pin_15);
}

/******************************************************************************/
/*                        SPI1 TX DMA传输一次                                 */
/******************************************************************************/

void spi1_tx_dma_transfer_once(void)
{
	DMA2->LIFCR=(1<<27);//清除TCIF3标志
	DMA2_Stream3->CR|=1<<0; //开启DMA传输
}

/******************************************************************************/
/*                       SPI1 RX DMA传输一次                                  */
/******************************************************************************/

void spi1_rx_dma_transfer_once(void)
{
	DMA2->LIFCR=(1<<21);//清除TCIF2标志
	DMA2_Stream2->CR|=1<<0; //开启DMA传输
}
