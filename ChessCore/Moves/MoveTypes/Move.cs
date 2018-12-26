using System;
using System.Collections.Generic;

namespace ChessCore
{
    public class Move
    {
        #region Notation consts

        protected const char CHECK_NOTATION = '+';
        protected const char CHECK_MATE_NOTATION = '#';
        protected const char CAPTURE_NOTATION = 'x';

        #endregion

        #region Fields

        protected readonly Square _ambiguousMoveStartingSquare;

        #endregion

        #region Properties

        internal Square StartingSquare { get; }

        internal Square EndingSquare { get; }

        internal bool IsCheck { get; set; }

        internal bool IsCheckMate { get; set; }

        internal bool Played { get; private set; }

        internal virtual bool IsCapture { get; }

        internal Piece MovedPiece { get; }

        internal Piece CapturedPiece { get; }

        #endregion

        #region Constructors

        public Move(bool isCapture, Piece movedPiece, Piece capturedPiece, Square startingSquare, Square endingSquare, Square ambiguousMoveStartingSquare)
        {
            IsCapture = isCapture;
            MovedPiece = movedPiece;
            EndingSquare = endingSquare;
            StartingSquare = startingSquare;
            CapturedPiece = capturedPiece;
            _ambiguousMoveStartingSquare = ambiguousMoveStartingSquare;
        }

        public Move(Square startingSquare, Square endingSquare, Square ambiguousMoveStartingSquare)
            :this(endingSquare.ContainsPiece(),
                  startingSquare.Piece,
                  endingSquare.Piece,
                  startingSquare,
                  endingSquare,
                  ambiguousMoveStartingSquare)
        { }

        #endregion

        #region Play move

        internal void PlayMove()
        {
            OnMovePlayed();

            Played = true;
        }

        protected virtual void OnMovePlayed()
        {
            MovedPiece.OnPieceMoved(EndingSquare);
            CapturedPiece?.OnPieceCaptured();
        }

        internal virtual MoveOperations GetMoveOperations()
        {
            var ret = new MoveOperations(MovedPiece.Color);
            if (IsCapture)
                ret.CapturedPieces.Add(CapturedPiece);
            ret.MovedPieces.Add(new PieceMoveInfo(MovedPiece, StartingSquare, EndingSquare));
            return ret;
        }

        #endregion

        #region Notation

        protected string GetCheckNotation()
        {
            return IsCheckMate ?
                CHECK_MATE_NOTATION.ToString() :
                IsCheck ? CHECK_NOTATION.ToString() : String.Empty;
        }

        protected string GetCaptureNotation(bool isCapture)
        {
            return isCapture ?
                CAPTURE_NOTATION.ToString()
                : String.Empty;
        }

        protected string GetDisambiguating()
        {
            if (_ambiguousMoveStartingSquare == null)
                return String.Empty;
            else if (_ambiguousMoveStartingSquare.Coordinate.File == StartingSquare.Coordinate.File)
                return StartingSquare.Coordinate.Rank.ToString();
            else
                return MoveUtilities.GetFileFromInt(StartingSquare.Coordinate.File);
        }

        protected string GetPieceNotation()
        {
            var startingFile = MoveUtilities.GetFileFromInt(StartingSquare.Coordinate.File);
            return MovedPiece is Pawn && IsCapture
                ? startingFile
                : MovedPiece.GetNotation();
        }

        public override string ToString()
        {
            var piece = GetPieceNotation();
            var endingFile = MoveUtilities.GetFileFromInt(EndingSquare.Coordinate.File);
            var rank = EndingSquare.Coordinate.Rank;
            var isCapture = GetCaptureNotation(IsCapture);
            var check = GetCheckNotation();
            var disambiguating = GetDisambiguating();

            return string.Format("{0}{1}{2}{3}{4}{5}", piece,
                                                       disambiguating,
                                                       isCapture,
                                                       endingFile,
                                                       rank,
                                                       check);
        }

        #endregion

        #region Helpers

        internal MoveAllowEnPassantInfo GetAllowEnPassantOnNextMoveInfo()
        {
            return AllowEnPassantOnNextMoves()
                ? MoveAllowEnPassantInfo.BuildAllowedMove(EndingSquare.Coordinate.File, MovedPiece.Color)
                : MoveAllowEnPassantInfo.BuildNotAllowedMove();

        }

        private bool AllowEnPassantOnNextMoves()
        {
            return MovedPiece is Pawn
               && EndingSquare.Coordinate.GetAbsRankDifference(StartingSquare.Coordinate) == 2;
        }

        #endregion
    }

    #region Info retrieving classes

    internal class MoveOperations
    {
        internal IList<Piece> CapturedPieces { get; }
        internal IList<PieceMoveInfo> MovedPieces { get; }
        internal IList<PieceCoordinatePair> AddedPieces { get; }
        internal Color MovingColor { get; }

        internal MoveOperations(Color movingColor)
        {
            MovingColor = movingColor;
            CapturedPieces = new List<Piece>();
            MovedPieces = new List<PieceMoveInfo>();
            AddedPieces = new List<PieceCoordinatePair>();
        }
    }

    #endregion
}