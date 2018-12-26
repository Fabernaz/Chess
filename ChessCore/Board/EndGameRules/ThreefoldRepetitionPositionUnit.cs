using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCore
{
    internal class ThreefoldRepetitionPositionUnit
    {
        private readonly SquareCoordinate _startingSquare;
        private readonly SquareCoordinate _endingSquare;
        private readonly Piece _piece;
        private readonly Color _nextTurn;

        internal ThreefoldRepetitionPositionUnit(SquareCoordinate startingSquare, 
                                                 SquareCoordinate endingSquare, 
                                                 Piece piece,
                                                 Color nextTurn)
        {
            _startingSquare = startingSquare;
            _endingSquare = endingSquare;
            _piece = piece;
            _nextTurn = nextTurn;
        }

        public override bool Equals(object obj)
        {
            var other = obj as ThreefoldRepetitionPositionUnit;

            return other._startingSquare.Equals(_startingSquare)
                && other._endingSquare.Equals(_endingSquare)
                && other._piece.Equals(_piece)
                && other._nextTurn.Equals(_nextTurn);
        }

        public override int GetHashCode()
        {
            return _startingSquare.GetHashCode()
                ^ _endingSquare.GetHashCode()
                ^ _piece.GetHashCode()
                ^ _nextTurn.GetHashCode();
        }

        public static bool operator ==(ThreefoldRepetitionPositionUnit x, ThreefoldRepetitionPositionUnit y)
        {
            if (x is null)
                return false;

            return x.Equals(y);
        }
        public static bool operator !=(ThreefoldRepetitionPositionUnit x, ThreefoldRepetitionPositionUnit y)
        {
            return !(x == y);
        }
    }
}
