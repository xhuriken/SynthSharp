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
            // KISS: We look for our custom control "PortView" in the visual tree
            var portView = FindParent<PortView>(e.OriginalSource as DependencyObject);

            if (portView != null && portView.DataContext is PortViewModel portVm)
            {
                // CLEAN: We ask the view directly for the center coordinates
                portVm.CenterPoint = portView.GetAnchorCenter(this);

                Trace.WriteLine($"[DRAG START] Port clicked: {portVm.Name}");
                _vm.Rack.StartDrag(portVm);

                Mouse.Capture(this);
                e.Handled = true;
            }
        }

        private void OnWindowMouseMove(object sender, MouseEventArgs e)
        {
            _vm.Rack.UpdateDrag(e.GetPosition(this));
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
                    // CLEAN: Update center for perfect magnetic snap
                    targetPort.CenterPoint = portView.GetAnchorCenter(this);
                    return HitTestResultBehavior.Stop;
                }
                return HitTestResultBehavior.Continue;
            }), new PointHitTestParameters(mousePos));

            if (targetPort != null)
                Trace.WriteLine($"[DRAG END] Target found: {targetPort.Name}");
            else
                Trace.WriteLine("[DRAG END] Dropped in void");

            _vm.Rack.EndDrag(targetPort);
            Mouse.Capture(null);
        }

        // =========================================================
        //                 HELPERS
        // =========================================================

        // Generic helper to find a parent of a specific type in the Visual Tree
        // Much cleaner than checking DataContext manually
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