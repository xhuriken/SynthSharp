using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Audio
{
    public class Cable
    {
        // Le cable deviens débile et ne fait que relier AudioInput et AudioOutput
        public AudioOutput Source { get; }
        public AudioInput Destination { get; }

        public Cable(AudioOutput source, AudioInput destination)
        {
            Source = source;
            Destination = destination;

            // On branche "physiquement"
            Destination.ConnectedOutput = Source;
        }

        public void Disconnect()
        {
            if (Destination != null)
            {
                Destination.ConnectedOutput = null;
            }
        }
    }
}
