using SynthTest.Core.Abstractions;
using SynthTest.Core.Dsp.Processors;
using SynthTest.Presentation.ViewModels.Base;
using SynthTest.Presentation.ViewModels.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Presentation.ViewModels.Modules
{
    public class ControlledAmplifierViewModel : ModuleViewModelBase
    {
        private readonly ControlledAmplifierNode _node;
        public override IAudioNode Node => _node;
        public override string Name => "VCA";
        public float Level
        {
            get => _node.Level;
            set
            {
                _node.Level = value;
                NotifyPropertyChanged();
            }
        }
        public ControlledAmplifierViewModel(ControlledAmplifierNode node)
        {
            _node = node;

            Inputs.Add(new InputPortViewModel("IN", this, (src) => _node.InputAudio.AddSource(src), (src) => _node.InputAudio.RemoveSource(src)));
            Inputs.Add(new InputPortViewModel("CV", this, (src) => _node.InputCv.AddSource(src),    (src) => _node.InputCv.RemoveSource(src)));

            Outputs.Add(new OutputPortViewModel("OUT", this, _node));
        }
    }
}
