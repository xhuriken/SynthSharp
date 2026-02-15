using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Core.Abstractions
{
    /// <summary>
    /// Interface defining any component capable of processing audio data into a buffer. VCO, MIXER, AUDIOOUT, etc... all implement this interface.
    /// </summary>
    public interface IAudioNode
    {
        // Used to fill the NAudio Buffer
        /// <summary>
        /// Fill the buffer with audio data
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="context"></param>
        void ProcessBlock(float[] buffer, int offset, int count, AudioContext context);
    }
}
