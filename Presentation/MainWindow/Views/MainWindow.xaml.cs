using System.Windows;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BoardControl _board;

        public MainWindow(MainWindowVM mainWindowVM,
                          IImagesFactory imagesFactory)
        {
            InitializeComponent();
            DataContext = mainWindowVM;

            _board = new BoardControl(mainWindowVM.Board, imagesFactory);
            BoardGrid.Children.Add(_board);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _board.Resize();
        }
    }
}
