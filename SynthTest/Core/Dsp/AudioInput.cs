using SynthTest.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Core.Dsp
{
    /// <summary>
    /// Represent an audio input, he can recieve multiple sources and sum together.
    /// </summary>
    public class AudioInput : IAudioNode
    {
        // List of all the nodes who are plugged in this input, we will sum them together in ProcessBlock
        private readonly List<IAudioNode> _sources = new List<IAudioNode>();
        private float[] _sumBuffer; // temp buffer to sum all the sources

        // Add a source (when we plug cable in)
        public void AddSource(IAudioNode node)
        {
            if (!_sources.Contains(node)) // Avoid adding the same source multiple times
                _sources.Add(node);
        }

        // Remove a source (when we unplug cable)
        public void RemoveSource(IAudioNode node)
        {
            if (_sources.Contains(node)) // Avoid removing a source that is not in the list
                _sources.Remove(node);
        }

        public void ProcessBlock(float[] buffer, int offset, int count, AudioContext context)
        {
            if (_sources.Count == 0) // if no source is connected, just clear the buffer and do nothing for performance
            {
                Array.Clear(buffer, offset, count);
                return;
            }

            EnsureBufferCapacity(count);

            //Array.Clear(buffer, offset, count); // WARNING: survey this fucking line, he can cause troubleshooting

            // Sum all sources together
            foreach (var source in _sources)
            {
                Array.Clear(_sumBuffer, 0, count); // clean temp buff

                // Fill temp buff with source signal
                source.ProcessBlock(_sumBuffer, 0, count, context);

                // Sum temp buff to Read buffer (WARNING TO THE +=)
                for (int i = 0; i < count; i++)
                {
                    buffer[offset + i] += _sumBuffer[i];
                }
            }
        }

        private void EnsureBufferCapacity(int count)
        {
            if (_sumBuffer == null || _sumBuffer.Length < count)
            {
                _sumBuffer = new float[count];
            }
        }
    }
}
