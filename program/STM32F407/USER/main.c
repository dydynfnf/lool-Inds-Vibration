#include "sys.h"
#include "stm32f4xx_gpio.h"
#include "delay.h"

#include "led.h"
#include "exti.h"
#include "stm32f4xx_it.h"
#include "ads1271.h"

unsigned char spi1_rx[6], spi1_tx[6];
unsigned char spi2_rx[6], spi2_tx[6];
short ad1,ad2;
short ad3,ad4;

int main(void)
{
	NVIC_PriorityGroupConfig(NVIC_PriorityGroup_2);//设置系统中断优先级分组2
	delay_init(168);
	led_init();
	ads1271_init();
	exti_init();
	
	//spi1_rx_dma_transfer_once();//spi1 rx dma 传输一次
	//spi1_tx_dma_transfer_once();//spi1 tx dma 传输一次
	
	while(1)
	{
		led_y(0);
		delay_ms(800);
		
		led_g(1);
		delay_ms(100);
		
		led_g(0);
		led_y(1);
		delay_ms(100);
	}
}

