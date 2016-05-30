#include "ethernet.h"	       
#include "w5300.h"
#include "delay.h"

#define Bank1_SRAM2_ADDR    ((u32)(0x64000000))

/******************************************************************************/
/*                            ETHERNET初始化                                  */
/******************************************************************************/

void ethernet_config(void)
{	
	GPIO_InitTypeDef  GPIO_InitStructure;
	FSMC_NORSRAMInitTypeDef  FSMC_NORSRAMInitStructure;
  FSMC_NORSRAMTimingInitTypeDef  readWriteTiming; 
	
	///////////////////////////////////////////////////////////////////////
	////                      ETHERNET GPIO配置                        ////
	///////////////////////////////////////////////////////////////////////
	
	RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_GPIOD | RCC_AHB1Periph_GPIOE 
	| RCC_AHB1Periph_GPIOF | RCC_AHB1Periph_GPIOG, ENABLE);//使能PD,PE,PF,PG时钟

	GPIO_InitStructure.GPIO_Pin = (3<<0)|(3<<4)|(0XFF<<8);//PD0,1,4,5,8~15 AF OUT
  GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF;//复用输出
  GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;//推挽输出
  GPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz;//100MHz
  GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_UP;//上拉
  GPIO_Init(GPIOD, &GPIO_InitStructure);//初始化  
	
  GPIO_InitStructure.GPIO_Pin = (3<<0)|(0X1FF<<7);//PE0,1,7~15,AF OUT
  GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF;//复用输出
  GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;//推挽输出
  GPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz;//100MHz
  GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_UP;//上拉
  GPIO_Init(GPIOE, &GPIO_InitStructure);//初始化  
	
 	GPIO_InitStructure.GPIO_Pin = (0X3F<<0)|(0XF<<12); 	//PF0~5,12~15
  GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF;//复用输出
  GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;//推挽输出
  GPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz;//100MHz
  GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_UP;//上拉
  GPIO_Init(GPIOF, &GPIO_InitStructure);//初始化  

	GPIO_InitStructure.GPIO_Pin =(0X3F<<0)| GPIO_Pin_9;//PG0~5,9
  GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF;//复用输出
  GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;//推挽输出
  GPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz;//100MHz
  GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_UP;//上拉
  GPIO_Init(GPIOG, &GPIO_InitStructure);//初始化 
 
 
  GPIO_PinAFConfig(GPIOD,GPIO_PinSource0,GPIO_AF_FSMC);//PD0,AF12
  GPIO_PinAFConfig(GPIOD,GPIO_PinSource1,GPIO_AF_FSMC);//PD1,AF12
  GPIO_PinAFConfig(GPIOD,GPIO_PinSource4,GPIO_AF_FSMC);
  GPIO_PinAFConfig(GPIOD,GPIO_PinSource5,GPIO_AF_FSMC); 
  GPIO_PinAFConfig(GPIOD,GPIO_PinSource8,GPIO_AF_FSMC); 
  GPIO_PinAFConfig(GPIOD,GPIO_PinSource9,GPIO_AF_FSMC);
  GPIO_PinAFConfig(GPIOD,GPIO_PinSource10,GPIO_AF_FSMC);
  GPIO_PinAFConfig(GPIOD,GPIO_PinSource11,GPIO_AF_FSMC);
	GPIO_PinAFConfig(GPIOD,GPIO_PinSource12,GPIO_AF_FSMC);
  GPIO_PinAFConfig(GPIOD,GPIO_PinSource13,GPIO_AF_FSMC);
  GPIO_PinAFConfig(GPIOD,GPIO_PinSource14,GPIO_AF_FSMC);
  GPIO_PinAFConfig(GPIOD,GPIO_PinSource15,GPIO_AF_FSMC);//PD15,AF12
 
  GPIO_PinAFConfig(GPIOE,GPIO_PinSource0,GPIO_AF_FSMC);
  GPIO_PinAFConfig(GPIOE,GPIO_PinSource1,GPIO_AF_FSMC);
	GPIO_PinAFConfig(GPIOE,GPIO_PinSource7,GPIO_AF_FSMC);//PE7,AF12
  GPIO_PinAFConfig(GPIOE,GPIO_PinSource8,GPIO_AF_FSMC);
  GPIO_PinAFConfig(GPIOE,GPIO_PinSource9,GPIO_AF_FSMC);
  GPIO_PinAFConfig(GPIOE,GPIO_PinSource10,GPIO_AF_FSMC);
  GPIO_PinAFConfig(GPIOE,GPIO_PinSource11,GPIO_AF_FSMC);
  GPIO_PinAFConfig(GPIOE,GPIO_PinSource12,GPIO_AF_FSMC);
  GPIO_PinAFConfig(GPIOE,GPIO_PinSource13,GPIO_AF_FSMC);
  GPIO_PinAFConfig(GPIOE,GPIO_PinSource14,GPIO_AF_FSMC);
  GPIO_PinAFConfig(GPIOE,GPIO_PinSource15,GPIO_AF_FSMC);//PE15,AF12
 
  GPIO_PinAFConfig(GPIOF,GPIO_PinSource0,GPIO_AF_FSMC);//PF0,AF12
  GPIO_PinAFConfig(GPIOF,GPIO_PinSource1,GPIO_AF_FSMC);//PF1,AF12
  GPIO_PinAFConfig(GPIOF,GPIO_PinSource2,GPIO_AF_FSMC);//PF2,AF12
  GPIO_PinAFConfig(GPIOF,GPIO_PinSource3,GPIO_AF_FSMC);//PF3,AF12
  GPIO_PinAFConfig(GPIOF,GPIO_PinSource4,GPIO_AF_FSMC);//PF4,AF12
  GPIO_PinAFConfig(GPIOF,GPIO_PinSource5,GPIO_AF_FSMC);//PF5,AF12
  GPIO_PinAFConfig(GPIOF,GPIO_PinSource12,GPIO_AF_FSMC);//PF12,AF12
  GPIO_PinAFConfig(GPIOF,GPIO_PinSource13,GPIO_AF_FSMC);//PF13,AF12
  GPIO_PinAFConfig(GPIOF,GPIO_PinSource14,GPIO_AF_FSMC);//PF14,AF12
  GPIO_PinAFConfig(GPIOF,GPIO_PinSource15,GPIO_AF_FSMC);//PF15,AF12
	
  GPIO_PinAFConfig(GPIOG,GPIO_PinSource0,GPIO_AF_FSMC);
  GPIO_PinAFConfig(GPIOG,GPIO_PinSource1,GPIO_AF_FSMC);
  GPIO_PinAFConfig(GPIOG,GPIO_PinSource2,GPIO_AF_FSMC);
  GPIO_PinAFConfig(GPIOG,GPIO_PinSource3,GPIO_AF_FSMC);
  GPIO_PinAFConfig(GPIOG,GPIO_PinSource4,GPIO_AF_FSMC);
  GPIO_PinAFConfig(GPIOG,GPIO_PinSource5,GPIO_AF_FSMC);
  GPIO_PinAFConfig(GPIOG,GPIO_PinSource9,GPIO_AF_FSMC);
	
	///////////////////////////////////////////////////////////////////////
	////                           FSMC配置                            ////
	///////////////////////////////////////////////////////////////////////
	
 	RCC_AHB3PeriphClockCmd(RCC_AHB3Periph_FSMC,ENABLE);//使能FSMC时钟  
	
 	readWriteTiming.FSMC_AddressSetupTime = 0x0a;	 //地址建立时间（ADDSET）为8个HCLK 8/168M=48ns
  readWriteTiming.FSMC_AddressHoldTime = 0x00;	 //地址保持时间（ADDHLD）模式A未用到	
  readWriteTiming.FSMC_DataSetupTime = 0x0a;		 ////数据保持时间（DATAST）为8个HCLK 8*6=48ns	 	 
  readWriteTiming.FSMC_BusTurnAroundDuration = 0x00;
  readWriteTiming.FSMC_CLKDivision = 0x00;
  readWriteTiming.FSMC_DataLatency = 0x00;
  readWriteTiming.FSMC_AccessMode = FSMC_AccessMode_A;	 //模式A 
	
	FSMC_NORSRAMInitStructure.FSMC_Bank = FSMC_Bank1_NORSRAM2;// 使用NE2
  FSMC_NORSRAMInitStructure.FSMC_DataAddressMux = FSMC_DataAddressMux_Disable; 
  FSMC_NORSRAMInitStructure.FSMC_MemoryType =FSMC_MemoryType_SRAM;// FSMC_MemoryType_SRAM;  //SRAM   
  FSMC_NORSRAMInitStructure.FSMC_MemoryDataWidth = FSMC_MemoryDataWidth_8b;//存储器数据宽度为16bit  
  FSMC_NORSRAMInitStructure.FSMC_BurstAccessMode =FSMC_BurstAccessMode_Disable;// FSMC_BurstAccessMode_Disable; 
  FSMC_NORSRAMInitStructure.FSMC_WaitSignalPolarity = FSMC_WaitSignalPolarity_Low;
	FSMC_NORSRAMInitStructure.FSMC_AsynchronousWait=FSMC_AsynchronousWait_Disable;
  FSMC_NORSRAMInitStructure.FSMC_WrapMode = FSMC_WrapMode_Disable;   
  FSMC_NORSRAMInitStructure.FSMC_WaitSignalActive = FSMC_WaitSignalActive_BeforeWaitState;  
  FSMC_NORSRAMInitStructure.FSMC_WriteOperation = FSMC_WriteOperation_Enable;	//存储器写使能 
  FSMC_NORSRAMInitStructure.FSMC_WaitSignal = FSMC_WaitSignal_Disable;  
  FSMC_NORSRAMInitStructure.FSMC_ExtendedMode = FSMC_ExtendedMode_Disable; // 读写使用相同的时序
  FSMC_NORSRAMInitStructure.FSMC_WriteBurst = FSMC_WriteBurst_Disable;  
  FSMC_NORSRAMInitStructure.FSMC_ReadWriteTimingStruct = &readWriteTiming;
  FSMC_NORSRAMInitStructure.FSMC_WriteTimingStruct = &readWriteTiming; //读写同样时序

  FSMC_NORSRAMInit(&FSMC_NORSRAMInitStructure);  //初始化FSMC配置

 	FSMC_NORSRAMCmd(FSMC_Bank1_NORSRAM2, ENABLE);  // 使能BANK1 NE2

	///////////////////////////////////////////////////////////////////////
	////                           W_RST配置                           ////
	///////////////////////////////////////////////////////////////////////
	
	RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_GPIOD, ENABLE);//使能GPIOD时钟

  GPIO_InitStructure.GPIO_Pin = GPIO_Pin_3;//GPIOD3
  GPIO_InitStructure.GPIO_Mode = GPIO_Mode_OUT;//普通输出模式
  GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;//推挽输出
  GPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz;//100MHz
  GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_UP;//上拉
  GPIO_Init(GPIOD, &GPIO_InitStructure);//初始化GPIO
											
}

