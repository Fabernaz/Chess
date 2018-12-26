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

    public enum PromoteTo
    {
        Queen,
        Rook,
        Bishop,
        Knight
    }

    public enum GameEndedReason
    {
        CheckMate,
        TimeUp,
        Abandon,
        Resign,
        StaleMate,
        FiftyRule,
        ThreefoldRepetition
    }

    public enum GameOutcome
    {
        WhiteWins,
        Draw,
        BlackWins
    }
}
