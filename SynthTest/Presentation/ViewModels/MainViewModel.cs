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

            var masterNode = new AudioOutputNode();
            var masterVm = new AudioOutputViewModel(masterNode);

            // 2. On l'ajoute au Rack (pour qu'il s'affiche)
            Rack.AddModule(masterVm);

            // 3. C'EST ICI LE SECRET : On dit à NAudio "Ta source, c'est ce Node là"
            // Tout le reste du son viendra de ce qui est branché dans ce MasterNode.
            _audioEngine.InputNode = masterNode;

            // Enable our synthesizer dude !
            _audioEngine.Play();
        }

    }
}
