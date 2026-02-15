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
        /// <summary>
        /// The DSP node associated with this output port, representing the audio signal source that can be connected to input ports of other modules.<br />
        /// TLDR : It's the sound of THAT node I'll distribute.
        /// </summary>
        public IAudioNode Node { get; }

        public OutputPortViewModel(string name, ModuleViewModelBase parent, IAudioNode node) : base(name, PortType.Output, parent)
        {
            Node = node;
        }
    }
}
