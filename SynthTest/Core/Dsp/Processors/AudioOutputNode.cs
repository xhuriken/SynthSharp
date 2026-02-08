using SynthTest.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Core.Dsp.Processors
{
    public class AudioOutputNode : IAudioNode
    {
        // Input of master
        public IAudioNode Input { get; set; }

        public void ProcessBlock(float[] buffer, int offset, int count, AudioContext context)
        {
            if (Input != null)
            {
                Input.ProcessBlock(buffer, offset, count, context);
            }
            else
            {
                Array.Clear(buffer, offset, count);
            }
        }
    }
}
