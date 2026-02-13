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
using System.Windows;
using System.Windows.Input;

namespace SynthTest.Presentation.ViewModels
{
    public class RackViewModel : ViewModelBase
    {
        // List of EVERY module in the rack, we'll bind that to an ItemsControl in the view
        public ObservableCollection<ModuleViewModelBase> Modules { get; } = new();
        // List of EVERY cable in the rack, we'll bind that to an ItemsControl in the view too
        public ObservableCollection<CableViewModel> Cables { get; } = new();

        private CableViewModel _dragCable;

        public ICommand AddVcoCommand { get; }
        public ICommand AddMixerCommand { get; }
        public ICommand StartCreateCableCommand { get; } // right click on port to start creating a cable

        public RackViewModel()
        {
            AddVcoCommand = new RelayCommand(() => AddModule(ModuleType.VCO));
            AddMixerCommand = new RelayCommand(() => AddModule(ModuleType.Mixer));

            // This command starts the cabling process from a port
            StartCreateCableCommand = new RelayCommand<PortViewModel>((p) => StartCreateCable(p));
        }

        // Add a module to the rack, we'll call that from the view when we click on "Add Module" button
        public void AddModule(ModuleType type)
        {
            var module = ModuleFactory.CreateModule(type);

            module.DeleteCommand = new RelayCommand(() => RemoveModule(module));
            Modules.Add(module);
        }

        // For manually add an module, for example when we load a preset, we can use this method to add the module to the rack without going through the factory
        public void AddModule(ModuleViewModelBase module)
        {
            Modules.Add(module);
        }

        // Later we'll need to remove module from the rack, so we can add that method too
        public void RemoveModule(ModuleViewModelBase module)
        {
            // AudioOut cannot be deleted (Business Rule)
            if (module is AudioOutputViewModel) return;

            if (Modules.Contains(module))
            {
                // Remove all cables connected to this module
                // We use ToList() to avoid "Collection Modified" exception during loop
                var cablesToRemove = Cables.Where(c => c.Source.ParentModule == module || c.Destination.ParentModule == module).ToList();
                foreach (var cable in cablesToRemove)
                {
                    cable.Delete();
                }

                // Remove the module
                Modules.Remove(module);
            }
        }

        public void RemoveCable(CableViewModel cable)
        {
            if (Cables.Contains(cable))
            {
                Cables.Remove(cable);
                // Also remove the cable from the connected ports moduleViewModelBase
                if (cable.Source != null) cable.Source.ConnectedCables.Remove(cable);
                if (cable.Destination != null) cable.Destination.ConnectedCables.Remove(cable);
            }
        }

        public void TryCreateCable(OutputPortViewModel source, InputPortViewModel destination, Point p1, Point p2)
        {
            // Right click in void or not in an input
            if (source == null || destination == null) return;

            // Check if an cable already exist on this port (monophonie)
            // If it does, remove it before creating the new one

            // TODO: In the future, we might want to support polyphonic cables.
            // So we should not dispose the existing cable but rather add a new one and let the user choose which one to disconnect if they want to.

            var existingCable = Cables.FirstOrDefault(c => c.Destination == destination);
            if (existingCable != null)
            {
                existingCable.Delete();
            }

            // Create the cable and add it to the list
            var newCable = new CableViewModel(source, destination, (c) => RemoveCable(c))
            {
                StartPoint = p1,
                EndPoint = p2
            };

            Cables.Add(newCable);
        }


        // ----------------------------------------- DRAG

        /// <summary>
        /// Called when Left Click on a port.
        /// Logic: If port is empty -> Create new cable.
        ///        If port has cables -> Unplug the last one.
        /// </summary>
        public void StartDrag(PortViewModel port)
        {
            if (port == null) return;

            // 1. We look for cables connected to this port
            var attachedCables = Cables.Where(c => c.Source == port || c.Destination == port).ToList();

            if (attachedCables.Count == 0)
            {
                // EMPTY PORT -> CREATE NEW
                StartCreateCable(port);
            }
            else
            {
                // OCCUPIED PORT -> MOVE THE LAST ONE (LIFO)
                var lastCable = attachedCables.Last();
                StartMoveCable(lastCable, port);
            }
        }

