using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Audio
{
    public class MixerModule : ISignalSource
    {
        // Liste des entrees du mixer. 
        // Chaque entree est une ISignalSource (ca peut etre un Cable, ou un Module direct en théorie, mais faisons des cables)
        private readonly List<ISignalSource> _inputs = new List<ISignalSource>();

        // Buffer temporaire pour additionner les signaux sans ecraser le resultat final directement
        private float[] _mixBuffer;

        /// <summary>
        /// Adds a signal source to the collection of inputs for this instance.
        /// </summary>
        /// <param name="input">The signal source to add. Cannot be null.</param>
        public void AddInput(ISignalSource input)
        {
            _inputs.Add(input);
        }

        public void Generate(float[] buffer, int count, int sampleRate)
        {
           //Array.Clear(buffer, 0, count);

            // On s'assure d'avoir un buffer temporaire de la bonne taille et existant
            // la taille changera jamais normalement, par contre il sera null la premiere fois !
            if (_mixBuffer == null || _mixBuffer.Length < count)
            {
                _mixBuffer = new float[count];
            }

            foreach (var input in _inputs)
            {
                // On demande a l'input de genererate son son dans le buffer temp
                input.Generate(_mixBuffer, count, sampleRate);

                // Mixer !
                for (int i = 0; i < count; i++)
                {
                    // On additionne le signal temp au buffer du Read
                    buffer[i] += _mixBuffer[i];
                }
            }
        }
    }
}
