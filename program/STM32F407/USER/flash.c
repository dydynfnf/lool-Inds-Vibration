#include "flash.h"

/******************************************************************************/
/*                                写flash                                     */
/*                        *pBuffer指向写入的数组                              */
/*                        ByteToWrite写入多少字节                             */
/*                          失败返回0 成功返回1                               */
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
	FLASH_Unlock();									//解锁 
  FLASH_DataCacheCmd(DISABLE);//FLASH擦除期间,必须禁止数据缓存
	status=FLASH_EraseSector(FLASH_Sector_11,VoltageRange_3);//VCC=2.7~3.6V之间,擦除数据
	if(status!=FLASH_COMPLETE)
		return 0;      //返回，操作失败
	while(AddressNum--)
	{
		if(FLASH_ProgramWord(WriteAddr,*pBuffer)!=FLASH_COMPLETE)
			return 0;
		WriteAddr+=4;
		pBuffer++;
	}
	FLASH_DataCacheCmd(ENABLE);	//FLASH擦除结束,开启数据缓存
	FLASH_Lock();//上锁
	return 1;
}

/******************************************************************************/
/*                                读flash                                     */
/*                        *pBuffer指向读入的数组                              */
/*                        ByteToWrite读入多少字节                             */
/*                          失败返回0 成功返回1                               */
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
/*                              设备配置参数                                  */
/******************************************************************************/

struct Device_Config device_config;

/******************************************************************************/
/*                           保存设备配置参数                                 */
/******************************************************************************/

void save_device_config(void)
{
	FlashWrite((u32 *)&device_config, 100);
}

/******************************************************************************/
/*                           读取设备配置参数                                 */
/******************************************************************************/

void read_device_config(void)
{
	FlashRead((u32 *)&device_config, 100);
}
