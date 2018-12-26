namespace ChessCore
{
    internal class PieceMoveInfo
    {
        public Piece MovedPiece { get; }
        public Square StartingSquare { get; }
        public Square EndingSquare { get; }
        public bool IsCapture { get; }

        public PieceMoveInfo(Piece movedPiece, Square startingSquare, Square endingSquare)
        {
            MovedPiece = movedPiece;
            StartingSquare = startingSquare;
            EndingSquare = endingSquare;
            IsCapture = endingSquare.ContainsPiece();
        }
    }
}
