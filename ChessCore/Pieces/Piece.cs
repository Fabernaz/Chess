using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessCore
{
    public abstract class Piece : Model
    {
        //Never concretize this collection, LINQ deferred execution ensures that not-in-check 
        //moves are calculated after black move
        protected IEnumerable<Position> _availableMoves;

        public Color Color { get; }

        public Position? Position { get; set; }

        public bool IsBeenCaptured { get { return Position != null; } }

        public Uri ImageUri { get; }

        public bool HasBeenMoved { get; private set; }

        public abstract bool CanPin { get; }

        public abstract bool IsPieceMove(Position startingPosition, Position endingPosition, Piece capturedPiece);

        public void OnPieceMoved(Position newPosition)
        {
            HasBeenMoved = true;
            Position = newPosition;
        }

        public void OnPieceCaptured()
        {
            Position = null;
            _availableMoves = null;
        }

        public Piece(Color color, Position position)
        {
            Color = color;
            Position = position;
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

        protected abstract IEnumerable<Position> GetNewControlledSquares(Board board);

        public abstract string GetMoveRepresentation();

        //TODO: this is presentation logic!!
        protected abstract Uri GetImageUri();

        public IEnumerable<Position> GetAvailableMoves()
        {
            return _availableMoves;
        }

        public IEnumerable<Piece> GetAttackedPieces(Board board)
        {
            return _availableMoves.Where(p => board.IsAnyPieceOfDifferentColorInPosition(p, Color))
                                  .Select(p => board.GetCell(p).Piece);
        }

        internal void ResetAvailableMoves(Board board)
        {
            _availableMoves = Position.HasValue
                ? GetAvailableMoves(board)
                         .Where(s => !board.WouldBeInCheckAfterMove(board.GetCell(Position.Value), board.GetCell(s)))
                : new List<Position>();
        }

        //Never concretize this collection, LINQ deferred execution ensures that not-in-check 
        //moves are calculated after black move
        internal IEnumerable<Position> GetControlledSquares(Board board)
        {
            return Position.HasValue
                ? GetNewControlledSquares(board)
                : new List<Position>();
        }

        //Never concretize this collection, LINQ deferred execution ensures that not-in-check 
        //moves are calculated after black move
        protected abstract IEnumerable<Position> GetAvailableMoves(Board board);
    }
}
