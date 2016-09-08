#include "flash.h"

/******************************************************************************/
/*                                дflash                                     */
/*                        *pBufferָ��д�������                              */
/*                        ByteToWriteд������ֽ�                             */
/*                          ʧ�ܷ���0 �ɹ�����1                               */
/******************************************************************************/

u8 FlashWrite(u32 *pBuffer,u32 ByteToWrite)
{
	u32 AddressNum;
	u32 WriteAddr=0x080E0000;
	FLASH_Status status = FLASH_COMPLETE;
	if(ByteToWrite%4==0)
		AddressNum=ByteToWrite/4;
	else
		AddressNum=ByteToWrite/4+1;
	FLASH_Unlock();									//���� 
  FLASH_DataCacheCmd(DISABLE);//FLASH�����ڼ�,�����ֹ���ݻ���
	status=FLASH_EraseSector(FLASH_Sector_11,VoltageRange_3);//VCC=2.7~3.6V֮��,��������
	if(status!=FLASH_COMPLETE)
		return 0;      //���أ�����ʧ��
	while(AddressNum--)
	{
		if(FLASH_ProgramWord(WriteAddr,*pBuffer)!=FLASH_COMPLETE)
			return 0;
		WriteAddr+=4;
		pBuffer++;
	}
	FLASH_DataCacheCmd(ENABLE);	//FLASH��������,�������ݻ���
	FLASH_Lock();//����
	return 1;
}

/******************************************************************************/
/*                                ��flash                                     */
/*                        *pBufferָ����������                              */
/*                        ByteToWrite��������ֽ�                             */
/*                          ʧ�ܷ���0 �ɹ�����1                               */
/******************************************************************************/

void FlashRead(u32 *pBuffer,u32 NumToWrite)
{
	u32 i;
	u32 WriteAddr=0x080E0000;
	for(i=0;i<NumToWrite;i++)
	{
		*pBuffer =*(vu32*)WriteAddr;
		pBuffer+=1;
		WriteAddr+=4;
	}
}

/******************************************************************************/
/*                              �豸���ò���                                  */
/******************************************************************************/

struct Device_Config device_config;

/******************************************************************************/
/*                           �����豸���ò���                                 */
/******************************************************************************/

void save_device_config(void)
{
	FlashWrite((u32 *)&device_config, 100);
}

/******************************************************************************/
/*                           ��ȡ�豸���ò���                                 */
/******************************************************************************/

void read_device_config(void)
{
	FlashRead((u32 *)&device_config, 100);
}
