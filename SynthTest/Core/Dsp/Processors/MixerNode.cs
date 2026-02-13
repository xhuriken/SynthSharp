using SynthTest.Core.Abstractions;
using SynthTest.Core.Dsp.Synthesis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Core.Dsp.Processors
{
    public class MixerNode : IAudioNode
    {
        public AudioInput Input1 { get; } = new AudioInput();
        public AudioInput Input2 { get; } = new AudioInput();
        public AudioInput Input3 { get; } = new AudioInput();
        public AudioInput Input4 { get; } = new AudioInput();

        private LinearRamp _ramp1 = new LinearRamp(0.05f);
        private LinearRamp _ramp2 = new LinearRamp(0.05f);
        private LinearRamp _ramp3 = new LinearRamp(0.05f);
        private LinearRamp _ramp4 = new LinearRamp(0.05f);
        private LinearRamp _rampOut = new LinearRamp(0.05f);

        private float _vol1 = 1.0f;
        private float _vol2 = 1.0f;
        private float _vol3 = 1.0f;
        private float _vol4 = 1.0f;
        private float _volOut = 1.0f;

        public float Vol1
        {
            get => _vol1;
            set { _vol1 = value; _ramp1.Value = value; }
        }
        public float Vol2
        {
            get => _vol2;
            set { _vol2 = value; _ramp2.Value = value; }
        }
        public float Vol3
        {
            get => _vol3;
            set { _vol3 = value; _ramp3.Value = value; }
        }
        public float Vol4
        {
            get => _vol4;
            set { _vol4 = value; _ramp4.Value = value; }
        }
        public float VolOut
        {
            get => _volOut;
            set { _volOut = value; _rampOut.Value = value; }
        }

        private float[] _mixBuffer;

        public MixerNode()
        {
            _ramp1.Value = _vol1;
            _ramp2.Value = _vol2;
            _ramp3.Value = _vol3;
            _ramp4.Value = _vol4;
            _rampOut.Value = _volOut;
        }

        public void ProcessBlock(float[] buffer, int offset, int count, AudioContext context)
        {
            EnsureBufferCapacity(count);
            //Array.Clear(buffer, offset, count);

            // Mix 4 Input one by one
            MixInput(Input1, _ramp1, buffer, offset, count, context);
            MixInput(Input2, _ramp2, buffer, offset, count, context);
            MixInput(Input3, _ramp3, buffer, offset, count, context);
            MixInput(Input4, _ramp4, buffer, offset, count, context);

            // Mix result with VolOut
            for (int i = 0; i < count; i++) { 
                buffer[offset + i] *= _rampOut.Next(); 
            }
        }

        private void MixInput(IAudioNode input, LinearRamp volumeRamp, float[] outputBuffer, int offset, int count, AudioContext context)
        {
            if (input == null) return;

            Array.Clear(_mixBuffer, 0, count);

            // Get the input signal into the local buffer
            input.ProcessBlock(_mixBuffer, 0, count, context);

            for (int i = 0; i < count; i++)
            {
                float smoothedVolume = volumeRamp.Next();
                outputBuffer[offset + i] += _mixBuffer[i] * smoothedVolume;
            }
        }

        /// <summary>
        /// Ensures that the internal buffer is allocated and has at least the specified capacity.
        /// </summary>
        /// <param name="count">The minimum number of elements required in the buffer. Must be greater than zero.</param>
        private void EnsureBufferCapacity(int count)
        {
            if (_mixBuffer == null || _mixBuffer.Length < count)
                _mixBuffer = new float[count];
        }
    }
}
