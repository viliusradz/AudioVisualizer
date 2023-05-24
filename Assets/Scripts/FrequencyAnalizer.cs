using CSCore.SoundIn;
using CSCore.Streams;
using CSCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using CSCore.CoreAudioAPI;
using System.Text.RegularExpressions;
using System;
using UnityEngine.Windows.WebCam;
using CSCore.Utils;
using CSCore.DSP;
using System.Diagnostics.Tracing;
using System.Linq;
//using System.Numerics;
//using Complex = System.Numerics.Complex;
// AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Blackman);
//https://github.com/filoe/cscore
//^very powerfull audio libruary
//https://github.com/hallidev/UnityWASAPILoopbackAudio
//^project with the libruary


public class FrequencyAnalizer : MonoBehaviour
{
    public bool fftApplied = true;
    [Header("Filtering")]
    public bool fftFiltering = false;
    public float lowShelfGain = 1f;
    public float bandWidth = 1f;
    public float filterFrequency = 0.1f;
    public float fftScaleCoefficient = 100;


    public bool noFFTRepeats = true;



    public static FrequencyAnalizer inst;
    public UnityEvent<float[]> soundSamples = new UnityEvent<float[]>();
    private ISampleSource sampleSource;
    private SoundInSource soundSource;
    private WasapiLoopbackCapture soundIn;
    [HideInInspector]
    public int buffSize;

    private Complex[] data;
    private float[] dataFloat;
    private int sampleExpo = 10;

    private int samplesDevideBy = 4;
    int index = 0;
    int lastBuffer = 0;

    private void Awake()
    {
        if (inst == null)
            inst = this;
        else
            this.enabled = false;
        soundIn = new();
        soundIn.Initialize();
        soundSource = new(soundIn);

        sampleSource = soundSource.ToSampleSource();

        buffSize = (int)Math.Floor(Math.Log(sampleSource.WaveFormat.SampleRate * sampleSource.WaveFormat.Channels, 2));
        buffSize =(int) Math.Pow(2, buffSize);
        if (noFFTRepeats)
            buffSize /= samplesDevideBy;

        soundIn.Start();

        Application.targetFrameRate = 60;
    }
    private void Update()
    {
        sampleExpo = (int)Math.Floor(Math.Log(sampleSource.WaveFormat.SampleRate * sampleSource.WaveFormat.Channels, 2));
        var buffer = new float[(int)MathF.Pow(2, sampleExpo)];
        //var buffer = new float[sampleSource.WaveFormat.SampleRate * sampleSource.WaveFormat.Channels];
        if (buffer.Length != lastBuffer)
        {
            data = new Complex[(int)Math.Pow(2, sampleExpo)];
            dataFloat = new float[data.Length];
        }
        lastBuffer = buffer.Length;


        sampleSource.Read(buffer, 0, buffer.Length);

        if (fftApplied)
        {
            ApplyFFT(buffer);
            soundSamples.Invoke(dataFloat);
        }
        else
        {
            soundSamples.Invoke(buffer);
        }
    }

    private void OnDestroy()
    {
        soundIn.Stop();
        soundSource.Dispose();
        sampleSource.Dispose();
        soundIn.Dispose();
    }

