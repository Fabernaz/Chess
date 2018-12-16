namespace ChessCore
{
    internal class PieceMove
    {
        internal Position EndingPosition { get; }
        internal Position StartingPosition { get; }
        internal Piece MovedPiece { get; }
        internal Piece CapturedPiece { get; }
        internal bool IsCapture { get { return CapturedPiece != null; } }

        internal PieceMove(Position startingPosition, 
                           Position endingPosition,
                           Piece movedPiece,
                           Piece capturedPiece)
        {
            StartingPosition = startingPosition;
            EndingPosition = endingPosition;
            MovedPiece = movedPiece;
            CapturedPiece = capturedPiece;
        }
    }
}
