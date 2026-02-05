using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave.SampleProviders;

namespace SynthTest.Audio
{
    public class VcoModule : ISignalSource
    {
        private float _phase;
        public float Frequency { get; set; } = 440f;
        public SignalGeneratorType Type { get; set; } = SignalGeneratorType.Sin;

        public void Generate(float[] buffer, int count, int sampleRate)
        {
            for (int i = 0; i < count; i++)
            {
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
                _phase += (float)(2.0 * Math.PI * Frequency / sampleRate);

                // On reste entre 0 et 2PI (pour pas overflow)
                if (_phase > 2.0 * Math.PI)
                {
                    _phase -= (float)(2.0 * Math.PI);
                }
            }
        }
    }
}
