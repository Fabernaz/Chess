using System;
using System.Collections.Generic;
using ChessCore.Properties;

namespace ChessCore
{
    public class Rook : Piece
    {
        public override bool CanPin => true;

        public Rook(Color color, SquareCoordinate position)
            :base(color, position)
        { }

        protected override IEnumerable<SquareCoordinate> GetAvailableMoves(Board board)
        {
            return MoveUtilities.GetLineAvailability(CurrentCoordinate.Value, board, Color, SquareInfluenceType.Mobility);
        }

        protected override IEnumerable<SquareCoordinate> GetNewControlledSquares(Board board)
        {
            return MoveUtilities.GetLineAvailability(CurrentCoordinate.Value, board, Color, SquareInfluenceType.Control);
        }

        public override bool IsPieceMove(SquareCoordinate startingCoordinate, SquareCoordinate endingCoordinate, Piece capturedPiece)
        {
            return startingCoordinate.IsOnRankOrFile(endingCoordinate);
        }

        protected override Uri GetImageUri()
        {
            return new Uri("pack://application:,,,/Pieces/Images/"
                + Color.ToString()
                + "Rook.png");
        }

        public override string GetMoveRepresentation()
        {
            return Resource.RookLetter;
        }
    }
}
