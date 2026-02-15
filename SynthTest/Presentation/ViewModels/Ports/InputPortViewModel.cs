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
        // "Wich fonction execute when we add a new cable in this input ?"
        /// <summary>
        /// Wich action execute when we add a new cable in this input ? <br /> ACTUALLY : node.Input.AddSource(IAudioNode)
        /// </summary>
        private readonly Action<IAudioNode> _addSourceAction;
        /// <summary>
        /// Wich action execute when we remove a cable in this input ? <br /> ACTUALLY : node.Input.RemoveSource(IAudioNode)<
        /// </summary>
        private readonly Action<IAudioNode> _removeSourceAction;

        public InputPortViewModel(string name, ModuleViewModelBase parent, Action<IAudioNode> addAction, Action<IAudioNode> removeAction) : base(name, PortType.Input, parent)
        {
            _addSourceAction = addAction;
            _removeSourceAction = removeAction;
        }

        /// <summary>
        /// Make the connection with the source node (the node that is connected to this input) <br />
        /// Called at PatchCable's creation. (It execute AddSource from AudioInput.cs)<br />
        /// </summary>
        /// <param name="source"></param>
        public void AddConnection(IAudioNode source) => _addSourceAction?.Invoke(source); // Exampl: Mixer.Input1 = sourceNode

        /// <summary>
        /// Remove the connection with the source node (the node that is connected to this input) <br />
        /// Called at PatchCable's deletion. (It execute RemoveSource from AudioInput.cs)<br />
        /// </summary>
        /// <param name="source"></param>
        public void RemoveConnection(IAudioNode source) => _removeSourceAction?.Invoke(source); // Ex: Mixer.Input1 = null

    }
}