        /// <summary>
        /// Force creation of a new cable (Used by Right Click Context Menu)
        /// </summary>
        public void StartCreateCable(PortViewModel port)
        {
            

            if (port is OutputPortViewModel outPort)
            {
                // Create new cable from Output
                _dragCable = new CableViewModel((c) => Cables.Remove(c))
                {
                    Source = outPort,
                    StartPoint = outPort.CenterPoint,
                    EndPoint = outPort.CenterPoint // Will follow mouse
                };
                Cables.Add(_dragCable);
            }
            else if (port is InputPortViewModel inPort)
            {
                // Create new cable from Input (Reverse cabling)
                _dragCable = new CableViewModel((c) => Cables.Remove(c))
                {
                    Destination = inPort,
                    StartPoint = inPort.CenterPoint, // Will follow mouse
                    EndPoint = inPort.CenterPoint
                };
                Cables.Add(_dragCable);
            }
            UpdatePortsVisualState();
        }

        /// <summary>
        /// Unplug an existing cable from a specific port to move it
        /// </summary>
        private void    StartMoveCable(CableViewModel cable, PortViewModel portClicked)
        {
            _dragCable = cable;
            _dragCable.IsDragging = true;

            // We need to know which side we are unplugging
            if (portClicked == cable.Source)
            {
                // UNPLUG FROM SOURCE (Output)
                // We keep the destination connected, but we cut the audio
                if (_dragCable.Destination != null)
                    _dragCable.Destination.RemoveConnection(_dragCable.Source?.Node);

                _dragCable.Source = null;
                // The StartPoint will now follow the mouse
            }
            else if (portClicked == cable.Destination)
            {
                // UNPLUG FROM DESTINATION (Input)
                if (_dragCable.Destination != null)
                    _dragCable.Destination.RemoveConnection(_dragCable.Source?.Node);

                _dragCable.Destination = null;
                // The EndPoint will now follow the mouse
            }

            UpdatePortsVisualState();
        }

        // Called by MouseMove
        public void UpdateDrag(Point mousePos)
        {
            if (_dragCable != null)
            {
                _dragCable.UpdateDrag(mousePos);
            }
        }

        // Called by MouseUp
        public void EndDrag(PortViewModel targetPort)
        {
            ResetPortsVisualState();
            if (_dragCable == null) return;

            bool isValid = false;

            if (targetPort != null)
            {
                // CASE 1: We have a Source, looking for Input
                if (_dragCable.Source != null && targetPort is InputPortViewModel targetIn)
                {
                    // Check strict duplication (same source -> same dest)
                    bool alreadyExists = Cables.Any(c => c != _dragCable && c.Source == _dragCable.Source && c.Destination == targetIn);

                    if (!alreadyExists)
                    {
                        _dragCable.Destination = targetIn;
                        _dragCable.Destination.AddConnection(_dragCable.Source.Node); // Audio Connect
                        isValid = true;
                    }
                }
                // CASE 2: We have a Destination, looking for Output
                else if (_dragCable.Destination != null && targetPort is OutputPortViewModel targetOut)
                {
                    bool alreadyExists = Cables.Any(c => c != _dragCable && c.Source == targetOut && c.Destination == _dragCable.Destination);

                    if (!alreadyExists)
                    {
                        _dragCable.Source = targetOut;
                        _dragCable.Destination.AddConnection(targetOut.Node); // Audio Connect
                        isValid = true;
                    }
                }
            }

            if (isValid)
            {
                // Fix the cable position to the centers
                _dragCable.IsDragging = false;
                _dragCable.StartPoint = _dragCable.Source.CenterPoint;
                _dragCable.EndPoint = _dragCable.Destination.CenterPoint;
            }
            else
            {
                // Bad drop -> Delete cable
                Cables.Remove(_dragCable);
            }

            _dragCable = null;
        }

        // Helper for Context Menu: Get all cables on a port
        public IEnumerable<CableViewModel> GetCablesOnPort(PortViewModel port)
        {
            return Cables.Where(c => c.Source == port || c.Destination == port);
        }

        private void UpdatePortsVisualState()
        {
            bool lookingForInput = _dragCable.Source != null;

            foreach (var module in Modules)
            {
                foreach (var input in module.Inputs) input.IsValidDragTarget = lookingForInput;
                foreach (var output in module.Outputs) output.IsValidDragTarget = !lookingForInput;
            }
        }

        private void ResetPortsVisualState()
        {
            foreach (var module in Modules)
            {
                foreach (var input in module.Inputs) input.IsValidDragTarget = true;
                foreach (var output in module.Outputs) output.IsValidDragTarget = true;
            }
        }
    }
}
