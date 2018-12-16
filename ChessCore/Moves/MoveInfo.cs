namespace ChessCore
{
    internal class PieceMove
    {
        internal SquareCoordinate EndingSquare { get; }
        internal SquareCoordinate StartingSquare { get; }
        internal Piece MovedPiece { get; }
        internal Piece CapturedPiece { get; }
        internal bool IsCapture { get { return CapturedPiece != null; } }

        internal PieceMove(SquareCoordinate startingCoordinate, 
                           SquareCoordinate endingCoordinate,
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
