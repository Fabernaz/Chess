using System;
using System.Collections.Generic;
using System.Linq;
using ChessCore.Properties;

namespace ChessCore
{
    public class Bishop : Piece
    {
        public override bool CanPin => true;

        public Bishop(Color color)
            :base(color)
        { }

        public override bool IsPieceMove(Square startingSquare, Square endingSquare, Piece capturedPiece)
        {
            return startingSquare.Coordinate.IsOnDiagonal(endingSquare.Coordinate);
        }

        protected override IEnumerable<SquareCoordinate> GetAvailableMoves(Board board)
        {
            return MoveUtilities.GetDiagonalAvailability(CurrentSquare.Coordinate, board, Color, SquareInfluenceType.Mobility)
                ;
        }

        protected override IEnumerable<SquareCoordinate> GetNewControlledSquares(Board board)
        {
            return MoveUtilities.GetDiagonalAvailability(CurrentSquare.Coordinate, board, Color, SquareInfluenceType.Control)
                ;
        }

        public override string GetMoveRepresentation()
        {
            return Resource.BishopLetter;
        }
    }
}
