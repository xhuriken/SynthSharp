using SynthTest.Core.Dsp.Generators;
using SynthTest.Presentation.ViewModels.Base;
using SynthTest.Presentation.ViewModels.Modules;
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
        public ObservableCollection<ModuleViewModelBase> Modules { get; }
            = new ObservableCollection<ModuleViewModelBase>();

        // Add a module to the rack, we'll call that from the view when we click on "Add Module" button
        public void AddModule(ModuleViewModelBase moduleVm)
        {
            Modules.Add(moduleVm);
        }

        // Later we'll need to remove module from the rack, so we can add that method too
        // public void RemoveModule(ModuleViewModelBase moduleVm) { blablabla }
    }
}
