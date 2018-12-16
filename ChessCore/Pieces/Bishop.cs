using System;
using System.Collections.Generic;
using System.Linq;
using ChessCore.Properties;

namespace ChessCore
{
    public class Bishop : Piece
    {
        public override bool CanPin => true;

        public Bishop(Color color, SquareCoordinate startingCoordinate)
            :base(color, startingCoordinate)
        { }

        protected override Uri GetImageUri()
        {
            return new Uri("pack://application:,,,/Pieces/Images/" 
                + Color.ToString() 
                + "Bishop.png");
        }

        public override bool IsPieceMove(SquareCoordinate startingCoordinate, SquareCoordinate endingCoordinate, Piece capturedPiece)
        {
            return startingCoordinate.IsOnDiagonal(endingCoordinate);
        }

        protected override IEnumerable<SquareCoordinate> GetAvailableMoves(Board board)
        {
            return MoveUtilities.GetDiagonalAvailability(CurrentCoordinate.Value, board, Color, SquareInfluenceType.Mobility)
                ;
        }

        protected override IEnumerable<SquareCoordinate> GetNewControlledSquares(Board board)
        {
            return MoveUtilities.GetDiagonalAvailability(CurrentCoordinate.Value, board, Color, SquareInfluenceType.Control)
                ;
        }

        public override string GetMoveRepresentation()
        {
            return Resource.BishopLetter;
        }
    }
}
