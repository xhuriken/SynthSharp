using SynthTest.Core.Abstractions;
using SynthTest.Presentation.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Presentation.ViewModels.Ports
{
    public class InputPortViewModel : PortViewModel
    {
        // callback : this port know how to connect his dsp node
        private readonly Action<IAudioNode> _connectAction;

        public InputPortViewModel(string name, ModuleViewModelBase parent, Action<IAudioNode> connectAction) : base(name, PortType.Input, parent)
        {
            _connectAction = connectAction;
        }

        // Called at PatchCable's creation
        public void Connect(IAudioNode sourceNode)
        {
            _connectAction(sourceNode); // Exampl: Mixer.Input1 = sourceNode
        }

        // Called at PatchCable's deletion
        public void Disconnect()
        {
            _connectAction(null); // Ex: Mixer.Input1 = null
        }
    }
}
