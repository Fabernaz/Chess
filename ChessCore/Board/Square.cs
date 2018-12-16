using System;

namespace ChessCore
{
    public class Square
    {
        private Piece _piece;
        private bool _isControlledByBlack;
        private bool _isControlledByWhite;

        public BoardCellColor Color { get; }

        public SquareCoordinate Coordinate { get; }

        public bool IsControlledByWhite
        {
            get { return _isControlledByWhite; }
            internal set
            {
                if (_isControlledByWhite == value)
                    return;

                _isControlledByWhite = value;
                ControlledByWhiteChanged?.Invoke(this, new EventArgs());
            }
        }

        public bool IsControlledByBlack
        {
            get { return _isControlledByBlack; }
            internal set
            {
                if (_isControlledByBlack == value)
                    return;

                _isControlledByBlack = value;
                ControlledByBlackChanged?.Invoke(this, new EventArgs());
            }
        }

        public Piece Piece
        {
            get { return _piece; }
            internal set
            {
                _piece = value;
                PieceChanged?.Invoke(this, new EventArgs());
            }
        }

        public event EventHandler PieceChanged;
        public event EventHandler ControlledByWhiteChanged;
        public event EventHandler ControlledByBlackChanged;

        public Square(SquareCoordinate position)
        {
            Coordinate = position;
            Color = GetColor();
        }

        private BoardCellColor GetColor()
        {
            return (Coordinate.Rank - Coordinate.File) % 2 == 0 ?
                        BoardCellColor.Dark :
                        BoardCellColor.Light;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Square))
                return false;

            var valObj = obj as Square;
            return valObj.Coordinate == Coordinate;
        }

        public bool ContainsPiece()
        {
            return Piece != null;
        }

        public bool ContainsOpponentPiece(Color color)
        {
            return ContainsPiece()
                && Piece.Color.IsOpponentColor(color);
        }

        public override int GetHashCode()
        {
            return Coordinate.GetHashCode();
        }
    }
}
