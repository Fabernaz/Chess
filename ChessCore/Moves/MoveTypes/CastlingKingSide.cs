using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCore
{
    public class CastlingKingSide : MoveBase
    {
        private readonly Board _board;
        private readonly King _king;
        private readonly Rook _rook;
        private readonly Square _kingStartingPosition;
        private readonly Square _kingEndingPosition;
        private readonly Square _rookStartingPosition;
        private readonly Square _rookEndingPosition;

        public CastlingKingSide(King king, Rook rook, Board board)
            : base(true)
        {
            _board = board;
            _king = king;
            _rook = rook;

            _kingStartingPosition = board.GetSquare(_king.GetCastleStartingPosition());
            _kingEndingPosition = board.GetSquare(_king.GetCastleEndingPosition(CastleType.KingSide));
            _rookStartingPosition = board.GetSquare(_king.GetCastleRookStartingSquare(CastleType.KingSide));
            _rookEndingPosition = board.GetSquare(_king.GetCastleRookEndingPosition(CastleType.KingSide));
        }

        internal override PiecesAffectedByMove GetAffectedPieces()
        {
            return new PiecesAffectedByMove(_king, null, _rook);
        }

        internal override Square GetMovedPieceEndingSquare()
        {
            return _board.GetSquare(_king.GetCastleEndingPosition(CastleType.KingSide));
        }

        internal override MoveOperations GetMoveOperations()
        {
            var ret = new MoveOperations();

            ret.MovedPieces.Add(new PieceMove(_kingStartingPosition, _kingEndingPosition, _king, null));
            ret.MovedPieces.Add(new PieceMove(_rookStartingPosition, _rookEndingPosition, _rook, null));

            return ret;
        }

        protected override void OnMovePlayed()
        {
            _king.OnPieceMoved(_kingEndingPosition);
            _rook.OnPieceMoved(_rookEndingPosition);
        }

        #region Notation

        public override string ToString()
        {
            return "O-O";
        }

        #endregion
    }
}
