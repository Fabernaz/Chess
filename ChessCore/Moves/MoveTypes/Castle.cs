using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCore
{
    public class Castle : Move
    {
        private readonly CastleType _type;
        private readonly Board _board;
        private readonly Rook _rook;
        private readonly Square _rookStartingPosition;
        private readonly Square _rookEndingPosition;

        private King King { get { return MovedPiece as King; } }

        public Castle(King king, Rook rook, Board board, CastleType type)
            : base(true, 
                   king,
                   null,
                   board.GetSquare(king.GetCastleStartingPosition()),
                   board.GetSquare(king.GetCastleEndingPosition(type)),
                   null)
        {
            _type = type;
            _board = board;
            _rook = rook;
            _rookStartingPosition = board.GetSquare(King.GetCastleRookStartingSquare(type));
            _rookEndingPosition = board.GetSquare(King.GetCastleRookEndingPosition(type));
        }

        internal override MoveOperations GetMoveOperations()
        {
            var ret = new MoveOperations(King.Color);

            ret.MovedPieces.Add(new PieceMoveInfo(King, StartingSquare, EndingSquare));
            ret.MovedPieces.Add(new PieceMoveInfo(_rook, _rookStartingPosition, _rookEndingPosition));

            return ret;
        }

        protected override void OnMovePlayed()
        {
            base.OnMovePlayed();
            _rook.OnPieceMoved(_rookEndingPosition);
        }

        #region Notation

        public override string ToString()
        {
            var ret = "O-O";
            if (_type == CastleType.QueenSide)
                ret += "-O";
            ret += GetCheckNotation();
            return ret;
        }

        #endregion
    }
}
