#include "sys.h"
#include "stm32f4xx_gpio.h"
#include "stm32f4xx_it.h"

#include "delay.h"
#include "timer.h"
#include "flash.h"
#include "led.h"
#include "ads1271.h"
#include "sram.h"
#include "power.h"
#include "ethernet.h"
#include "agreement.h"
#include "iwgd.h"

int main(void)
{
	int ret;
	
	NVIC_PriorityGroupConfig(NVIC_PriorityGroup_2);//设置系统中断优先级分组2
	delay_init(162);//延时初始化
	tim3_init(162);//时钟初始化
	sram_init();//SRAM初始化
	ads1271_init(0);//ad初始化 参数为0 高速模式
	
	led_init();//LED初始化
	read_device_config();//读取flash中设备配置
	current_source_init();//恒流源初始化
	
	ethernet_init();//网络初始化
	tcp_sever();//建立tcp服务器
	while(!is_con());//等待连接
	led_link(1);//开启连接灯

	while(1)
	{
		//INT处理部分
		ret = deal_int();
		if((ret == NET_ERR) || (ret == NET_DISCONNECT))
			sys_restart();//重启
		
		//PRE处理部分
		ret = deal_pre();
		if((ret == NET_ERR) || (ret == NET_DISCONNECT))
			sys_restart();//重启
		
		//DIV处理
		ret = deal_div();
		if((ret == NET_ERR) || (ret == NET_DISCONNECT))
			sys_restart();//重启
		
		//STA处理
		ret = deal_sta();
		if((ret == NET_ERR) || (ret == NET_DISCONNECT))
			sys_restart();//重启
		
		//数据传输
		while(1)
		{
			ret = deal_data();
			if((ret == NET_ERR) || (ret == NET_DISCONNECT))
				sys_restart();//重启
		}
	}
}

