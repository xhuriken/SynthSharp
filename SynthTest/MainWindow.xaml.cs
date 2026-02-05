using SynthTest.ViewModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SynthTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        #region Property Changed
        /// --------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        /// --------------------------------------------------------------------------------
        #endregion

        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            // On cree le ViewModel qui contient notre VCO
            _viewModel = new MainViewModel();

            // On lie l'interface au ViewModel pour les data bindings, j'ai expliquer a nino
            this.DataContext = _viewModel;
        }

        // Liaison du front au back
        private void OnPlayClick(object sender, RoutedEventArgs e)
        {
            _viewModel.Master.Play();
        }

        // Liaison du front au back
        private void OnStopClick(object sender, RoutedEventArgs e)
        {
            _viewModel.Master.Stop();
        }

        // Called when the window is closed 
        protected override void OnClosed(EventArgs e)
        {
            // On libère la carte son du pc (le meme principe que tu peu pas modifier un fichier texte si il est ouvert autre part)
            _viewModel.Master.Dispose();
            base.OnClosed(e);
        }
    }
}