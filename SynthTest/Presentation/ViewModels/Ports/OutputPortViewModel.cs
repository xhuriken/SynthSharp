using SynthTest.Core.Abstractions;
using SynthTest.Presentation.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Presentation.ViewModels.Ports
{
    public class OutputPortViewModel : PortViewModel
    {
        // Dsp Node we'll distribute voltage
        public IAudioNode Node { get; }

        public OutputPortViewModel(string name, ModuleViewModelBase parent, IAudioNode node) : base(name, PortType.Output, parent)
        {
            Node = node;
        }
    }
}
