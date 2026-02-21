using SynthTest.Core.Dsp.Generators;
using SynthTest.Core.Dsp.Processors;
using SynthTest.Presentation.ViewModels.Base;
using SynthTest.Presentation.ViewModels.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Infrastructure
{
    /// <summary>
    /// Specifies the available types of modules in a modular synthesizer system.
    /// </summary>
    /// <remarks>Use this enumeration to identify or configure module components</remarks>
    public enum ModuleType
    {
        VCO,
        Mixer,
        VCA,
        ADSR
    }

    /// <summary>
    /// Provides factory methods for creating module view models based on the specified module type.
    /// </summary>
    /// <remarks>Use this class to instantiate view models for supported module types in a consistent manner.
    /// The factory ensures that each module view model is initialized with its corresponding DSP node. This class is
    /// static and cannot be instantiated.</remarks>
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

                case ModuleType.VCA:
                    var vcaNode = new VcaNode();
                    return new VcaViewModel(vcaNode);
                case ModuleType.ADSR:
                    var adsrNode = new AdsrNode();
                    return new AdsrViewModel(adsrNode);

                default:
                    throw new ArgumentException("Unknown your fucking module type");
            }
        }
    }
}
