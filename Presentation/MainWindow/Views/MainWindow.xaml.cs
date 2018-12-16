using System.Drawing;
using System.Windows;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BoardControl _board;

        public MainWindow(MainWindowVM mainWindowVM)
        {
            InitializeComponent();
            DataContext = mainWindowVM;

            _board = new BoardControl(mainWindowVM.Board);
            BoardGrid.Children.Add(_board);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _board.Resize();
        }
    }
}
