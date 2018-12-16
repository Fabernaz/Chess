using System;
using System.Collections.Generic;

namespace ChessCore
{
    public class Pawn : Piece
    {
        public override bool CanPin => false;

        public Pawn(Color color, Position startingPosition)
            :base(color, startingPosition)
        { }

        protected override IEnumerable<Position> GetAvailableMoves(Board board)
        {
            return MoveUtilities.GetPawnAvailability(Position.Value, board, Color, SquareInfluenceType.Mobility)
                ;
        }

        protected override IEnumerable<Position> GetNewControlledSquares(Board board)
        {
            return MoveUtilities.GetPawnAvailability(Position.Value, board, Color, SquareInfluenceType.Control)
                ;
        }

        internal IEnumerable<Position> GetFrontSquares()
        {
            return MoveUtilities.GetPawnFrontSquares(Position.Value, Color);
        }

        public override bool IsPieceMove(Position startingPosition, Position endingPosition, Piece capturedPiece)
        {
            return IsValidCapturingMove(startingPosition, endingPosition, capturedPiece)
                || IsValidNonCapturingMove(startingPosition, endingPosition, capturedPiece);
        }

        private bool IsValidNonCapturingMove(Position startingPosition, Position endingPosition, Piece capturedPiece)
        {
            var vectorRankMove = Color.MovingDirection * startingPosition.GetRankDifference(endingPosition);
            return capturedPiece == null
                && startingPosition.IsOnSameFile(endingPosition)
                && vectorRankMove > 0 
                && vectorRankMove <= (HasBeenMoved ? 1 : 2);
        }

        private bool IsValidCapturingMove(Position startingPosition, Position endingPosition, Piece capturedPiece)
        {
            return capturedPiece != null
                && startingPosition.IsOnDiagonalAdjacentSquare(endingPosition);
        }

        protected override Uri GetImageUri()
        {
            return new Uri("pack://application:,,,/Pieces/Images/"
                + Color.ToString()
                + "Pawn.png");
        }

        public override string GetMoveRepresentation()
        {
            return string.Empty;
        }
    }
}
