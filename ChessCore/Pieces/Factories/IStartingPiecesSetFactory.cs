using System.Collections.Generic;

namespace ChessCore
{
    public interface IStartingPiecesSetFactory
    {
        IEnumerable<PieceCoordinatePair> GetPiecesInStartingPosition();
    }

    public class PieceCoordinatePair
    {
        public Piece Piece { get; }
        public SquareCoordinate SquareCoordinate { get; }
        public PieceCoordinatePair(Piece piece, SquareCoordinate squareCoordinate)
        {
            Piece = piece;
            SquareCoordinate = squareCoordinate;
        }
    }
}
