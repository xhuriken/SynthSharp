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
    public class AudioOutputViewModel : ModuleViewModelBase
    {
        private readonly AudioOutputNode _node;
        public override IAudioNode Node => _node;
        public override string Name => "AUDIO OUT";

        public float Vol
        {
            get => _node.Vol;
            set
            {
                _node.Vol = value;
                NotifyPropertyChanged();
            }
        }
        public AudioOutputViewModel(AudioOutputNode node)
        {
            _node = node;

            Inputs.Add(new InputPortViewModel("IN", this, (src) => _node.Input.AddSource(src), (src) => _node.Input.RemoveSource(src)));
        }
    }
}
