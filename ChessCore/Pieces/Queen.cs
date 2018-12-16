using System;
using System.Collections.Generic;
using System.Linq;
using ChessCore.Properties;

namespace ChessCore
{
    public class Queen : Piece
    {
        public override bool CanPin => true;

        public Queen(Color color, Position position)
            :base(color, position)
        { }

        protected override IEnumerable<Position> GetAvailableMoves(Board board)
        {
            return MoveUtilities.GetDiagonalAvailability(Position.Value, board, Color, SquareInfluenceType.Mobility)
              .Concat(MoveUtilities.GetLineAvailability(Position.Value, board, Color, SquareInfluenceType.Mobility));
        }

        protected override IEnumerable<Position> GetNewControlledSquares(Board board)
        {
            return MoveUtilities.GetDiagonalAvailability(Position.Value, board, Color, SquareInfluenceType.Control)
              .Concat(MoveUtilities.GetLineAvailability(Position.Value, board, Color, SquareInfluenceType.Control));
        }

        public override bool IsPieceMove(Position startingPosition, Position endingPosition, Piece capturedPiece)
        {
            return startingPosition.IsOnRankFileOrDiagonal(endingPosition);
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
