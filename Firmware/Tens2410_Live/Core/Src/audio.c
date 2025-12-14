#include "audio.h"

static arm_rfft_fast_instance_f32 fft_handler;
static float_t fft_input[AUDIO_BUFFER_SIZE / 2];
static float_t smoothed_bands[NUM_FREQ_BANDS] = {0};

//const uint16_t band_edges[NUM_FREQ_BANDS + 1] = {0, 100, 300, 800, 1500, 3000, 5000, 7000, 8000};
uint16_t band_edges[NUM_FREQ_BANDS + 1];

#define SMOOTHING_ALPHA 0.2f
#define RMS_DIVISOR     1500000U  // Tune this based on testing

// Per-band gain multipliers (tune these — higher = boost that band)
static const float_t band_gain[NUM_FREQ_BANDS] = {
    1.2f,//1.0f,   // Band 0 (lowest) — usually needs least boost
    0.9f,//1.2f,   // Band 1
    1.5f,   // Band 2
    2.0f,   // Band 3
    4.5f,//3.0f,   // Band 4
    7.0f,//5.0f,   // Band 5 (mids/highs — big boost)
    9.0f,//8.0f,   // Band 6
    12.0f   // Band 7 (highest — max boost for visibility)
};

void AudioInit(Audio_HandleTypeDef* haudio, DFSDM_Filter_HandleTypeDef* hfilter) {
    haudio->hfilter = hfilter;
    haudio->bufferFull = false;
    haudio->enabled = false;

    // Dynamically compute log-spaced band edges (min 50 Hz to Nyquist ~8000 Hz)
    const uint16_t min_freq = 50;   // Skip <50 Hz noise/DC
    const uint16_t max_freq = 6000; // Nyquist half (adjust if sample_rate changes)

    float_t log_start = logf((float_t)min_freq);
    float_t log_end = logf((float_t)max_freq);
    float_t log_step = (log_end - log_start) / (float_t)NUM_FREQ_BANDS;

    band_edges[0] = min_freq;
    for (uint8_t i = 1; i <= NUM_FREQ_BANDS; i++) {
        band_edges[i] = (uint16_t)expf(log_start + (float_t)i * log_step);
    }


    arm_rfft_fast_init_f32(&fft_handler, AUDIO_BUFFER_SIZE / 2);

    // Start DMA for interleaved stereo output
    HAL_DFSDM_FilterRegularStart_DMA(hfilter, (int32_t*)haudio->audio_buffer_interleaved, AUDIO_BUFFER_SIZE);
    haudio->enabled = true;
}

void AudioEnable(Audio_HandleTypeDef* haudio, bool enabled) {
    haudio->enabled = enabled;
    if (enabled) {
        HAL_DFSDM_FilterRegularStart_DMA(haudio->hfilter, (int32_t*)haudio->audio_buffer_interleaved, AUDIO_BUFFER_SIZE);
    } else {
        HAL_DFSDM_FilterRegularStop_DMA(haudio->hfilter);
        haudio->bufferFull = false;
    }
}

// Callback — keep this in audio.c
//void HAL_DFSDM_FilterRegConvCpltCallback(DFSDM_Filter_HandleTypeDef *hdfsdm_filter) {
//    if (hdfsdm_filter == &hdfsdm1_filter0) {
//        haud.bufferFull = true;  // haud is your global Audio_HandleTypeDef in main.c
//    }
//}

void AudioCheck(Audio_HandleTypeDef* haudio) {
    if (haudio->bufferFull && haudio->enabled) {
        AudioProcessBuffer(haudio);
        haudio->bufferFull = false;
    }
}

