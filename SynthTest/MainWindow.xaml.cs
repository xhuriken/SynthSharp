using SynthTest.Infrastructure;
using SynthTest.Presentation.MainViewModel;
using SynthTest.Presentation.ViewModels;
using SynthTest.Presentation.ViewModels.Base;
using SynthTest.Presentation.ViewModels.Menus;
using SynthTest.Presentation.ViewModels.Modules;
using SynthTest.Presentation.ViewModels.Ports;
using SynthTest.Presentation.Views;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SynthTest
{
    public partial class MainWindow : Window
    {
        private MainViewModel _vm;

        // Dragging State
        private bool _isDragging = false;
        //private OutputPortViewModel _sourceDragPort;
        //private Point _startPoint;

        public MainWindow()
        {
            InitializeComponent();
            _vm = new MainViewModel();
            DataContext = _vm;

            Trace.WriteLine("[MAIN] Window Initialized");
        }


        private void OnWindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            // We look for our custom control "PortView" in the visual tree
            var portView = FindParent<PortView>(e.OriginalSource as DependencyObject);

            if (portView != null && portView.DataContext is PortViewModel portVm)
            {
                // We ask the view directly for the center coordinates
                portVm.CenterPoint = portView.GetAnchorCenter(this);

                Trace.WriteLine($"[DRAG START] Port clicked: {portVm.Name}");
                _vm.Rack.StartCableDrag(portVm);
                _isDragging = true;

                Mouse.Capture(this);
                e.Handled = true;
            }
        }

        private void OnWindowMouseMove(object sender, MouseEventArgs e)
        {
            _vm.Rack.UpdateCableDrag(e.GetPosition(this));
        }

        private void OnWindowMouseUp(object sender, MouseButtonEventArgs e)
        {
            PortViewModel targetPort = null;
            Point mousePos = e.GetPosition(this);

            // HitTest to find what is under the mouse
            VisualTreeHelper.HitTest(this, null, new HitTestResultCallback(result =>
            {
                // We are looking for a PortView
                var portView = FindParent<PortView>(result.VisualHit);

                if (portView != null && portView.DataContext is PortViewModel port)
                {
                    targetPort = port;
                    // Update center for perfect magnetic snap
                    targetPort.CenterPoint = portView.GetAnchorCenter(this);
                    return HitTestResultBehavior.Stop;
                }
                return HitTestResultBehavior.Continue;
            }), new PointHitTestParameters(mousePos));

            if (targetPort != null)
                Trace.WriteLine($"[DRAG END] Target found: {targetPort.Name}");
            else
                Trace.WriteLine("[DRAG END] Dropped in void");

            _isDragging = false;
            _vm.Rack.EndCableDrag(targetPort);
            Mouse.Capture(null);
        }

        /// <summary>
        /// Handles the Right Click event globally to determine which custom menu to show.
        /// Priority: Port > Module > Rack.
        /// </summary>
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            var hitElement = e.OriginalSource as DependencyObject; // its like raycast in unity

            // Is a port ?
            var portView = FindParent<PortView>(e.OriginalSource as DependencyObject);
            if (portView != null && portView.DataContext is PortViewModel portVm)
            {
                ShowMenu(BuildPortMenu(portVm, portView));
                e.Handled = true; //Stop here, we don't want the rack's or module's menu to open
                return;
            }

            // IS a module ?
            var moduleVm = FindParentDataContext<ModuleViewModelBase>(hitElement);
            if (moduleVm != null)
            {
                ShowMenu(BuildModuleMenu(moduleVm));
                e.Handled = true;
                return;
            }

            // no... so its rack !!!
            ShowMenu(BuildRackMenu());
        }

        private void ShowMenu(List<MenuItemViewModel> items)
        {
            if (items == null || items.Count == 0) return;

            if (_isDragging) return;

            // Bind the list to the Popup content
            MenuPopup.DataContext = items;
            MenuPopup.IsOpen = true;
        }

        #region Menu Builders

        private List<MenuItemViewModel> BuildRackMenu()
        {
            return new List<MenuItemViewModel>
            {
                MenuItemViewModel.Action("Add VCO", _vm.Rack.AddModuleCommand, ModuleType.VCO),
                MenuItemViewModel.Action("Add Mixer", _vm.Rack.AddModuleCommand, ModuleType.Mixer),
                MenuItemViewModel.Action("Add VCA", _vm.Rack.AddModuleCommand, ModuleType.VCA),
                // Add more global actions here later like "fuck your mom" button biaatch
            };
        }

        private List<MenuItemViewModel> BuildModuleMenu(ModuleViewModelBase module)
        {
            // AudioOut cannot be deleted
            if (module is AudioOutputViewModel)
                return null;

            return new List<MenuItemViewModel>
            {
                // Header 
                new MenuItemViewModel { Header = $"Module: {module.Name}", IsSeparator = false },
                MenuItemViewModel.Separator(),
                MenuItemViewModel.Action("Delete Module", module.DeleteCommand, null, isDestructive: true)
            };
        }

        private List<MenuItemViewModel> BuildPortMenu(PortViewModel port, PortView view)
        {
            var items = new List<MenuItemViewModel>();

            // Action 1: Create Cable (With logic for visual start)
            var createCmd = new RelayCommand(() =>
            {
                port.CenterPoint = view.GetAnchorCenter(this);
                _vm.Rack.StartCreateCable(port);
                Mouse.Capture(this);
                MenuPopup.IsOpen = false; // Close menu immediately
            });

            items.Add(MenuItemViewModel.Action("Create Cable", createCmd));

            // Action 2: Existing Cables, we can delete it (LATER, UNPLUG AND DRAG IT!!)
            if (port.ConnectedCables.Any())
            {
                items.Add(MenuItemViewModel.Separator());

                // We add a label item (no command, just beautiful text)
                items.Add(new MenuItemViewModel { Header = "Connections:" });

                foreach (var cable in port.ConnectedCables.ToList())
                {
                    string info = cable.Source == port
                        ? $"To {cable.Destination?.ParentModule.Name}"
                        : $"From {cable.Source?.ParentModule.Name}";

                    // cable.DeleteCommand
                    items.Add(MenuItemViewModel.Action($"Disconnect ({info})", cable.DeleteCommand, null, isDestructive: true));
                }
            }

            return items;
        }

        #endregion

        #region Tree Helpers

        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            while (child != null)
            {
                if (child is T parent) return parent;
                child = VisualTreeHelper.GetParent(child);
            }
            return null;
        }

        private T FindParentDataContext<T>(DependencyObject child) where T : class
        {
            while (child != null)
            {
                if (child is FrameworkElement fe && fe.DataContext is T found) return found;
                child = VisualTreeHelper.GetParent(child);
            }
            return null;
        }

        #endregion

        private void OnMenuCheckItemClick(object sender, RoutedEventArgs e)
        {
            MenuPopup.IsOpen = false;
        }
    }
}
