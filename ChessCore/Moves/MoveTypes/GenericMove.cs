using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCore
{
    public class GenericMove : MoveBase
    {
        #region Fields

        private readonly GenericMove _ambiguousMove;
        private readonly Piece _promotedTo;
        private readonly Position _startingPosition;
        private readonly Position _endingPosition;
        private readonly Piece _movedPiece;
        private readonly Piece _capturedPiece;
        private readonly PieceMove _moveInfo;

        #endregion

        #region Constructors

        internal GenericMove(Position startingPosition,
                             Position endingPosition,
                             Piece movedPiece,
                             Piece capturedPiece,
                             bool isCapture,
                             GenericMove ambiguousMove,
                             Piece promotedTo)
            : base(isCapture)
        {
            _startingPosition = startingPosition;
            _endingPosition = endingPosition;
            _movedPiece = movedPiece;
            _capturedPiece = capturedPiece;
            _ambiguousMove = ambiguousMove;
            _promotedTo = promotedTo;

            _moveInfo = new PieceMove(_startingPosition,
                                     _endingPosition,
                                     _movedPiece,
                                     _capturedPiece);
        }

        #endregion

        #region Play

        internal override void OnMovePlayed()
        {
            _movedPiece.OnPieceMoved(_endingPosition);
            _capturedPiece?.OnPieceCaptured();
        }

        internal override MoveOperations GetMoveOperations()
        {
            var ret = new MoveOperations();
            if (IsCapture)
                ret.CapturedPieces.Add(_capturedPiece);
            ret.MovedPieces.Add(_moveInfo);
            return ret;
        }

        #endregion

        #region Notation

        public override string ToString()
        {
            var piece = GetPieceNotation();
            var endingFile = MoveUtilities.GetFileFromInt(_endingPosition.File);
            var rank = _endingPosition.Rank;
            var isCapture = GetCaptureRepresentation(IsCapture);
            var check = GetIsCheckNotation();
            var disambiguating = GetDisambiguating();
            var promotion = GetPromotion();

            return string.Format("{0}{1}{2}{3}{4}{5}{6}", piece,
                                                          disambiguating,
                                                          isCapture,
                                                          endingFile,
                                                          rank,
                                                          promotion,
                                                          check);
        }

        private string GetPromotion()
        {
            return _promotedTo == null
                ? String.Empty
                : _promotedTo.GetMoveRepresentation();
        }

        private string GetDisambiguating()
        {
            if (_ambiguousMove == null)
                return String.Empty;
            else if (_ambiguousMove._startingPosition.Rank == _startingPosition.Rank)
                return MoveUtilities.GetFileFromInt(_startingPosition.File);
            else
                return _startingPosition.Rank.ToString();
        }

        private string GetPieceNotation()
        {
            var startingFile = MoveUtilities.GetFileFromInt(_startingPosition.File);
            return _movedPiece is Pawn && IsCapture
                ? startingFile
                : _movedPiece.GetMoveRepresentation();
        }

        #endregion

        #region Helpers

        internal override PiecesAffectedByMove GetAffectedPieces()
        {
            return new PiecesAffectedByMove(_movedPiece, _capturedPiece);
        }

        internal override NextMoveAllowedEnPassant GetAllowEnPassantOnNextMoveInfo()
        {
            return AllowEnPassantOnNextMoves()
                ? NextMoveAllowedEnPassant.BuildAllowedMove(_endingPosition.File, _movedPiece.Color)
                : NextMoveAllowedEnPassant.BuildNotAllowedMove();

        }

        internal override Position GetMovedPieceEndingPosition()
        {
            return _endingPosition;
        }

        private bool AllowEnPassantOnNextMoves()
        {
            return _movedPiece is Pawn
               && _endingPosition.GetAbsRankDifference(_startingPosition) == 2;
        }

        #endregion
    }
}
