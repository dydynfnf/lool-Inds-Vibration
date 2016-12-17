#ifndef __FLASH_H
#define __FLASH_H

#include "sys.h"

struct Device_Config //固定长度 100byte
{
	u8 sip[4];//ip地址
	u8 sub[4];//子网掩码
	u8 ga[4];//网关
		
	short drift1;//通道1零漂系数
	short drift2;//通道1零漂系数
	short drift3;//通道1零漂系数
	short drift4;//通道1零漂系数
	
	u8 cur;//恒流源
	
	u8 res[79];//预留
};

u8 FlashWrite(u32 *pBuffer,u32 ByteToWrite);
void FlashRead(u32 *pBuffer,u32 NumToWrite);
void save_device_config(void);
void read_device_config(void);


#endif
