using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessCore
{
    public abstract class Piece
    {
        protected IEnumerable<SquareCoordinate> _availableMoves;

        public Color Color { get; }

        public Square CurrentSquare { get; set; }

        public bool Captured { get; set; }

        public bool HasBeenMoved { get; private set; }

        public abstract bool CanPin { get; }

        public abstract bool IsPieceMove(Square startingCoordinate, Square endingCoordinate, Piece capturedPiece);

        public void OnPieceMoved(Square newPosition)
        {
            HasBeenMoved = true;
            CurrentSquare = newPosition;
        }

        public void OnPieceCaptured()
        {
            Captured = true;
            _availableMoves = null;
        }

        #region Constructors

        public Piece(Color color)
        {
            Color = color;
        }

        #endregion

        public virtual bool CanJumpOverPieces()
        {
            return false;
        }

        public object Clone()
        {
            return base.MemberwiseClone();
        }

        protected abstract IEnumerable<SquareCoordinate> GetNewControlledSquares(Board board);

        public abstract string GetNotation();

        public IEnumerable<SquareCoordinate> GetAvailableMoves()
        {
            return _availableMoves;
        }

        public IEnumerable<Piece> GetAttackedPieces(Board board)
        {
            return _availableMoves.Where(coordinate => board.IsAnyOpponentPieceInSquare(coordinate, Color))
                                  .Select(coordinate => board[coordinate].Piece);
        }

        internal void ResetAvailableMoves(Board board)
        {
            _availableMoves = CurrentSquare != null
                ? GetAvailableMoves(board)
                         .Where(coordinate => !board.WouldBeInCheckAfterMove(CurrentSquare, board[coordinate]))
                : new List<SquareCoordinate>();
        }

        internal IEnumerable<SquareCoordinate> GetControlledSquares(Board board)
        {
            return CurrentSquare != null
                ? GetNewControlledSquares(board)
                : new List<SquareCoordinate>();
        }

        protected abstract IEnumerable<SquareCoordinate> GetAvailableMoves(Board board);
    }
}
