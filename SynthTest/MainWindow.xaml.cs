using SynthTest.Infrastructure;
using SynthTest.Presentation.MainViewModel;
using SynthTest.Presentation.ViewModels;
using SynthTest.Presentation.ViewModels.Ports;
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


        // DRAG START
        private void OnWindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Get element under mouse (could be the Ellipse, an TextBlock, a lot of WPF things)
            var element = e.OriginalSource as FrameworkElement;

            Trace.WriteLine($"[MOUSE DOWN] Hit: {element?.GetType().Name} | DataContext: {element?.DataContext?.GetType().Name}");

            // This element is an OutputPort ?
            if (element?.DataContext is OutputPortViewModel outputPort)
            {
                Trace.WriteLine($"[DRAG START] Output Port Found: {outputPort.Name}");

                _sourceDragPort = outputPort; // set it for later
                _isDragging = true;

                _startPoint = e.GetPosition(this);

                // Visual.
                // TODO: STOCK CENTER OF PORT AND USE IT HERE
                DragLine.X1 = _startPoint.X;
                DragLine.Y1 = _startPoint.Y;
                DragLine.X2 = _startPoint.X;
                DragLine.Y2 = _startPoint.Y;
                DragLine.Visibility = Visibility.Visible;

                Mouse.Capture(this);
                e.Handled = true;
            }
        }

        // MOUVEMENT
        private void OnWindowMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                // Update the current cable line
                Point currentPos = e.GetPosition(this);
                DragLine.X2 = currentPos.X;
                DragLine.Y2 = currentPos.Y;
            }
        }

        // END OF DRAG
        private void OnWindowMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                e.Handled = true;

                // Get Mouse Position
                Point mousePos = e.GetPosition(this);


                // [I DID NOT KNOW ABOUT THIS WPF FEATURE TODO: READ THE .NET DOC FOR THAT]
                // HIT TEST
                // We ask to the visual wpf tree "which things are under us ?"
                InputPortViewModel foundInput = null;

                VisualTreeHelper.HitTest(this, null, new HitTestResultCallback(result =>
                {
                    var elementFounded = result.VisualHit as FrameworkElement;

                    // We check all element under mouse until is an InputPortViewModel (because it could be a TextBlock, an Ellipse, a Border, etc...
                    while (elementFounded != null)
                    {
                        if (elementFounded.DataContext is InputPortViewModel inputVm)
                        {
                            foundInput = inputVm;
                            return HitTestResultBehavior.Stop; // We FUCKING FOUND IT
                        }
                        // We go to the parent
                        elementFounded = VisualTreeHelper.GetParent(elementFounded) as FrameworkElement;
                    }

                    // If is not an input, we continue to search
                    return HitTestResultBehavior.Continue;
                }), new PointHitTestParameters(mousePos));


                // IS IT AN INPUT ?
                if (foundInput != null)
                {
                    Trace.WriteLine($"[DRAG END] SUCCÈS ! Input trouvé : {foundInput.Name}");

                    try
                    {
                        _vm.Rack.TryCreateCable(_sourceDragPort, foundInput);

                        // WE DRAW AN UGLY LINE
                        // (TODO: REPLACE IT WITH A REAL CABLE CONTROL)
                        DrawPermanentCable(_startPoint, mousePos);

                        StopDragging();

                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine($"[ERREUR] {ex.Message}");
                        StopDragging();

                    }
                }
                else
                {
                    Trace.WriteLine("[DRAG END] Raté (Rien sous la souris)");
                    StopDragging();

                }
            }
        }

        private void StopDragging()
        {
            _isDragging = false;
            _sourceDragPort = null;
            DragLine.Visibility = Visibility.Collapsed;
            Mouse.Capture(null);
            Trace.WriteLine("[DRAG] Stopped");
        }

        private void DrawPermanentCable(Point start, Point end)
        {
            Line cable = new Line
            {
                Stroke = Brushes.Red,
                StrokeThickness = 3,
                X1 = start.X,
                Y1 = start.Y,
                X2 = end.X,
                Y2 = end.Y,
                IsHitTestVisible = false
            };
            CableLayer.Children.Add(cable);
        }
    }
}