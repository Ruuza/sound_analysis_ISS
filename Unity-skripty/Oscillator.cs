using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour
{

    [SerializeField]
    private float frequency = 440.0f;
    public float Frequency
    {
        get { return frequency; }
        set { frequency = value; }
    }

    private float volume = 0;
    public float Volume
    {
        get { return volume; }
        set
        {
            if (value > 0.9)
                volume = 0.9f;
            else
                volume = value;
        }
    }

    public float gain;

    private float f_sampling;
    private double increment;
    private double phase;

    [SerializeField]
    private float deviation;
    [SerializeField]
    private float mean;

    System.Random rand = new System.Random();

    private void Start()
    {
        f_sampling = AudioSettings.outputSampleRate;
    }



    private void OnAudioFilterRead(float[] data, int channels)
    {
        increment = frequency * 2 * Mathf.PI / f_sampling;

        for (int i = 0; i < data.Length; i += channels)
        {
            phase += increment;
            data[i] = Mathf.Sin((float)phase);

            // OPTION 1: SQUARE ONLY
            if (data[i] >= 0)
            {
                data[i] = volume * 0.6f;
            }
            else
            {
                data[i] = -volume * 0.6f;
            }

            for (int ch = 1; ch < channels; ch++)
            {
                data[i + ch] = data[i];
            }

            if (phase > (Mathf.PI * 2))
            {
                phase -= Mathf.PI * 2;
            }
        }
    }

    public float RandomGaussian(float minValue = 0.0f, float maxValue = 1.0f)
    {
        float u, v, S;

        do
        {
            u = 2.0f * (float)rand.NextDouble() - 1.0f;
            v = 2.0f * (float)rand.NextDouble() - 1.0f;
            S = u * u + v * v;
        }
        while (S >= 1.0f);

        // Standard Normal Distribution
        float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);

        // Normal Distribution centered between the min and max value
        // and clamped following the "three-sigma rule"
        float mean = (minValue + maxValue) / 2.0f;
        float sigma = (maxValue - mean) / 3.0f;
        return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
    }
}
