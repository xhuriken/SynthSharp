using SynthTest.Core.Dsp.Generators;
using SynthTest.Core.Dsp.Processors;
using SynthTest.Infrastructure.AudioDrivers;
using SynthTest.Presentation.ViewModels;
using SynthTest.Presentation.ViewModels.Base;
using SynthTest.Presentation.ViewModels.Modules;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Presentation.MainViewModel
{
    // Its here we'll cable all node manually in code for now
    public class MainViewModel : ViewModelBase
    {
        private readonly NAudioEngine _audioEngine;

        // The main view have a Rack
        public RackViewModel Rack { get; } = new RackViewModel();

        public MainViewModel()
        {
            // Init the Audio Engine
            _audioEngine = new NAudioEngine();

            // Create the DSP Node (Module)
            var vco1 = new OscillatorNode { Frequency = 220, Type = OscillatorType.Sin };
            var vco2 = new OscillatorNode { Frequency = 440, Type = OscillatorType.SawTooth };
            var mixer = new MixerNode();

            // Manual Cable Management (Hardcoded biatchhhh)
            // VCO1 -> Mixer Input 1
            // VCO2 -> Mixer Input 2
            mixer.Input1 = vco1;
            mixer.Input2 = vco2;

            // Mixer -> Master Output
            _audioEngine.InputNode = mixer;


            // Create the ViewModel for the interface
            Rack.AddModule(new OscillatorViewModel(vco1));
            Rack.AddModule(new OscillatorViewModel(vco2));
            Rack.AddModule(new MixerViewModel(mixer));

            // Enable our synthesizer dude !
            _audioEngine.Play();
        }

    }
}
