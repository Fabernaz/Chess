using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCore
{
    internal class ValidMoveFactory
    {
        internal bool TryCreateValidMove(Square startingSquare,
                                         Square endingSquare,
                                         Board board,
                                         out MoveBase move)
        {
            return TryEnPassant(startingSquare, endingSquare, board, out move)
                || TryCastle(board, startingSquare, endingSquare, out move)
                || TryGenericMove(startingSquare, endingSquare, board, out move);
        }

        #region En passant

        private MoveBase GetEnPassantMove(Square startingSquare,
                                          Square endingSquare,
                                          Board board)
        {
            return new EnPassant(startingSquare.Piece as Pawn,
                                 startingSquare,
                                 endingSquare,
                                 board.GetLastMovedPiece() as Pawn,
                                 board.GetLastMove().GetMovedPieceEndingSquare());
        }

        private bool TryEnPassant(Square startingSquare,
                                  Square endingSquare,
                                  Board board,
                                  out MoveBase move)
        {
            move = null;
            var isValid = GetIsValidEnPassantMove(startingSquare, endingSquare, board);
            if (isValid)
                move = GetEnPassantMove(startingSquare, endingSquare, board);
            return isValid;
        }

        private bool GetIsValidEnPassantMove(Square startingSquare,
                                             Square endingSquare,
                                             Board board)
        {
            if (!(startingSquare.Piece is Pawn))
                return false;

            var lastMove = board.GetLastMove();
            if (lastMove == null)
                return false;

            var lastMoveEnPassantInfo = lastMove.GetAllowEnPassantOnNextMoveInfo();
            if (!lastMoveEnPassantInfo.IsAllowed)
                return false;

            return lastMoveEnPassantInfo.IsEnPassantRankAndFile(startingSquare.Piece.Color,
                                                                startingSquare.Coordinate.Rank,
                                                                endingSquare.Coordinate.Rank,
                                                                startingSquare.Coordinate.File,
                                                                endingSquare.Coordinate.File);
        }

        #endregion

        #region Castling

        private bool TryCastle(Board board, Square startingSquare, Square endingSquare, out MoveBase move)
        {
            move = null;
            CastleType? castleType = null;
            King king = null;

            var isValid = GetIsValidCastleMove(board, startingSquare, endingSquare, out castleType, out king);
            if (isValid)
                move = GetCastleMove(board, castleType.Value, king);

            return isValid;
        }

        private bool GetIsValidCastleMove(Board board, Square startingSquare, Square endingSquare, out CastleType? castleType, out King king)
        {
            castleType = null;

            king = startingSquare.Piece as King;
            if (king == null)
                return false;

            return IsCastleMove(king, startingSquare, endingSquare, out castleType)
                && MoveUtilities.CanCastle(king, board, castleType.Value);
        }

        private MoveBase GetCastleMove(Board board, CastleType castleType, King king)
        {
            var rook = board.GetSquare(king.GetCastleRookStartingSquare(castleType)).Piece as Rook;

            switch (castleType)
            {
                case CastleType.KingSide:
                    return new CastlingKingSide(king, rook, board);
                case CastleType.QueenSide:
                    return new CastlingQueenSide(king, rook, board);
                default:
                    throw new NotImplementedException();
            }
        }

        private bool IsCastleMove(King king, Square startingSquare, Square endingSquare, out CastleType? type)
        {
            type = null;

            if (startingSquare.Coordinate != king.GetCastleStartingPosition())
                return false;

            if (endingSquare.Coordinate == king.GetCastleEndingPosition(CastleType.KingSide))
            {
                type = CastleType.KingSide;
                return true;
            }
            else if (endingSquare.Coordinate == king.GetCastleEndingPosition(CastleType.QueenSide))
            {
                type = CastleType.QueenSide;
                return true;
            }

            return false;
        }

        #endregion

        #region Generic move

        private bool TryGenericMove(Square startingSquare,
                                          Square endingSquare,
                                          Board board,
                                          out MoveBase move)
        {
            move = null;

            var isValid = IsValidGenericMove(board, startingSquare, endingSquare, startingSquare.Piece, endingSquare.Piece);
            if (isValid)
                move = new Move(startingSquare,
                                endingSquare,
                                startingSquare.Piece,
                                endingSquare.Piece,
                                endingSquare.Piece != null,
                                GetAmbiguousMove(board, endingSquare.Coordinate, startingSquare.Piece),
                                null);

            return isValid;
        }

        private Move GetAmbiguousMove(Board board, SquareCoordinate endingCoordinate, Piece movedPiece)
        {
            if (!board.ExistsPlayingPieceOfSameTypeAndColor(movedPiece)
                || movedPiece is Bishop)
                return null;

            return null;
        }

        #region Move validity

        private bool IsValidGenericMove(Board board, Square startingSquare, Square endingSquare, Piece movedPiece, Piece capturedPiece)
        {
            return IsPossibleMove(board, startingSquare, endingSquare, movedPiece, capturedPiece)
                && IsNotCheck(board, startingSquare, endingSquare, movedPiece);
        }

        private bool IsPossibleMove(Board board, Square startingSquare, Square endingSquare, Piece movedPiece, Piece capturedPiece)
        {
            return movedPiece != null
                && IsAllowedMove(board, startingSquare, endingSquare, movedPiece, capturedPiece)
                && IsAllowedCapture(startingSquare, endingSquare, movedPiece, capturedPiece);
        }

        private bool IsNotCheck(Board board, Square startingCoordinate, Square endingCoordinate, Piece movedPiece)
        {
            return IsNotGettingInCheck(board, startingCoordinate.Coordinate, endingCoordinate.Coordinate, movedPiece)
                && KingIsNotInCheck(board, startingCoordinate, endingCoordinate, movedPiece.Color);
        }

        private bool KingIsNotInCheck(Board board, Square startingCoordinate, Square endingCoordinate, Color movingColor)
        {
            return !board.WouldBeInCheckAfterMove(startingCoordinate, endingCoordinate);
        }

        private bool IsNotGettingInCheck(Board board, SquareCoordinate startingCoordinate, SquareCoordinate endingCoordinate, Piece movedPiece)
        {
            return movedPiece is King
                ? IsNotCheckOnKingMove(board, startingCoordinate, endingCoordinate, movedPiece.Color)
                : IsNotBreakingAbsolutePin(board, startingCoordinate, endingCoordinate, movedPiece.Color);
        }

        private bool IsNotBreakingAbsolutePin(Board board, SquareCoordinate startingCoordinate, SquareCoordinate endingCoordinate, Color movingSide)
        {
            var kingPosition = board.GetKingSquare(movingSide);
            return !board.IsInOpponentControlAfterMove(board.GetSquare(startingCoordinate),
                                                       board.GetSquare(endingCoordinate),
                                                       kingPosition);
        }

        private bool IsNotCheckOnKingMove(Board board, SquareCoordinate startingCoordinate, SquareCoordinate endingCoordinate, Color kingColor)
        {
            return !board.IsControlledByOppositeColor(endingCoordinate, kingColor)
                && !board.IsInOpponentControlAfterMove(board.GetSquare(startingCoordinate),
                                                       board.GetSquare(endingCoordinate),
                                                       board.GetSquare(endingCoordinate));
        }

        private bool IsAllowedMove(Board board, Square startingSquare, Square endingSquare, Piece movedPiece, Piece capturedPiece)
        {
            return movedPiece.IsPieceMove(startingSquare, endingSquare, capturedPiece)
                && InBetweenPiecesValid(board, startingSquare, endingSquare, movedPiece);
        }

        private bool InBetweenPiecesValid(Board board, Square startingSquare, Square endingSquare, Piece movedPiece)
        {
            return movedPiece.CanJumpOverPieces()
                || !board.IsAnyPieceInBetween(startingSquare, endingSquare);
        }

        private bool IsAllowedCapture(Square startingSquare, Square endingSquare, Piece movedPiece, Piece capturedPiece)
        {
            return capturedPiece == null
                || movedPiece.Color.IsOpponentColor(capturedPiece.Color);
        }

        #endregion

        #endregion
    }
}
