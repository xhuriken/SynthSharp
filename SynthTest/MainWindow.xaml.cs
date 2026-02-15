using SynthTest.Infrastructure;
using SynthTest.Presentation.MainViewModel;
using SynthTest.Presentation.ViewModels;
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
        private OutputPortViewModel _sourceDragPort;
        private Point _startPoint;

        public MainWindow()
        {
            InitializeComponent();
            _vm = new MainViewModel();
            DataContext = _vm;

            Trace.WriteLine("[MAIN] Window Initialized");
        }

        // Controls for add modules in rack
        private void OnAddVcoClick(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("[MAIN] Add VCO Clicked");
            _vm.Rack.AddModule(ModuleType.VCO);
        }

        private void OnAddMixerClick(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("[MAIN] Add Mixer Clicked");
            _vm.Rack.AddModule(ModuleType.Mixer);
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

            _vm.Rack.EndCableDrag(targetPort);
            Mouse.Capture(null);
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            // Is a port ?
            var portView = FindParent<PortView>(e.OriginalSource as DependencyObject);
            if (portView != null && portView.DataContext is PortViewModel portVm)
            {
                OpenPortMenu(portView, portVm);
                e.Handled = true; //Stop here, we don't want the rack's context menu to open
                return;
            }

            // Else, its the rack so we let the rack's context menu open
            base.OnMouseRightButtonUp(e);
        }

        /// <summary>
        /// Open a context menu for a port with options to create a cable or manage existing connections.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="portVm"></param>
        private void OpenPortMenu(PortView view, PortViewModel portVm)
        {
            ContextMenu menu = new ContextMenu();

            // Create menu item for creating a new cable from this port
            MenuItem createItem = new MenuItem { Header = "Create Cable", FontWeight = FontWeights.Bold };
            createItem.Click += (s, args) =>
            {
                // TODO: use the command ??
                portVm.CenterPoint = view.GetAnchorCenter(this);
                _vm.Rack.StartCreateCable(portVm);
                Mouse.Capture(this); // for the drag
            };
            menu.Items.Add(createItem);

            // LIST OF CONNECTIONS (if any)
            if (portVm.ConnectedCables.Any())
            {
                menu.Items.Add(new Separator());
                menu.Items.Add(new MenuItem { Header = "Connections:", IsEnabled = false, Foreground = Brushes.Gray });

                // we list all connected cables with an option to delete them
                foreach (var cable in portVm.ConnectedCables.ToList())
                {
                    // Text
                    string info = cable.Source == portVm
                        ? $"-> To {cable.Destination?.ParentModule.Name}"
                        : $"<- From {cable.Source?.ParentModule.Name}";

                    MenuItem cableItem = new MenuItem { Header = $"{info}  [x]" };

                    cableItem.Command = cable.DeleteCommand;
                    cableItem.Foreground = Brushes.DarkRed;

                    menu.Items.Add(cableItem);
                }
            }

            menu.IsOpen = true;
        }

        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            while (child != null)
            {
                if (child is T parent) return parent;
                child = VisualTreeHelper.GetParent(child);
            }
            return null;
        }
    }
}
