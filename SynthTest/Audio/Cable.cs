using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Audio
{
    public class Cable : ISignalSource
    {
        // La source branchee a l'autre bout du cable
        public ISignalSource Source { get; set; }

        public void Generate(float[] buffer, int count, int sampleRate)
        {
            if (Source != null)
            {
                // Si branché, on tire le signal de la source
                Source.Generate(buffer, count, sampleRate);
            }
            else
            {
                // Cable en l'air = RIENNN
                Array.Clear(buffer, 0, count);
            }
        }
    }
}
