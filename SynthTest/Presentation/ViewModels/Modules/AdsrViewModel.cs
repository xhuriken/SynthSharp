using SynthTest.Core.Abstractions;
using SynthTest.Core.Dsp.Generators;
using SynthTest.Presentation.ViewModels.Base;
using SynthTest.Presentation.ViewModels.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Presentation.ViewModels.Modules
{
    /// <summary>
    /// Represents the view model for an ADSR Envelope generator.
    /// Provide properties for UI sliders and a virtual Gate button.
    /// </summary>
    public class AdsrViewModel : ModuleViewModelBase
    {
        private readonly AdsrNode _node;
        public override IAudioNode Node => _node;
        public override string Name => "ADSR";

        public AdsrViewModel(AdsrNode node)
        {
            _node = node;

            // The ADSR only outputs a CV modulation signal
            Outputs.Add(new OutputPortViewModel("CV OUT", this, _node));
        }

        /// <summary>
        /// Gets or sets the Attack time in seconds (0.0 to 5.0).
        /// </summary>
        public float Attack
        {
            get => _node.AttackTime;
            set { _node.AttackTime = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the Decay time in seconds (0.0 to 5.0).
        /// </summary>
        public float Decay
        {
            get => _node.DecayTime;
            set { _node.DecayTime = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the Sustain level multiplier (0.0 to 1.0).
        /// </summary>
        public float Sustain
        {
            get => _node.SustainLevel;
            set { _node.SustainLevel = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the Release time in seconds (0.0 to 5.0).
        /// </summary>
        public float Release
        {
            get => _node.ReleaseTime;
            set { _node.ReleaseTime = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the manual gate trigger. 
        /// Bound to a ToggleButton in the UI to simulate a key press.
        /// </summary>
        public bool GateTrigger
        {
            get => _node.IsGateOpen;
            set { _node.IsGateOpen = value; NotifyPropertyChanged(); }
        }
    }
}
