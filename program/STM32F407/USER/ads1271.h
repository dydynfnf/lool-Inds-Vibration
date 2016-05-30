#ifndef __ADS1271_H
#define __ADS1271_H
#include "sys.h"

void ads1271_init(void);
void sync_config(void);
void pdwn_calibration(void);
void sync(void);

#endif
