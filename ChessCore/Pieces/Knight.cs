using System;
using System.Collections.Generic;
using ChessCore.Properties;

namespace ChessCore
{
    public class Knight : Piece
    {
        public override bool CanPin => false;

        public Knight(Color color)
            : base(color)
        {}

        protected override IEnumerable<SquareCoordinate> GetAvailableMoves(Board board)
        {
            return MoveUtilities.GetKnightAvailability(CurrentSquare.Coordinate, board, Color, SquareInfluenceType.Mobility)
                ;
        }

        protected override IEnumerable<SquareCoordinate> GetNewControlledSquares(Board board)
        {
            return MoveUtilities.GetKnightAvailability(CurrentSquare.Coordinate, board, Color, SquareInfluenceType.Control)
                ;
        }

        public override bool IsPieceMove(Square startingSquare, Square endingSquare, Piece capturedPiece)
        {
            return startingSquare.Coordinate.IsOnTwoOneL(endingSquare.Coordinate);
        }

        public override bool CanJumpOverPieces()
        {
            return true;
        }

        public override string GetNotation()
        {
            return Resource.KnightLetter;
        }
    }
}
