using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCore
{
    internal class ValidMoveFactory
    {
        internal bool TryCreateValidMove(BoardCell startingCell,
                                         BoardCell endingCell,
                                         Board board,
                                         out MoveBase move)
        {
            return TryEnPassant(startingCell, endingCell, board, out move)
                || TryCastle(board, startingCell, endingCell, out move)
                || TryGenericMove(startingCell, endingCell, board, out move);
        }

        #region En passant

        private MoveBase GetEnPassantMove(BoardCell startingCell,
                                          BoardCell endingCell,
                                          Board board)
        {
            return new EnPassant(startingCell.Piece as Pawn,
                                 startingCell.Position,
                                 endingCell.Position,
                                 board.GetLastMovedPiece() as Pawn,
                                 board.GetLastMove().GetMovedPieceEndingPosition());
        }

        private bool TryEnPassant(BoardCell startingCell,
                                  BoardCell endingCell,
                                  Board board,
                                  out MoveBase move)
        {
            move = null;
            var isValid = GetIsValidEnPassantMove(startingCell, endingCell, board);
            if (isValid)
                move = GetEnPassantMove(startingCell, endingCell, board);
            return isValid;
        }

        private bool GetIsValidEnPassantMove(BoardCell startingCell,
                                             BoardCell endingCell,
                                             Board board)
        {
            if (!(startingCell.Piece is Pawn))
                return false;

            var lastMove = board.GetLastMove();
            if (lastMove == null)
                return false;

            var lastMoveEnPassantInfo = lastMove.GetAllowEnPassantOnNextMoveInfo();
            if (!lastMoveEnPassantInfo.IsAllowed)
                return false;

            return lastMoveEnPassantInfo.IsEnPassantRankAndFile(startingCell.Piece.Color,
                                                                startingCell.Position.Rank,
                                                                endingCell.Position.Rank,
                                                                startingCell.Position.File,
                                                                endingCell.Position.File);
        }

        #endregion

        #region Castling

        private bool TryCastle(Board board, BoardCell startingCell, BoardCell endingCell, out MoveBase move)
        {
            move = null;
            CastleType? castleType = null;
            King king = null;

            var isValid = GetIsValidCastleMove(board, startingCell, endingCell, out castleType, out king);
            if (isValid)
                move = GetCastleMove(board, castleType.Value, king);

            return isValid;
        }

        private bool GetIsValidCastleMove(Board board, BoardCell startingCell, BoardCell endingCell, out CastleType? castleType, out King king)
        {
            castleType = null;

            king = startingCell.Piece as King;
            if (king == null)
                return false;

            return IsCastleMove(king, startingCell, endingCell, out castleType)
                && MoveUtilities.CanCastle(king, board, castleType.Value);
        }

        private MoveBase GetCastleMove(Board board, CastleType castleType, King king)
        {
            var rook = board.GetCell(king.GetCastleRookStartingSquare(castleType)).Piece as Rook;

            switch (castleType)
            {
                case CastleType.KingSide:
                    return new CastlingKingSide(king, rook);
                case CastleType.QueenSide:
                    return new CastlingQueenSide(king, rook);
                default:
                    throw new NotImplementedException();
            }
        }

        private bool IsCastleMove(King king, BoardCell startingCell, BoardCell endingCell, out CastleType? type)
        {
            type = null;

            if (startingCell.Position != king.GetCastleStartingPosition())
                return false;

            if (endingCell.Position == king.GetCastleEndingPosition(CastleType.KingSide))
            {
                type = CastleType.KingSide;
                return true;
            }
            else if (endingCell.Position == king.GetCastleEndingPosition(CastleType.QueenSide))
            {
                type = CastleType.QueenSide;
                return true;
            }

            return false;
        }

        #endregion

        #region Generic move

        private bool TryGenericMove(BoardCell startingCell,
                                          BoardCell endingCell,
                                          Board board,
                                          out MoveBase move)
        {
            move = null;

            var isValid = IsValidGenericMove(board, startingCell, endingCell, startingCell.Piece, endingCell.Piece);
            if (isValid)
                move = new GenericMove(startingCell.Position,
                                       endingCell.Position,
                                       startingCell.Piece,
                                       endingCell.Piece,
                                       endingCell.Piece != null,
                                       GetAmbiguousMove(board, endingCell.Position, startingCell.Piece),
                                       null);

            return isValid;
        }

        private GenericMove GetAmbiguousMove(Board board, Position endingPosition, Piece movedPiece)
        {
            if (!board.AnyPlayingPieceOfSameTypeAndColor(movedPiece)
                || movedPiece is Bishop)
                return null;

            return null;
        }

        #region Move validity

        private bool IsValidGenericMove(Board board, BoardCell startingPosition, BoardCell endingPosition, Piece movedPiece, Piece capturedPiece)
        {
            return IsPossibleMove(board, startingPosition.Position, endingPosition.Position, movedPiece, capturedPiece)
                && IsNotCheck(board, startingPosition, endingPosition, movedPiece);
        }

        private bool IsPossibleMove(Board board, Position startingPosition, Position endingPosition, Piece movedPiece, Piece capturedPiece)
        {
            return movedPiece != null
                && IsAllowedMove(board, startingPosition, endingPosition, movedPiece, capturedPiece)
                && IsAllowedCapture(startingPosition, endingPosition, movedPiece, capturedPiece);
        }

        private bool IsNotCheck(Board board, BoardCell startingPosition, BoardCell endingPosition, Piece movedPiece)
        {
            return IsNotGettingInCheck(board, startingPosition.Position, endingPosition.Position, movedPiece)
                && KingIsNotInCheck(board, startingPosition, endingPosition, movedPiece.Color);
        }

        private bool KingIsNotInCheck(Board board, BoardCell startingPosition, BoardCell endingPosition, Color movingColor)
        {
            return !board.WouldBeInCheckAfterMove(startingPosition, endingPosition);
        }

        private bool IsNotGettingInCheck(Board board, Position startingPosition, Position endingPosition, Piece movedPiece)
        {
            return movedPiece is King
                ? IsNotCheckOnKingMove(board, startingPosition, endingPosition, movedPiece.Color)
                : IsNotBreakingAbsolutePin(board, startingPosition, endingPosition, movedPiece.Color);
        }

        private bool IsNotBreakingAbsolutePin(Board board, Position startingPosition, Position endingPosition, Color movingSide)
        {
            var kingPosition = board.GetKingCell(movingSide);
            return !board.IsInOpponentControlAfterMove(board.GetCell(startingPosition),
                                                       board.GetCell(endingPosition),
                                                       kingPosition);
        }

        private bool IsNotCheckOnKingMove(Board board, Position startingPosition, Position endingPosition, Color kingColor)
        {
            return !board.IsControlledByOppositeColor(endingPosition, kingColor)
                && !board.IsInOpponentControlAfterMove(board.GetCell(startingPosition),
                                                       board.GetCell(endingPosition),
                                                       board.GetCell(endingPosition));
        }

        private bool IsAllowedMove(Board board, Position startingPosition, Position endingPosition, Piece movedPiece, Piece capturedPiece)
        {
            return movedPiece.IsPieceMove(startingPosition, endingPosition, capturedPiece)
                && InBetweenPiecesValid(board, startingPosition, endingPosition, movedPiece);
        }

        private bool InBetweenPiecesValid(Board board, Position startingPosition, Position endingPosition, Piece movedPiece)
        {
            return movedPiece.CanJumpOverPieces()
                || !board.IsAnyPieceInBetween(startingPosition, endingPosition);
        }

        private bool IsAllowedCapture(Position startingPosition, Position endingPosition, Piece movedPiece, Piece capturedPiece)
        {
            return capturedPiece == null
                || movedPiece.Color.IsOpponentColor(capturedPiece.Color);
        }

        #endregion

        #endregion
    }
}
