#include "dma.h"

/******************************************************************************/
/*                              DMA2��ʼ��                                    */
/*                   SPI1_TX--DMA2--CHANNEL3--STREAM3                         */
/*                   SPI1_RX--DMA2--CHANNEL3--STREAM2                         */
/******************************************************************************/

unsigned char spi1_rx[6], spi1_tx[6];
void dma2_init(void)
{
	DMA_InitTypeDef  DMA_InitStructure;
	NVIC_InitTypeDef   NVIC_InitStructure;
	
	///////////////////////////////////////////////////////////////////////
	////                     SPI1_TX DMA����                           ////
	///////////////////////////////////////////////////////////////////////
	
	RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_DMA2,ENABLE);//DMA2ʱ��ʹ�� 
	DMA_DeInit(DMA2_Stream3);
	
	while (DMA_GetCmdStatus(DMA2_Stream3) != DISABLE){}//�ȴ�DMA������ 
	
  /* ���� DMA Stream */
	DMA_InitStructure.DMA_Channel = DMA_Channel_3;  //ͨ��ѡ��
	DMA_InitStructure.DMA_PeripheralBaseAddr = (u32)&SPI1->DR;//DMA�����ַ
	DMA_InitStructure.DMA_Memory0BaseAddr = (u32)spi1_tx;//DMA �洢��0��ַ
	DMA_InitStructure.DMA_DIR = DMA_DIR_MemoryToPeripheral;//�洢��������ģʽ
	DMA_InitStructure.DMA_BufferSize = 6;//���ݴ����� 
	DMA_InitStructure.DMA_PeripheralInc = DMA_PeripheralInc_Disable;//���������ģʽ
	DMA_InitStructure.DMA_MemoryInc = DMA_MemoryInc_Enable;//�洢������ģʽ
	DMA_InitStructure.DMA_PeripheralDataSize = DMA_PeripheralDataSize_Byte;//�������ݳ���:8λ
	DMA_InitStructure.DMA_MemoryDataSize = DMA_MemoryDataSize_Byte;//�洢�����ݳ���:8λ
	DMA_InitStructure.DMA_Mode = DMA_Mode_Normal;// ʹ����ͨģʽ 
	DMA_InitStructure.DMA_Priority = DMA_Priority_VeryHigh;//������ȼ�
	DMA_InitStructure.DMA_FIFOMode = DMA_FIFOMode_Disable;         
	DMA_InitStructure.DMA_FIFOThreshold = DMA_FIFOThreshold_Full;
	DMA_InitStructure.DMA_MemoryBurst = DMA_MemoryBurst_Single;//�洢��ͻ�����δ���
	DMA_InitStructure.DMA_PeripheralBurst = DMA_PeripheralBurst_Single;//����ͻ�����δ���
	
	DMA_Init(DMA2_Stream3, &DMA_InitStructure);//��ʼ��DMA Stream
	SPI_I2S_DMACmd( SPI1, SPI_I2S_DMAReq_Tx, ENABLE);//ʹ��SPI1_TX DMA���� 

	
	///////////////////////////////////////////////////////////////////////
	////                     SPI1_RX DMA����                           ////
	///////////////////////////////////////////////////////////////////////
	
	RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_DMA2,ENABLE);//DMA2ʱ��ʹ�� 
	DMA_DeInit(DMA2_Stream2);
	
	while (DMA_GetCmdStatus(DMA2_Stream2) != DISABLE){}//�ȴ�DMA������ 
	
  /* ���� DMA Stream */
	DMA_InitStructure.DMA_Channel = DMA_Channel_3;  //ͨ��ѡ��
	DMA_InitStructure.DMA_PeripheralBaseAddr = (u32)&SPI1->DR;//DMA�����ַ
	DMA_InitStructure.DMA_Memory0BaseAddr = (u32)spi1_rx;//DMA �洢��0��ַ
	DMA_InitStructure.DMA_DIR = DMA_DIR_PeripheralToMemory;//���赽�洢��ģʽ
	DMA_InitStructure.DMA_BufferSize = 6;//���ݴ����� 
	DMA_InitStructure.DMA_PeripheralInc = DMA_PeripheralInc_Disable;//���������ģʽ
	DMA_InitStructure.DMA_MemoryInc = DMA_MemoryInc_Enable;//�洢������ģʽ
	DMA_InitStructure.DMA_PeripheralDataSize = DMA_PeripheralDataSize_Byte;//�������ݳ���:8λ
	DMA_InitStructure.DMA_MemoryDataSize = DMA_MemoryDataSize_Byte;//�洢�����ݳ���:8λ
	DMA_InitStructure.DMA_Mode = DMA_Mode_Normal;// ʹ����ͨģʽ 
	DMA_InitStructure.DMA_Priority = DMA_Priority_VeryHigh;//������ȼ�
	DMA_InitStructure.DMA_FIFOMode = DMA_FIFOMode_Disable;         
	DMA_InitStructure.DMA_FIFOThreshold = DMA_FIFOThreshold_Full;
	DMA_InitStructure.DMA_MemoryBurst = DMA_MemoryBurst_Single;//�洢��ͻ�����δ���
	DMA_InitStructure.DMA_PeripheralBurst = DMA_PeripheralBurst_Single;//����ͻ�����δ���
	
	DMA_ITConfig(DMA2_Stream2, DMA_IT_TC, ENABLE);//����DMA2_Stream2��������ж�
	DMA_Init(DMA2_Stream2, &DMA_InitStructure);//��ʼ��DMA Stream
	SPI_I2S_DMACmd( SPI1, SPI_I2S_DMAReq_Rx, ENABLE);//ʹ��SPI1_TX DMA����
	
	///////////////////////////////////////////////////////////////////////
	////                        NVIC����                               ////
	///////////////////////////////////////////////////////////////////////
	
	NVIC_InitStructure.NVIC_IRQChannel = DMA2_Stream2_IRQn;
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 1;
	NVIC_InitStructure.NVIC_IRQChannelSubPriority = 2;
	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;
	NVIC_Init(&NVIC_InitStructure);
	
}