    private void ApplyFFT(float[] buffer)
    {

        index = 0;
        //foreach (var val in buffer)
        //{
        //    data[index].Real = val;
        //    index++;
        //}
        for (int i = 0; i < data.Length; i++)
        {
            data[i].Real = buffer[i];
        }
        FastFourierTransformation.Fft(data, sampleExpo);
        if (noFFTRepeats)
        {
            for (int i = 0; i < data.Length / samplesDevideBy; i++)
            {
                dataFloat[i] = data[i] * fftScaleCoefficient;
            }
        }
        else
        {
            for (int i = 0; i < data.Length; i++)
            {
                dataFloat[i] = data[i] * fftScaleCoefficient;
            }
        }
        if (fftFiltering)
        {
            //FilterData(dataFloat, sampleSource.WaveFormat.SampleRate, filterFrequency);
            //FilterData1(dataFloat, sampleSource.WaveFormat.SampleRate, filterFrequency, lowShelfGain);
            //FilterData2(dataFloat, (int)Math.Pow(2, sampleExpo), filterFrequency, bandWidth, lowShelfGain);
            //FilterData3(dataFloat, sampleSource.WaveFormat.SampleRate, filterFrequency);
            //FilterData4(dataFloat, (int)Math.Pow(2, sampleExpo), filterFrequency, lowShelfGain);
        }
    }
    private static void FilterData(float[] data,int sampleRate, float frequency)
    {
        LowpassFilter filter = new LowpassFilter(sampleRate, (double)frequency);
        filter.Process(data);
    }    
    private static void FilterData1(float[] data,int sampleRate, float frequency, float gain)
    {
        LowShelfFilter filter = new LowShelfFilter(sampleRate, (double)frequency,(double) gain);
        filter.Process(data);
    }    
    private static void FilterData2(float[] data,int sampleRate, float frequency,float bandwidth, float gain)
    {
        PeakFilter filter = new PeakFilter(sampleRate, (double)frequency,(double) bandwidth,(double) gain);
        filter.Process(data);
    }    
    private static void FilterData3(float[] data,int sampleRate, float frequency)
    {
        NotchFilter filter = new NotchFilter(sampleRate, (double)frequency);
        filter.Process(data);
    }
    private static void FilterData4(float[] data, int sampleRate, float frequency, float gain)
    {
        HighShelfFilter filter = new HighShelfFilter(sampleRate, (double)frequency, (double)gain);
        filter.Process(data);
    }

    //public static Complex[] FFT(float[] signal)
    //{
    //    int n = signal.Length;

    //    // convert signal to complex numbers
    //    Complex[] signalComplex = new Complex[n];
    //    for (int i = 0; i < n; i++)
    //    {
    //        signalComplex[i] = new Complex(signal[i], 0);
    //    }

    //    // calculate FFT
    //    Complex[] spectrum = FastFourierTransformation.Fft(signalComplex);

    //    return spectrum;
    //}

    //public static int BitReverse(int n, int bits)
    //{
    //    int reversedN = n;
    //    int count = bits - 1;

    //    n >>= 1;
    //    while (n > 0)
    //    {
    //        reversedN = (reversedN << 1) | (n & 1);
    //        count--;
    //        n >>= 1;
    //    }

    //    return ((reversedN << count) & ((1 << bits) - 1));
    //}

    ///* Uses Cooley-Tukey iterative in-place algorithm with radix-2 DIT case
    // * assumes no of points provided are a power of 2 */
    //public static void FFT(Complex[] buffer)
    //{

    //    int bits = (int)Math.Log(buffer.Length, 2);
    //    for (int j = 1; j < buffer.Length / 2; j++)
    //    {

    //        int swapPos = BitReverse(j, bits);
    //        var temp = buffer[j];
    //        buffer[j] = buffer[swapPos];
    //        buffer[swapPos] = temp;
    //    }

    //    for (int N = 2; N <= buffer.Length; N <<= 1)
    //    {
    //        for (int i = 0; i < buffer.Length; i += N)
    //        {
    //            for (int k = 0; k < N / 2; k++)
    //            {

    //                int evenIndex = i + k;
    //                int oddIndex = i + k + (N / 2);
    //                var even = buffer[evenIndex];
    //                var odd = buffer[oddIndex];

    //                double term = -2 * Math.PI * k / (double)N;
    //                Complex exp = new Complex(Math.Cos(term), Math.Sin(term)) * odd;

    //                buffer[evenIndex] = even + exp;
    //                buffer[oddIndex] = even - exp;
    //            }
    //        }
    //    }
    //}

