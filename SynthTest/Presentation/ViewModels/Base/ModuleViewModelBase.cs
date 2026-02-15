using SynthTest.Core.Abstractions;
using SynthTest.Presentation.ViewModels.Ports;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SynthTest.Presentation.ViewModels.Base
{
    // This is an abstract class : We cannot instantiate juste a module, 
    // It must be a specific module like Oscillator, Mixer, etc...
    /// <summary>
    /// Serves as the abstract base class for all module view models, providing common properties and functionality for
    /// modules such as oscillators, mixers, and other audio components.
    /// </summary>
    public abstract class ModuleViewModelBase : ViewModelBase
    {
        // every module must have a reference to his DSP node, like that we can control it from the interface
        public abstract IAudioNode Node { get; }

        /// <summary>
        /// Gets the name of this module. The name is displayed in the view.
        /// </summary>
        public abstract string Name { get; }

        // List for UI of the ports (itemsControl)
        /// <summary>
        /// Gets the collection of input ports displayed in the user interface.
        /// </summary>
        /// <remarks>His elements are added in the specific moduleViewModel</remarks>
        public ObservableCollection<InputPortViewModel> Inputs { get; } = new();
        /// <summary>
        /// Gets the collection of output ports associated with this view model.
        /// </summary>
        /// <remarks>His elements are added in the specific moduleViewModel</remarks>
        public ObservableCollection<OutputPortViewModel> Outputs { get; } = new();
        /// <summary>
        /// Gets or sets the command that deletes the selected item or items.
        /// </summary>
        public ICommand DeleteCommand { get; set; }

        // later we'll stock his position in the canvas
        // public double X { get; set; }
        // public double Y { get; set; }
    }
}
