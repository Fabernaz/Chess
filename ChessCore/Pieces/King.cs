using System;
using System.Collections.Generic;
using ChessCore.Properties;

namespace ChessCore
{
    public class King : Piece
    {
        public override bool CanPin => false;

        public King(Color color)
            : base(color)
        { }

        internal SquareCoordinate GetCastleRookEndingPosition(CastleType type)
        {
            switch (type)
            {
                case CastleType.KingSide:
                    return new SquareCoordinate(Color.FirstRank, 6);
                case CastleType.QueenSide:
                    return new SquareCoordinate(Color.FirstRank, 4);
                default:
                    throw new NotImplementedException();
            }
        }

        internal ISet<SquareCoordinate> GetCastlingSquares(CastleType type)
        {
            switch (type)
            {
                case CastleType.KingSide:
                    return new HashSet<SquareCoordinate>
                    {
                        new SquareCoordinate(Color.FirstRank, 5),
                        new SquareCoordinate(Color.FirstRank, 6),
                        new SquareCoordinate(Color.FirstRank, 7)
                    };
                case CastleType.QueenSide:
                    return new HashSet<SquareCoordinate>
                    {
                        new SquareCoordinate(Color.FirstRank, 5),
                        new SquareCoordinate(Color.FirstRank, 4),
                        new SquareCoordinate(Color.FirstRank, 3)
                    };
                default:
                    throw new NotImplementedException();
            }
        }

        internal SquareCoordinate GetCastleRookStartingSquare(CastleType type)
        {
            switch (type)
            {
                case CastleType.KingSide:
                    return new SquareCoordinate(Color.FirstRank, 8);
                case CastleType.QueenSide:
                    return new SquareCoordinate(Color.FirstRank, 1);
                default:
                    throw new NotImplementedException();
            }
        }

        internal SquareCoordinate GetCastleEndingPosition(CastleType type)
        {
            switch (type)
            {
                case CastleType.KingSide:
                    return new SquareCoordinate(Color.FirstRank, 7);
                case CastleType.QueenSide:
                    return new SquareCoordinate(Color.FirstRank, 3);
                default:
                    throw new NotImplementedException();
            }
        }

        internal IEnumerable<SquareCoordinate> GetCastleInBetweenPositions(CastleType type)
        {
            switch (type)
            {
                case CastleType.KingSide:
                    yield return new SquareCoordinate(Color.FirstRank, 6);
                    yield return new SquareCoordinate(Color.FirstRank, 7);
                    break;
                case CastleType.QueenSide:
                    yield return new SquareCoordinate(Color.FirstRank, 4);
                    yield return new SquareCoordinate(Color.FirstRank, 3);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        internal SquareCoordinate GetCastleStartingPosition()
        {
            return new SquareCoordinate(Color.FirstRank, 5);
        }

        protected override IEnumerable<SquareCoordinate> GetAvailableMoves(Board board)
        {
            return MoveUtilities.GetKingAvailability(this, CurrentSquare.Coordinate, board, SquareInfluenceType.Mobility)
                ;
        }

        protected override IEnumerable<SquareCoordinate> GetNewControlledSquares(Board board)
        {
            return MoveUtilities.GetKingAvailability(this, CurrentSquare.Coordinate, board, SquareInfluenceType.Control)
                ;
        }

        public override bool IsPieceMove(Square startingSquare, Square endingSquare, Piece capturedPiece)
        {
            return MoveUtilities.IsKingValidMove(startingSquare.Coordinate, endingSquare.Coordinate);
        }

        public override string GetNotation()
        {
            return Resource.KingLetter;
        }
    }

    public enum CastleType
    {
        KingSide,
        QueenSide
    }
}
