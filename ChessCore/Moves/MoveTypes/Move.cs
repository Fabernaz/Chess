using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCore
{
    public class Move : MoveBase
    {
        #region Fields

        private readonly Move _ambiguousMove;
        private readonly Piece _promotedTo;
        private readonly SquareCoordinate _startingCoordinate;
        private readonly SquareCoordinate _endingCoordinate;
        private readonly Piece _movedPiece;
        private readonly Piece _capturedPiece;
        private readonly PieceMove _moveInfo;

        #endregion

        #region Constructors

        internal Move(SquareCoordinate startingCoordinate,
                             SquareCoordinate endingCoordinate,
                             Piece movedPiece,
                             Piece capturedPiece,
                             bool isCapture,
                             Move ambiguousMove,
                             Piece promotedTo)
            : base(isCapture)
        {
            _startingCoordinate = startingCoordinate;
            _endingCoordinate = endingCoordinate;
            _movedPiece = movedPiece;
            _capturedPiece = capturedPiece;
            _ambiguousMove = ambiguousMove;
            _promotedTo = promotedTo;

            _moveInfo = new PieceMove(_startingCoordinate,
                                     _endingCoordinate,
                                     _movedPiece,
                                     _capturedPiece);
        }

        #endregion

        #region Play

        internal override void OnMovePlayed()
        {
            _movedPiece.OnPieceMoved(_endingCoordinate);
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
            var endingFile = MoveUtilities.GetFileFromInt(_endingCoordinate.File);
            var rank = _endingCoordinate.Rank;
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
            else if (_ambiguousMove._startingCoordinate.Rank == _startingCoordinate.Rank)
                return MoveUtilities.GetFileFromInt(_startingCoordinate.File);
            else
                return _startingCoordinate.Rank.ToString();
        }

        private string GetPieceNotation()
        {
            var startingFile = MoveUtilities.GetFileFromInt(_startingCoordinate.File);
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
                ? NextMoveAllowedEnPassant.BuildAllowedMove(_endingCoordinate.File, _movedPiece.Color)
                : NextMoveAllowedEnPassant.BuildNotAllowedMove();

        }

        internal override SquareCoordinate GetMovedPieceEndingSquare()
        {
            return _endingCoordinate;
        }

        private bool AllowEnPassantOnNextMoves()
        {
            return _movedPiece is Pawn
               && _endingCoordinate.GetAbsRankDifference(_startingCoordinate) == 2;
        }

        #endregion
    }
}
