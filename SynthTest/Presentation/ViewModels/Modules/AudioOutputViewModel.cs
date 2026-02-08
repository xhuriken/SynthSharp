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

        public AudioOutputViewModel(AudioOutputNode node)
        {
            _node = node;

            Inputs.Add(new InputPortViewModel("MAIN IN", this, (source) => _node.Input = source));
        }

        public override IAudioNode Node => _node;
        public override string Name => "AUDIO OUT";
    }
}
