using SynthTest.Core.Dsp.Generators;
using SynthTest.Core.Dsp.Processors;
using SynthTest.Presentation.ViewModels.Base;
using SynthTest.Presentation.ViewModels.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Infrastructure
{
    public enum ModuleType
    {
        VCO,
        Mixer
    }

    public static class ModuleFactory
    {
        public static ModuleViewModelBase CreateModule(ModuleType type)
        {
            switch (type)
            {
                case ModuleType.VCO:
                    var vcoNode = new OscillatorNode(); // Create DSP
                    return new OscillatorViewModel(vcoNode); //Create VM + his Ports

                case ModuleType.Mixer:
                    var mixerNode = new MixerNode();
                    return new MixerViewModel(mixerNode);

                default:
                    throw new ArgumentException("Unknown module type");
            }
        }
    }
}
