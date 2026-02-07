using SynthTest.Core.Abstractions;
using SynthTest.Core.Dsp.Processors;
using SynthTest.Presentation.ViewModels.Base;
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

        public MixerViewModel(MixerNode node)
        {
            _node = node;
        }

        public override IAudioNode Node => _node;
        public override string Name => "Mixer 4CH"
            ;

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
