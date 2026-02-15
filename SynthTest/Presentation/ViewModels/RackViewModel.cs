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
        /// <summary>
        /// The collection of modules contained in the rack.
        /// </summary>
        /// <remarks>The returned collection is observable, allowing UI elements or other components to
        /// react to changes such as additions or removals of modules. Items in the collection are of type
        /// ModuleViewModelBase, which represents individual modules within the application. ACTUALLY, all ModuleVM who are inside, are automatically synced and showed on WPF</remarks>
        public ObservableCollection<ModuleViewModelBase> Modules { get; } = new();
        /// <summary>
        /// The collection of cables contained in the rack.
        /// </summary>
        /// <remarks>The returned collection is observable and can be used for data binding in UI
        /// frameworks such as WPF. Modifications to the collection will automatically notify listeners of
        /// changes. ACTUALLY, all CableVM who are inside, are automatically synced and showed on WPF</remarks>
        public ObservableCollection<CableViewModel> Cables { get; } = new();

        /// <summary>
        /// The temporary cable used during drag-and-drop operations. This field holds a reference to the cable being
        /// </summary>
        private CableViewModel _dragCable;

        /// <summary>
        /// Gets the command used to add a new module to the application. It use RelayCommand() to execute the AddModule method with a specific ModuleType parameter when invoked. This command is typically bound to UI elements, such as buttons, allowing users to trigger the addition of new modules to the rack.
        /// </summary>
        public ICommand AddModuleCommand { get; }
        /// <summary>
        /// Gets the command that initiates the cable creation process from a selected port.
        /// </summary>
        /// <remarks>This command is typically used in user interface scenarios where a cable is created
        /// by right-clicking on a port. Invoking the command begins the cable creation workflow, allowing the user to
        /// select the destination port.</remarks>
        public ICommand StartCreateCableCommand { get; } 

        public RackViewModel()
        {
            AddModuleCommand = new RelayCommand<ModuleType>((type) => AddModule(type));

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
                    cable.RequestDelete();
                }

                // Remove the module
                Modules.Remove(module);
            }
            // --------------------------------
            // TEMPORARY BUG !! TO FIX :
            // --------------------------------
            // For now, we dont manage the position of the modules in the rack, so when we remove a module, all the modules on the right of it will be shifted to the left.
            // If some cables are connected to the shifted modules, they will be stuck at their original position
        }

        public void RemoveCable(CableViewModel cable)
        {
            if (Cables.Contains(cable))
            {
                // AUDIO DISCONECTION
                cable.Disconnect();

                Cables.Remove(cable);
                // Also remove the cable from the connected ports moduleViewModelBase
                if (cable.Source != null) cable.Source.ConnectedCables.Remove(cable);
                if (cable.Destination != null) cable.Destination.ConnectedCables.Remove(cable);
            }
        }

        /// <summary>
        /// ONLY allowed to finalize a cable connection.
        /// Used by: EndDrag, loadPreset later
        /// </summary>
        public CableViewModel CreateCable(OutputPortViewModel source, InputPortViewModel destination, Point p1, Point p2)
        {
            if (source == null || destination == null) return null;

            // Check if the connection didnt already exist (same source, same destination)
            bool alreadyExists = Cables.Any(c => c.Source == source && c.Destination == destination);
            if (alreadyExists)
            {
                return null;
            }

            // Create the cable (CableViewModel make the link with the DSP)
            var newCable = new CableViewModel(source, destination, (c) => RemoveCable(c))
            {
                StartPoint = p1,
                EndPoint = p2
            };

            // Update the connected cables list of the ports (for visual feedback and right click menu)
            source.ConnectedCables.Add(newCable);
            destination.ConnectedCables.Add(newCable);
            Cables.Add(newCable); // Add the cable to the rack's cable list

            return newCable;
        }


        // DRAG -----------------------------------------

        /// <summary>
        /// Called when Left Click on a port. <br />
        /// If port is empty -> Create new cable.<br />
        /// If port has cables -> Unplug the last one.
        /// </summary>
        public void StartCableDrag(PortViewModel port)
        {
            if (port == null) return;

            // We search for cables connected to this port
            var connectedCables = Cables.Where(c => c.Source == port || c.Destination == port).ToList();

            if (connectedCables.Count == 0)
            {
                // EMPTY PORT -> CREATE NEW
                StartCreateCable(port);
            }
            else
            {
                // OCCUPIED PORT -> MOVE THE LAST ONE PLUGGED IN THIS PORT
                var lastCable = connectedCables.Last();
                StartMoveCable(lastCable, port);
            }
        }

        /// <summary>
        /// Force creation of a new cable (Used by Right Click Context Menu)
        /// </summary>
        public void StartCreateCable(PortViewModel port)
        {
            // if it's an output port
            if (port is OutputPortViewModel outPort)
            {
                // Create new cable from Output
                _dragCable = new CableViewModel((c) => Cables.Remove(c))
                {
                    Source = outPort,
                    StartPoint = outPort.CenterPoint,
                    EndPoint = outPort.CenterPoint // Will follow mouse
                };
                Cables.Add(_dragCable); // We add the drag cable to the rack's cable list so it gets rendered, but it has no destination yet
            }
            else if (port is InputPortViewModel inPort) // if it's an input port
            {
                // Create new cable from Input (Reverse cabling)
                _dragCable = new CableViewModel((c) => Cables.Remove(c))
                {
                    Destination = inPort,
                    StartPoint = inPort.CenterPoint, // Will follow mouse
                    EndPoint = inPort.CenterPoint
                };
                Cables.Add(_dragCable); // We add the drag cable to the rack's cable list so it gets rendered, but it has no destination yet
            }
            // Update the disponibility visual ports
            UpdatePortsVisualState();
        }

        /// <summary>
        /// Unplug an existing cable from a specific port to move it
        /// </summary>
        private void StartMoveCable(CableViewModel cable, PortViewModel portClicked)
        {
            _dragCable = cable;
            _dragCable.IsDragging = true;

            // remove it temps, for the menu, to avoid ANY FUCKING bugs with the context menu
            if (_dragCable.Source != null) _dragCable.Source.ConnectedCables.Remove(_dragCable);
            if (_dragCable.Destination != null) _dragCable.Destination.ConnectedCables.Remove(_dragCable);

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

        /// <summary>
        /// Updates the position of the cable being dragged based on the specified mouse position.
        /// </summary>
        /// <param name="mousePos">The current position of the mouse cursor, used to update the cable's drag location.</param>
        public void UpdateCableDrag(Point mousePos)
        {
            _dragCable?.UpdateDrag(mousePos);
        }

        // Called by MouseUp
        public void EndCableDrag(PortViewModel targetPort)
        {
            ResetPortsVisualState();
            if (_dragCable == null) return;

            OutputPortViewModel finalSource = _dragCable.Source;
            InputPortViewModel finalDest = _dragCable.Destination;

            // If the target port exist, we check if is an Input or an Output and we assign it to the correct side of the cable
            if (targetPort != null)
            {
                if (finalSource != null && targetPort is InputPortViewModel targetIn)
                {
                    finalDest = targetIn;
                }
                else if (finalDest != null && targetPort is OutputPortViewModel targetOut)
                {
                    finalSource = targetOut;
                }
            }

            // create the cable !
            var createdCable = CreateCable(finalSource, finalDest, _dragCable.Source?.CenterPoint ?? _dragCable.StartPoint, targetPort?.CenterPoint ?? _dragCable.EndPoint);

            // Remove the temp drag cable from the rack
            Cables.Remove(_dragCable);

            // SET the position of the created cable to the same position as his asigned ports
            if (createdCable != null)
            {
                createdCable.StartPoint = finalSource.CenterPoint;
                createdCable.EndPoint = finalDest.CenterPoint;
            }

            _dragCable = null;
        }

        // Helper for Context Menu: Get all cables on a port
        /// <summary>
        /// Returns a collection of cables that are connected to the specified port.
        /// </summary>
        /// <param name="port">The port for which to retrieve all connected cables. Cannot be null.</param>
        /// <returns>An enumerable collection of cables where the specified port is either the source or destination. The
        /// collection will be empty if no cables are connected to the port.</returns>
        public IEnumerable<CableViewModel> GetCablesOnPort(PortViewModel port)
        {
            return Cables.Where(c => c.Source == port || c.Destination == port);
        }

        // When dragging, we want to visually indicate which ports are valid targets (dark) and which are not (normal)
        /// <summary>
        /// Updates the visual state of all input and output ports to reflect their validity as drag targets during a
        /// cable drag operation.
        /// </summary>
        /// <remarks>This method is typically called during drag-and-drop interactions to visually
        /// indicate which ports can accept a connection. Input ports are marked as valid drag targets when a cable is
        /// being dragged from a source; output ports are marked as valid when no source is present.</remarks>
        private void UpdatePortsVisualState()
        {
            bool lookingForInput = _dragCable.Source != null;

            foreach (var module in Modules)
            { 
                foreach (var input in module.Inputs) input.IsValidDragTarget = lookingForInput;
                foreach (var output in module.Outputs) output.IsValidDragTarget = !lookingForInput;
            }
        }

        // When we end the drag, we want to reset all ports to their default visual state (all normal)
        /// <summary>
        /// Resets all input and output ports to their default visual state, marking them as valid drag targets.
        /// </summary>
        /// <remarks>Call this method after completing a drag operation to ensure that all ports are
        /// restored to their normal interactive state. This is typically used to clear any temporary visual indicators
        /// set during drag-and-drop interactions.</remarks>
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
