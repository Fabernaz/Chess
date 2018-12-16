using System.Collections.Generic;

namespace ChessCore
{
    public interface IStartingPiecesSetFactory
    {
        IEnumerable<Piece> GetPiecesInStartingPosition();
    }
}
