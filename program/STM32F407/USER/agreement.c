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
/*                             ������Ư                                       */
/*                       channel:Ҫ�����ͨ�� 1-4                             */
/*                     precision:��Ư�������� 1-255                           */
/*                            ������Ư���                                    */
/******************************************************************************/

short calculate_drift(u8 channel, u8 precision)
{
	int sum;
	u32 i;
	short drift_last = 0,drift_now = 0;
	
	while(1)
	{
		sum = 0;
		
		for(i=0; i<=1000; i++)//���
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
		drift_now = sum/1000;//ͨ����ƽ��
		
		if(drift_last-drift_now < precision && drift_last-drift_now > -precision)//�ȴ�����ȶ�
		{
			break;
		}
		
		drift_last = drift_now;//����һ��ֵ
		
		//��ʱ1S
		TIM3_ms = 0;
		while(TIM3_ms <= 1000);
	}
	
	return drift_now;
}

/******************************************************************************/
/*                'INT','IPC','CAL','CUR'�����������                       */
/*                   ���󷵻�NET_DISCONNECT / NET_ERR                         */
/*                             �ɹ�����0                                      */
/******************************************************************************/

unsigned char deal_int(void)
{
	unsigned char state;
	unsigned long len;
	unsigned char command[16];
	
	while(1)
	{
		w5300_read(&state,Sn_SSR1(0),1);//��������Ƿ�Ͽ�
		if(state!=SOCK_ESTABLISHED)//���ӶϿ�
		{
			return NET_DISCONNECT;//ʧ��
		}
		
		len=recv_len();//�յ����ݳ���
		if(len>0)
		{
			recv_data(command,len);//��������
			if(command[0]=='I'&&command[1]=='P'&&command[2]=='C')//�յ�'IPC'
			{
				//����ip��ַ
				device_config.sip[0] = command[3];
				device_config.sip[1] = command[4];
				device_config.sip[2] = command[5];
				device_config.sip[3] = command[6];
				
				//������������
				device_config.sub[0] = command[7];
				device_config.sub[1] = command[8];
				device_config.sub[2] = command[9];
				device_config.sub[3] = command[10];
				
				//��������
				device_config.ga[0] = command[11];
				device_config.ga[1] = command[12];
				device_config.ga[2] = command[13];
				device_config.ga[3] = command[14];
				
				//�������õ�flash
				save_device_config();
				
				//Ӧ��
				command[0]='A';//'ACK'Ӧ��
				command[1]='C';
				command[2]='K';
				send_data(command,16);//����Ӧ��
				
				return NET_ERR;//����NET_ERRʹ��������
			}
			else if(command[0]=='C'&&command[1]=='A'&&command[2]=='L')//�յ�'CAL'
			{
				//��ͨ����Ư
				device_config.drift1 = calculate_drift(1, 10);
				device_config.drift2 = calculate_drift(2, 10);
				device_config.drift3 = calculate_drift(3, 10);
				device_config.drift4 = calculate_drift(4, 10);
				
				//�������õ�flash
				save_device_config();
				
				//Ӧ��
				command[0]='A';//'ACK'Ӧ��
				command[1]='C';
				command[2]='K';
				send_data(command,8);//����Ӧ��
				
				return NET_ERR;//����NET_ERRʹ��������
			}
			else if(command[0]=='C'&&command[1]=='U'&&command[2]=='R')
			{
				//���ú���Դ
				device_config.cur = command[7];
				
				//�������õ�flash
				save_device_config();
				
				//Ӧ��
				command[0]='A';//'ACK'Ӧ��
				command[1]='C';
				command[2]='K';
				send_data(command,8);//����Ӧ��
				
				return NET_ERR;//����NET_ERRʹ��������
			}
			else if(command[0]=='I'&&command[1]=='N'&&command[2]=='T')//�յ�'INT'
			{
				command[0]='A';//'ACK'Ӧ��
				command[1]='C';
				command[2]='K';
				command[4]=1;//ת�ٲ����
				command[5]=6;//�¶Ȳ����
				command[6]=1;//��ʪ�Ȳ����
				command[7]=4;//��ͨ����
				send_data(command,8);//����Ӧ��
				
				return 0;//�ɹ�
			}
		}
	}
	
}
	
