using System;
using System.Collections.Generic;
using ChessCore.Properties;

namespace ChessCore
{
    public class Knight : Piece
    {
        public override bool CanPin => false;

        public Knight(Color color, Position position)
            : base(color, position)
        {}

        protected override IEnumerable<Position> GetAvailableMoves(Board board)
        {
            return MoveUtilities.GetKnightAvailability(Position.Value, board, Color, SquareInfluenceType.Mobility)
                ;
        }

        protected override IEnumerable<Position> GetNewControlledSquares(Board board)
        {
            return MoveUtilities.GetKnightAvailability(Position.Value, board, Color, SquareInfluenceType.Control)
                ;
        }

        public override bool IsPieceMove(Position startingPosition, Position endingPosition, Piece capturedPiece)
        {
            return MoveUtilities.IsOnTwoOneL(startingPosition, endingPosition);
        }

        public override bool CanJumpOverPieces()
        {
            return true;
        }


        protected override Uri GetImageUri()
        {
            return new Uri("pack://application:,,,/Pieces/Images/"
                + Color.ToString()
                + "Knight.png");
        }

        public override string GetMoveRepresentation()
        {
            return Resource.KnightLetter;
        }
    }
}
