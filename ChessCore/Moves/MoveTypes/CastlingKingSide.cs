﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCore
{
    public class CastlingKingSide : MoveBase
    {
        private readonly King _king;
        private readonly Rook _rook;
        private readonly Position _kingStartingPosition;
        private readonly Position _kingEndingPosition;
        private readonly Position _rookStartingPosition;
        private readonly Position _rookEndingPosition;

        public CastlingKingSide(King king, Rook rook)
            : base(true)
        {
            _king = king;
            _rook = rook;

            _kingStartingPosition = _king.GetCastleStartingPosition();
            _kingEndingPosition = _king.GetCastleEndingPosition(CastleType.KingSide);
            _rookStartingPosition = _king.GetCastleRookStartingSquare(CastleType.KingSide);
            _rookEndingPosition = _king.GetCastleRookEndingPosition(CastleType.KingSide);
        }

        internal override PiecesAffectedByMove GetAffectedPieces()
        {
            return new PiecesAffectedByMove(_king, null, _rook);
        }

        internal override Position GetMovedPieceEndingPosition()
        {
            return _king.GetCastleEndingPosition(CastleType.KingSide);
        }

        internal override MoveOperations GetMoveOperations()
        {
            var ret = new MoveOperations();

            ret.MovedPieces.Add(new PieceMove(_kingStartingPosition, _kingEndingPosition, _king, null));
            ret.MovedPieces.Add(new PieceMove(_rookStartingPosition, _rookEndingPosition, _rook, null));

            return ret;
        }

        internal override void OnMovePlayed()
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
