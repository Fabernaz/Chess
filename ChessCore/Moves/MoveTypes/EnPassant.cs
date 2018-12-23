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
        private readonly Square _startingMovedPawnSquare;
        private readonly Square _endingMovedPawnSquare;
        private readonly Pawn _capturedPawn;
        private readonly Square _capturedPawnSquare;

        #endregion

        #region Constructors

        internal EnPassant(Pawn movedPawn,
                           Square startingMovedPawnSquare,
                           Square endingMovedPawnSquare,
                           Pawn capturedPawn,
                           Square capturedPawnPosition)
            : base(true)
        {
            _movedPawn = movedPawn;
            _startingMovedPawnSquare = startingMovedPawnSquare;
            _endingMovedPawnSquare = endingMovedPawnSquare;
            _capturedPawn = capturedPawn;
            _capturedPawnSquare = capturedPawnPosition;
        }

        #endregion

        #region Move

        protected override void OnMovePlayed()
        {
            _movedPawn.OnPieceMoved(_endingMovedPawnSquare);
            _capturedPawn.OnPieceCaptured();
        }

        #endregion

        #region Notation

        public override string ToString()
        {
            return string.Format("{0}x{1}{2}e.p.{3}", MoveUtilities.GetFileFromInt(_startingMovedPawnSquare.Coordinate.File),
                                                      MoveUtilities.GetFileFromInt(_endingMovedPawnSquare.Coordinate.File),
                                                      _endingMovedPawnSquare.Coordinate.Rank,
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

            ret.MovedPieces.Add(new PieceMove(_startingMovedPawnSquare,
                                              _endingMovedPawnSquare,
                                              _movedPawn,
                                              _capturedPawn));
            ret.CapturedPieces.Add(_capturedPawn);

            return ret;
        }

        internal override Square GetMovedPieceEndingSquare()
        {
            return _endingMovedPawnSquare;
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
