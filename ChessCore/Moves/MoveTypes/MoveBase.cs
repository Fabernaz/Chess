using System;
using System.Collections.Generic;

namespace ChessCore
{
    public abstract class MoveBase
    {
        protected bool _isCheck;

        internal bool Played { get; private set; }

        public virtual bool IsCapture { get; }

        public MoveBase(bool isCapture)
        {
            IsCapture = isCapture;
        }

        internal virtual void PlayMove()
        {
            OnMovePlayed();

            Played = true;
        }

        internal abstract void OnMovePlayed();

        internal abstract MoveOperations GetMoveOperations();

        internal abstract PiecesAffectedByMove GetAffectedPieces();

        internal abstract SquareCoordinate GetMovedPieceEndingSquare();

        internal virtual NextMoveAllowedEnPassant GetAllowEnPassantOnNextMoveInfo()
        {
            return NextMoveAllowedEnPassant.BuildNotAllowedMove();
        }

        protected string GetIsCheckNotation()
        {
            return _isCheck ?
                "+" :
                String.Empty;
        }

        protected string GetCaptureRepresentation(bool isCapture)
        {
            return isCapture ?
                "x"
                : String.Empty;
        }
    }

    #region Info retrieving classes

    internal class PiecesAffectedByMove
    {
        internal Piece MovedPiece { get; set; }
        internal Piece SecondaryPiece { get; set; }
        internal Piece CapturedPiece { get; set; }

        public PiecesAffectedByMove(Piece movedPiece)
            : this(movedPiece, null)
        { }

        public PiecesAffectedByMove(Piece movedPiece, Piece capturedPiece)
            : this(movedPiece, capturedPiece, null)
        { }

        public PiecesAffectedByMove(Piece movedPiece, Piece capturedPiece, Piece secondaryPiece)
        {
            MovedPiece = movedPiece;
            SecondaryPiece = secondaryPiece;
            CapturedPiece = capturedPiece;
        }
    }

    internal class MoveOperations
    {
        internal IList<Piece> CapturedPieces { get; }
        internal IList<PieceMove> MovedPieces { get; }

        internal MoveOperations()
        {
            CapturedPieces = new List<Piece>();
            MovedPieces = new List<PieceMove>();
        }
    }

    #endregion
}