/******************************************************************************/
/*                           'PRE'�����                                    */
/*                   ���󷵻�NET_DISCONNECT / NET_ERR                         */
/*                             �ɹ�����0                                      */
/******************************************************************************/

unsigned char pre;
unsigned char deal_pre(void)
{
	unsigned char state;
	unsigned long len;
	unsigned char command[8];
	
	while(1)
	{
		w5300_read(&state,Sn_SSR1(0),1);//��������Ƿ�Ͽ�
		if(state!=SOCK_ESTABLISHED)//���ӶϿ�
		{
			return NET_DISCONNECT;//ʧ��
		}
		
		len=recv_len();//�յ����ݳ���
		if(len>0)
		{
			recv_data(command,len);//��������
			if(command[0]=='P'&&command[1]=='R'&&command[2]=='E')//�յ�'PRE'
			{
				pre=command[7];//��ȡԤ��Ƶֵ
				ads1271_clk_scaler(pre);//����ADʱ��ΪԤ��Ƶֵ
				
				command[0]='A';//'ACK'Ӧ��
				command[1]='C';
				command[2]='K';
				send_data(command,8);//����Ӧ��
				
				return 0;//�ɹ�
			}
		}
	}
}
	
/******************************************************************************/
/*                           'DIV'�����                                    */
/*                   ���󷵻�NET_DISCONNECT / NET_ERR                         */
/*                             �ɹ�����0                                      */
/******************************************************************************/
/*                        ��ָ����ʱδ������                                  */
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
		w5300_read(&state,Sn_SSR1(0),1);//��������Ƿ�Ͽ�
		if(state!=SOCK_ESTABLISHED)//���ӶϿ�
		{
			return NET_DISCONNECT;//ʧ��
		}
		
		len=recv_len();//�յ����ݳ���
		if(len>0)
		{
			recv_data(command,len);//��������
			if(command[0]=='D'&&command[1]=='I'&&command[2]=='V')//�յ�'DIV'
			{
				div[command[6]]=command[7];//ͨ����Ƶֵ
				
				command[0]='A';//'ACK'Ӧ��
				command[1]='C';
				command[2]='K';
				send_data(command,8);//����Ӧ��
				
				i++;
				if(i>=4)//4ͨ��
				{
					return 0;//�ɹ�
				}
			}
		}
	}
}
	
/******************************************************************************/
/*                           'STA'�����                                    */
/*                   ���󷵻�NET_DISCONNECT / NET_ERR                         */
/*                             �ɹ�����0                                      */
/******************************************************************************/

unsigned char deal_sta(void)
{
	unsigned char state;
	unsigned long len;
	unsigned char command[8];
	
	while(1)
	{
		w5300_read(&state,Sn_SSR1(0),1);//��������Ƿ�Ͽ�
		if(state!=SOCK_ESTABLISHED)//���ӶϿ�
		{
			return NET_DISCONNECT;//ʧ��
		}
		
		len=recv_len();//�յ����ݳ���
		if(len>0)
		{
			recv_data(command,len);//��������
			if(command[0]=='S'&&command[1]=='T'&&command[2]=='A')//�յ�'STA'
			{
				return 0;//�ɹ�
			}
		}
	}
}
	
