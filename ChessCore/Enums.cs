using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCore
{
    public enum BoardCellColor
    {
        Light,
        Dark
    }

    public enum MoveType
    {
        None,
        Generic,
        CastlingKing,
        CastlingQueen,
        EnPassant
    }
}
