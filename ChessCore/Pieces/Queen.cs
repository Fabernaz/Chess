using System;
using System.Collections.Generic;
using System.Linq;
using ChessCore.Properties;

namespace ChessCore
{
    public class Queen : Piece
    {
        public override bool CanPin => true;

        public Queen(Color color, SquareCoordinate position)
            :base(color, position)
        { }

        protected override IEnumerable<SquareCoordinate> GetAvailableMoves(Board board)
        {
            return MoveUtilities.GetDiagonalAvailability(CurrentCoordinate.Value, board, Color, SquareInfluenceType.Mobility)
              .Concat(MoveUtilities.GetLineAvailability(CurrentCoordinate.Value, board, Color, SquareInfluenceType.Mobility));
        }

        protected override IEnumerable<SquareCoordinate> GetNewControlledSquares(Board board)
        {
            return MoveUtilities.GetDiagonalAvailability(CurrentCoordinate.Value, board, Color, SquareInfluenceType.Control)
              .Concat(MoveUtilities.GetLineAvailability(CurrentCoordinate.Value, board, Color, SquareInfluenceType.Control));
        }

        public override bool IsPieceMove(SquareCoordinate startingCoordinate, SquareCoordinate endingCoordinate, Piece capturedPiece)
        {
            return startingCoordinate.IsOnRankFileOrDiagonal(endingCoordinate);
        }

        protected override Uri GetImageUri()
        {
            return new Uri("pack://application:,,,/Pieces/Images/"
                + Color.ToString()
                + "Queen.png");
        }

        public override string GetMoveRepresentation()
        {
            return Resource.QueenLetter;
        }
    }
}
