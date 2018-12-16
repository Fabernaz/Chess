using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCore
{
    internal class EnPassant : MoveBase
    {
        #region Fields

        private readonly Pawn _movedPawn;
        private readonly Position _startingMovedPawnPosition;
        private readonly Position _endingMovedPawnPosition;
        private readonly Pawn _capturedPawn;
        private readonly Position _capturedPawnPosition;

        #endregion

        #region Constructors

        internal EnPassant(Pawn movedPawn,
                           Position startingMovedPawnPosition,
                           Position endingMovedPawnPosition,
                           Pawn capturedPawn,
                           Position capturedPawnPosition)
            : base(true)
        {
            _movedPawn = movedPawn;
            _startingMovedPawnPosition = startingMovedPawnPosition;
            _endingMovedPawnPosition = endingMovedPawnPosition;
            _capturedPawn = capturedPawn;
            _capturedPawnPosition = capturedPawnPosition;
        }

        #endregion

        #region Move

        internal override void OnMovePlayed()
        {
            _movedPawn.OnPieceMoved(_endingMovedPawnPosition);
            _capturedPawn.OnPieceCaptured();
        }

        #endregion

        #region Notation

        public override string ToString()
        {
            return string.Format("{0}x{1}{2}e.p.{3}", MoveUtilities.GetFileFromInt(_startingMovedPawnPosition.File),
                                                      MoveUtilities.GetFileFromInt(_endingMovedPawnPosition.File),
                                                      _endingMovedPawnPosition.Rank,
                                                      GetIsCheckNotation());
        }

        #endregion

        #region Helpers

        internal override PiecesAffectedByMove GetAffectedPieces()
        {
            return new PiecesAffectedByMove(_movedPawn, _capturedPawn);
        }

        internal override MoveOperations GetMoveOperations()
        {
            var ret = new MoveOperations();

            ret.MovedPieces.Add(new PieceMove(_startingMovedPawnPosition,
                                              _endingMovedPawnPosition,
                                              _movedPawn,
                                              _capturedPawn));
            ret.CapturedPieces.Add(_capturedPawn);

            return ret;
        }

        internal override Position GetMovedPieceEndingPosition()
        {
            return _endingMovedPawnPosition;
        }

        #endregion
    }

    internal class NextMoveAllowedEnPassant
    {
        private int _lastMoveFile;

        internal bool IsAllowed { get; }

        internal static NextMoveAllowedEnPassant BuildNotAllowedMove()
        {
            return new NextMoveAllowedEnPassant(false, 0, null);
        }

        internal static NextMoveAllowedEnPassant BuildAllowedMove(int rank, Color movedPieceColor)
        {
            return new NextMoveAllowedEnPassant(true, rank, movedPieceColor);
        }

        private NextMoveAllowedEnPassant(bool isAllowed, int file, Color movedPieceColor)
        {
            _lastMoveFile = file;
            IsAllowed = isAllowed;
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
