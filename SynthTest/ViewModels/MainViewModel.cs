using NAudio.Wave;
using SynthTest.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.ViewModel
{
    public class MainViewModel
    {
        // Modules
        public VcoModule Vco { get; }
        public MixerModule Mixer { get; }
        public MasterOutModule Master { get; }

        // cables
        public Cable CableVcoToMixer { get; }  // cable 1
        public Cable CableMixerToMaster { get; } // cable 2

        public MainViewModel()
        {
            // Instanciation des modules
            Vco = new VcoModule();
            Mixer = new MixerModule();
            Master = new MasterOutModule();

            // Instanciation des cable
            CableVcoToMixer = new Cable();
            CableMixerToMaster = new Cable();

            // VCO -> cable 1 -> Mixer
            CableVcoToMixer.Source = Vco;
            Mixer.AddInput(CableVcoToMixer);  // cable dans le mixer

            //Mixer -> cable 2 -> Master
            CableMixerToMaster.Source = Mixer;
            Master.MainInput = CableMixerToMaster; // cable dans le Master
        }
    }
}
