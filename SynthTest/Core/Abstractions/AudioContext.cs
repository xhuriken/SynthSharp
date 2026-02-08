using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Core.Abstractions
{
    /// <summary>
    /// Dynamic Audio Context, he is used for drive dynamic data who can be used for diverse module (like Record, VCOs, Sequencer, Clocks...)
    /// </summary>
    public class AudioContext
    {
        // Index of the current played sample (used for _phase synchro for example) (not now, but Later !!)
        public long CurrentSampleIndex { get; set; }

        // For record module (example) for avoid to record ALWAYS all data who enter inside but i'm not sure sure for it
        public bool IsPlaying { get; set; }

        // Local Sample rate, for avoid to call AudioConfig ALWAYS in 
        public int SampleRate { get; set; }
    }
}
