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
        public VcoModule Vco { get; }
        public MasterOutModule Master { get; }

        public MainViewModel()
        {
            Vco = new VcoModule();
            Master = new MasterOutModule();

            // On "branche" le cable virtuel
            Master.MainInput = Vco;
        }
    }
}
