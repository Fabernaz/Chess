using System;
using System.Collections.Generic;
using System.Linq;
using ChessCore.Properties;

namespace ChessCore
{
    public class Queen : Piece
    {
        public override bool CanPin => true;

        public Queen(Color color)
            :base(color)
        { }

        protected override IEnumerable<SquareCoordinate> GetAvailableMoves(Board board)
        {
            return MoveUtilities.GetDiagonalAvailability(CurrentSquare.Coordinate, board, Color, SquareInfluenceType.Mobility)
              .Concat(MoveUtilities.GetLineAvailability(CurrentSquare.Coordinate, board, Color, SquareInfluenceType.Mobility));
        }

        protected override IEnumerable<SquareCoordinate> GetNewControlledSquares(Board board)
        {
            return MoveUtilities.GetDiagonalAvailability(CurrentSquare.Coordinate, board, Color, SquareInfluenceType.Control)
              .Concat(MoveUtilities.GetLineAvailability(CurrentSquare.Coordinate, board, Color, SquareInfluenceType.Control));
        }

        public override bool IsPieceMove(Square startingSquare, Square endingSquare, Piece capturedPiece)
        {
            return startingSquare.Coordinate.IsOnRankFileOrDiagonal(endingSquare.Coordinate);
        }

        public override string GetMoveRepresentation()
        {
            return Resource.QueenLetter;
        }
    }
}
