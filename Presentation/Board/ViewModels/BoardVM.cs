using System.Collections.Generic;
using System.Linq;
using ChessCore;

namespace Presentation
{
    public interface IBoardVM
    {
        IEnumerable<BoardCellVM> Cells { get; }
        Color NextMoveTurn { get; }

        void OnMoveMade(BoardCellVM starting, BoardCellVM ending);
    }

    public class BoardVM : ReactiveModelViewModelBase<Board>, IBoardVM
    {
        private List<BoardCellVM> _availableSquares;
        private BoardCellVM _startingSquare;

        public IEnumerable<BoardCellVM> Cells { get; }

        public Color NextMoveTurn { get { return Model.NextMoveTurn; } }

        public BoardVM(Board model)
            : base(model)
        {
            Cells = model.GetSquares()
                         .Select(m => new BoardCellVM(m, this))
                         .ToList(); //This is to end deferred execution
        }

        public void OnMoveMade(BoardCellVM startingCell, BoardCellVM endingCell)
        {
            Model.TryPlayMove(startingCell.Model, endingCell.Model);
        }

        public void MoveStarted(BoardCellVM startingSquare)
        {
            _availableSquares = new List<BoardCellVM>();
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
