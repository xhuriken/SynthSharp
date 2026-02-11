using SynthTest.Presentation.ViewModels.Base;
using SynthTest.Presentation.ViewModels.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SynthTest.Presentation.ViewModels
{
    public class CableViewModel : ViewModelBase
    {
        public OutputPortViewModel Source { get; }
        public InputPortViewModel Destination { get; }

        private readonly Action<CableViewModel> _deleteAction; // delete action callback

        private Point _startPoint;
        public Point StartPoint
        {
            get => _startPoint;
            set { _startPoint = value; NotifyPropertyChanged(); UpdateCurve(); }
        }

        private Point _endPoint;
        public Point EndPoint
        {
            get => _endPoint;
            set { _endPoint = value; NotifyPropertyChanged(); UpdateCurve(); }
        }

        // BEZIER CONTROL POINTS
        private Point _controlPoint1;
        public Point ControlPoint1 { get => _controlPoint1; set { _controlPoint1 = value; NotifyPropertyChanged(); } }

        private Point _controlPoint2;
        public Point ControlPoint2 { get => _controlPoint2; set { _controlPoint2 = value; NotifyPropertyChanged(); } }

        public ICommand DeleteCommand { get; }

        public CableViewModel(OutputPortViewModel source, InputPortViewModel destination, Action<CableViewModel> deleteAction)
        {
            Source = source;
            Destination = destination;

            _deleteAction = deleteAction;
            DeleteCommand = new RelayCommand(Delete);

            // At the creation of his visual of the cable, we do the DSP connection !
            Destination.Connect(Source.Node);
        }

        private void UpdateCurve()
        {
            // GRAVITY (AI: TODO REMAKE IT)

            // Calc distance
            double distance = Math.Sqrt(Math.Pow(EndPoint.X - StartPoint.X, 2) + Math.Pow(EndPoint.Y - StartPoint.Y, 2));

            // More distance = more sag, but with a minimum of 50 pixels of sag to avoid too straight lines
            double sag = Math.Max(distance * 0.5, 50);

            // calcul control point
            ControlPoint1 = new Point(StartPoint.X, StartPoint.Y + sag);
            ControlPoint2 = new Point(EndPoint.X, EndPoint.Y + sag);
        }

        public void Delete()
        {
            // Audio dispose
            Dispose();
            // Ask to the rack to remove this cable from its collection (UI)
            _deleteAction?.Invoke(this);
        }

        public void Dispose()
        {
            // At the deletion his visual, we do the DSP disconnection !
            Destination.Disconnect();
        }
    }
}
