using System;
using System.Collections.Generic;

namespace ChessCore
{
    public class Pawn : Piece
    {
        public override bool CanPin => false;

        public Pawn(Color color, SquareCoordinate startingCoordinate)
            :base(color, startingCoordinate)
        { }

        protected override IEnumerable<SquareCoordinate> GetAvailableMoves(Board board)
        {
            return MoveUtilities.GetPawnAvailability(CurrentCoordinate.Value, board, Color, SquareInfluenceType.Mobility)
                ;
        }

        protected override IEnumerable<SquareCoordinate> GetNewControlledSquares(Board board)
        {
            return MoveUtilities.GetPawnAvailability(CurrentCoordinate.Value, board, Color, SquareInfluenceType.Control)
                ;
        }

        internal IEnumerable<SquareCoordinate> GetFrontSquares()
        {
            return MoveUtilities.GetPawnFrontSquares(CurrentCoordinate.Value, Color);
        }

        public override bool IsPieceMove(SquareCoordinate startingCoordinate, SquareCoordinate endingCoordinate, Piece capturedPiece)
        {
            return IsValidCapturingMove(startingCoordinate, endingCoordinate, capturedPiece)
                || IsValidNonCapturingMove(startingCoordinate, endingCoordinate, capturedPiece);
        }

        private bool IsValidNonCapturingMove(SquareCoordinate startingCoordinate, SquareCoordinate endingCoordinate, Piece capturedPiece)
        {
            var vectorRankMove = Color.MovingDirection * startingCoordinate.GetRankDifference(endingCoordinate);
            return capturedPiece == null
                && startingCoordinate.IsOnSameFile(endingCoordinate)
                && vectorRankMove > 0 
                && vectorRankMove <= (HasBeenMoved ? 1 : 2);
        }

        private bool IsValidCapturingMove(SquareCoordinate startingCoordinate, SquareCoordinate endingCoordinate, Piece capturedPiece)
        {
            return capturedPiece != null
                && startingCoordinate.IsOnDiagonalAdjacentSquare(endingCoordinate);
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
