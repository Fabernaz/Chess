using System;
using System.Collections.Generic;
using ChessCore;
using ReactiveUI;

namespace Presentation
{
    public class BoardCellVM : ReactiveModelViewModelBase<Square>
    {
        private bool _movingPiece;
        private bool _dioBoia;

        public BoardCellColor Color { get { return Model.Color; } }

        public BoardVM Board { get; }

        public SquareCoordinate Position { get { return Model.Coordinate; } }

        public bool IsControlledByWhite { get { return Model.IsControlledByWhite; } }

        public bool IsControlledByBlack { get { return Model.IsControlledByBlack; } }

        public Piece Piece { get { return Model.Piece; } }

        public bool PlayableMoveForPlayer
        {
            get { return _dioBoia; }
            set { this.RaiseAndSetIfChanged(ref _dioBoia, value); }
        }

        public bool MovingPiece
        {
            get { return _movingPiece; }
            set { this.RaiseAndSetIfChanged(ref _movingPiece, value); }
        }

        public BoardCellVM(Square model, BoardVM board)
            : base(model)
        {
            Board = board;

            Model.PieceChanged += (sender, e) => { this.RaisePropertyChanged(nameof(Piece)); };
            Model.ControlledByBlackChanged += (sender, e) => { this.RaisePropertyChanged(nameof(IsControlledByBlack)); };
            Model.ControlledByWhiteChanged += (sender, e) => { this.RaisePropertyChanged(nameof(IsControlledByWhite)); };
        }
    }
}
