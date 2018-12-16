using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessCore
{
    public abstract class Piece
    {
        //Never concretize this collection, LINQ deferred execution ensures that not-in-check 
        //moves are calculated after black move
        protected IEnumerable<SquareCoordinate> _availableMoves;

        public Color Color { get; }

        public SquareCoordinate? CurrentCoordinate { get; set; }

        public bool IsBeenCaptured { get { return CurrentCoordinate != null; } }

        public Uri ImageUri { get; }

        public bool HasBeenMoved { get; private set; }

        public abstract bool CanPin { get; }

        public abstract bool IsPieceMove(SquareCoordinate startingCoordinate, SquareCoordinate endingCoordinate, Piece capturedPiece);

        public void OnPieceMoved(SquareCoordinate newPosition)
        {
            HasBeenMoved = true;
            CurrentCoordinate = newPosition;
        }

        public void OnPieceCaptured()
        {
            CurrentCoordinate = null;
            _availableMoves = null;
        }

        public Piece(Color color, SquareCoordinate position)
        {
            Color = color;
            CurrentCoordinate = position;
            ImageUri = GetImageUri();
        }

        public virtual bool CanJumpOverPieces()
        {
            return false;
        }

        public object Clone()
        {
            return base.MemberwiseClone();
        }

        protected abstract IEnumerable<SquareCoordinate> GetNewControlledSquares(Board board);

        public abstract string GetMoveRepresentation();

        //TODO: this is presentation logic!!
        protected abstract Uri GetImageUri();

        public IEnumerable<SquareCoordinate> GetAvailableMoves()
        {
            return _availableMoves;
        }

        public IEnumerable<Piece> GetAttackedPieces(Board board)
        {
            return _availableMoves.Where(p => board.IsAnyOpponentPieceInSquare(p, Color))
                                  .Select(p => board.GetSquare(p).Piece);
        }

        internal void ResetAvailableMoves(Board board)
        {
            _availableMoves = CurrentCoordinate.HasValue
                ? GetAvailableMoves(board)
                         .Where(s => !board.WouldBeInCheckAfterMove(board.GetSquare(CurrentCoordinate.Value), board.GetSquare(s)))
                : new List<SquareCoordinate>();
        }

        //Never concretize this collection, LINQ deferred execution ensures that not-in-check 
        //moves are calculated after black move
        internal IEnumerable<SquareCoordinate> GetControlledSquares(Board board)
        {
            return CurrentCoordinate.HasValue
                ? GetNewControlledSquares(board)
                : new List<SquareCoordinate>();
        }

        //Never concretize this collection, LINQ deferred execution ensures that not-in-check 
        //moves are calculated after black move
        protected abstract IEnumerable<SquareCoordinate> GetAvailableMoves(Board board);
    }
}
