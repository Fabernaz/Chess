using System;
using System.Collections.Generic;
using ChessCore.Properties;

namespace ChessCore
{
    public class Rook : Piece
    {
        public override bool CanPin => true;

        public Rook(Color color, Position position)
            :base(color, position)
        { }

        protected override IEnumerable<Position> GetAvailableMoves(Board board)
        {
            return MoveUtilities.GetLineAvailability(Position.Value, board, Color, SquareInfluenceType.Mobility);
        }

        protected override IEnumerable<Position> GetNewControlledSquares(Board board)
        {
            return MoveUtilities.GetLineAvailability(Position.Value, board, Color, SquareInfluenceType.Control);
        }

        public override bool IsPieceMove(Position startingPosition, Position endingPosition, Piece capturedPiece)
        {
            return startingPosition.IsOnRankOrFile(endingPosition);
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
