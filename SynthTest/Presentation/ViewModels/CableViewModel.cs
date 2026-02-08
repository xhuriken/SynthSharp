using SynthTest.Presentation.ViewModels.Base;
using SynthTest.Presentation.ViewModels.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Presentation.ViewModels
{
    public class CableViewModel : ViewModelBase
    {
        public OutputPortViewModel Source { get; }
        public InputPortViewModel Destination { get; }

        public CableViewModel(OutputPortViewModel source, InputPortViewModel destination)
        {
            Source = source;
            Destination = destination;

            // At the creation of his visual of the cable, we do the DSP connection !
            Destination.Connect(Source.Node);
        }

        public void Dispose()
        {
            // At the deletion his visual, we do the DSP disconnection !
            Destination.Disconnect();
        }
    }
}
