using SynthTest.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Presentation.ViewModels.Base
{
    // This is an abstract class : We cannot instantiate juste a module, 
    // It must be a specific module like Oscillator, Mixer, etc...
    public abstract class ModuleViewModelBase : ViewModelBase
    {
        // every module must have a reference to his DSP node, like that we can control it from the interface
        public abstract IAudioNode Node { get; }

        public abstract string Name { get; }

        // later we'll stock his position in the canvas
        // public double X { get; set; }
        // public double Y { get; set; }
    }
}
