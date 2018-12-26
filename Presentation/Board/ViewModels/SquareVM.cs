using System;
using System.Collections.Generic;
using ChessCore;
using ReactiveUI;

namespace Presentation
{
    public class SquareVM : ReactiveObject
    {
        private bool _movingPiece;
        private bool _playableMoveForPlayer;

        public Square Square { get; }

        public BoardCellColor Color { get { return Square.Color; } }

        public BoardVM Board { get; }

        public SquareCoordinate Position { get { return Square.Coordinate; } }

        public bool IsControlledByWhite { get { return Square.IsControlledByWhite; } }

        public bool IsControlledByBlack { get { return Square.IsControlledByBlack; } }

        public Piece Piece { get { return Square.Piece; } }

        public bool PlayableMoveForPlayer
        {
            get { return _playableMoveForPlayer; }
            set { this.RaiseAndSetIfChanged(ref _playableMoveForPlayer, value); }
        }

        public bool MovingPiece
        {
            get { return _movingPiece; }
            set { this.RaiseAndSetIfChanged(ref _movingPiece, value); }
        }

        public SquareVM(Square square, BoardVM board)
        {
            Board = board;
            Square = square;

            Square.PieceChanged += (sender, e) => { this.RaisePropertyChanged(nameof(Piece)); };
            Square.ControlledByBlackChanged += (sender, e) => { this.RaisePropertyChanged(nameof(IsControlledByBlack)); };
            Square.ControlledByWhiteChanged += (sender, e) => { this.RaisePropertyChanged(nameof(IsControlledByWhite)); };
        }
    }
}
