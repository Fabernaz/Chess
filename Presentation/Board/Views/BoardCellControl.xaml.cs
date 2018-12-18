using System;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ChessCore;
using ReactiveUI;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for Square.xaml
    /// </summary>
    public partial class BoardCellControl : UserControl
    {
        private static readonly Uri DARK_SQUARE_URI = new Uri("pack://application:,,,/Board/Images/DarkSquare.jpg");
        private static readonly Uri LIGHT_SQUARE_URI = new Uri("pack://application:,,,/Board/Images/LightSquare.jpg");
        private readonly SquareVM _vm;
        private readonly BoardVM _board;
        private readonly IImagesFactory _imageFactory;
        private Cursor _cursor;

        public SquareCoordinate Position { get; }

        public BoardCellControl(SquareVM vm,
                                IImagesFactory imagesFactory)
        {
            _vm = vm;
            _board = vm.Board;
            DataContext = vm;
            _imageFactory = imagesFactory;

            Position = vm.Position;
            Background = GetColorFromVM(vm.Color);
            BorderBrush = System.Windows.Media.Brushes.Black;
            SetGridCoordinate();

            InitializeComponent();

            vm.WhenAny(x => x.Piece, x => x.Value)
                .Subscribe(SetImage);
        }

        private void SetImage(Piece piece)
        {
            Image.ImageSource = _imageFactory.GetPieceImage(piece);
        }

        private void SetGridCoordinate()
        {
            Grid.SetColumn(this, Position.File - 1);
            Grid.SetRow(this, 8 - Position.Rank);
        }

        private ImageBrush GetColorFromVM(BoardCellColor color)
        {
            switch (color)
            {
                case BoardCellColor.Dark:
                    return new ImageBrush(new BitmapImage(DARK_SQUARE_URI));
                case BoardCellColor.Light:
                    return new ImageBrush(new BitmapImage(LIGHT_SQUARE_URI));
                default:
                    throw new NotImplementedException();
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.LeftButton == MouseButtonState.Pressed && HasRightColorPiece())
            {
                _board.MoveStarted(_vm);
                DragDrop.DoDragDrop(this, DataContext, DragDropEffects.All);
            }
        }

        private void UserControl_Drop(object sender, DragEventArgs e)
        {
            var startingCellVM = (SquareVM)e.Data.GetData(typeof(SquareVM));
            var endingCellVM = this.DataContext as SquareVM;

            if (startingCellVM != endingCellVM)
                startingCellVM.Board.OnMoveMade(startingCellVM, endingCellVM);

            _board.MoveEnded();

            if (_cursor != null && startingCellVM.Square != endingCellVM.Square)
                _cursor.Dispose();
        }

        private bool HasRightColorPiece()
        {
            var vm = DataContext as SquareVM;
            return vm.Piece != null
                && vm.Piece.Color == vm.Board.NextMoveTurn;
        }

        private void UserControl_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            var cellControl = sender as BoardCellControl;
            var imageSize = cellControl.ActualHeight - 2 * cellControl.TopRow.ActualHeight;
            _cursor = _imageFactory.GetPieceCursor(((SquareVM)DataContext).Piece, imageSize);
            Mouse.SetCursor(_cursor);

            e.Handled = true;
        }
    }
}
