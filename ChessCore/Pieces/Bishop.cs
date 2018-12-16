using System;
using System.Collections.Generic;
using System.Linq;
using ChessCore.Properties;

namespace ChessCore
{
    public class Bishop : Piece
    {
        public override bool CanPin => true;

        public Bishop(Color color, Position startingPosition)
            :base(color, startingPosition)
        { }

        protected override Uri GetImageUri()
        {
            return new Uri("pack://application:,,,/Pieces/Images/" 
                + Color.ToString() 
                + "Bishop.png");
        }

        public override bool IsPieceMove(Position startingPosition, Position endingPosition, Piece capturedPiece)
        {
            return startingPosition.IsOnDiagonal(endingPosition);
        }

        protected override IEnumerable<Position> GetAvailableMoves(Board board)
        {
            return MoveUtilities.GetDiagonalAvailability(Position.Value, board, Color, SquareInfluenceType.Mobility)
                ;
        }

        protected override IEnumerable<Position> GetNewControlledSquares(Board board)
        {
            return MoveUtilities.GetDiagonalAvailability(Position.Value, board, Color, SquareInfluenceType.Control)
                ;
        }

        public override string GetMoveRepresentation()
        {
            return Resource.BishopLetter;
        }
    }
}
