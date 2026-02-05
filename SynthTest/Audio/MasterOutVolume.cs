using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Audio
{
    // https://learn.microsoft.com/fr-fr/dotnet/csharp/language-reference/keywords/sealed
    // Ce module fait le pont avec la carte son via ISampleProvider
    public sealed class MasterOutModule : ISampleProvider, IDisposable, INotifyPropertyChanged
    {
        #region PropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private readonly WaveOutEvent _outputDevice;

        // Le module branche (le VCO ou un Mixer quoi)
        // Ici on attend un CABLE ou un MODULE direct
        public ISignalSource MainInput { get; set; }

        public WaveFormat WaveFormat { get; } = WaveFormat.CreateIeeeFloatWaveFormat(44100, 1);
        
        private LinearRamp _volumeRamp;
        private float _volume = 0.2f;
        public float Volume
        {
            get => _volume;
            set 
            {
                _volume = value;
                _volumeRamp.Value = _volume;
                NotifyPropertyChanged(); 
            }
        }
        private bool _isMuted;
        public bool IsMuted
        {
            get => _isMuted;
            set 
            { 
                _isMuted = value; 
                NotifyPropertyChanged(); 
            }
        }

        public MasterOutModule()
        {
            // POINT IMPORTANT ! On gère la lattence de 
            _outputDevice = new WaveOutEvent { DesiredLatency = 100 };
            _outputDevice.Init(this); // On demande a NAudio de lire ce module

            _volumeRamp = new LinearRamp(44100, 0.05f);
            _volumeRamp.Value = _volume;
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

            if(Volume == 1f) return count; // Si le volume est à 1, on peut skip la multiplication pour gagner du temps en saaaaaaaah (on met jamais a 1 ça pète les oreils)

            for (int n = 0; n < count; n++)
            {
                float smoothedVolume = _volumeRamp.Next();
                // l'offset est facultatif mais le read le demande de base, j'ai pas encore trouver le cas ou l'offset est different de 0 mais bon
                buffer[offset + n] *= smoothedVolume; // On * par le volume
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
