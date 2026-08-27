using OCSFoundationOptimizer.ViewModels;
using System.Windows;

namespace OCSFoundationOptimizer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();
        }
    }
}