void AudioProcessBuffer(Audio_HandleTypeDef* haudio) {
    // 1. De-interleave stereo (already good)
    for (uint32_t i = 0; i < AUDIO_BUFFER_SIZE / 2; i++) {
        haudio->audio_buffer_left[i]  = haudio->audio_buffer_interleaved[2 * i];
        haudio->audio_buffer_right[i] = haudio->audio_buffer_interleaved[2 * i + 1];
    }

    // 2. Normalize left/right to [-1.0, 1.0] for RMS (fix for 0 values)
    float_t norm_left[AUDIO_BUFFER_SIZE / 2];
    float_t norm_right[AUDIO_BUFFER_SIZE / 2];
    for (uint32_t i = 0; i < AUDIO_BUFFER_SIZE / 2; i++) {
        norm_left[i] = (float_t)haudio->audio_buffer_left[i] / 8388608.0f;  // 2^23 max
        norm_right[i] = (float_t)haudio->audio_buffer_right[i] / 8388608.0f;
    }

    // Compute RMS on normalized floats
    float_t rms_left_f = 0.0f, rms_right_f = 0.0f;
    arm_rms_f32(norm_left, AUDIO_BUFFER_SIZE / 2, &rms_left_f);
    arm_rms_f32(norm_right, AUDIO_BUFFER_SIZE / 2, &rms_right_f);

    float_t rms_avg_f = (rms_left_f + rms_right_f) / 2.0f;

    // Scale to 0-100 (no need for int rescale — rms_f is already 0-1 ish)
    haudio->audioValLeft  = (uint8_t)(rms_left_f * 100.0f * 5.0f);  // Multiplier boost for sensitivity
    haudio->audioValRight = (uint8_t)(rms_right_f * 100.0f * 5.0f);
    haudio->audioValTotal = (uint8_t)(rms_avg_f * 100.0f * 5.0f);   // Tune *5.0f up/down based on max yell

    haudio->audioValLeft  = (haudio->audioValLeft  > 100) ? 100 : haudio->audioValLeft;
    haudio->audioValRight = (haudio->audioValRight > 100) ? 100 : haudio->audioValRight;
    haudio->audioValTotal = (haudio->audioValTotal > 100) ? 100 : haudio->audioValTotal;

    // 3. Prepare averaged input for FFT (already normalized here too)
    for (uint32_t i = 0; i < AUDIO_BUFFER_SIZE / 2; i++) {
        float_t avg = (norm_left[i] + norm_right[i]) / 2.0f;
        fft_input[i] = avg;
    }

    // 4. FFT
    float_t fft_output[AUDIO_BUFFER_SIZE];
    arm_rfft_fast_f32(&fft_handler, fft_input, fft_output, 0);
    float_t magnitude_sq[AUDIO_BUFFER_SIZE / 2];
    arm_cmplx_mag_squared_f32(fft_output, magnitude_sq, AUDIO_BUFFER_SIZE / 2);

    // 5. Bin to bands
    float_t band_energy[NUM_FREQ_BANDS] = {0};
    uint32_t bin_width = 25000 / AUDIO_BUFFER_SIZE;  // Adjust if sample rate differs

    for (uint32_t band = 0; band < NUM_FREQ_BANDS; band++) {
        uint32_t start = band_edges[band] / bin_width;
        uint32_t end   = band_edges[band + 1] / bin_width;
        uint32_t bins  = end - start;
        if (bins == 0) continue;

        float_t sum = 0.0f;
        for (uint32_t b = start; b < end; b++) sum += magnitude_sq[b];
        band_energy[band] = sum / (float_t)bins;
    }

    // 6. Per-band normalization
    for (uint32_t b = 0; b < NUM_FREQ_BANDS; b++) {
    	float_t energy = band_energy[b] + 0.0001f;
    	float_t norm = sqrtf(energy) * 15.0f * band_gain[b];  // <-- Add * band_gain[b]
    	norm = (norm > 100.0f) ? 100.0f : norm;

    	smoothed_bands[b] = SMOOTHING_ALPHA * smoothed_bands[b] + (1.0f - SMOOTHING_ALPHA) * norm;
    	haudio->freqBandVal[b] = (uint8_t)smoothed_bands[b];
    }
}
