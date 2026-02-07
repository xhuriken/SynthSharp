using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Audio
{
    public class AudioInput : ISignalSource
    {
        public AudioOutput ConnectedOutput { get; set; }

        public void Generate(float[] buffer, int offset, int count, int sampleRate)
        {
            if (ConnectedOutput != null)
            {
                // Si on a un cable brancher, on tire le son de la sortie qui est connectée
                ConnectedOutput.Generate(buffer, offset, count, sampleRate);
            }
            else
            {
                // Rien brancher = Silence
                Array.Clear(buffer, offset, count);
            }
        }

        // Helper pour savoir si le cable est brancher vers un output
        public bool IsConnected => ConnectedOutput != null;


    }
}
