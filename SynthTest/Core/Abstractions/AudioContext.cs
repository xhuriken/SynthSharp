using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Core.Abstractions
{
    public class AudioContext
    {
        public int SampleRate { get; set; } = 44100;
        public double DeltaTime => 1.0 / SampleRate; // Used for phases calculation
    }
}
