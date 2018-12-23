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
        private readonly Square _startingSquare;
        private readonly Square _endingSquare;
        private readonly Piece _movedPiece;
        private readonly Piece _capturedPiece;
        private readonly PieceMove _moveInfo;

        #endregion

        #region Constructors

        internal Move(Square startingCoordinate,
                      Square endingCoordinate,
                      Piece movedPiece,
                      Piece capturedPiece,
                      bool isCapture,
                      Move ambiguousMove,
                      Piece promotedTo)
            : base(isCapture)
        {
            _startingSquare = startingCoordinate;
            _endingSquare = endingCoordinate;
            _movedPiece = movedPiece;
            _capturedPiece = capturedPiece;
            _ambiguousMove = ambiguousMove;
            _promotedTo = promotedTo;

            _moveInfo = new PieceMove(_startingSquare,
                                     _endingSquare,
                                     _movedPiece,
                                     _capturedPiece);
        }

        #endregion

        #region Play

        protected override void OnMovePlayed()
        {
            _movedPiece.OnPieceMoved(_endingSquare);
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
            var endingFile = MoveUtilities.GetFileFromInt(_endingSquare.Coordinate.File);
            var rank = _endingSquare.Coordinate.Rank;
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
            else if (_ambiguousMove._startingSquare.Coordinate.Rank == _startingSquare.Coordinate.Rank)
                return MoveUtilities.GetFileFromInt(_startingSquare.Coordinate.File);
            else
                return _startingSquare.Coordinate.Rank.ToString();
        }

        private string GetPieceNotation()
        {
            var startingFile = MoveUtilities.GetFileFromInt(_startingSquare.Coordinate.File);
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
                ? NextMoveAllowedEnPassant.BuildAllowedMove(_endingSquare.Coordinate.File, _movedPiece.Color)
                : NextMoveAllowedEnPassant.BuildNotAllowedMove();

        }

        internal override Square GetMovedPieceEndingSquare()
        {
            return _endingSquare;
        }

        private bool AllowEnPassantOnNextMoves()
        {
            return _movedPiece is Pawn
               && _endingSquare.Coordinate.GetAbsRankDifference(_startingSquare.Coordinate) == 2;
        }

        #endregion
    }
}
