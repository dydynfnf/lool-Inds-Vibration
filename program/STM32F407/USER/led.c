#include "led.h" 

/******************************************************************************/
/*                              LED初始化                                     */
/******************************************************************************/

void led_init(void)
{    	 
  GPIO_InitTypeDef  GPIO_InitStructure;

  RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_GPIOE, ENABLE);//使能GPIOE时钟

  GPIO_InitStructure.GPIO_Pin = GPIO_Pin_3 | GPIO_Pin_4 | GPIO_Pin_5;
  GPIO_InitStructure.GPIO_Mode = GPIO_Mode_OUT;//普通输出模式
  GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;//推挽输出
  GPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz;//100MHz
  GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_UP;//上拉
  GPIO_Init(GPIOE, &GPIO_InitStructure);//初始化GPIO
	
	GPIO_SetBits(GPIOE,GPIO_Pin_3 | GPIO_Pin_4 | GPIO_Pin_5);//设置低，灯灭
}

/******************************************************************************/
/*                              采样灯-红灯                                   */
/*                           输入参数为1-->亮                                 */
/*                           输入参数为0-->灭                                 */
/******************************************************************************/

void led_sample(unsigned char status)
{
	if(status)
		GPIO_ResetBits(GPIOE,GPIO_Pin_3);
	else
		GPIO_SetBits(GPIOE,GPIO_Pin_3);
}

/******************************************************************************/
/*                              连接灯-绿灯                                   */
/*                           输入参数为1-->亮                                 */
/*                           输入参数为0-->灭                                 */
/******************************************************************************/

void led_link(unsigned char status)
{
	if(status)
		GPIO_ResetBits(GPIOE,GPIO_Pin_4);
	else
		GPIO_SetBits(GPIOE,GPIO_Pin_4);
}

/******************************************************************************/
/*                              电流灯-黄灯                                   */
/*                           输入参数为1-->亮                                 */
/*                           输入参数为0-->灭                                 */
/******************************************************************************/

void led_current(unsigned char status)
{
	if(status)
		GPIO_ResetBits(GPIOE,GPIO_Pin_5);
	else
		GPIO_SetBits(GPIOE,GPIO_Pin_5);
}

/******************************************************************************/
/*                               采样灯翻转                                     */
/******************************************************************************/

void led_sample_toggle(void)
{
	GPIO_ToggleBits(GPIOE,GPIO_Pin_3);
}

/******************************************************************************/
/*                               连接灯翻转                                     */
/******************************************************************************/

void led_link_toggle(void)
{
	GPIO_ToggleBits(GPIOE,GPIO_Pin_4);
}

/******************************************************************************/
/*                               电流灯翻转                                     */
/******************************************************************************/

void led_current_toggle(void)
{
	GPIO_ToggleBits(GPIOE,GPIO_Pin_5);
}
