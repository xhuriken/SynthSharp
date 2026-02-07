using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Audio
{

    // PORT DE SORTIE (Prise OUTPUT dun module)
    public class AudioOutput : ISignalSource
    {
        // La fonction qui génère le son pour cette sortie. C'est une fonction qu'on reçoit du module parent (ex: VCO) (delegate)
        // qui sait comment générer le son en fonction de sa logique interne.
        private readonly Action<float[], int, int, int> _generator;

        public AudioOutput(Action<float[], int, int, int> generator)
        {
            _generator = generator;
        }

        public void Generate(float[] buffer, int offset, int count, int sampleRate)
        {
            // On appelle la logique de génération du module parent
            _generator?.Invoke(buffer, offset, count, sampleRate);
        }

    }
}
