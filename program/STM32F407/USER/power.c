#include "power.h"
#include "led.h"
#include "flash.h"

/******************************************************************************/
/*                          	  恒流源控制 																		*/
/*												输入参数为1-->打开恒流源														*/
/*												输入参数为0-->关闭恒流源                            */
/******************************************************************************/

void current_source_control(char status)
{
	if(1 == status)
	{
		GPIO_SetBits(GPIOE,GPIO_Pin_6);//设置高，打开恒流源
		led_current(1);//电流灯亮
	}
	else if(0 == status)
	{
		GPIO_ResetBits(GPIOE,GPIO_Pin_6);//设置低，关闭恒流源开通
		led_current(0);//电流灯灭
	}
	else
	{
		GPIO_ResetBits(GPIOE,GPIO_Pin_6);//其他，恒流源关断
		led_current(0);//电流灯灭
	}
}

/******************************************************************************/
/*                          	  恒流源初始化																	*/
/******************************************************************************/

extern struct Device_Config device_config;
void current_source_init(void)
{
	GPIO_InitTypeDef  GPIO_InitStructure;

  RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_GPIOE, ENABLE);//使能GPIOE时钟

  GPIO_InitStructure.GPIO_Pin = GPIO_Pin_6;
  GPIO_InitStructure.GPIO_Mode = GPIO_Mode_OUT;//普通输出模式
  GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;//推挽输出
  GPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz;//100MHz
  GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_UP;//上拉
  GPIO_Init(GPIOE, &GPIO_InitStructure);//初始化GPIO
	
	if(device_config.cur)
	{
		current_source_control(1);
	}
	else
	{
		current_source_control(0);
	}
}
