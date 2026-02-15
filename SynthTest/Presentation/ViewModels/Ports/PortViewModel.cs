using SynthTest.Presentation.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SynthTest.Presentation.ViewModels.Ports
{
    public enum PortType { Input, Output }
    public abstract class PortViewModel : ViewModelBase
    {
        /// <summary>
        /// Gets the collection of cables currently connected to this port.
        /// </summary>
        public ObservableCollection<CableViewModel> ConnectedCables { get; } = new ObservableCollection<CableViewModel>();
        /// <summary>
        /// Gets the display name of the port.
        /// </summary>
        public string Name { get; } // FM ? V/Oct ? etc...
        /// <summary>
        /// Gets the type of the port represented by this instance. Input or Output
        /// </summary>
        public PortType Type { get; }
        /// <summary>
        /// The parent module that contains this port.
        /// </summary>
        public ModuleViewModelBase ParentModule { get; }
        
        private bool _isValidDragTarget = true;
        /// <summary>
        /// Gets or sets a value indicating whether the current element is a valid target for drag-and-drop operations.
        /// </summary>
        public bool IsValidDragTarget
        {
            get => _isValidDragTarget;
            set { _isValidDragTarget = value; NotifyPropertyChanged(); }
        }

        private Point _centerPoint;
        /// <summary>
        /// Gets or sets the center point of the port (inside his elipse).
        /// </summary>
        public Point CenterPoint
        {
            get => _centerPoint;
            set { _centerPoint = value; NotifyPropertyChanged(); }
        }
        /// <summary>
        /// Gets or sets the command for creates a new cable derived of this port.
        /// </summary>
        public ICommand CreateCableCommand { get; set; }
        /// <summary>
        /// Initializes a new instance of the PortViewModel class with the specified name, port type, and parent module.
        /// </summary>
        /// <param name="name">The display name of the port. Cannot be null.</param>
        /// <param name="type">The type of the port, indicating its direction or function.</param>
        /// <param name="parent">The parent module to which this port belongs. Cannot be null.</param>
        public PortViewModel(string name, PortType type, ModuleViewModelBase parent)
        {
            Name = name;
            Type = type;
            ParentModule = parent;
        }
    }
}
