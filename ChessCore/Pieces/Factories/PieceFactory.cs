using System;

namespace ChessCore
{
    internal class PieceFactory
    {
        internal Piece CreatePiece(PromoteTo promoteTo, Color color)
        {
            switch(promoteTo)
            {
                case PromoteTo.Bishop:
                    return new Bishop(color);
                case PromoteTo.Knight:
                    return new Knight(color);
                case PromoteTo.Queen:
                    return new Queen(color);
                case PromoteTo.Rook:
                    return new Rook(color);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
