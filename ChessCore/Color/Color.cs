namespace ChessCore
{
    public interface IColor
    {
        bool IsBlack { get; }
        bool IsWhite { get; }
    }

    public class Color
    {
        #region Fields

        private readonly PieceColor _Color;

        #endregion

        #region Properties

        internal int MovingDirection { get; }
        internal int FirstRank { get; }
        internal int EnPassantStartingRank { get; }
        internal int EnPassantEndingRank { get; }
        internal int PawnFirstRank { get; }
        internal Color OpponentColor => _Color == PieceColor.White ? Black : White;

        public bool IsBlack { get; }
        public bool IsWhite { get; }

        #endregion

        #region Constructors

        internal static Color Black => new Color(PieceColor.Black);
        internal static Color White => new Color(PieceColor.White);

        private Color(PieceColor color)
        {
            _Color = color;
            EnPassantStartingRank = color == PieceColor.White ? 4 : 5;
            PawnFirstRank = color == PieceColor.White ? 2 : 7;
            IsWhite = color == PieceColor.White;
            IsBlack = color == PieceColor.Black;
            MovingDirection = color == PieceColor.White ? 1 : -1;
            FirstRank = color == PieceColor.White ? 1 : 8;
            EnPassantEndingRank = color == PieceColor.White ? 6 : 3;
        }

        #endregion

        internal bool IsSameColor(Color color)
        {
            return Equals(color);
        }

        internal bool IsOpponentColor(Color color)
        {
            return !Equals(color);
        }

        #region Equality override

        public override bool Equals(object obj)
        {
            var other = obj as Color;

            if (other is null)
                return false;

            return _Color == other._Color;
        }

        public override int GetHashCode()
        {
            return _Color.GetHashCode();
        }

        public static bool operator ==(Color obj1, Color obj2)
        {
            return obj1.IsSameColor(obj2);
        }

        public static bool operator !=(Color obj1, Color obj2)
        {
            return obj1.IsOpponentColor(obj2);
        }

        #endregion

        public override string ToString()
        {
            return _Color == PieceColor.White
                ? "White"
                : "Black";

            //TODO: presentation logic!!
        }

        private enum PieceColor
        {
            White,
            Black
        }
    }
}

