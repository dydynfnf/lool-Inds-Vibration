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
	
	NVIC_PriorityGroupConfig(NVIC_PriorityGroup_2);//����ϵͳ�ж����ȼ�����2
	delay_init(162);//��ʱ��ʼ��
	tim3_init(162);//ʱ�ӳ�ʼ��
	sram_init();//SRAM��ʼ��
	ads1271_init(0);//ad��ʼ�� ����Ϊ0 ����ģʽ
	
	led_init();//LED��ʼ��
	read_device_config();//��ȡflash���豸����
	current_source_init();//����Դ��ʼ��
	
	ethernet_init();//�����ʼ��
	tcp_sever();//����tcp������
	while(!is_con());//�ȴ�����
	led_link(1);//�������ӵ�

	while(1)
	{
		//INT������
		ret = deal_int();
		if((ret == NET_ERR) || (ret == NET_DISCONNECT))
			sys_restart();//����
		
		//PRE������
		ret = deal_pre();
		if((ret == NET_ERR) || (ret == NET_DISCONNECT))
			sys_restart();//����
		
		//DIV����
		ret = deal_div();
		if((ret == NET_ERR) || (ret == NET_DISCONNECT))
			sys_restart();//����
		
		//STA����
		ret = deal_sta();
		if((ret == NET_ERR) || (ret == NET_DISCONNECT))
			sys_restart();//����
		
		//���ݴ���
		while(1)
		{
			ret = deal_data();
			if((ret == NET_ERR) || (ret == NET_DISCONNECT))
				sys_restart();//����
		}
	}
}

