using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Audio
{
    // Interface qui definit ce qu'est un module de synthese.
    // Tout ce qui genere ou modifie du son doit avoir cette methode.
    // Tout module (VCO, LFO, Filtre, tout le tralala) implémente ceci imo
    public interface ISignalSource
    {
        // Method pour remplir un buffer de "tensions" (float de volt)
        void Generate(float[] buffer, int offset, int count, int sampleRate);
    }
}