#define device 16 //设备号 取值2~255 决定物理地址 ip地址 端口号 

/********************xintf?********************/
void w5300_write(unsigned char *pbuffer,unsigned int add,unsigned char n)
{
	for(;n>0;n--)
	{
        *(vu8*)(Bank1_SRAM2_ADDR+add)=*pbuffer;
		add++;
		pbuffer++;
	}
}

/********************xintf?********************/
void w5300_read(unsigned char *pbuffer,unsigned int add,unsigned char n)
{
	for(;n>0;n--)
	{
		*pbuffer=*(vu8*)(Bank1_SRAM2_ADDR+add);
		add++;
		pbuffer++;
	}
}

/********************w5300???********************/
unsigned char r_gar[4]={192,168,1,1};
unsigned char r_subr[4]={255,255,255,0};
unsigned char r_shar[6]={0x3c,0x97,0x0e,0xbd,0xb5,device};
unsigned char r_sipr[4]={192,168,1,device};

void ethernet_init(void)
{
	unsigned char gar[4]={192,168,1,1};
	unsigned char subr[4]={255,255,255,0};
	unsigned char shar[6]={0x3c,0x97,0x0e,0xbd,0xb5,device};
	unsigned char sipr[4]={192,168,1,device};
	unsigned char data[2];

	ethernet_config();
	//??w5300
    GPIO_SetBits(GPIOD,GPIO_Pin_3);
	delay_ms(10);
    GPIO_ResetBits(GPIOD,GPIO_Pin_3);
	delay_ms(1);
    GPIO_SetBits(GPIOD,GPIO_Pin_3);
	delay_ms(10);	

	data[0]=0x38,data[1]=0x00;
	w5300_write(data,MR,2);//w5300??????
	data[0]=0xff,data[1]=0xff;
	w5300_write(data,IR,2);//??????
	data[0]=0x00,data[1]=0x00;
	w5300_write(data,IMR,2);//??????
	w5300_write(shar,SHAR,6);//??????
	w5300_write(gar,GAR,4);//??ip??
	w5300_write(subr,SUBR,4);//????
	w5300_write(sipr,SIPR,4);//??ip
	data[0]=0x07,data[1]=0xd0;
	w5300_write(data,RTR,2);//????200ms
	data[0]=0x00,data[1]=0x08;
	w5300_write(data,RCR,2);//????8
	data[0]=0x20;
	w5300_write(data,TMS01R,1);//tx0????32k
	w5300_write(data,RMS01R,1);//rx0????32k
	data[0]=0x00,data[1]=0xff;
	w5300_write(data,MTYPER,2);//?????? 0~7tx 8~15rx
	data[0]=0x53,data[1]=0x00;
	w5300_write(data,IDR,2);//??id 5300
	
	w5300_read(r_shar,SHAR,6);//??????
	w5300_read(r_gar,GAR,4);//??ip??
	w5300_read(r_subr,SUBR,4);//????
	w5300_read(r_sipr,SIPR,4);//??ip
}

