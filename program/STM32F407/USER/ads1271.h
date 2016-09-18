#ifndef __ADS1271_H
#define __ADS1271_H
#include "sys.h"

void ads1271_init(char mode);
void ads1271_mode(char mode);
void ads1271_clk_scaler(char scaler);
void sync_config(void);
void pdwn_calibration(void);
void sync(void);

#endif
