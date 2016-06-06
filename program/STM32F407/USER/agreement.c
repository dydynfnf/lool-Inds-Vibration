#include "agreement.h"

extern unsigned int TIM3_ms;

/******************************************************************************/
/*                           'INT'�����                                    */
/*                             ���󷵻�0                                      */
/*                             �ɹ�����1                                      */
/******************************************************************************/

unsigned char deal_int(void)
{
	unsigned long len;
	unsigned char command[8];
	
	TIM3_ms=0;//��ʱ����
	while(1)
	{
		if(TIM3_ms>=2000)//2�볬ʱ
		{
			return 0;//ʧ��
		}
		len=recv_len();//�յ����ݳ���
		if(len>0)
		{
			recv_data(command,len);//��������
			if(command[0]=='I'&&command[1]=='N'&&command[2]=='T')//�յ�'INT'
			{
				command[0]='A';//'ACK'Ӧ��
				command[1]='C';
				command[2]='K';
				command[4]=1;//ת�ٲ����
				command[5]=6;//�¶Ȳ����
				command[6]=1;//��ʪ�Ȳ����
				command[7]=4;//��ͨ����
				send_data(command,8);//����Ӧ��
				
				return 1;//�ɹ�
			}
		}
	}
	
}
	
/******************************************************************************/
/*                           'PRE'�����                                    */
/*                             ���󷵻�0                                      */
/*                             �ɹ�����1                                      */
/******************************************************************************/

unsigned char pre;
unsigned char deal_pre(void)
{
	unsigned long len;
	unsigned char command[8];
	
	TIM3_ms=0;//��ʱ����
	while(1)
	{
		if(TIM3_ms>=2000)//2�볬ʱ
		{
			return 0;//ʧ��
		}
		len=recv_len();//�յ����ݳ���
		if(len>0)
		{
			recv_data(command,len);//��������
			if(command[0]=='P'&&command[1]=='R'&&command[2]=='E')//�յ�'PRE'
			{
				pre=command[7];//��ȡԤ��Ƶֵ
				
				command[0]='A';//'ACK'Ӧ��
				command[1]='C';
				command[2]='K';
				send_data(command,8);//����Ӧ��
				
				return 1;//�ɹ�
			}
		}
	}
}
	
/******************************************************************************/
/*                           'DIV'�����                                    */
/*                             ���󷵻�0                                      */
/*                             �ɹ�����1                                      */
/******************************************************************************/

unsigned char div[4];
unsigned char deal_div(void)
{
	unsigned char i = 0;
	unsigned long len;
	unsigned char command[8];
	
	i=0;
	TIM3_ms=0;//��ʱ����
	while(1)
	{
		if(TIM3_ms>=2000)//2�볬ʱ
		{
			return 0;//ʧ��
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
					return 1;//�ɹ�
				}
			}
		}
	}
}
	
/******************************************************************************/
/*                           'STA'�����                                    */
/*                             ���󷵻�0                                      */
/*                             �ɹ�����1                                      */
/******************************************************************************/

unsigned char deal_sta(void)
{
	unsigned long len;
	unsigned char command[8];
	
	while(1)
	{
		if(TIM3_ms>=2000)//2�볬ʱ
		{
			return 0;//ʧ��
		}
		len=recv_len();//�յ����ݳ���
		if(len>0)
		{
			recv_data(command,len);//��������
			if(command[0]=='S'&&command[1]=='T'&&command[2]=='A')//�յ�'STA'
			{
				return 1;//�ɹ�
			}
		}
	}
}
	
/******************************************************************************/
/*                             ���ݴ���                                       */
/*                             ���󷵻�0                                      */
/*                             �ɹ�����1                                      */
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
	
	w5300_read(&state,Sn_SSR1(0),1);//��������Ƿ�Ͽ�
	if(state!=SOCK_ESTABLISHED)
	{
		return 0;//ʧ��
	}
	
	p_package = package;//���ݰ�ָ��
	
	*p_package++ = 'D';//��ͷ
	*p_package++ = 'A';
	*p_package++ = 'T';
	
	*p_package++ = (package_num)>>8;//����Ÿ߰�λ
	*p_package++ = package_num;//����ŵͰ�λ
	package_num++;//�����+1
	if(package_num>93750/(pre+1)/500)//�����1~93750/(pre+1)/500
	{
		package_num=1;
	}
	
	while(dw<=1000);//�ȴ��ڴ�����㹻�洢���
	
	for(i=0;i<1000;i++)//ͨ��1����д�����ݰ�
	{
		*p_package++ = *p_ad1_buffer++;
	}
	if(p_ad1_buffer - ad1_buffer >= Max_Buffer)//�ж�ͨ��1ָ���Ƿ񳬳�����
	{
		p_ad1_buffer = ad1_buffer;
	}
	
	for(i=0;i<1000;i++)//ͨ��2����д�����ݰ�
	{
		*p_package++ = *p_ad2_buffer++;
	}
	if(p_ad2_buffer - ad2_buffer >= Max_Buffer)//�ж�ͨ��2ָ���Ƿ񳬳�����
	{
		p_ad2_buffer = ad2_buffer;
	}
	
	for(i=0;i<1000;i++)//ͨ��3����д�����ݰ�
	{
		*p_package++ = *p_ad3_buffer++;
	}
	if(p_ad3_buffer - ad3_buffer >= Max_Buffer)//�ж�ͨ��3ָ���Ƿ񳬳�����
	{
		p_ad3_buffer = ad3_buffer;
	}
	
	for(i=0;i<1000;i++)//ͨ��4����д�����ݰ�
	{
		*p_package++ = *p_ad4_buffer++;
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
	
	return 1;//�ɹ�
}
