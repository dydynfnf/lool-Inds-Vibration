#include "agreement.h"
#include "led.h"
#include "flash.h"
#include "sram.h"
#include "ads1271.h"

extern unsigned int TIM3_ms;
extern struct Device_Config device_config;
extern unsigned char spi1_rx[6];
extern unsigned char spi2_rx[6];

/******************************************************************************/
/*                             计算零漂                                       */
/*                       channel:要计算的通道 1-4                             */
/*                     precision:零漂矫正精度 1-255                           */
/*                            返回零漂结果                                    */
/******************************************************************************/

short calculate_drift(u8 channel, u8 precision)
{
	int sum;
	u32 i;
	short drift_last = 0,drift_now = 0;
	
	while(1)
	{
		sum = 0;
		
		for(i=0; i<=1000; i++)//求和
		{
			switch(channel)
			{
				case 1:
					sum += (short)( (spi1_rx[3]<<8) | spi1_rx[4] );break;
				case 2:
					sum += (short)( (spi1_rx[0]<<8) | spi1_rx[1] );break;
				case 3:
					sum += (short)( (spi2_rx[3]<<8) | spi2_rx[4] );break;
				case 4:
					sum += (short)( (spi2_rx[0]<<8) | spi2_rx[1] );break;
				default:
					return 0;
			}
		}
		drift_now = sum/1000;//通道求平均
		
		if(drift_last-drift_now < precision && drift_last-drift_now > -precision)//等待误差稳定
		{
			break;
		}
		
		drift_last = drift_now;//存上一次值
		
		//延时1S
		TIM3_ms = 0;
		while(TIM3_ms <= 1000);
	}
	
	return drift_now;
}

/******************************************************************************/
/*                'INT','IPC','CAL','CUR'等配置命令处理                       */
/*                   错误返回NET_DISCONNECT / NET_ERR                         */
/*                             成功返回0                                      */
/******************************************************************************/

unsigned char deal_int(void)
{
	unsigned char state;
	unsigned long len;
	unsigned char command[16];
	
	while(1)
	{
		w5300_read(&state,Sn_SSR1(0),1);//检测连接是否断开
		if(state!=SOCK_ESTABLISHED)//连接断开
		{
			return NET_DISCONNECT;//失败
		}
		
		len=recv_len();//收到数据长度
		if(len>0)
		{
			recv_data(command,len);//接收数据
			if(command[0]=='I'&&command[1]=='P'&&command[2]=='C')//收到'IPC'
			{
				//配置ip地址
				device_config.sip[0] = command[3];
				device_config.sip[1] = command[4];
				device_config.sip[2] = command[5];
				device_config.sip[3] = command[6];
				
				//配置子网掩码
				device_config.sub[0] = command[7];
				device_config.sub[1] = command[8];
				device_config.sub[2] = command[9];
				device_config.sub[3] = command[10];
				
				//配置网关
				device_config.ga[0] = command[11];
				device_config.ga[1] = command[12];
				device_config.ga[2] = command[13];
				device_config.ga[3] = command[14];
				
				//保存配置到flash
				save_device_config();
				
				//应答
				command[0]='A';//'ACK'应答
				command[1]='C';
				command[2]='K';
				send_data(command,16);//发送应答
				
				return NET_ERR;//返回NET_ERR使程序重启
			}
			else if(command[0]=='C'&&command[1]=='A'&&command[2]=='L')//收到'CAL'
			{
				//求通道零漂
				device_config.drift1 = calculate_drift(1, 10);
				device_config.drift2 = calculate_drift(2, 10);
				device_config.drift3 = calculate_drift(3, 10);
				device_config.drift4 = calculate_drift(4, 10);
				
				//保存配置到flash
				save_device_config();
				
				//应答
				command[0]='A';//'ACK'应答
				command[1]='C';
				command[2]='K';
				send_data(command,8);//发送应答
				
				return NET_ERR;//返回NET_ERR使程序重启
			}
			else if(command[0]=='C'&&command[1]=='U'&&command[2]=='R')
			{
				//设置恒流源
				device_config.cur = command[7];
				
				//保存配置到flash
				save_device_config();
				
				//应答
				command[0]='A';//'ACK'应答
				command[1]='C';
				command[2]='K';
				send_data(command,8);//发送应答
				
				return NET_ERR;//返回NET_ERR使程序重启
			}
			else if(command[0]=='I'&&command[1]=='N'&&command[2]=='T')//收到'INT'
			{
				command[0]='A';//'ACK'应答
				command[1]='C';
				command[2]='K';
				command[4]=1;//转速测点数
				command[5]=6;//温度测点数
				command[6]=1;//温湿度测点数
				command[7]=4;//振动通道数
				send_data(command,8);//发送应答
				
				return 0;//成功
			}
		}
	}
	
}
	
