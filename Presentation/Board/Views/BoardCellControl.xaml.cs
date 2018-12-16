using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ChessCore;
using CommonServiceLocator;
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
        private readonly BoardCellVM _vm;
        private readonly BoardVM _board;

        public Position Position { get; }

        public BoardCellControl(BoardCellVM vm)
        {
            _vm = vm;
            _board = vm.Board;
            DataContext = vm;

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
            Image.ImageSource = piece == null ? null : new BitmapImage(piece.ImageUri);
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
            var startingCellVM = (BoardCellVM)e.Data.GetData(typeof(BoardCellVM));
            var endingCellVM = this.DataContext as BoardCellVM;

            if (startingCellVM != endingCellVM)
                startingCellVM.Board.OnMoveMade(startingCellVM, endingCellVM);

            _board.MoveEnded();
        }

        private bool HasRightColorPiece()
        {
            var vm = DataContext as BoardCellVM;
            return vm.Piece != null
                && vm.Piece.Color == vm.Board.NextMoveTurn;
        }

        private void UserControl_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            var cellControl = sender as BoardCellControl;
            var imageSize = cellControl.ActualHeight - 2 * cellControl.TopRow.ActualHeight;
            var imageUri = ((BoardCellVM)DataContext).Piece.ImageUri;
            var cursor = CursorHelper.CreateCursor(GetCursorTemplate(imageUri, imageSize), true);
            Mouse.SetCursor(cursor);

            e.Handled = true;
        }

        private Grid GetCursorTemplate(Uri uri, double imageSize)
        {
            var im = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(uri),
                Width = imageSize,
                Height = imageSize
            };

            var grid = new Grid();
      
            grid.Children.Add(im);
            return grid;
        }
    }
}
