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
        public VcoModule Vco1 { get; }
        public VcoModule Vco2 { get; }
        public VcoModule Vco3 { get; }
        public VcoModule Vco4 { get; }
        public MixerModule Mixer { get; }
        public MasterOutModule Master { get; }

        // cables
        public Cable Cable1 { get; }
        public Cable Cable2 { get; }
        public Cable Cable3 { get; }
        public Cable Cable4 { get; }
        public Cable Cable5 { get; }

        public MainViewModel()
        {
            // Instanciation des modules
            Vco1 = new VcoModule();
            Vco2 = new VcoModule(220);
            Vco3 = new VcoModule(120);
            Vco4 = new VcoModule(420);
            Mixer = new MixerModule();
            Master = new MasterOutModule();

            // VCOs -> Mixer
            Cable1 = new Cable(Vco1.Output, Mixer.Input1);
            Cable2 = new Cable(Vco2.Output, Mixer.Input2);
            Cable3 = new Cable(Vco3.Output, Mixer.Input3);
            Cable4 = new Cable(Vco4.Output, Mixer.Input4);


            // Sortie Mixer -> Input MasterOut
            Cable5 = new Cable(Mixer.Output, Master.Input);
        }
    }
}