/******************************************************************************/
/*                           'PRE'命令处理                                    */
/*                   错误返回NET_DISCONNECT / NET_ERR                         */
/*                             成功返回0                                      */
/******************************************************************************/

unsigned char pre;
unsigned char deal_pre(void)
{
	unsigned char state;
	unsigned long len;
	unsigned char command[8];
	
	while(1)
	{
		w5300_read(&state,Sn_SSR1(0),1);//检测连接是否断开
		if(state!=SOCK_ESTABLISHED)//连接断开
		{
			return NET_DISCONNECT;//失败
		}
		
		len=recv_len();//收到数据长度
		if(len>0)
		{
			recv_data(command,len);//接收数据
			if(command[0]=='P'&&command[1]=='R'&&command[2]=='E')//收到'PRE'
			{
				pre=command[7];//获取预分频值
				ads1271_clk_scaler(pre);//设置AD时钟为预分频值
				
				command[0]='A';//'ACK'应答
				command[1]='C';
				command[2]='K';
				send_data(command,8);//发送应答
				
				return 0;//成功
			}
		}
	}
}
	
/******************************************************************************/
/*                           'DIV'命令处理                                    */
/*                   错误返回NET_DISCONNECT / NET_ERR                         */
/*                             成功返回0                                      */
/******************************************************************************/
/*                        该指令暂时未作处理                                  */
/******************************************************************************/

unsigned char div[4];
unsigned char deal_div(void)
{
	unsigned char state;
	unsigned char i = 0;
	unsigned long len;
	unsigned char command[8];
	
	i=0;
	while(1)
	{
		w5300_read(&state,Sn_SSR1(0),1);//检测连接是否断开
		if(state!=SOCK_ESTABLISHED)//连接断开
		{
			return NET_DISCONNECT;//失败
		}
		
		len=recv_len();//收到数据长度
		if(len>0)
		{
			recv_data(command,len);//接收数据
			if(command[0]=='D'&&command[1]=='I'&&command[2]=='V')//收到'DIV'
			{
				div[command[6]]=command[7];//通道分频值
				
				command[0]='A';//'ACK'应答
				command[1]='C';
				command[2]='K';
				send_data(command,8);//发送应答
				
				i++;
				if(i>=4)//4通道
				{
					return 0;//成功
				}
			}
		}
	}
}
	
/******************************************************************************/
/*                           'STA'命令处理                                    */
/*                   错误返回NET_DISCONNECT / NET_ERR                         */
/*                             成功返回0                                      */
/******************************************************************************/

unsigned char deal_sta(void)
{
	unsigned char state;
	unsigned long len;
	unsigned char command[8];
	
	while(1)
	{
		w5300_read(&state,Sn_SSR1(0),1);//检测连接是否断开
		if(state!=SOCK_ESTABLISHED)//连接断开
		{
			return NET_DISCONNECT;//失败
		}
		
		len=recv_len();//收到数据长度
		if(len>0)
		{
			recv_data(command,len);//接收数据
			if(command[0]=='S'&&command[1]=='T'&&command[2]=='A')//收到'STA'
			{
				return 0;//成功
			}
		}
	}
}
	
/******************************************************************************/
/*                             数据传输                                       */
/*                   错误返回NET_DISCONNECT / NET_ERR                         */
/*                             成功返回0                                      */
/******************************************************************************/

unsigned char package[5000];
extern unsigned char  ad1_buffer[Max_Buffer],
											ad2_buffer[Max_Buffer],
											ad3_buffer[Max_Buffer],
											ad4_buffer[Max_Buffer];

unsigned char * p_package = package;
unsigned char * p_ad1_buffer = ad1_buffer,
							* p_ad2_buffer = ad2_buffer,
							* p_ad3_buffer = ad3_buffer,
							* p_ad4_buffer = ad4_buffer;

