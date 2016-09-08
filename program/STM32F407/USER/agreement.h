#ifndef _PWM_H
#define _PWM_H
#include "sys.h"
#include "timer.h"
#include "sram.h"
#include "ethernet.h"

short calculate_drift(u8 channel, u8 precision);
unsigned char deal_config_command(void);
unsigned char deal_int(void);
unsigned char deal_pre(void);
unsigned char deal_div(void);
unsigned char deal_sta(void);
unsigned char deal_data(void);
	
#endif
