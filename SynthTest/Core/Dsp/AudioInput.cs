using SynthTest.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Core.Dsp
{
    /// <summary>
    /// Represent an audio input, he can recieve multiple sources and sum together. It's an funnel (multiple sound source -> one sound)
    /// </summary>
    public class AudioInput : IAudioNode
    {
        /// <summary>
        /// List of all the nodes who are plugged in this input, we will sum them together in ProcessBlock
        /// </summary>
        private readonly List<IAudioNode> _sourcesList = new List<IAudioNode>();

        /// <summary>
        /// Array with fix size than will read the DSP (ITS FOR AVOID THREAD EATING EACH OTHER)
        /// </summary>
        private IAudioNode[] _sourcesArray = Array.Empty<IAudioNode>();


        // temp buffer to sum all the sources
        private float[] _sumBuffer;

        // Add a source (when we plug cable in)
        /// <summary>
        /// Helper for add a new source to this input to proces
        /// </summary>
        /// <param name="node"></param>
        public void AddSource(IAudioNode node)
        {
            if (!_sourcesList.Contains(node)) // Avoid adding the same source multiple times
            {
                _sourcesList.Add(node);
                _sourcesArray = _sourcesList.ToArray(); // Copy the List in Array
            }
        }

        // Remove a source (when we unplug cable)
        /// <summary>
        /// Helper for remove a source from this input to process
        /// </summary>  
        /// <param name="node"></param>
        public void RemoveSource(IAudioNode node)
        {
            if (_sourcesList.Contains(node)) // Avoid removing a source that is not in the list
            {
                _sourcesList.Remove(node);
                _sourcesArray = _sourcesList.ToArray();
            }
        }

        /// <summary>
        /// Sum all the sources connected to this input and fill the buffer with the result. If no source is connected, just clear the buffer for performance.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="context"></param>
        public void ProcessBlock(float[] buffer, int offset, int count, AudioContext context)
        {
            var currentSources = _sourcesArray;

            if (currentSources.Length == 0) // if no source is connected, just clear the buffer and do nothing for performance
            {
                Array.Clear(buffer, offset, count);
                return;
            }

            EnsureBufferCapacity(count);

            // Sum all sources together
            for (int s = 0; s < currentSources.Length; s++)
            {
                var source = currentSources[s];

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
