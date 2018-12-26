using System;
using Common;

namespace ChessCore
{
    internal class TemporaryMoveDisposable : IDisposable
    {
        internal Square StartingSquare { get; }
        internal Square EndingSquare { get; }
        internal Piece MovedPiece { get; }
        internal Piece CapturedPiece { get; }
        internal Square MovedPieceSquare { get; }
        internal bool IsCapture { get; }

        internal event EventHandler Disposing;

        public TemporaryMoveDisposable(Square startingSquare, Square endingSquare)
        {
            Guard.ArgumentNotNull(startingSquare, nameof(startingSquare));
            StartingSquare = startingSquare;
            MovedPiece = startingSquare.Piece;
            MovedPieceSquare = MovedPiece.CurrentSquare;
            Guard.ArgumentNotNull(endingSquare, nameof(endingSquare));
            IsCapture = endingSquare.Piece != null;
            EndingSquare = endingSquare;
            CapturedPiece = endingSquare.Piece;
            MovedPieceSquare = CapturedPiece?.CurrentSquare;

            PlayMove();
        }

        private void PlayMove()
        {
            StartingSquare.Piece = null;
            EndingSquare.Piece = MovedPiece;

            MovedPiece.CurrentSquare = EndingSquare;
            if(IsCapture)
                CapturedPiece.Captured = true;
        }

        private void RevertMove()
        {
            StartingSquare.Piece = MovedPiece;
            EndingSquare.Piece = CapturedPiece;

            MovedPiece.CurrentSquare = StartingSquare;
            if (IsCapture)
            {
                CapturedPiece.Captured = false;
                CapturedPiece.CurrentSquare = EndingSquare;
            }
                
        }

        public void Dispose()
        {
            Disposing?.Invoke(this, new EventArgs());
            RevertMove();
        }
    }
}