/******************************************************************************/
/*                             ���ݴ���                                       */
/*                   ���󷵻�NET_DISCONNECT / NET_ERR                         */
/*                             �ɹ�����0                                      */
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
	
	w5300_read(&state,Sn_SSR1(0),1);//��������Ƿ�Ͽ�
	if(state!=SOCK_ESTABLISHED)//���ӶϿ�
	{
		return NET_DISCONNECT;//ʧ��
	}
	
	p_package = package;//���ݰ�ָ��
	
	*p_package++ = 'D';//��ͷ
	*p_package++ = 'A';
	*p_package++ = 'T';
	
	*p_package++ = (package_num)>>8;//����Ÿ߰�λ
	*p_package++ = package_num;//����ŵͰ�λ
	package_num++;//�����+1
	if(package_num == 93750/(pre+1)/500/2)//������������
	{
		led_sample_toggle();//��������˸
	}
	else if(package_num > 93750/(pre+1)/500)//�����1~93750/(pre+1)/500
	{
		led_sample_toggle();//��������˸
		package_num=1;
	}
	
	while(dw<=1000);//�ȴ��ڴ�����㹻�洢���
	
	for(i=0;i<500;i++)//ͨ��1����д�����ݰ�
	{
		//ȥ��ͨ����Ư
		short_data = (short)(*p_ad1_buffer<<8 | *(p_ad1_buffer+1)) - device_config.drift1;
		
		//д�����ݰ�
		*p_package = (u8)(short_data>>8);
		*(p_package+1) = (u8)short_data;
		
		//ָ�����
		p_ad1_buffer+=2;
		p_package+=2;
	}
	if(p_ad1_buffer - ad1_buffer >= Max_Buffer)//�ж�ͨ��1ָ���Ƿ񳬳�����
	{
		p_ad1_buffer = ad1_buffer;
	}
	
	for(i=0;i<500;i++)//ͨ��2����д�����ݰ�
	{
		//ȥ��ͨ����Ư
		short_data = (short)(*p_ad2_buffer<<8 | *(p_ad2_buffer+1)) - device_config.drift2;
		
		//д�����ݰ�
		*p_package = (u8)(short_data>>8);
		*(p_package+1) = (u8)short_data;
		
		//ָ�����
		p_ad2_buffer+=2;
		p_package+=2;
	}
	if(p_ad2_buffer - ad2_buffer >= Max_Buffer)//�ж�ͨ��2ָ���Ƿ񳬳�����
	{
		p_ad2_buffer = ad2_buffer;
	}
	
	for(i=0;i<500;i++)//ͨ��3����д�����ݰ�
	{
		//ȥ��ͨ����Ư
		short_data = (short)(*p_ad3_buffer<<8 | *(p_ad3_buffer+1)) - device_config.drift3;
		
		//д�����ݰ�
		*p_package = (u8)(short_data>>8);
		*(p_package+1) = (u8)short_data;
		
		//ָ�����
		p_ad3_buffer+=2;
		p_package+=2;
	}
	if(p_ad3_buffer - ad3_buffer >= Max_Buffer)//�ж�ͨ��3ָ���Ƿ񳬳�����
	{
		p_ad3_buffer = ad3_buffer;
	}
	
	for(i=0;i<500;i++)//ͨ��4����д�����ݰ�
	{
		//ȥ��ͨ����Ư
		short_data = (short)(*p_ad4_buffer<<8 | *(p_ad4_buffer+1)) - device_config.drift4;
		
		//д�����ݰ�
		*p_package = (u8)(short_data>>8);
		*(p_package+1) = (u8)short_data;
		
		//ָ�����
		p_ad4_buffer+=2;
		p_package+=2;
	}
	if(p_ad4_buffer - ad4_buffer >= Max_Buffer)//�ж�ͨ��4ָ���Ƿ񳬳�����
	{
		p_ad4_buffer = ad4_buffer;
	}
	
	dw-=1000;//�洢���-1000
	
	*p_package++ = 0;//ת�ٸ߰�λ
	*p_package++ = 0;//ת�ٵͰ�λ
	
	for(i=0;i<6;i++)//�¶�
	{
		*p_package++ = 0;//�¶�������������
		*p_package++ = 0;//�¶�����С������
	}
	
	*p_package++ = 0;//�¶Ȳ���
	*p_package++ = 0;//ʪ�Ȳ���

	*p_package++ = '_';//��β
	*p_package++ = 'P';
	*p_package++ = 'S';
	*p_package++ = 'A';
	*p_package++ = 'I';
	if( ( (p_package-package) & 0x01 ) == 1 )//������������
	{
		*p_package++ = '_';
	}

	send_data(package, p_package-package);//�������ݰ�
	
	return 0;//�ɹ�
}
