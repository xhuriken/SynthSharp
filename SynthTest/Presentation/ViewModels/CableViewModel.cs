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
        /// <summary>
        /// The source output port of the cable. This property holds a reference to the OutputPortViewModel that represents the starting point of the cable connection.
        /// </summary>
        public OutputPortViewModel Source { get; set; }
        /// <summary>
        /// The destination input port of the cable. This property holds a reference to the InputPortViewModel that represents the endpoint of the cable connection.
        /// </summary>
        public InputPortViewModel Destination { get; set; }

        private bool _isDragging;
        public bool IsDragging
        {
            get => _isDragging;
            set { _isDragging = value; NotifyPropertyChanged(); }
        }

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
        private Point _cp1; public Point ControlPoint1 { get => _cp1; set { _cp1 = value; NotifyPropertyChanged(); } }
        private Point _cp2; public Point ControlPoint2 { get => _cp2; set { _cp2 = value; NotifyPropertyChanged(); } }

        /// <summary>
        /// Delete Action Callback actually he execute RemoveCable from RackVM)
        /// </summary>
        private readonly Action<CableViewModel> _deleteAction; // delete action callback
        /// <summary>
        /// ICommand for deleting the cable. When executed, it triggers the RequestDelete method, which in turn invokes the _deleteAction callback to request the deletion of this cable from the RackViewModel or any managing entity.
        /// </summary>
        public ICommand DeleteCommand { get; }

        // CONSTRUCTOR FOR REAL CABLE
        public CableViewModel(OutputPortViewModel source, InputPortViewModel destination, Action<CableViewModel> deleteAction)
        {
            Source = source;
            Destination = destination;

            _deleteAction = deleteAction;
            DeleteCommand = new RelayCommand(RequestDelete);

            // At the creation of his visual of the cable, we do the DSP connection !
            Destination.AddConnection(Source.Node);
        }

        // CONSTRUCTOR FOR DRAGGING CABLE (WITHOUT SOURCE AND DESTINATION, we'll set them at the end of the drag)
        public CableViewModel(Action<CableViewModel> deleteAction)
        {
            _deleteAction = deleteAction;
            IsDragging = true;
        }

        /// <summary>
        /// Assign the position of the cables during the drag with the mouse position.
        /// </summary>
        /// <param name="mousePos">Given by the MainWindow -> RackVM -> us</param>
        /// <remarks>He will automaticaly update the right side of the cable.</remarks>
        public void UpdateDrag(Point mousePos)
        {
            // If i have a source, its the end point who follow the mouse
            if (Source != null && Destination == null)
            {
                StartPoint = Source.CenterPoint;
                EndPoint = mousePos;
            }
            // of other, its the start point who follow the mouse
            else if (Destination != null && Source == null)
            {
                StartPoint = mousePos; 
                EndPoint = Destination.CenterPoint;
            }
        }

        /// <summary>
        /// Updates the control points of the curve to create a sagging effect between the start and end points,
        /// simulating gravity. (Beziers)
        /// </summary>
        private void UpdateCurve()
        {
            // GRAVITY WITH BEZIER (AI: TODO REMAKE IT)

            // Calc distance
            double distance = Math.Sqrt(Math.Pow(EndPoint.X - StartPoint.X, 2) + Math.Pow(EndPoint.Y - StartPoint.Y, 2));

            // More distance = more sag, but with a minimum of 50 pixels of sag to avoid too straight lines
            double sag = Math.Max(distance * 0.5, 50);

            // calcul control point
            ControlPoint1 = new Point(StartPoint.X, StartPoint.Y + sag);
            ControlPoint2 = new Point(EndPoint.X, EndPoint.Y + sag);
        }

        /// <summary>
        /// Disconnects the cable | AUDIO SIDE
        /// </summary>
        public void Disconnect()
        {
            if (Source != null && Destination != null)
            {
                Destination.RemoveConnection(Source.Node);
            }
        }

        /// <summary>
        /// Deletes the connection, removing any associated audio and visual links.
        /// </summary>
        public void RequestDelete()
        {
            _deleteAction?.Invoke(this); // Request the deletion to the RackViewModel
        }
    }
}
