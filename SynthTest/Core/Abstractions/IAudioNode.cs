using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Core.Abstractions
{
    public interface IAudioNode
    {
        // Used to fill the NAudio Buffer
        void ProcessBlock(float[] buffer, int offset, int count, AudioContext context);
    }
}
