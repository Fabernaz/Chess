using System;
using System.Linq;
using Common;

namespace ChessCore
{
    internal class ValidMoveFactory
    {
        private readonly InfoAsker<PromoteTo> _promoteToAsker;
        private readonly PieceFactory _pieceFactory;

        internal ValidMoveFactory(InfoAsker<PromoteTo> infoAsker)
        {
            Guard.ArgumentNotNull(infoAsker, nameof(infoAsker));
            _promoteToAsker = infoAsker;

            _pieceFactory = new PieceFactory();
        }

        internal bool TryCreateValidMove(Square startingSquare,
                                         Square endingSquare,
                                         Board board,
                                         out Move move)
        {
            return TryPawnPromotion(startingSquare, endingSquare, out move)
                || TryEnPassant(startingSquare, endingSquare, board, out move)
                || TryCastle(board, startingSquare, endingSquare, out move)
                || TryGenericMove(startingSquare, endingSquare, board, out move);
        }

        #region Pawn promotion

        private bool TryPawnPromotion(Square startingSquare, Square endingSquare, out Move move)
        {
            move = null;
            var isValid = GetIsValidPawnPromotion(startingSquare, endingSquare);
            if (isValid)
                move = GetPawnPromotion(startingSquare, endingSquare);
            return isValid;
        }

        private Move GetPawnPromotion(Square startingSquare, Square endingSquare)
        {
            var promotedTo = _pieceFactory.CreatePiece(_promoteToAsker.AskAndWaitForInfo(),
                                                       startingSquare.Piece.Color);

            return new PawnPromotion(startingSquare,
                                     endingSquare,
                                     promotedTo,
                                     null);
        }

        private bool GetIsValidPawnPromotion(Square startingSquare, Square endingSquare)
        {
            return startingSquare.Piece is Pawn
                && startingSquare.Piece.Color.LastRank == endingSquare.Coordinate.Rank;
        }

        #endregion

        #region En passant

        private Move GetEnPassantMove(Square startingSquare,
                                          Square endingSquare,
                                          Board board)
        {
            return new EnPassant(startingSquare,
                                 endingSquare,
                                 board.GetLastMove().EndingSquare,
                                 null);
        }

        private bool TryEnPassant(Square startingSquare,
                                  Square endingSquare,
                                  Board board,
                                  out Move move)
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
            if (!lastMoveEnPassantInfo.AllowEnPassant)
                return false;

            return lastMoveEnPassantInfo.IsEnPassantRankAndFile(startingSquare.Piece.Color,
                                                                startingSquare.Coordinate.Rank,
                                                                endingSquare.Coordinate.Rank,
                                                                startingSquare.Coordinate.File,
                                                                endingSquare.Coordinate.File);
        }

        #endregion

        #region Castling

        private bool TryCastle(Board board, Square startingSquare, Square endingSquare, out Move move)
        {
            move = null;
            CastleType? castleType = null;
            King king = null;

            var isValid = GetIsValidCastleMove(board, startingSquare, endingSquare, out castleType, out king);
            if (isValid)
                move = GetCastleMove(board, king, castleType.Value);

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

        private Move GetCastleMove(Board board, King king, CastleType castleType)
        {
            var rook = board.GetSquare(king.GetCastleRookStartingSquare(castleType)).Piece as Rook;

            return new Castle(king, rook, board, castleType);
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

        #region Base move

        private bool TryGenericMove(Square startingSquare,
                                    Square endingSquare,
                                    Board board,
                                    out Move move)
        {
            move = null;

            var isValid = IsValidGenericMove(board, startingSquare, endingSquare, startingSquare.Piece, endingSquare.Piece);
            if (isValid)
                move = new Move(startingSquare,
                                endingSquare,
                                GetAmbiguousMoveStartingSquare(board, endingSquare, startingSquare.Piece));

            return isValid;
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

        #region Ambiguous move

        private Square GetAmbiguousMoveStartingSquare(Board board, Square endingSquare, Piece movedPiece)
        {
            var samePieces = board.GetAllPlayingPiecesOfSameTypeAndColor(movedPiece.GetType(), movedPiece.Color)
                                  .Where(p => !p.Equals(movedPiece));

            foreach(var piece in samePieces)
            {
                var concurrentPieceStartingPosition = samePieces.FirstOrDefault(p => p.GetAvailableMoves().ToList().Contains(endingSquare.Coordinate))?.CurrentSquare;
                if (concurrentPieceStartingPosition != null)
                    return concurrentPieceStartingPosition;
            }

            return null;
        }

        #endregion

    }
}
