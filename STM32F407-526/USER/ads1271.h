#ifndef __ADS1271_H
#define __ADS1271_H
#include "sys.h"

void ads1271_init(void);
void sync_config(void);
void pdwn_calibration(void);
void sync(void);
void spi1_tx_dma_transfer_once(void);
void spi1_rx_dma_transfer_once(void);

#endif
