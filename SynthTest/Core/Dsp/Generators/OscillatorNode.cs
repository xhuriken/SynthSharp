using SynthTest.Core.Abstractions;
using SynthTest.Core.Dsp.Synthesis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Core.Dsp.Generators
{
    public enum OscillatorType { Sin, Square, SawTooth, Triangle }

    public class OscillatorNode : IAudioNode
    {
        private float _phase;
        private LinearRamp _frequencyRamp;

        private float _frequency = 440f;
        public float Frequency
        {
            get => _frequency;
            set { 
                _frequency = value;
                _frequencyRamp.Value = value; 
            }
        }

        // Sin by default
        public OscillatorType Type { get; set; } = OscillatorType.Sin;

        public OscillatorNode()
        {
            _frequencyRamp = new LinearRamp(44100, 0.05f);
            _frequencyRamp.Value = _frequency;
        }

        public void ProcessBlock(float[] buffer, int offset, int count, AudioContext context)
        {
            for (int i = 0; i < count; i++)
            {
                float smoothedFreq = _frequencyRamp.Next();
                float sampleValue = 0f;

                switch (Type)
                {
                    case OscillatorType.Sin:
                        sampleValue = (float)Math.Sin(_phase);
                        break;
                    case OscillatorType.Square:
                        sampleValue = _phase < Math.PI ? 1.0f : -1.0f;
                        break;
                    case OscillatorType.SawTooth:
                        sampleValue = (float)(2.0 * (_phase / (2.0 * Math.PI)) - 1.0);
                        break;
                    case OscillatorType.Triangle:
                        sampleValue = (float)(Math.Abs((_phase / Math.PI) - 1.0) * 2.0 - 1.0);
                        break;
                }

                buffer[offset + i] = sampleValue;

                _phase += (float)(2.0 * Math.PI * smoothedFreq / context.SampleRate);
                if (_phase > 2.0 * Math.PI) _phase -= (float)(2.0 * Math.PI);
            }
        }
    }
}