/********************w5300 tcp sever??********************/
void tcp_sever(void)
{
	unsigned char data[2];
	A1:
	data[0]=0x01,data[1]=0x21;
	w5300_write(data,Sn_MR(0),2);//???? ?????ack tcp??
	data[0]=0x00,data[0]=0x00;
	w5300_write(data,Sn_IMR(0),2);//??????
	data[0]=0xff,data[0]=0xff;
	w5300_write(data,Sn_IR(0),2);//??????
	data[0]=0x04,data[1]=0xb0+device;
	w5300_write(data,Sn_PORTR(0),2);//??? 1200+device
	data[0]=0x05,data[1]=0xb4;
	w5300_write(data,Sn_MSSR(0),2);//????1460
	data[0]=0x0a;
	w5300_write(data,Sn_KPALVTR(0),1);//?????? 0x0a*5=20s
	data[0]=0x00,data[1]=0x80;
	w5300_write(data,Sn_TTLR(0),2);//ttl???128
	
	data[0]=Sn_CR_OPEN;
	w5300_write(data,Sn_CR1(0),1);//??socket
	w5300_read(data,Sn_SSR1(0),1);//?socket??
	if(data[0]!=SOCK_INIT)//?????
	{
		data[0]=Sn_CR_CLOSE;
		w5300_write(data,Sn_CR1(0),1);//??socket
		goto A1;
	}

	data[0]=Sn_CR_LISTEN;
	w5300_write(data,Sn_CR1(0),1);//socket??
	w5300_read(data,Sn_SSR1(0),1);//?socket??
	if(data[0]!=SOCK_LISTEN)//????
	{
		data[0]=Sn_CR_CLOSE;
		w5300_write(data,Sn_CR1(0),1);//??socket
		goto A1;
	}
}

