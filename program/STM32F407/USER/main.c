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
	NVIC_PriorityGroupConfig(NVIC_PriorityGroup_2);//����ϵͳ�ж����ȼ�����2
	delay_init(162);//��ʱ��ʼ��
	tim3_init(162);//ʱ�ӳ�ʼ��
	sram_init();//SRAM��ʼ��
	ads1271_init(0);//ad��ʼ��
	
	led_init();//LED��ʼ��
	read_device_config();//��ȡflash���豸����
	current_source_init();//����Դ��ʼ��
	
	ethernet_init();//�����ʼ��
	tcp_sever();//����tcp������
	while(!is_con());//�ȴ�����
	led_link(1);//�������ӵ�
	
	while(1)
	{		
		if(deal_int() == 0)//INT������
		{
			sys_restart();//����
		}
		
		if(deal_pre() == 0)//PRE������
		{
			sys_restart();//����
		}

		if(deal_div() == 0)//DIV����
		{
			sys_restart();//����
		}
	
		if(deal_sta() == 0)//STA����
		{
			sys_restart();//����
		}
	
		while(1)
		{
			if(deal_data() == 0)//���ݴ���
			{
				sys_restart();//����
			}
		}
	}
}

