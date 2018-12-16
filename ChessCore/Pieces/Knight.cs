using System;
using System.Collections.Generic;
using ChessCore.Properties;

namespace ChessCore
{
    public class Knight : Piece
    {
        public override bool CanPin => false;

        public Knight(Color color, SquareCoordinate position)
            : base(color, position)
        {}

        protected override IEnumerable<SquareCoordinate> GetAvailableMoves(Board board)
        {
            return MoveUtilities.GetKnightAvailability(CurrentCoordinate.Value, board, Color, SquareInfluenceType.Mobility)
                ;
        }

        protected override IEnumerable<SquareCoordinate> GetNewControlledSquares(Board board)
        {
            return MoveUtilities.GetKnightAvailability(CurrentCoordinate.Value, board, Color, SquareInfluenceType.Control)
                ;
        }

        public override bool IsPieceMove(SquareCoordinate startingCoordinate, SquareCoordinate endingCoordinate, Piece capturedPiece)
        {
            return MoveUtilities.IsOnTwoOneL(startingCoordinate, endingCoordinate);
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
