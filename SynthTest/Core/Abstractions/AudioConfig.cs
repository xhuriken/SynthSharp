using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Core.Abstractions
{
    public class AudioConfig
    {
        public static int SampleRate { get; set; } = 44100;
        public static double DeltaTime => 1.0 / SampleRate; // Used for phases calculation
    }
}
