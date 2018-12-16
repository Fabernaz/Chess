using System;
using System.Collections.Generic;
using ChessCore.Properties;

namespace ChessCore
{
    public class King : Piece
    {
        public override bool CanPin => false;

        public King(Color color, Position startingPosition)
            : base(color, startingPosition)
        { }

        internal Position GetCastleRookEndingPosition(CastleType type)
        {
            switch (type)
            {
                case CastleType.KingSide:
                    return new Position(Color.FirstRank, 6);
                case CastleType.QueenSide:
                    return new Position(Color.FirstRank, 4);
                default:
                    throw new NotImplementedException();
            }
        }

        internal ISet<Position> GetCastlingSquares(CastleType type)
        {
            switch (type)
            {
                case CastleType.KingSide:
                    return new HashSet<Position>
                    {
                        new Position(Color.FirstRank, 5),
                        new Position(Color.FirstRank, 6),
                        new Position(Color.FirstRank, 7)
                    };
                case CastleType.QueenSide:
                    return new HashSet<Position>
                    {
                        new Position(Color.FirstRank, 5),
                        new Position(Color.FirstRank, 4),
                        new Position(Color.FirstRank, 3)
                    };
                default:
                    throw new NotImplementedException();
            }
        }

        internal Position GetCastleRookStartingSquare(CastleType type)
        {
            switch (type)
            {
                case CastleType.KingSide:
                    return new Position(Color.FirstRank, 8);
                case CastleType.QueenSide:
                    return new Position(Color.FirstRank, 1);
                default:
                    throw new NotImplementedException();
            }
        }

        internal Position GetCastleEndingPosition(CastleType type)
        {
            switch (type)
            {
                case CastleType.KingSide:
                    return new Position(Color.FirstRank, 7);
                case CastleType.QueenSide:
                    return new Position(Color.FirstRank, 3);
                default:
                    throw new NotImplementedException();
            }
        }

        internal IEnumerable<Position> GetCastleInBetweenPositions(CastleType type)
        {
            switch (type)
            {
                case CastleType.KingSide:
                    yield return new Position(Color.FirstRank, 6);
                    yield return new Position(Color.FirstRank, 7);
                    break;
                case CastleType.QueenSide:
                    yield return new Position(Color.FirstRank, 4);
                    yield return new Position(Color.FirstRank, 3);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        internal Position GetCastleStartingPosition()
        {
            return new Position(Color.FirstRank, 5);
        }

        protected override IEnumerable<Position> GetAvailableMoves(Board board)
        {
            return MoveUtilities.GetKingAvailability(this, Position.Value, board, SquareInfluenceType.Mobility)
                ;
        }

        protected override IEnumerable<Position> GetNewControlledSquares(Board board)
        {
            return MoveUtilities.GetKingAvailability(this, Position.Value, board, SquareInfluenceType.Control)
                ;
        }

        public override bool IsPieceMove(Position startingPosition, Position endingPosition, Piece capturedPiece)
        {
            return MoveUtilities.IsKingValidMove(startingPosition, endingPosition);
        }

        public override string GetMoveRepresentation()
        {
            return Resource.KingLetter;
        }

        protected override Uri GetImageUri()
        {
            return new Uri("pack://application:,,,/Pieces/Images/"
                + Color.ToString()
                + "King.png");
        }
    }

    public enum CastleType
    {
        KingSide,
        QueenSide
    }
}
