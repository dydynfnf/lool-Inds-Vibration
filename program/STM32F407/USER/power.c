#include "power.h"
#include "led.h"
#include "flash.h"

/******************************************************************************/
/*                          	  ����Դ���� 																		*/
/*												�������Ϊ1-->�򿪺���Դ														*/
/*												�������Ϊ0-->�رպ���Դ                            */
/******************************************************************************/

void current_source_control(char status)
{
	if(1 == status)
	{
		GPIO_SetBits(GPIOE,GPIO_Pin_6);//���øߣ��򿪺���Դ
		led_current(1);//��������
	}
	else if(0 == status)
	{
		GPIO_ResetBits(GPIOE,GPIO_Pin_6);//���õͣ��رպ���Դ��ͨ
		led_current(0);//��������
	}
	else
	{
		GPIO_ResetBits(GPIOE,GPIO_Pin_6);//����������Դ�ض�
		led_current(0);//��������
	}
}

/******************************************************************************/
/*                          	  ����Դ��ʼ��																	*/
/******************************************************************************/

extern struct Device_Config device_config;
void current_source_init(void)
{
	GPIO_InitTypeDef  GPIO_InitStructure;

  RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_GPIOE, ENABLE);//ʹ��GPIOEʱ��

  GPIO_InitStructure.GPIO_Pin = GPIO_Pin_6;
  GPIO_InitStructure.GPIO_Mode = GPIO_Mode_OUT;//��ͨ���ģʽ
  GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;//�������
  GPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz;//100MHz
  GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_UP;//����
  GPIO_Init(GPIOE, &GPIO_InitStructure);//��ʼ��GPIO
	
	if(device_config.cur)
	{
		current_source_control(1);
	}
	else
	{
		current_source_control(0);
	}
}
