#include "ads1271.h"
#include "pwm.h"
#include "delay.h"
#include "spi.h"
#include "dma.h"

/******************************************************************************/
/*                             ADS1271��ʼ��                                  */
/******************************************************************************/

void ads1271_init(void)
{
  pwm_init(1,4);//ʱ��Ƶ�� 84/1/4 = 21MHz
	sync_config();//sync
	pdwn_calibration();//��У׼
  sync();//ͬ��
	spi1_init();//spi1��ʼ��
	spi2_init();//spi2��ʼ��
//	dma_init();//dma��ʼ��
}

/******************************************************************************/
/*                               SYNC����                                     */
/******************************************************************************/

void sync_config(void)
{
	GPIO_InitTypeDef  GPIO_InitStructure;
	
	RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_GPIOB, ENABLE);//ʹ��GPIOBʱ��

  GPIO_InitStructure.GPIO_Pin = GPIO_Pin_15;//GPIOB15
  GPIO_InitStructure.GPIO_Mode = GPIO_Mode_OUT;//��ͨ���ģʽ
  GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;//�������
  GPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz;//100MHz
  GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_UP;//����
  GPIO_Init(GPIOB, &GPIO_InitStructure);//��ʼ��GPIO
}

/******************************************************************************/
/*                             �͹��� ��У׼                                  */
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
/*                                 ͬ��                                       */
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
/*                        SPI1 TX DMA����һ��                                 */
/******************************************************************************/

void spi1_tx_dma_transfer_once(void)
{
	DMA2->LIFCR=(1<<27);//���TCIF3��־
	DMA2_Stream3->CR|=1<<0; //����DMA����
}

/******************************************************************************/
/*                       SPI1 RX DMA����һ��                                  */
/******************************************************************************/

void spi1_rx_dma_transfer_once(void)
{
	DMA2->LIFCR=(1<<21);//���TCIF2��־
	DMA2_Stream2->CR|=1<<0; //����DMA����
}
