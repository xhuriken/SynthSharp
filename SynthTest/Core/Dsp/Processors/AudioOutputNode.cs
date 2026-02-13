using SynthTest.Core.Abstractions;
using SynthTest.Core.Dsp.Synthesis;
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
        public AudioInput Input = new AudioInput();

        private float _vol = 0.2f;
        private LinearRamp _ramp = new LinearRamp(0.05f);

        public AudioOutputNode() 
        {
            _ramp.Value = _vol;
        }

        public float Vol
        {
            get => _vol;
            set { _vol = value; _ramp.Value = value; }
        }
        public void ProcessBlock(float[] buffer, int offset, int count, AudioContext context)
        {
            if (Input != null)
            {
                Input.ProcessBlock(buffer, offset, count, context);

                for (int i = offset; i < offset + count; i++) 
                { 
                    buffer[i] *= _ramp.Next(); 
                }
            }
            else
            {
                Array.Clear(buffer, offset, count);
            }
        }
    }
}
