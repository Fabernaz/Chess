using System;
using System.Collections.Generic;
using ChessCore.Properties;

namespace ChessCore
{
    public class Rook : Piece
    {
        public override bool CanPin => true;

        public Rook(Color color)
            :base(color)
        { }

        protected override IEnumerable<SquareCoordinate> GetAvailableMoves(Board board)
        {
            return MoveUtilities.GetLineAvailability(CurrentSquare.Coordinate, board, Color, SquareInfluenceType.Mobility);
        }

        protected override IEnumerable<SquareCoordinate> GetNewControlledSquares(Board board)
        {
            return MoveUtilities.GetLineAvailability(CurrentSquare.Coordinate, board, Color, SquareInfluenceType.Control);
        }

        public override bool IsPieceMove(Square startingSquare, Square endingSquare, Piece capturedPiece)
        {
            return startingSquare.Coordinate.IsOnRankOrFile(endingSquare.Coordinate);
        }

        public override string GetNotation()
        {
            return Resource.RookLetter;
        }
    }
}
