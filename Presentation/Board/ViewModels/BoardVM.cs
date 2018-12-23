using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ChessCore;
using ReactiveUI;

namespace Presentation
{
    public interface IBoardVM
    {
        IEnumerable<SquareVM> Cells { get; }
        Color NextMoveTurn { get; }

        void OnMoveMade(SquareVM starting, SquareVM ending);
    }

    public class BoardVM : ReactiveViewModelBase, IBoardVM
    {
        private List<SquareVM> _availableSquares;
        private SquareVM _startingSquare;
        private Board _board;
        private string _blackTime;
        private string _whiteTime;

        public IEnumerable<SquareVM> Cells { get; }

        public ObservableCollection<MovePair> MovesPlayed { get; }

        public Color NextMoveTurn { get { return _board.NextMoveTurn; } }

        public string BlackTime
        {
            get { return _blackTime; }
            private set { this.RaiseAndSetIfChanged(ref _blackTime, value); }
        }

        public string WhiteTime
        {
            get { return _whiteTime; }
            private set { this.RaiseAndSetIfChanged(ref _whiteTime, value); }
        }

        public BoardVM(Board board)
        {
            _board = board;
            MovesPlayed = new ObservableCollection<MovePair>();

            _board.MovePlayed += OnMovePlayed;
            _board.BlackTimerChanged += (sender, e) =>
            {
                BlackTime = _board.GetBlackTime();
            };
            BlackTime = _board.GetBlackTime();
            _board.WhiteTimerChanged += (sender, e) =>
            {
                WhiteTime = _board.GetWhiteTime();
            };
            WhiteTime = _board.GetWhiteTime();

            Cells = board.GetSquares()
                         .Select(m => new SquareVM(m, this))
                         .ToList();
        }

        private void OnMovePlayed(object sender, EventArgs e)
        {
            MovesPlayed.Clear();
            MovesPlayed.AddRange(_board.GetAllMovesPlayed());
        }

        public void OnMoveMade(SquareVM startingCell, SquareVM endingCell)
        {
            _board.TryPlayMove(startingCell.Square, endingCell.Square);
        }

        public void MoveStarted(SquareVM startingSquare)
        {
            _availableSquares = new List<SquareVM>();
            _startingSquare = startingSquare;

            startingSquare.MovingPiece = true;

            var availableSquares = startingSquare.Piece.GetAvailableMoves();
            foreach (var square in availableSquares)
            {
                var cellVM = Cells.Single(c => c.Position == square);
                cellVM.PlayableMoveForPlayer = true;
                _availableSquares.Add(cellVM);
            }
        }

        public void MoveEnded()
        {
            _startingSquare.MovingPiece = false;

            foreach (var square in _availableSquares)
                square.PlayableMoveForPlayer = false;
        }
    }
}