unsigned short package_num;
unsigned int dw;

unsigned char deal_data(void)
{
	unsigned int i;
	unsigned char state;
	short short_data;
	
	w5300_read(&state,Sn_SSR1(0),1);//检测连接是否断开
	if(state!=SOCK_ESTABLISHED)//连接断开
	{
		return NET_DISCONNECT;//失败
	}
	
	p_package = package;//数据包指针
	
	*p_package++ = 'D';//包头
	*p_package++ = 'A';
	*p_package++ = 'T';
	
	*p_package++ = (package_num)>>8;//包序号高八位
	*p_package++ = package_num;//包序号低八位
	package_num++;//包序号+1
	if(package_num == 93750/(pre+1)/500/2)//半个包发送完成
	{
		led_sample_toggle();//采样灯闪烁
	}
	else if(package_num > 93750/(pre+1)/500)//包序号1~93750/(pre+1)/500
	{
		led_sample_toggle();//采样灯闪烁
		package_num=1;
	}
	
	while(dw<=1000);//等待内存具有足够存储深度
	
	for(i=0;i<500;i++)//通道1数据写入数据包
	{
		//去除通道零漂
		short_data = (short)(*p_ad1_buffer<<8 | *(p_ad1_buffer+1)) - device_config.drift1;
		
		//写入数据包
		*p_package = (u8)(short_data>>8);
		*(p_package+1) = (u8)short_data;
		
		//指针递增
		p_ad1_buffer+=2;
		p_package+=2;
	}
	if(p_ad1_buffer - ad1_buffer >= Max_Buffer)//判断通道1指针是否超出缓存
	{
		p_ad1_buffer = ad1_buffer;
	}
	
	for(i=0;i<500;i++)//通道2数据写入数据包
	{
		//去除通道零漂
		short_data = (short)(*p_ad2_buffer<<8 | *(p_ad2_buffer+1)) - device_config.drift2;
		
		//写入数据包
		*p_package = (u8)(short_data>>8);
		*(p_package+1) = (u8)short_data;
		
		//指针递增
		p_ad2_buffer+=2;
		p_package+=2;
	}
	if(p_ad2_buffer - ad2_buffer >= Max_Buffer)//判断通道2指针是否超出缓存
	{
		p_ad2_buffer = ad2_buffer;
	}
	
	for(i=0;i<500;i++)//通道3数据写入数据包
	{
		//去除通道零漂
		short_data = (short)(*p_ad3_buffer<<8 | *(p_ad3_buffer+1)) - device_config.drift3;
		
		//写入数据包
		*p_package = (u8)(short_data>>8);
		*(p_package+1) = (u8)short_data;
		
		//指针递增
		p_ad3_buffer+=2;
		p_package+=2;
	}
	if(p_ad3_buffer - ad3_buffer >= Max_Buffer)//判断通道3指针是否超出缓存
	{
		p_ad3_buffer = ad3_buffer;
	}
	
	for(i=0;i<500;i++)//通道4数据写入数据包
	{
		//去除通道零漂
		short_data = (short)(*p_ad4_buffer<<8 | *(p_ad4_buffer+1)) - device_config.drift4;
		
		//写入数据包
		*p_package = (u8)(short_data>>8);
		*(p_package+1) = (u8)short_data;
		
		//指针递增
		p_ad4_buffer+=2;
		p_package+=2;
	}
	if(p_ad4_buffer - ad4_buffer >= Max_Buffer)//判断通道4指针是否超出缓存
	{
		p_ad4_buffer = ad4_buffer;
	}
	
	dw-=1000;//存储深度-1000
	
	*p_package++ = 0;//转速高八位
	*p_package++ = 0;//转速低八位
	
	for(i=0;i<6;i++)//温度
	{
		*p_package++ = 0;//温度数据整数部分
		*p_package++ = 0;//温度数据小数部分
	}
	
	*p_package++ = 0;//温度部分
	*p_package++ = 0;//湿度部分

	*p_package++ = '_';//包尾
	*p_package++ = 'P';
	*p_package++ = 'S';
	*p_package++ = 'A';
	*p_package++ = 'I';
	if( ( (p_package-package) & 0x01 ) == 1 )//包长度是奇数
	{
		*p_package++ = '_';
	}

	send_data(package, p_package-package);//发送数据包
	
	return 0;//成功
}
