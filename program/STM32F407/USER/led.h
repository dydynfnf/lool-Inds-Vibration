#ifndef __LED_H
#define __LED_H
#include "sys.h"

void led_init(void);
void led_sample(unsigned char status);
void led_link(unsigned char status);
void led_current(unsigned char status);
void led_sample_toggle(void);
void led_link_toggle(void);
void led_current_toggle(void);

#endif
