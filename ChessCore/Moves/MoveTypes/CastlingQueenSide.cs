using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCore
{
    public class CastlingQueenSide : MoveBase
    {
        private readonly Board _board;
        private readonly King _king;
        private readonly Rook _rook;
        private readonly SquareCoordinate _kingStartingPosition;
        private readonly SquareCoordinate _kingEndingPosition;
        private readonly SquareCoordinate _rookStartingPosition;
        private readonly SquareCoordinate _rookEndingPosition;

        public CastlingQueenSide(King king, Rook rook, Board board)
            : base(true)
        {
            _board = board;
            _king = king;
            _rook = rook;

            _kingStartingPosition = _king.GetCastleStartingPosition();
            _kingEndingPosition = _king.GetCastleEndingPosition(CastleType.QueenSide);
            _rookStartingPosition = _king.GetCastleRookStartingSquare(CastleType.QueenSide);
            _rookEndingPosition = _king.GetCastleRookEndingPosition(CastleType.QueenSide);
        }

        internal override PiecesAffectedByMove GetAffectedPieces()
        {
            return new PiecesAffectedByMove(_king, null, _rook);
        }

        internal override Square GetMovedPieceEndingSquare()
        {
            return _board.GetSquare(_king.GetCastleEndingPosition(CastleType.QueenSide));
        }

        internal override MoveOperations GetMoveOperations()
        {
            var ret = new MoveOperations();

            ret.MovedPieces.Add(new PieceMove(_board.GetSquare(_kingStartingPosition), _board.GetSquare(_kingEndingPosition), _king, null));
            ret.MovedPieces.Add(new PieceMove(_board.GetSquare(_rookStartingPosition), _board.GetSquare(_rookEndingPosition), _rook, null));

            return ret;
        }

        protected override void OnMovePlayed()
        {
            _king.OnPieceMoved(_board.GetSquare(_kingEndingPosition));
            _rook.OnPieceMoved(_board.GetSquare(_rookEndingPosition));
        }

        #region Notation

        public override string ToString()
        {
            return "O-O";
        }

        #endregion
    }
}