/********************w5300 tcp client??********************/
void tcp_client(void)
{
	unsigned char sn_dipr[4]={192,168,1,103};//??ip
	unsigned char data[2];
	A1:
	data[0]=0x01,data[1]=0x21;
	w5300_write(data,Sn_MR(0),2);//???? ?????ack tcp??
	data[0]=0x00,data[0]=0x00;
	w5300_write(data,Sn_IMR(0),2);//??????
	data[0]=0xff,data[0]=0xff;
	w5300_write(data,Sn_IR(0),2);//??????
	w5300_write(sn_dipr,Sn_DIPR(0),4);//??ip
	data[0]=0x04,data[1]=0xb0+device;
	w5300_write(data,Sn_PORTR(0),2);//??? 1200+device
	w5300_write(data,Sn_DPORTR0(0),2);//????? 1200+device
	data[0]=0x05,data[1]=0xb4;
	w5300_write(data,Sn_MSSR(0),2);//????1460
	data[0]=0x0a;
	w5300_write(data,Sn_KPALVTR(0),1);//?????? 50s
	data[0]=0x00,data[1]=0x80;
	w5300_write(data,Sn_TTLR(0),2);//ttl???128
	
	data[0]=Sn_CR_OPEN;
	w5300_write(data,Sn_CR1(0),1);//??socket
	w5300_read(data,Sn_SSR1(0),1);//?socket??
	if(data[0]!=SOCK_INIT)//?????
	{
		data[0]=Sn_CR_CLOSE;
		w5300_write(data,Sn_CR1(0),1);//??socket
		goto A1;
	}

	data[0]=Sn_CR_CONNECT;
	w5300_write(data,Sn_CR1(0),1);//socket??
}

