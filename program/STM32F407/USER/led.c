#include "led.h" 

/******************************************************************************/
/*                              LED��ʼ��                                     */
/******************************************************************************/

void led_init(void)
{    	 
  GPIO_InitTypeDef  GPIO_InitStructure;

  RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_GPIOE, ENABLE);//ʹ��GPIOEʱ��

  GPIO_InitStructure.GPIO_Pin = GPIO_Pin_3 | GPIO_Pin_4 | GPIO_Pin_5;
  GPIO_InitStructure.GPIO_Mode = GPIO_Mode_OUT;//��ͨ���ģʽ
  GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;//�������
  GPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz;//100MHz
  GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_UP;//����
  GPIO_Init(GPIOE, &GPIO_InitStructure);//��ʼ��GPIO
	
	GPIO_SetBits(GPIOE,GPIO_Pin_3 | GPIO_Pin_4 | GPIO_Pin_5);//���õͣ�����
}

/******************************************************************************/
/*                             ��� 1�� 0��                                   */
/******************************************************************************/

void led_r(unsigned char status)
{
	if(status)
		GPIO_ResetBits(GPIOE,GPIO_Pin_3);
	else
		GPIO_SetBits(GPIOE,GPIO_Pin_3);
}

/******************************************************************************/
/*                             �̵� 1�� 0��                                   */
/******************************************************************************/

void led_g(unsigned char status)
{
	if(status)
		GPIO_ResetBits(GPIOE,GPIO_Pin_4);
	else
		GPIO_SetBits(GPIOE,GPIO_Pin_4);
}

/******************************************************************************/
/*                             �Ƶ� 1�� 0��                                   */
/******************************************************************************/

void led_y(unsigned char status)
{
	if(status)
		GPIO_ResetBits(GPIOE,GPIO_Pin_5);
	else
		GPIO_SetBits(GPIOE,GPIO_Pin_5);
}

/******************************************************************************/
/*                               ��� ��ת                                    */
/******************************************************************************/

void led_r_toggle(void)
{
	GPIO_ToggleBits(GPIOE,GPIO_Pin_3);
}

/******************************************************************************/
/*                               �̵� ��ת                                    */
/******************************************************************************/

void led_g_toggle(void)
{
	GPIO_ToggleBits(GPIOE,GPIO_Pin_4);
}

/******************************************************************************/
/*                               �Ƶ� ��ת                                    */
/******************************************************************************/

void led_y_toggle(void)
{
	GPIO_ToggleBits(GPIOE,GPIO_Pin_5);
}
