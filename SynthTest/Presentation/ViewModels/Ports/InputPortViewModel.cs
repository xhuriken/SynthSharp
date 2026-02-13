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
        private readonly Action<IAudioNode> _addSourceAction;
        private readonly Action<IAudioNode> _removeSourceAction;

        public InputPortViewModel(string name, ModuleViewModelBase parent, Action<IAudioNode> addAction, Action<IAudioNode> removeAction) : base(name, PortType.Input, parent)
        {
            _addSourceAction = addAction;
            _removeSourceAction = removeAction;
        }

        // Called at PatchCable's creation
        public void AddConnection(IAudioNode source) => _addSourceAction?.Invoke(source); // Exampl: Mixer.Input1 = sourceNode

        // Called at PatchCable's deletion
        public void RemoveConnection(IAudioNode source) => _removeSourceAction?.Invoke(source); // Ex: Mixer.Input1 = null

    }
}