/******************************************************************************/
/*                              DMA1��ʼ��                                    */
/*                   SPI1_TX--DMA2--CHANNEL0--STREAM4                         */
/*                   SPI1_RX--DMA2--CHANNEL0--STREAM3                         */
/******************************************************************************/

unsigned char spi2_rx[6], spi2_tx[6];
void dma1_init(void)
{
	DMA_InitTypeDef  DMA_InitStructure;
	NVIC_InitTypeDef   NVIC_InitStructure;
	
	///////////////////////////////////////////////////////////////////////
	////                     SPI2_TX DMA����                           ////
	///////////////////////////////////////////////////////////////////////
	
	RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_DMA1,ENABLE);//DMA1ʱ��ʹ�� 
	DMA_DeInit(DMA1_Stream4);
	
	while (DMA_GetCmdStatus(DMA1_Stream4) != DISABLE){}//�ȴ�DMA������ 
	
  /* ���� DMA Stream */
	DMA_InitStructure.DMA_Channel = DMA_Channel_0;  //ͨ��ѡ��
	DMA_InitStructure.DMA_PeripheralBaseAddr = (u32)&SPI2->DR;//DMA�����ַ
	DMA_InitStructure.DMA_Memory0BaseAddr = (u32)spi2_tx;//DMA �洢��0��ַ
	DMA_InitStructure.DMA_DIR = DMA_DIR_MemoryToPeripheral;//�洢��������ģʽ
	DMA_InitStructure.DMA_BufferSize = 6;//���ݴ����� 
	DMA_InitStructure.DMA_PeripheralInc = DMA_PeripheralInc_Disable;//���������ģʽ
	DMA_InitStructure.DMA_MemoryInc = DMA_MemoryInc_Enable;//�洢������ģʽ
	DMA_InitStructure.DMA_PeripheralDataSize = DMA_PeripheralDataSize_Byte;//�������ݳ���:8λ
	DMA_InitStructure.DMA_MemoryDataSize = DMA_MemoryDataSize_Byte;//�洢�����ݳ���:8λ
	DMA_InitStructure.DMA_Mode = DMA_Mode_Normal;// ʹ����ͨģʽ 
	DMA_InitStructure.DMA_Priority = DMA_Priority_VeryHigh;//������ȼ�
	DMA_InitStructure.DMA_FIFOMode = DMA_FIFOMode_Disable;         
	DMA_InitStructure.DMA_FIFOThreshold = DMA_FIFOThreshold_Full;
	DMA_InitStructure.DMA_MemoryBurst = DMA_MemoryBurst_Single;//�洢��ͻ�����δ���
	DMA_InitStructure.DMA_PeripheralBurst = DMA_PeripheralBurst_Single;//����ͻ�����δ���
	
	DMA_Init(DMA1_Stream4, &DMA_InitStructure);//��ʼ��DMA Stream
	SPI_I2S_DMACmd( SPI2, SPI_I2S_DMAReq_Tx, ENABLE);//ʹ��SPI2_TX DMA���� 

	
	///////////////////////////////////////////////////////////////////////
	////                     SPI2_RX DMA����                           ////
	///////////////////////////////////////////////////////////////////////
	
	RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_DMA1,ENABLE);//DMA2ʱ��ʹ�� 
	DMA_DeInit(DMA1_Stream3);
	
	while (DMA_GetCmdStatus(DMA1_Stream3) != DISABLE){}//�ȴ�DMA������ 
	
  /* ���� DMA Stream */
	DMA_InitStructure.DMA_Channel = DMA_Channel_0;  //ͨ��ѡ��
	DMA_InitStructure.DMA_PeripheralBaseAddr = (u32)&SPI2->DR;//DMA�����ַ
	DMA_InitStructure.DMA_Memory0BaseAddr = (u32)spi2_rx;//DMA �洢��0��ַ
	DMA_InitStructure.DMA_DIR = DMA_DIR_PeripheralToMemory;//���赽�洢��ģʽ
	DMA_InitStructure.DMA_BufferSize = 6;//���ݴ����� 
	DMA_InitStructure.DMA_PeripheralInc = DMA_PeripheralInc_Disable;//���������ģʽ
	DMA_InitStructure.DMA_MemoryInc = DMA_MemoryInc_Enable;//�洢������ģʽ
	DMA_InitStructure.DMA_PeripheralDataSize = DMA_PeripheralDataSize_Byte;//�������ݳ���:8λ
	DMA_InitStructure.DMA_MemoryDataSize = DMA_MemoryDataSize_Byte;//�洢�����ݳ���:8λ
	DMA_InitStructure.DMA_Mode = DMA_Mode_Normal;// ʹ����ͨģʽ 
	DMA_InitStructure.DMA_Priority = DMA_Priority_VeryHigh;//������ȼ�
	DMA_InitStructure.DMA_FIFOMode = DMA_FIFOMode_Disable;         
	DMA_InitStructure.DMA_FIFOThreshold = DMA_FIFOThreshold_Full;
	DMA_InitStructure.DMA_MemoryBurst = DMA_MemoryBurst_Single;//�洢��ͻ�����δ���
	DMA_InitStructure.DMA_PeripheralBurst = DMA_PeripheralBurst_Single;//����ͻ�����δ���
	
	DMA_ITConfig(DMA1_Stream3, DMA_IT_TC, ENABLE);//����DMA2_Stream2��������ж�
	DMA_Init(DMA1_Stream3, &DMA_InitStructure);//��ʼ��DMA Stream
	SPI_I2S_DMACmd( SPI2, SPI_I2S_DMAReq_Rx, ENABLE);//ʹ��SPI2_TX DMA����
	
	///////////////////////////////////////////////////////////////////////
	////                        NVIC����                               ////
	///////////////////////////////////////////////////////////////////////
	
	NVIC_InitStructure.NVIC_IRQChannel = DMA1_Stream3_IRQn;
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 1;
	NVIC_InitStructure.NVIC_IRQChannelSubPriority = 3;
	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;
	NVIC_Init(&NVIC_InitStructure);
	
}

/******************************************************************************/
/*                        SPI1 TX DMA����һ��                                 */
/******************************************************************************/

void spi1_tx_dma_transfer_once(void)
{
	DMA2->LIFCR=(1<<27);//���TCIF3��־
	DMA2_Stream3->CR|=1<<0; //����DMA����
}

/******************************************************************************/
/*                       SPI1 RX DMA����һ��                                  */
/******************************************************************************/

void spi1_rx_dma_transfer_once(void)
{
	DMA2->LIFCR=(1<<21);//���TCIF2��־
	DMA2_Stream2->CR|=1<<0; //����DMA����
}

/******************************************************************************/
/*                        SPI2 TX DMA����һ��                                 */
/******************************************************************************/

void spi2_tx_dma_transfer_once(void)
{
	DMA1->HIFCR=(1<<5);//���TCIF4��־
	DMA1_Stream4->CR|=1<<0; //����DMA����
}

/******************************************************************************/
/*                       SPI2 RX DMA����һ��                                  */
/******************************************************************************/

void spi2_rx_dma_transfer_once(void)
{
	DMA1->LIFCR=(1<<27);//���TCIF3��־
	DMA1_Stream3->CR|=1<<0; //����DMA����
}