/********************w5300 ????********************/
unsigned char is_con(void)
{
	unsigned char data;
	w5300_read(&data,Sn_SSR1(0),1);//??????
	if(data==SOCK_ESTABLISHED)//????????
	{
		return(1);
	}
	else
	{
		return(0);
	}
}

/********************w5300 ??????********************/
unsigned long recv_len(void)
{
	unsigned char data[4];
	unsigned long len;
	w5300_read(data,Sn_RX_RSR(0),4);//???????
	len=((data[1]&0x01)<<16)|(data[2]<<8)|(data[3]);//??????
	return(len);
}

/********************w5300 ??????********************/
unsigned char is_send(void)
{
	unsigned char data;
	w5300_read(&data,Sn_IR1(0),1);//?????
	if(data&0x10)//Sn_IR sendok?
	{
		data=0x10;
		w5300_write(&data,Sn_IR1(0),1);//?????
		return(1);
	}
	else
	{
		return(0);
	}
}

/********************w5300 ????********************/
void recv_data(unsigned char *pbuffer,unsigned long pack_size)
{
	unsigned long read_cnt,i;
	unsigned char data;
	if(pack_size/2*2!=pack_size)
	{
		read_cnt=(pack_size+1)/2;
		for(i=0;i<read_cnt;i++)
		{
			w5300_read(pbuffer,Sn_RX_FIFOR(0),2);
			pbuffer+=2;
		}
	}
	else
	{
		read_cnt=pack_size/2;
		for(i=0;i<read_cnt;i++)
		{
			w5300_read(pbuffer,Sn_RX_FIFOR(0),2);
			pbuffer+=2;
		}
	}
	data=Sn_CR_RECV;
	w5300_read(&data,Sn_CR1(0),1);
	w5300_read(&data,Sn_MR(0),1);//rx_fifo?????????????
								 //???tx_fifo
}

/********************w5300 ????********************/
void send_data(unsigned char *pbuffer,unsigned long send_size)
{
	unsigned char data[4];
	unsigned long get_free_size,write_cnt,i;
		
A1:

	w5300_read(data,Sn_SSR1(0),1);//???????????????,????
	if(data[0]!=SOCK_ESTABLISHED) //??,??fifo???,?????????
	{
		return;
	}

	w5300_read(data,Sn_TX_FSR(0),4);
	get_free_size=((data[1]&0x01)<<16)|(data[2]<<8)|(data[3]);
	if(get_free_size<send_size)
	{
		goto A1;
	}

	if(send_size/2*2!=send_size)
	{	
		write_cnt=(send_size+1)/2;
		for(i=0;i<write_cnt;i++)
		{
			w5300_write(pbuffer,Sn_TX_FIFOR(0),2);
			pbuffer+=2;
		}
	}
	else
	{
		write_cnt=send_size/2;
		for(i=0;i<write_cnt;i++)
		{
			w5300_write(pbuffer,Sn_TX_FIFOR(0),2);
			pbuffer+=2;
		}
	}

	data[0]=0x00;
	data[1]=(send_size>>16)&0xff;
	data[2]=(send_size>>8)&0xff;
	data[3]=send_size&0xff;
	w5300_write(data,Sn_TX_WRSR(0),4);
	data[0]=Sn_CR_SEND;
	w5300_write(data,Sn_CR1(0),1);
}
