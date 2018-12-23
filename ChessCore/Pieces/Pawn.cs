using System;
using System.Collections.Generic;

namespace ChessCore
{
    public class Pawn : Piece
    {
        public override bool CanPin => false;

        public Pawn(Color color)
            :base(color)
        { }

        protected override IEnumerable<SquareCoordinate> GetAvailableMoves(Board board)
        {
            return MoveUtilities.GetPawnAvailability(CurrentSquare.Coordinate, board, Color, SquareInfluenceType.Mobility);
        }

        protected override IEnumerable<SquareCoordinate> GetNewControlledSquares(Board board)
        {
            return MoveUtilities.GetPawnAvailability(CurrentSquare.Coordinate, board, Color, SquareInfluenceType.Control);
        }

        internal IEnumerable<SquareCoordinate> GetFrontSquares()
        {
            return MoveUtilities.GetPawnFrontSquares(CurrentSquare.Coordinate, Color);
        }

        public override bool IsPieceMove(Square startingSquare, Square endingSquare, Piece capturedPiece)
        {
            return IsValidCapturingMove(startingSquare, endingSquare, capturedPiece)
                || IsValidNonCapturingMove(startingSquare, endingSquare, capturedPiece);
        }

        private bool IsValidNonCapturingMove(Square startingSquare, Square endingSquare, Piece capturedPiece)
        {
            var vectorRankMove = Color.MovingDirection * startingSquare.Coordinate.GetRankDifference(endingSquare.Coordinate);
            return capturedPiece == null
                && startingSquare.Coordinate.IsOnSameFile(endingSquare.Coordinate)
                && vectorRankMove > 0 
                && vectorRankMove <= (HasBeenMoved ? 1 : 2);
        }

        private bool IsValidCapturingMove(Square startingSquare, Square endingSquare, Piece capturedPiece)
        {
            return capturedPiece != null
                && startingSquare.Coordinate.IsOnDiagonalAdjacentSquare(endingSquare.Coordinate);
        }

        public override string GetMoveRepresentation()
        {
            return string.Empty;
        }
    }
}
