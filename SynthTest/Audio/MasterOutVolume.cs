using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Audio
{
    // Ce module fait le pont avec la carte son via ISampleProvider
    public sealed class MasterOutModule : ISampleProvider, IDisposable
    {
        private readonly WaveOutEvent _outputDevice;

        // Le module branche (le VCO ou un Mixer quoi)
        public ISignalSource MainInput { get; set; }

        public WaveFormat WaveFormat { get; } = WaveFormat.CreateIeeeFloatWaveFormat(44100, 1);

        public float Volume { get; set; } = 0.2f;
        public bool IsMuted { get; set; }

        public MasterOutModule()
        {
            // POINT IMPORTANT ! On gère la lattence de 
            _outputDevice = new WaveOutEvent { DesiredLatency = 100 };
            _outputDevice.Init(this); // On demande a NAudio de lire ce module
        }

        // Appel automatiquement par NAudio (j'ai pas trop compris comment et pourquoi dans la doc)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int Read(float[] buffer, int offset, int count)
        {
            //Array.Clear(buffer, offset, count);

            //if (IsMuted || MainInput == null)
            //{
            //    return count;
            //}

            // On demande au VCO de remplir le buffer
            MainInput.Generate(buffer, count, WaveFormat.SampleRate);

            // On applique le volume master
            for (int n = 0; n < count; n++)
            {
                // l'offset est facultatif mais le read le demande de base, j'ai pas encore trouver le cas ou l'offset est different de 0 mais bon
                buffer[offset + n] *= Volume; // On * par le volume
            }

            //Trace.WriteLine($"we send {count}");

            return count;
        }

        // Process de la sortie
        public void Play() => _outputDevice.Play();
        public void Stop() => _outputDevice.Stop();
        public void Dispose() => _outputDevice.Dispose();
    }
}
