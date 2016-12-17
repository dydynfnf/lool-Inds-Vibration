#ifndef __FLASH_H
#define __FLASH_H

#include "sys.h"

struct Device_Config //�̶����� 100byte
{
	u8 sip[4];//ip��ַ
	u8 sub[4];//��������
	u8 ga[4];//����
		
	short drift1;//ͨ��1��Ưϵ��
	short drift2;//ͨ��1��Ưϵ��
	short drift3;//ͨ��1��Ưϵ��
	short drift4;//ͨ��1��Ưϵ��
	
	u8 cur;//����Դ
	
	u8 res[79];//Ԥ��
};

u8 FlashWrite(u32 *pBuffer,u32 ByteToWrite);
void FlashRead(u32 *pBuffer,u32 NumToWrite);
void save_device_config(void);
void read_device_config(void);


#endif
