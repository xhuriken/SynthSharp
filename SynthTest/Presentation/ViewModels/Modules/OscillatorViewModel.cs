using SynthTest.Core.Abstractions;
using SynthTest.Core.Dsp.Generators;
using SynthTest.Presentation.ViewModels.Base;
using SynthTest.Presentation.ViewModels.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SynthTest.Presentation.ViewModels.Modules
{
    public class OscillatorViewModel : ModuleViewModelBase
    {
        private readonly OscillatorNode _node;
        public override IAudioNode Node => _node;
        public override string Name => "VCO";

        public OscillatorViewModel(OscillatorNode node)
        {
            _node = node;

            Outputs.Add(new OutputPortViewModel("OUT", this, _node));
        }

        public float Frequency
        {
            get => _node.Frequency;
            set
            {
                if (Math.Abs(_node.Frequency - value) > 0.01f)
                {
                    _node.Frequency = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public OscillatorType SelectedType
        {
            get => _node.Type;
            set
            {
                if (_node.Type != value)
                {
                    _node.Type = value;
                    NotifyPropertyChanged();
                }
            }
        }

        // Util for fill the ComboBox in the view
        public Array OscillatorTypes => Enum.GetValues(typeof(OscillatorType));
    }
}
