using NAudio.Wave;
using SynthTest.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Infrastructure.AudioDrivers
{
    public class NAudioEngine : ISampleProvider, IDisposable
    {
        private readonly WaveOutEvent _waveOut;
        private readonly AudioContext _context;

        // Ce qu'on écoute (Le Master)
        public IAudioNode InputNode { get; set; }

        public WaveFormat WaveFormat { get; }

        private float _TempVolume = 0.2f;

        public NAudioEngine(int sampleRate = 44100)
        {
            AudioConfig.SampleRate = sampleRate;
            _context = new AudioContext { SampleRate = sampleRate };
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, 1);

            _waveOut = new WaveOutEvent { DesiredLatency = 50 };
            _waveOut.Init(this); // "this" is an ISampleProvider
            // Go to https://github.com/naudio/NAudio/blob/master/Docs/WaveProviders.md
        }

        // Play and Stop control is more the ON/OFF of our modular synthesizer
        public void Play() => _waveOut.Play();
        public void Stop() => _waveOut.Stop();

        // HERE IS THE BASE OF ALL THE AUDIO CHAIN: This method is called by NAudio every time.
        // Our role is to fill the buffer with the audio data from our node chain (starting from InputNode)
        public int Read(float[] buffer, int offset, int count)
        {
            if (InputNode != null)
            {
                // Get the audio data from the node chain who are plugged in InputNode
                InputNode.ProcessBlock(buffer, offset, count, _context);
            }
            else
            {
                Array.Clear(buffer, offset, count);
            }

            for (int i = 0; i < count; i++)
            {
                buffer[offset + i] *= _TempVolume; // This is temp volume control, we'll add a audio Node later
            }

            return count;
        }

        public void Dispose()
        {
            _waveOut?.Dispose();
        }
    }
}
