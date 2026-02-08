using SynthTest.Core.Dsp.Generators;
using SynthTest.Infrastructure;
using SynthTest.Presentation.ViewModels.Base;
using SynthTest.Presentation.ViewModels.Modules;
using SynthTest.Presentation.ViewModels.Ports;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Presentation.ViewModels
{
    public class RackViewModel : ViewModelBase
    {
        // List of EVERY module in the rack, we'll bind that to an ItemsControl in the view
        public ObservableCollection<ModuleViewModelBase> Modules { get; } = new();
        // List of EVERY cable in the rack, we'll bind that to an ItemsControl in the view too
        public ObservableCollection<CableViewModel> Cables { get; } = new();

        // Add a module to the rack, we'll call that from the view when we click on "Add Module" button
        public void AddModule(ModuleType type)
        {
            var module = ModuleFactory.CreateModule(type);
            Modules.Add(module);
        }

        // For manually add an module, for example when we load a preset, we can use this method to add the module to the rack without going through the factory
        public void AddModule(ModuleViewModelBase module)
        {
            Modules.Add(module);
        }

        // Later we'll need to remove module from the rack, so we can add that method too
        // public void RemoveModule(ModuleViewModelBase moduleVm) { blablabla }


        public void TryCreateCable(OutputPortViewModel source, InputPortViewModel destination)
        {
            // Right click in void or not in an input
            if (source == null || destination == null) return;

            // Check if an cable already exist on this port (monophonie)
            // If it does, remove it before creating the new one

            // TODO: In the future, we might want to support polyphonic cables.
            // So we should not dispose the existing cable but rather add a new one and let the user choose which one to disconnect if they want to.

            var existing = Cables.FirstOrDefault(c => c.Destination == destination);
            if (existing != null)
            {
                existing.Dispose();
                Cables.Remove(existing);
            }

            // Create the cable and add it to the list
            var cable = new CableViewModel(source, destination);
            Cables.Add(cable);
        }
    }
}
