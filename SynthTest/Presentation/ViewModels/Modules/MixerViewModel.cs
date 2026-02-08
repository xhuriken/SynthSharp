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
    public class MixerViewModel : ModuleViewModelBase
    {
        // Reference to his DSP node
        private readonly MixerNode _node;
        public override IAudioNode Node => _node;
        public override string Name => "Mixer 4CH";

        public MixerViewModel(MixerNode node)
        {
            _node = node;

            // Input 1
            Inputs.Add(new InputPortViewModel("IN 1", this, (source) => _node.Input1 = source));
            // Input 2
            Inputs.Add(new InputPortViewModel("IN 2", this, (source) => _node.Input2 = source));
            // Input 3
            Inputs.Add(new InputPortViewModel("IN 3", this, (source) => _node.Input3 = source));
            // Input 4
            Inputs.Add(new InputPortViewModel("IN 4", this, (source) => _node.Input4 = source));


            // Output (Le Mixer est lui-même un Node, donc c'est lui la source)
            Outputs.Add(new OutputPortViewModel("OUT", this, _node));

        }


        public float Vol1
        {
            get => _node.Vol1;
            set
            {
                _node.Vol1 = value;
                NotifyPropertyChanged();
            }
        }

        public float Vol2 { 
            get => _node.Vol2; 
            set { 
                _node.Vol2 = value; 
                NotifyPropertyChanged(); 
            } 
        }
        public float Vol3 { 
            get => _node.Vol3; 
            set { 
                _node.Vol3 = value; 
                NotifyPropertyChanged(); 
            } 
        }
        public float Vol4 { 
            get => _node.Vol4; 
            set { 
                _node.Vol4 = value; 
                NotifyPropertyChanged(); 
            } 
        }
    }
}
