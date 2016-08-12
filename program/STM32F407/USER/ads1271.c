#include "ads1271.h"
#include "pwm.h"
#include "delay.h"
#include "spi.h"
#include "dma.h"
#include "exti.h"

/******************************************************************************/
/*                             ADS1271��ʼ��                                  */
/*											�������Ϊ0-->High-Speed mode													*/
/*										�������Ϊ1-->High-Resolution mode											*/
/*											�������Ϊ'z'-->Low-Power mode												*/
/******************************************************************************/

void ads1271_init(char mode)
{
	ads1271_mode(mode);
  pwm_init(1,3);//ʱ��Ƶ�� 81/1/3 = 27MHz
	sync_config();//sync
	pdwn_calibration();//��У׼
  sync();//ͬ��
	spi1_init();//spi1��ʼ��
	spi2_init();//spi2��ʼ��
	dma2_init();//dma2��ʼ��
	dma1_init();//dma1��ʼ��
	exti_init();//�жϳ�ʼ��
}

/******************************************************************************/
/*                               MODE����																			*/
/*											�������Ϊ0-->High-Speed mode													*/
/*										�������Ϊ1-->High-Resolution mode											*/
/*											�������Ϊ'z'-->Low-Power mode												*/
/******************************************************************************/

void ads1271_mode(char mode)
{
	GPIO_InitTypeDef  GPIO_InitStructure;
	
	RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_GPIOB, ENABLE);//ʹ��GPIOBʱ��

	if(0 == mode)//���0
  {
		GPIO_InitStructure.GPIO_Pin = GPIO_Pin_11;//GPIOB11
		GPIO_InitStructure.GPIO_Mode = GPIO_Mode_OUT;//��ͨ���ģʽ
		GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;//�������
		GPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz;//100MHz
		GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_UP;//����
		GPIO_Init(GPIOB, &GPIO_InitStructure);//��ʼ��GPIO
		GPIO_ResetBits(GPIOB,GPIO_Pin_11);//��0
	}
	else if(1 == mode)//���1
  {
		GPIO_InitStructure.GPIO_Pin = GPIO_Pin_11;//GPIOB11
		GPIO_InitStructure.GPIO_Mode = GPIO_Mode_OUT;//��ͨ���ģʽ
		GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;//�������
		GPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz;//100MHz
		GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_UP;//����
		GPIO_Init(GPIOB, &GPIO_InitStructure);//��ʼ��GPIO
		GPIO_SetBits(GPIOB,GPIO_Pin_11);//��1
	}
	else if('z' == mode)//����
	{
		GPIO_InitStructure.GPIO_Pin = GPIO_Pin_11;//GPIOB11
		GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN;//��ͨ���ģʽ
		GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_NOPULL;//����
		GPIO_Init(GPIOB, &GPIO_InitStructure);//��ʼ��GPIO
	}
	
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
