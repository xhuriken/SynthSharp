using SynthTest.Core.Abstractions;
using SynthTest.Core.Dsp.Synthesis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Core.Dsp.Processors
{
    public class VcaNode : IAudioNode
    {
        /// <summary>
        /// this is the input of the sound who are generate
        /// </summary>
        public AudioInput InputAudio { get; } = new AudioInput();
        /// <summary>
        /// This is an input who take signal between 0 and 1. The mod are multiplied to the Node.
        /// </summary>
        public AudioInput InputCv { get; } = new AudioInput();

        private LinearRamp _rampLevel = new LinearRamp(0.05f);
        private float _level = 1.0f;
        public float Level
        {
            get => _level;
            set { _level = value; _rampLevel.Value = value; }
        }

        public VcaNode()
        {
            _rampLevel.Value = _level;
        }

        private float[] _audioBuffer;
        private float[] _cvBuffer;

        public void ProcessBlock(float[] buffer, int offset, int count, AudioContext context)
        {
            // transport the InputAudio with InputMod multiplication (audio * Cv(between 0 and 1))
            EnsureBufferCapacity(count);

            Array.Clear(_audioBuffer, 0, count);
            InputAudio.ProcessBlock(_audioBuffer, offset, count, context);

            Array.Clear(_cvBuffer, 0, count);
            InputCv.ProcessBlock(_cvBuffer, offset, count, context);

            bool isCvConnected = InputCv.IsConnected;

            for (int i = 0; i < count; i++)
            {
                float knobLevel = _rampLevel.Next();

                // if something is connected to the CV input, we take him. else, we set to 1
                float cvValue = isCvConnected ? _cvBuffer[i] : 1.0f;

                float gain = knobLevel * cvValue;
                if (gain < 0f) gain = 0f; // in theorie, this will never happen, but, its industrial security !
                buffer[offset + i] = _audioBuffer[i] * gain;
            }
        }

        /// <summary>
        /// Ensures that the internal buffer is allocated and has at least the specified capacity.
        /// </summary>
        /// <param name="count">The minimum number of elements required in the buffer. Must be greater than zero.</param>
        private void EnsureBufferCapacity(int count)
        {
            if (_audioBuffer == null || _audioBuffer.Length < count)
            {
                _audioBuffer = new float[count];
                _cvBuffer = new float[count];
            }
        }
    }
}
