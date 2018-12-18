using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for SquaredUniformGrid.xaml
    /// </summary>
    public partial class BoardControl : UserControl
    {
        private const double IMAGE_PADDING_PERC = 0.11;
        private const double BORDER_THICKNESS_PERC = 0.035;
        private static readonly Uri BORDER_URI = new Uri("pack://application:,,,/Board/Images/DarkWood.jpg");
        private readonly IImagesFactory _piecesImageFactory;
        private BoardVM _vm;
        private List<BoardCellControl> _cells = new List<BoardCellControl>();

        public BoardControl(BoardVM vm,
                            IImagesFactory piecesImageFactory)
        {
            InitializeComponent();

            Border.BorderBrush = new ImageBrush(new BitmapImage(BORDER_URI));
            _piecesImageFactory = piecesImageFactory;

            _vm = vm;
            DataContext = vm;
            Init();
        }

        #region Init

        private void Init()
        {
            foreach (var cellVM in _vm.Cells)
            {
                var cell = GetCellFromVM(cellVM);
                _cells.Add(cell);
                GridBoard.Children.Add(cell);
            }

            Resize();
        }

        private BoardCellControl GetCellFromVM(SquareVM vm)
        {
            return new BoardCellControl(vm, _piecesImageFactory);
        }

        #endregion

        #region Resizing

        public void Resize()
        {
            if (_cells == null || !_cells.Any())
                return;

            var minSide = GetMinSide();
            var newBorderSize = GetBorderSize(minSide);
            var newCellSize = GetNewSize(minSide, newBorderSize);
            var newCellPaddingSize = GetCellPaddingSize(newCellSize);

            _cells.First().Height = newCellSize;
            Border.BorderThickness = new Thickness(newBorderSize);
            foreach (var cell in _cells)
            {
                cell.TopRow.Height = new GridLength(newCellPaddingSize);
                cell.BottomRow.Height = new GridLength(newCellPaddingSize);
                cell.LeftColumn.Width = new GridLength(newCellPaddingSize);
                cell.RightColumn.Width = new GridLength(newCellPaddingSize);
            }
        }

        private double GetCellPaddingSize(double cellSize)
        {
            return cellSize * IMAGE_PADDING_PERC;
        }

        private double GetNewSize(double boardSize, double borderSize)
        {
            return (boardSize - 2 * borderSize) / 8;
        }

        private double GetBorderSize(double newSize)
        {
            return newSize * BORDER_THICKNESS_PERC;
        }

        private double GetMinSide()
        {
            return Utils.GetMinSide(ActualHeight, ActualWidth);
        }

        #endregion
    }
}
