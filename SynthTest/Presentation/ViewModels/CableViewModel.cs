using SynthTest.Presentation.ViewModels.Base;
using SynthTest.Presentation.ViewModels.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SynthTest.Presentation.ViewModels
{
    public class CableViewModel : ViewModelBase
    {
        public OutputPortViewModel Source { get; }
        public InputPortViewModel Destination { get; }


        private Point _startPoint;
        public Point StartPoint
        {
            get => _startPoint;
            set { _startPoint = value; NotifyPropertyChanged(); }
        }

        private Point _endPoint;
        public Point EndPoint
        {
            get => _endPoint;
            set { _endPoint = value; NotifyPropertyChanged(); }
        }

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
