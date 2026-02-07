using SynthTest.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Core.Dsp.Processors
{
    public class MixerNode : IAudioNode
    {
        public IAudioNode Input1 { get; set; }
        public IAudioNode Input2 { get; set; }
        public IAudioNode Input3 { get; set; }
        public IAudioNode Input4 { get; set; }

        public float Vol1 { get; set; } = 1.0f;
        public float Vol2 { get; set; } = 1.0f;
        public float Vol3 { get; set; } = 1.0f;
        public float Vol4 { get; set; } = 1.0f;

        private float[] _mixBuffer;

        public void ProcessBlock(float[] buffer, int offset, int count, AudioContext context)
        {
            //Array.Clear(buffer, offset, count);
            EnsureBufferCapacity(count);

            // Mix 4 Input one by one
            MixInput(Input1, Vol1, buffer, offset, count, context);
            MixInput(Input2, Vol2, buffer, offset, count, context);
            MixInput(Input3, Vol3, buffer, offset, count, context);
            MixInput(Input4, Vol4, buffer, offset, count, context);
        }

        private void MixInput(IAudioNode input, float volume, float[] outputBuffer, int offset, int count, AudioContext context)
        {
            if (input == null || volume <= 0.001f) return;

            Array.Clear(_mixBuffer, 0, count);

            // Get the input signal into the local buffer
            input.ProcessBlock(_mixBuffer, 0, count, context);

            for (int i = 0; i < count; i++)
            {
                outputBuffer[offset + i] += _mixBuffer[i] * volume;
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
