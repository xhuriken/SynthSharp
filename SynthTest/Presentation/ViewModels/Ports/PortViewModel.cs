using SynthTest.Presentation.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Presentation.ViewModels.Ports
{
    public enum PortType { Input, Output }
    public abstract class PortViewModel : ViewModelBase
    {
        public string Name { get; } // FM ? V/Oct ? etc...
        public PortType Type { get; }

        // Who is the parent of this port ?
        public ModuleViewModelBase ParentModule { get; }

        public PortViewModel(string name, PortType type, ModuleViewModelBase parent)
        {
            Name = name;
            Type = type;
            ParentModule = parent;
        }
    }
}
