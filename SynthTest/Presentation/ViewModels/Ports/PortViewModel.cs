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
        // Which cables are connected to this port ? 
        public ObservableCollection<CableViewModel> ConnectedCables { get; } = new ObservableCollection<CableViewModel>();

        public string Name { get; } // FM ? V/Oct ? etc...
        public PortType Type { get; }

        // Who is the parent of this port ?
        public ModuleViewModelBase ParentModule { get; }

        private bool _isValidDragTarget = true;
        public bool IsValidDragTarget
        {
            get => _isValidDragTarget;
            set { _isValidDragTarget = value; NotifyPropertyChanged(); }
        }

        private Point _centerPoint;
        public Point CenterPoint
        {
            get => _centerPoint;
            set { _centerPoint = value; NotifyPropertyChanged(); }
        }
        public ICommand CreateCableCommand { get; set; }
        public PortViewModel(string name, PortType type, ModuleViewModelBase parent)
        {
            Name = name;
            Type = type;
            ParentModule = parent;
        }
    }
}
