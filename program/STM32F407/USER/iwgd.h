#ifndef __IWGD_H
#define __IWGD_H

#include "sys.h"

void IWDG_Init(u8 prer,u16 rlr);
void IWDG_Feed(void);
void sys_restart(void);

#endif
