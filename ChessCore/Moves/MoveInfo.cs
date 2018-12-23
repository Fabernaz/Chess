namespace ChessCore
{
    internal class PieceMove
    {
        internal Square EndingSquare { get; }
        internal Square StartingSquare { get; }
        internal Piece MovedPiece { get; }
        internal Piece CapturedPiece { get; }
        internal bool IsCapture { get { return CapturedPiece != null; } }

        internal PieceMove(Square startingCoordinate, 
                           Square endingCoordinate,
                           Piece movedPiece,
                           Piece capturedPiece)
        {
            StartingSquare = startingCoordinate;
            EndingSquare = endingCoordinate;
            MovedPiece = movedPiece;
            CapturedPiece = capturedPiece;
        }
    }
}
