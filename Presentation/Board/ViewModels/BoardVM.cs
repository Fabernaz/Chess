using System.Collections.Generic;
using System.Linq;
using ChessCore;

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
        private List<SquareVM> _AvailableSquares;
        private SquareVM _StartingSquare;
        private Board _Board;

        public IEnumerable<SquareVM> Cells { get; }

        public Color NextMoveTurn { get { return _Board.NextMoveTurn; } }

        public BoardVM(Board board)
        {
            _Board = board;

            Cells = board.GetSquares()
                         .Select(m => new SquareVM(m, this))
                         .ToList(); //This is to end deferred execution
        }

        public void OnMoveMade(SquareVM startingCell, SquareVM endingCell)
        {
            _Board.TryPlayMove(startingCell.Square, endingCell.Square);
        }

        public void MoveStarted(SquareVM startingSquare)
        {
            _AvailableSquares = new List<SquareVM>();
            _StartingSquare = startingSquare;

            startingSquare.MovingPiece = true;

            var availableSquares = startingSquare.Piece.GetAvailableMoves();
            foreach (var square in availableSquares)
            {
                var cellVM = Cells.Single(c => c.Position == square);
                cellVM.PlayableMoveForPlayer = true;
                _AvailableSquares.Add(cellVM);
            }
        }

        public void MoveEnded()
        {
            _StartingSquare.MovingPiece = false;

            foreach (var square in _AvailableSquares)
                square.PlayableMoveForPlayer = false;
        }
    }
}
