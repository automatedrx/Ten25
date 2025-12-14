#ifndef AUDIO_H
#define AUDIO_H

#include "main.h"
#include "arm_math.h"
#include <stdbool.h>

#define AUDIO_BUFFER_SIZE 1024  // Full interleaved buffer size (512 left + 512 right samples)
#define NUM_FREQ_BANDS 8

typedef struct {
    int32_t audio_buffer_interleaved[AUDIO_BUFFER_SIZE];  // Full DMA buffer (L/R interleaved)
    int32_t audio_buffer_left[AUDIO_BUFFER_SIZE / 2];
    int32_t audio_buffer_right[AUDIO_BUFFER_SIZE / 2];

    uint8_t audioValTotal;   // 0-100 overall level (averaged)
    uint8_t audioValLeft;    // 0-100 left channel level
    uint8_t audioValRight;   // 0-100 right channel level
    uint8_t freqBandVal[NUM_FREQ_BANDS];  // Final band levels

    DFSDM_Filter_HandleTypeDef* hfilter;
    volatile bool bufferFull;
    bool enabled;
} Audio_HandleTypeDef;

void AudioInit(Audio_HandleTypeDef* haudio, DFSDM_Filter_HandleTypeDef* hfilter);
void AudioEnable(Audio_HandleTypeDef* haudio, bool enabled);
void AudioCheck(Audio_HandleTypeDef* haudio);  // Call from main loop
void AudioProcessBuffer(Audio_HandleTypeDef* haudio);

#endif
