using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Audio
{
    public class VcoModule : ISignalSource, INotifyPropertyChanged
    {
        #region PropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private float _phase;
        private LinearRamp _frequencyRamp;
        private float _frequency = 440f;
        public float Frequency
        {
            get => _frequency;
            set
            {
                if (_frequency != value)
                {
                    _frequency = value;
                    //Ramp va viser cette new value progressivement
                    _frequencyRamp.Value = _frequency;
                    NotifyPropertyChanged();
                }
            }
        }
        private SignalGeneratorType _type = SignalGeneratorType.Sin;
        public SignalGeneratorType Type
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    _type = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public VcoModule()
        {
            // On initialise le Ramp avec une valeur par défaut (44100Hz standard)
            // On pourra le mettre à jour si le sampleRate change vraiment
            _frequencyRamp = new LinearRamp(44100, 0.05f); // 0.05s de lissage
            _frequencyRamp.Value = _frequency;
        }

        public void Generate(float[] buffer, int count, int sampleRate)
        {
            for (int i = 0; i < count; i++)
            {
                // A CHAQUE SAMPLE, on demande la prochaine petite étape de fréquence
                float smoothedFreq = _frequencyRamp.Next();

                float sampleValue = 0f;

                // En fonction du type de signal, on calcule la valeur du sample NOUS MEME pour avoir un vrai controle de voltage.
                // Le SignalGeneratorType si on l'utilise directement, on n'a pas de controle de voltage, c'est juste une boite noire qui nous sort du son directement, donc PAS MODULAIRE
                switch (Type)
                {
                    case SignalGeneratorType.Sin:
                        // Sin c'est une Sin
                        sampleValue = (float)Math.Sin(_phase);
                        break;

                    case SignalGeneratorType.Square:
                        // Square c'est genre sois 1 sois -1
                        if (_phase < Math.PI)
                        {
                            sampleValue = 1.0f;
                        }
                        else
                        {
                            sampleValue = -1.0f;
                        }
                        break;

                    case SignalGeneratorType.SawTooth:
                        // Gpt frr sayais la je suis ici pour test après le projet faut le faire bien les frère
                        // Formule : (Phase / PI) - 1 pour aller de -1 a 1
                        sampleValue = (float)(2.0 * (_phase / (2.0 * Math.PI)) - 1.0);
                        break;

                    case SignalGeneratorType.Triangle:
                        // bon la pareil j'ai abandonner j'ai pas reussi
                        sampleValue = (float)(Math.Abs((_phase / Math.PI) - 1.0) * 2.0 - 1.0);
                        break;
                }

                buffer[i] = sampleValue;

                // Avancement de la phase
                _phase += (float)(2.0 * Math.PI * smoothedFreq / sampleRate);

                // On reste entre 0 et 2PI (pour pas overflow)
                if (_phase > 2.0 * Math.PI)
                {
                    _phase -= (float)(2.0 * Math.PI);
                }
            }
        }
    }
}
