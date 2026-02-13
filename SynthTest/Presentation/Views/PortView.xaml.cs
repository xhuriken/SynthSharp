using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SynthTest.Presentation.Views
{
    /// <summary>
    /// Logique d'interaction pour PortView.xaml
    /// </summary>
    public partial class PortView : UserControl
    {
        public PortView()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Returns the exact center point of the Anchor (The fucking hole) relative to the main window.
        /// This creates a clean "API" for the MainWindow to ask for position.
        /// </summary>
        public Point GetAnchorCenter(UIElement relativeTo)
        {
            // We use the named element "AnchorElement" defined in XAML
            Point center = AnchorElement.TranslatePoint(
                new Point(AnchorElement.ActualWidth / 2, AnchorElement.ActualHeight / 2), relativeTo
            );
            return center;
        }
    }
}
