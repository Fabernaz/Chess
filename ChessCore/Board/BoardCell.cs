using System;

namespace ChessCore
{
    public class BoardCell : Model
    {
        private Piece _piece;
        private bool _isControlledByBlack;
        private bool _isControlledByWhite;

        public BoardCellColor Color { get; }

        public Position Position { get; }

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

        public BoardCell(Position position)
        {
            Position = position;
            Color = GetColor();
        }

        private BoardCellColor GetColor()
        {
            return (Position.Rank - Position.File) % 2 == 0 ?
                        BoardCellColor.Dark :
                        BoardCellColor.Light;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is BoardCell))
                return false;

            var valObj = obj as BoardCell;
            return valObj.Position == Position;
        }

        public bool ContainsPiece()
        {
            return Piece != null;
        }

        public bool ContainsPieceOfDifferentColor(Color color)
        {
            return ContainsPiece()
                && Piece.Color.IsOpponentColor(color);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }
    }
}
