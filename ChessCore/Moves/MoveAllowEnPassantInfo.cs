using System;

namespace ChessCore
{
    internal class MoveAllowEnPassantInfo
    {
        private int _lastMoveFile;

        internal bool AllowEnPassant { get; }

        internal static MoveAllowEnPassantInfo BuildNotAllowedMove()
        {
            return new MoveAllowEnPassantInfo(false, 0, null);
        }

        internal static MoveAllowEnPassantInfo BuildAllowedMove(int rank, Color movedPieceColor)
        {
            return new MoveAllowEnPassantInfo(true, rank, movedPieceColor);
        }

        private MoveAllowEnPassantInfo(bool isAllowed, int file, Color movedPieceColor)
        {
            _lastMoveFile = file;
            AllowEnPassant = isAllowed;
        }

        internal bool IsEnPassantRankAndFile(Color movedPawnColor, int startingRank, int endingRank, int startingFile, int endingFile)
        {
            return IsEnPassantFile(startingFile)
                && endingFile == _lastMoveFile
                && startingRank == movedPawnColor.OpponentColor.EnPassantStartingRank
                && endingRank == movedPawnColor.EnPassantEndingRank;
        }

        internal bool IsEnPassantFile(int startingFile)
        {
            return Math.Abs(startingFile - _lastMoveFile) == 1;
        }
    }
}
