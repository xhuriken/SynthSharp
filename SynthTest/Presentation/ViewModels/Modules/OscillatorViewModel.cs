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
    /// <summary>
    /// Represents the view model for a voltage-controlled oscillator (VCO) module, providing properties and
    /// functionality to control oscillator parameters and expose them to the user interface.
    /// </summary>
    public class OscillatorViewModel : ModuleViewModelBase
    {
        /// <summary>
        /// Reference to his right DSP node
        /// </summary>
        private readonly OscillatorNode _node;
        /// <summary>
        /// Reference to his right IAudioNode (so OsclillatorNode)
        /// </summary>
        public override IAudioNode Node => _node;
        /// <summary>
        /// The name of this module, displayed in the view.
        /// </summary>
        public override string Name => "VCO";

        public OscillatorViewModel(OscillatorNode node)
        {
            _node = node;

            Outputs.Add(new OutputPortViewModel("OUT", this, _node));
        }

        /// <summary>
        /// Gets or sets the frequency value associated with the node.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the currently selected oscillator type for this node.
        /// </summary>
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

        /// <summary>
        /// Gets an array containing all defined values of the OscillatorType enum. Used in view to populate the shape's combobox.
        /// </summary>
        public Array OscillatorTypes => Enum.GetValues(typeof(OscillatorType));
    }
}