    //public static float GetIntensity(Complex c)
    //{
    //    return (float)Math.Sqrt(c.Real * c.Real + c.Imaginary * c.Imaginary);
    //}

    //public static float HammingWindow(int n, int N)
    //{

    //    const float alpha = 0.54f;
    //    const float beta = 0.46f;

    //    return alpha - beta * (float)Math.Cos((2 * Math.PI * n) / (N - 1));
    //}

    //public static void FFt(Complex[] data, int exponent, FftMode mode = FftMode.Forward)
    //{
    //    int c = (int)Math.Pow(2, exponent);

    //    Inverse(data, c);

    //    int j0, j1, j2 = 1;

    //    float n0, n1, tr, ti, m;
    //    float v0 = -1, v1 = 0;

    //    int j, i;

    //    for (int l = 0; l < exponent; l++)
    //    {
    //        n0 = 1;
    //        n1 = 0;
    //        j1 = j2;
    //        j2 <<= 1;

    //        for (j = 0; j < j1; j++)
    //        {
    //            for (i = j; i < c; i += j2)
    //            {
    //                j0 = i + j1;
    //                //--
    //                tr = n0 * data[j0].Real - n1 * data[j0].Imaginary;
    //                ti = n0 * data[j0].Imaginary + n1 * data[j0].Real;
    //                //--
    //                data[j0].Real = data[i].Real - tr;
    //                data[j0].Imaginary = data[i].Imaginary - ti;
    //                //add
    //                data[i].Real += tr;
    //                data[i].Imaginary += ti;
    //            }

    //            m = v0 * n0 - v1 * n1;
    //            n1 = v1 * n0 + v0 * n1;
    //            n0 = m;
    //        }
    //        if (mode == FftMode.Forward)
    //        {
    //            v1 = (float)Math.Sqrt((1f - v0) / 2f);
    //        }
    //        else
    //        {
    //            v1 = (float)-Math.Sqrt((1f - v0) / 2f);
    //        }
    //        v0 = (float)Math.Sqrt((1f + v0) / 2f);
    //    }

    //    if (mode == FftMode.Forward)
    //    {
    //        Forward(data, c);
    //    }
    //}

    //public static void Forward(Complex[] data, int count)
    //{
    //    int length = count;
    //    for (int i = 0; i < length; i++)
    //    {
    //        data[i].Real /= length;
    //        data[i].Imaginary /= length;
    //    }
    //}

    //public static void Inverse(Complex[] data, int c)
    //{
    //    int z = 0;
    //    int n1 = c >> 1;

    //    for (int n0 = 0; n0 < c - 1; n0++)
    //    {
    //        if (n0 < z)
    //        {
    //            Swap(data, n0, z);
    //        }
    //        int l = n1;

    //        while (l < z)
    //        {
    //            z = z - 1;
    //            l >>= 1;
    //        }
    //        z += 1;
    //    }


    //}

    //public static void Swap(Complex[] data, int index, int index2)
    //{
    //    Complex tmp = data[index];
    //    data[index] = data[index2];
    //    data[index2] = tmp;
    //}







    //private WasapiCapture capture;
    //private IWaveSource source;
    //private BufferSource bSource;
    //private byte[] buffer;

    // Create a new WasapiCapture object with default capture device
    //capture = new();
    //capture.Initialize();


    //bSource = new(source, 32);
    //buffer = new byte[bSource.Length];

    // Start the capture and begin recording audio
    //capture.Start();

    // Subscribe to the DataAvailable event to receive audio data in real-time
    //bSource.Read(buffer,3, 10);
    //print(buffer[1]);
    //capture.DataAvailable += (obj, args) =>
    //{
    //    print(args.Data[0]);
    //};



    // Stop the capture and release resources when the script is destroyed
    //capture.Stop();
    //bSource.Dispose();
    //capture.Dispose();
    //source.Dispose();
}
