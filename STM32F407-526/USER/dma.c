#include "dma.h"

/******************************************************************************/
/*                              DMA初始化                                     */
/******************************************************************************/

extern unsigned char spi1_rx[6], spi1_tx[6];
void dma_init(void)
{
	DMA_InitTypeDef  DMA_InitStructure;
	NVIC_InitTypeDef   NVIC_InitStructure;
	
	///////////////////////////////////////////////////////////////////////
	////                     SPI1_TX DMA配置                           ////
	///////////////////////////////////////////////////////////////////////
	
	RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_DMA2,ENABLE);//DMA2时钟使能 
	DMA_DeInit(DMA2_Stream3);
	
	while (DMA_GetCmdStatus(DMA2_Stream3) != DISABLE){}//等待DMA可配置 
	
  /* 配置 DMA Stream */
	DMA_InitStructure.DMA_Channel = DMA_Channel_3;  //通道选择
	DMA_InitStructure.DMA_PeripheralBaseAddr = (u32)&SPI1->DR;//DMA外设地址
	DMA_InitStructure.DMA_Memory0BaseAddr = (u32)spi1_tx;//DMA 存储器0地址
	DMA_InitStructure.DMA_DIR = DMA_DIR_MemoryToPeripheral;//存储器到外设模式
	DMA_InitStructure.DMA_BufferSize = 6;//数据传输量 
	DMA_InitStructure.DMA_PeripheralInc = DMA_PeripheralInc_Disable;//外设非增量模式
	DMA_InitStructure.DMA_MemoryInc = DMA_MemoryInc_Enable;//存储器增量模式
	DMA_InitStructure.DMA_PeripheralDataSize = DMA_PeripheralDataSize_Byte;//外设数据长度:8位
	DMA_InitStructure.DMA_MemoryDataSize = DMA_MemoryDataSize_Byte;//存储器数据长度:8位
	DMA_InitStructure.DMA_Mode = DMA_Mode_Normal;// 使用普通模式 
	DMA_InitStructure.DMA_Priority = DMA_Priority_VeryHigh;//最高优先级
	DMA_InitStructure.DMA_FIFOMode = DMA_FIFOMode_Disable;         
	DMA_InitStructure.DMA_FIFOThreshold = DMA_FIFOThreshold_Full;
	DMA_InitStructure.DMA_MemoryBurst = DMA_MemoryBurst_Single;//存储器突发单次传输
	DMA_InitStructure.DMA_PeripheralBurst = DMA_PeripheralBurst_Single;//外设突发单次传输
	
	DMA_Init(DMA2_Stream3, &DMA_InitStructure);//初始化DMA Stream
	SPI_I2S_DMACmd( SPI1, SPI_I2S_DMAReq_Tx, ENABLE);//使能SPI1_TX DMA传输 

	
	///////////////////////////////////////////////////////////////////////
	////                     SPI1_RX DMA配置                           ////
	///////////////////////////////////////////////////////////////////////
	
	RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_DMA2,ENABLE);//DMA2时钟使能 
	DMA_DeInit(DMA2_Stream2);
	
	while (DMA_GetCmdStatus(DMA2_Stream2) != DISABLE){}//等待DMA可配置 
	
  /* 配置 DMA Stream */
	DMA_InitStructure.DMA_Channel = DMA_Channel_3;  //通道选择
	DMA_InitStructure.DMA_PeripheralBaseAddr = (u32)&SPI1->DR;//DMA外设地址
	DMA_InitStructure.DMA_Memory0BaseAddr = (u32)spi1_rx;//DMA 存储器0地址
	DMA_InitStructure.DMA_DIR = DMA_DIR_PeripheralToMemory;//外设到存储器模式
	DMA_InitStructure.DMA_BufferSize = 6;//数据传输量 
	DMA_InitStructure.DMA_PeripheralInc = DMA_PeripheralInc_Disable;//外设非增量模式
	DMA_InitStructure.DMA_MemoryInc = DMA_MemoryInc_Enable;//存储器增量模式
	DMA_InitStructure.DMA_PeripheralDataSize = DMA_PeripheralDataSize_Byte;//外设数据长度:8位
	DMA_InitStructure.DMA_MemoryDataSize = DMA_MemoryDataSize_Byte;//存储器数据长度:8位
	DMA_InitStructure.DMA_Mode = DMA_Mode_Normal;// 使用普通模式 
	DMA_InitStructure.DMA_Priority = DMA_Priority_VeryHigh;//最高优先级
	DMA_InitStructure.DMA_FIFOMode = DMA_FIFOMode_Disable;         
	DMA_InitStructure.DMA_FIFOThreshold = DMA_FIFOThreshold_Full;
	DMA_InitStructure.DMA_MemoryBurst = DMA_MemoryBurst_Single;//存储器突发单次传输
	DMA_InitStructure.DMA_PeripheralBurst = DMA_PeripheralBurst_Single;//外设突发单次传输
	
	DMA_ITConfig(DMA2_Stream2, DMA_IT_TC, ENABLE);//开启DMA2_Stream2传输完成中断
	DMA_Init(DMA2_Stream2, &DMA_InitStructure);//初始化DMA Stream
	SPI_I2S_DMACmd( SPI1, SPI_I2S_DMAReq_Rx, ENABLE);//使能SPI1_TX DMA传输
	
	///////////////////////////////////////////////////////////////////////
	////                        NVIC配置                               ////
	///////////////////////////////////////////////////////////////////////
	
	NVIC_InitStructure.NVIC_IRQChannel = DMA2_Stream2_IRQn;
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 1;
	NVIC_InitStructure.NVIC_IRQChannelSubPriority = 2;
	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;
	NVIC_Init(&NVIC_InitStructure);
	
}
