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
    public class MixerModule: INotifyPropertyChanged
    {
        #region PropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        // Liste des entrés/sorties du mixer. 
        // l'output qui en général sera branché vers le masterOut
        public AudioOutput Output { get; }

        public AudioInput Input1 { get; } = new AudioInput();
        public AudioInput Input2 { get; } = new AudioInput();
        public AudioInput Input3 { get; } = new AudioInput();
        public AudioInput Input4 { get; } = new AudioInput();

        // Slider volume Get Set
        private float _vol1 = 1.0f;
        public float Vol1 { get => _vol1; set { _vol1 = value; NotifyPropertyChanged(); } }

        private float _vol2 = 1.0f;
        public float Vol2 { get => _vol2; set { _vol2 = value; NotifyPropertyChanged(); } }

        private float _vol3 = 1.0f;
        public float Vol3 { get => _vol3; set { _vol3 = value; NotifyPropertyChanged(); } }

        private float _vol4 = 1.0f;
        public float Vol4 { get => _vol4; set { _vol4 = value; NotifyPropertyChanged(); } }

        // Buffer temporaire pour additionner les signaux sans ecraser le resultat final directement
        private float[] _mixBuffer;

        public MixerModule()
        {
            // La sortie du mixeur correspond à notre méthode de mixage de sont
            Output = new AudioOutput(GenerateMix);
        }


        public void GenerateMix(float[] buffer, int offset, int count, int sampleRate)
        {
            EnsureBufferCapacity(count);

            Array.Clear(buffer, offset, count);

            MixInput(Input1, Vol1, buffer, offset, count, sampleRate);
            MixInput(Input2, Vol2, buffer, offset, count, sampleRate);
            MixInput(Input3, Vol3, buffer, offset, count, sampleRate);
            MixInput(Input4, Vol4, buffer, offset, count, sampleRate);
        }

        private void MixInput(AudioInput input, float volume, float[] outBuffer, int offset, int count, int sampleRate)
        {
            if (!input.IsConnected || volume <= 0.005f) return; // Dont mix if the slider is +/- 0

            // Clear local buffer
            Array.Clear(_mixBuffer, 0, count);

            // Get the input signal into the local buffer
            input.Generate(_mixBuffer, 0, count, sampleRate);

            // Addition in the out buffer with volume control
            for (int i = 0; i < count; i++)
            {
                outBuffer[offset + i] += _mixBuffer[i] * volume;
            }
        }

        private void EnsureBufferCapacity(int count)
        {
            if (_mixBuffer == null || _mixBuffer.Length < count)
            {
                _mixBuffer = new float[count];
            }
        }
    }
}
