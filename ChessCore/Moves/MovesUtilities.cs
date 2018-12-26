using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace ChessCore
{
    internal static class MoveUtilities
    {
        #region Knight moves

        internal static bool IsOnTwoOneL(this SquareCoordinate startingCoordinate, SquareCoordinate endingCoordinate)
        {
            var rankIncrement = Math.Abs(startingCoordinate.Rank - endingCoordinate.Rank);
            var fileIncrement = Math.Abs(startingCoordinate.File - endingCoordinate.File);
            return IsATwoOneL(rankIncrement, fileIncrement)
                || IsATwoOneL(fileIncrement, rankIncrement);
        }

        private static bool IsATwoOneL(int side1Increment, int side2Increment)
        {
            return side1Increment == 2 && side2Increment == 1;
        }

        #endregion

        #region Rook moves

        internal static bool IsOnRankOrFile(this SquareCoordinate startingCoordinate, SquareCoordinate endingCoordinate)
        {
            return startingCoordinate.File == endingCoordinate.File
                || startingCoordinate.Rank == endingCoordinate.Rank;
        }

        #endregion

        #region Queen moves

        internal static bool IsOnRankFileOrDiagonal(this SquareCoordinate startingCoordinate, SquareCoordinate endingCoordinate)
        {
            return startingCoordinate.IsOnRankOrFile(endingCoordinate)
                || startingCoordinate.IsOnDiagonal(endingCoordinate);
        }

        #endregion

        #region Bishop moves

        internal static bool IsOnDiagonal(this SquareCoordinate startingCoordinate, SquareCoordinate endingCoordinate)
        {
            var rankIncrement = startingCoordinate.Rank - endingCoordinate.Rank;
            var fileIncrement = startingCoordinate.File - endingCoordinate.File;

            if (fileIncrement == 0)
                return false;

            return fileIncrement != 0
                && Math.Abs((double)rankIncrement / fileIncrement) == 1.00;
        }

        #endregion

        #region King moves

        internal static bool IsKingValidMove(SquareCoordinate startingCoordinate, SquareCoordinate endingCoordinate)
        {
            var rankDiff = Math.Abs(startingCoordinate.Rank - endingCoordinate.Rank);
            var fileDiff = Math.Abs(startingCoordinate.File - endingCoordinate.File);

            return (rankDiff != 0 || fileDiff != 0)
                && (rankDiff == 1 || rankDiff == 0)
                && (fileDiff == 1 || fileDiff == 0);
        }

        #endregion

        #region Pawn moves

        internal static bool IsOnSameFile(this SquareCoordinate startingCoordinate, SquareCoordinate endingCoordinate)
        {
            return startingCoordinate.File == endingCoordinate.File;
        }

        internal static int GetRankDifference(this SquareCoordinate startingCoordinate, SquareCoordinate endingCoordinate)
        {
            return endingCoordinate.Rank - startingCoordinate.Rank;
        }

        internal static int GetAbsRankDifference(this SquareCoordinate startingCoordinate, SquareCoordinate endingCoordinate)
        {
            return Math.Abs(startingCoordinate.GetRankDifference(endingCoordinate));
        }

        internal static int GetFileDifference(this SquareCoordinate startingCoordinate, SquareCoordinate endingCoordinate)
        {
            return endingCoordinate.File - startingCoordinate.File;
        }

        internal static bool IsOnDiagonalAdjacentSquare(this SquareCoordinate startingCoordinate, SquareCoordinate endingCoordinate)
        {
            return Math.Abs(startingCoordinate.GetRankDifference(endingCoordinate)) == 1
                && Math.Abs(startingCoordinate.GetFileDifference(endingCoordinate)) == 1;
        }

        #endregion

        #region In between positions

        internal static IEnumerable<SquareCoordinate> GetAllInBetweenSquares(this SquareCoordinate startingCoordinate, SquareCoordinate endingCoordinate)
        {
            var ret = new List<SquareCoordinate>();

            if (startingCoordinate.IsOnDiagonal(endingCoordinate))
                Extensions.AddRange(ret, GetDiagonalInBetweenPositions(startingCoordinate, endingCoordinate));
            if (startingCoordinate.IsOnRankOrFile(endingCoordinate))
                Extensions.AddRange(ret, GetStraightInBetweenPosition(startingCoordinate, endingCoordinate));

            return ret;
        }

        private static IEnumerable<SquareCoordinate> GetDiagonalInBetweenPositions(SquareCoordinate startingCoordinate, SquareCoordinate endingCoordinate)
        {
            var ret = new List<SquareCoordinate>();

            var shift = Math.Abs(endingCoordinate.Rank - startingCoordinate.Rank);
            var fileDirection = Math.Sign(endingCoordinate.File - startingCoordinate.File);
            var rankDirection = Math.Sign(endingCoordinate.Rank - startingCoordinate.Rank);

            for (int i = 1; i < shift; i++)
            {
                var rank = startingCoordinate.Rank + (rankDirection * i);
                var file = startingCoordinate.File + (fileDirection * i);
                ret.Add(new SquareCoordinate(rank, file));
            }

            return ret;
        }

        private static IEnumerable<SquareCoordinate> GetStraightInBetweenPosition(SquareCoordinate startingCoordinate, SquareCoordinate endingCoordinate)
        {
            var ret = new List<SquareCoordinate>();

            var fileShift = endingCoordinate.File - startingCoordinate.File;
            if (fileShift != 0)
            {
                var shiftDirection = Math.Sign(fileShift);
                var shiftAbs = Math.Abs(fileShift);
                for (int i = 1; i < shiftAbs; i++)
                {
                    var newFile = startingCoordinate.File + (shiftDirection * i);
                    ret.Add(new SquareCoordinate(startingCoordinate.Rank, newFile));
                }
            }
            else
            {
                var rankShift = endingCoordinate.Rank - startingCoordinate.Rank;
                var shiftDirection = Math.Sign(rankShift);
                var shiftAbs = Math.Abs(rankShift);
                for (int i = 1; i < shiftAbs; i++)
                {
                    var newRank = startingCoordinate.Rank + (shiftDirection * i);
                    ret.Add(new SquareCoordinate(newRank, startingCoordinate.File));
                }
            }

            return ret;
        }

        #endregion

        #region Moves availability

        #region Castle

        internal static bool CanCastle(King king, Board board, CastleType castleType)
        {
            return CastlePiecesHaveNotBeenMoved(king, board, castleType)
                && CastlingNoPiecesInBetween(king, board, castleType)
                && CastlingSquaresAreNotControlledByOpponent(king, board, castleType);
        }

        private static bool CastlingNoPiecesInBetween(King king, Board board, CastleType type)
        {
            foreach (var square in king.GetCastleInBetweenPositions(type))
                if (board.IsAnyPieceInSquare(square))
                    return false;
            return true;
        }

        private static bool CastlePiecesHaveNotBeenMoved(King king, Board board, CastleType castleType)
        {
            if (king.HasBeenMoved)
                return false;

            var rookSquare = board[king.GetCastleRookStartingSquare(castleType)];
            return rookSquare.Piece != null
                && !rookSquare.Piece.HasBeenMoved;
        }

        private static bool CastlingSquaresAreNotControlledByOpponent(King king, Board board, CastleType castleType)
        {
            foreach (var square in king.GetCastlingSquares(castleType))
                if (board.IsInOpponentControl(board[square], king.Color))
                    return false;

            return true;
        }

        #endregion

        internal static IEnumerable<SquareCoordinate> GetDiagonalAvailability(SquareCoordinate startingCoordinate, Board board, Color pieceColor, SquareInfluenceType availabilityType)
        {
            var ret = new List<SquareCoordinate>();

            //Upper right diagonal
            for (int i = 1; i < 8; i++)
            {
                var rank = startingCoordinate.Rank + i;
                var file = startingCoordinate.File + i;
                if (TryAddPieceGetIsLast(ret, startingCoordinate, rank, file, board, pieceColor, availabilityType))
                    break;
            }
            //Upper left diagonal
            for (int i = 1; i < 8; i++)
            {
                var rank = startingCoordinate.Rank + i;
                var file = startingCoordinate.File - i;
                if (TryAddPieceGetIsLast(ret, startingCoordinate, rank, file, board, pieceColor, availabilityType))
                    break;
            }
            //Bottom right diagonal
            for (int i = 1; i < 8; i++)
            {
                var rank = startingCoordinate.Rank - i;
                var file = startingCoordinate.File + i;
                if (TryAddPieceGetIsLast(ret, startingCoordinate, rank, file, board, pieceColor, availabilityType))
                    break;
            }
            //Bottom left diagonal
            for (int i = 1; i < 8; i++)
            {
                var rank = startingCoordinate.Rank - i;
                var file = startingCoordinate.File - i;
                if (TryAddPieceGetIsLast(ret, startingCoordinate, rank, file, board, pieceColor, availabilityType))
                    break;
            }

            return ret;
        }

        internal static IEnumerable<SquareCoordinate> GetLineAvailability(SquareCoordinate startingCoordinate, Board board, Color pieceColor, SquareInfluenceType availabilityType)
        {
            var ret = new List<SquareCoordinate>();

            //Upper line
            for (int i = 1; i < 8; i++)
            {
                var rank = startingCoordinate.Rank + i;
                var file = startingCoordinate.File;
                if (TryAddPieceGetIsLast(ret, startingCoordinate, rank, file, board, pieceColor, availabilityType))
                    break;
            }
            //Bottom line
            for (int i = 1; i < 8; i++)
            {
                var rank = startingCoordinate.Rank - i;
                var file = startingCoordinate.File;
                if (TryAddPieceGetIsLast(ret, startingCoordinate, rank, file, board, pieceColor, availabilityType))
                    break;
            }
            //Right line
            for (int i = 1; i < 8; i++)
            {
                var rank = startingCoordinate.Rank;
                var file = startingCoordinate.File + i;
                if (TryAddPieceGetIsLast(ret, startingCoordinate, rank, file, board, pieceColor, availabilityType))
                    break;
            }
            //Left line
            for (int i = 1; i < 8; i++)
            {
                var rank = startingCoordinate.Rank;
                var file = startingCoordinate.File - i;
                if (TryAddPieceGetIsLast(ret, startingCoordinate, rank, file, board, pieceColor, availabilityType))
                    break;
            }

            return ret;
        }

        internal static IEnumerable<SquareCoordinate> GetKingAvailability(King king, SquareCoordinate startingCoordinate, Board board, SquareInfluenceType availabilityType)
        {
            switch (availabilityType)
            {
                case SquareInfluenceType.Control:
                    return GetKingControl(startingCoordinate);
                case SquareInfluenceType.Mobility:
                    return GetKingMobility(king, startingCoordinate, board);
                default:
                    throw new NotImplementedException();
            }
        }

        internal static IEnumerable<SquareCoordinate> GetKingMobility(King king, SquareCoordinate startingCoordinate, Board board)
        {
            var ret = new List<SquareCoordinate>();

            SquareCoordinate? newPos = null;

            //Upper left
            if (TryGetNewPosition(startingCoordinate.Rank + 1, startingCoordinate.File - 1, out newPos))
                if (!board.IsControlledByColor(newPos.Value, king.Color.OpponentColor)
                    && board.CanPieceOfColorGoToSquare(newPos.Value, king.Color))
                    ret.Add(newPos.Value);
            //Upper middle
            if (TryGetNewPosition(startingCoordinate.Rank + 1, startingCoordinate.File, out newPos))
                if (!board.IsControlledByColor(newPos.Value, king.Color.OpponentColor)
                    && board.CanPieceOfColorGoToSquare(newPos.Value, king.Color))
                    ret.Add(newPos.Value);
            //Upper right
            if (TryGetNewPosition(startingCoordinate.Rank + 1, startingCoordinate.File + 1, out newPos))
                if (!board.IsControlledByColor(newPos.Value, king.Color.OpponentColor)
                    && board.CanPieceOfColorGoToSquare(newPos.Value, king.Color))
                    ret.Add(newPos.Value);
            //Middle right
            if (TryGetNewPosition(startingCoordinate.Rank, startingCoordinate.File + 1, out newPos))
                if (!board.IsControlledByColor(newPos.Value, king.Color.OpponentColor)
                    && board.CanPieceOfColorGoToSquare(newPos.Value, king.Color))
                    ret.Add(newPos.Value);
            //Middle left
            if (TryGetNewPosition(startingCoordinate.Rank, startingCoordinate.File - 1, out newPos))
                if (!board.IsControlledByColor(newPos.Value, king.Color.OpponentColor)
                    && board.CanPieceOfColorGoToSquare(newPos.Value, king.Color))
                    ret.Add(newPos.Value);
            //Bottom right
            if (TryGetNewPosition(startingCoordinate.Rank - 1, startingCoordinate.File + 1, out newPos))
                if (!board.IsControlledByColor(newPos.Value, king.Color.OpponentColor)
                    && board.CanPieceOfColorGoToSquare(newPos.Value, king.Color))
                    ret.Add(newPos.Value);
            //Bottom middle
            if (TryGetNewPosition(startingCoordinate.Rank - 1, startingCoordinate.File, out newPos))
                if (!board.IsControlledByColor(newPos.Value, king.Color.OpponentColor)
                    && board.CanPieceOfColorGoToSquare(newPos.Value, king.Color))
                    ret.Add(newPos.Value);
            //Bottom left
            if (TryGetNewPosition(startingCoordinate.Rank - 1, startingCoordinate.File - 1, out newPos))
                if (!board.IsControlledByColor(newPos.Value, king.Color.OpponentColor)
                    && board.CanPieceOfColorGoToSquare(newPos.Value, king.Color))
                    ret.Add(newPos.Value);

            //King side castle
            if (CanCastle(king, board, CastleType.KingSide))
                ret.Add(king.GetCastleEndingPosition(CastleType.KingSide));
            //Queen side castle
            if (CanCastle(king, board, CastleType.QueenSide))
                ret.Add(king.GetCastleEndingPosition(CastleType.QueenSide));

            return ret;
        }

        internal static IEnumerable<SquareCoordinate> GetKingControl(SquareCoordinate startingCoordinate)
        {
            var ret = new List<SquareCoordinate>();

            SquareCoordinate? newPos = null;

            //Old square
            ret.Add(startingCoordinate);
            //Upper left
            if (TryGetNewPosition(startingCoordinate.Rank + 1, startingCoordinate.File - 1, out newPos))
                ret.Add(newPos.Value);
            //Upper middle
            if (TryGetNewPosition(startingCoordinate.Rank + 1, startingCoordinate.File, out newPos))
                ret.Add(newPos.Value);
            //Upper right
            if (TryGetNewPosition(startingCoordinate.Rank + 1, startingCoordinate.File + 1, out newPos))
                ret.Add(newPos.Value);
            //Middle right
            if (TryGetNewPosition(startingCoordinate.Rank, startingCoordinate.File + 1, out newPos))
                ret.Add(newPos.Value);
            //Middle left
            if (TryGetNewPosition(startingCoordinate.Rank, startingCoordinate.File - 1, out newPos))
                ret.Add(newPos.Value);
            //Bottom right
            if (TryGetNewPosition(startingCoordinate.Rank - 1, startingCoordinate.File + 1, out newPos))
                ret.Add(newPos.Value);
            //Bottom middle
            if (TryGetNewPosition(startingCoordinate.Rank - 1, startingCoordinate.File, out newPos))
                ret.Add(newPos.Value);
            //Bottom left
            if (TryGetNewPosition(startingCoordinate.Rank - 1, startingCoordinate.File - 1, out newPos))
                ret.Add(newPos.Value);
            return ret;
        }

        //TODO position factory?
        internal static bool TryGetNewPosition(int rank, int file, out SquareCoordinate? position)
        {
            var isValidPos = IsValidPosition(rank, file);
            position = isValidPos ? new SquareCoordinate(rank, file) : (SquareCoordinate?)null;
            return isValidPos;
        }

        internal static IEnumerable<SquareCoordinate> GetKnightAvailability(SquareCoordinate startingCoordinate, Board board, Color pieceColor, SquareInfluenceType availabilityType)
        {
            var ret = new List<SquareCoordinate>();

            //Upper right, up
            var rank = startingCoordinate.Rank + 2;
            var file = startingCoordinate.File + 1;
            TryAddSquare(ret, startingCoordinate, rank, file, board, pieceColor, availabilityType);
            //Upper right, right
            rank = startingCoordinate.Rank + 1;
            file = startingCoordinate.File + 2;
            TryAddSquare(ret, startingCoordinate, rank, file, board, pieceColor, availabilityType);

            //Upper left, up
            rank = startingCoordinate.Rank + 2;
            file = startingCoordinate.File - 1;
            TryAddSquare(ret, startingCoordinate, rank, file, board, pieceColor, availabilityType);
            //Upper left, right
            rank = startingCoordinate.Rank + 1;
            file = startingCoordinate.File - 2;
            TryAddSquare(ret, startingCoordinate, rank, file, board, pieceColor, availabilityType);

            //Bottom right, bottom
            rank = startingCoordinate.Rank - 2;
            file = startingCoordinate.File + 1;
            TryAddSquare(ret, startingCoordinate, rank, file, board, pieceColor, availabilityType);
            //Bottom right, right
            rank = startingCoordinate.Rank - 1;
            file = startingCoordinate.File + 2;
            TryAddSquare(ret, startingCoordinate, rank, file, board, pieceColor, availabilityType);

            //Bottom left, bottom
            rank = startingCoordinate.Rank - 2;
            file = startingCoordinate.File - 1;
            TryAddSquare(ret, startingCoordinate, rank, file, board, pieceColor, availabilityType);
            //Bottom left, right
            rank = startingCoordinate.Rank - 1;
            file = startingCoordinate.File - 2;
            TryAddSquare(ret, startingCoordinate, rank, file, board, pieceColor, availabilityType);

            return ret;
        }

        #region  Pawn availability

        internal static IEnumerable<SquareCoordinate> GetPawnAvailability(SquareCoordinate startingCoordinate, Board board, Color pieceColor, SquareInfluenceType availabilityType)
        {
            switch (availabilityType)
            {
                case SquareInfluenceType.Control:
                    return GetPawnControl(startingCoordinate, board, pieceColor);
                case SquareInfluenceType.Mobility:
                    return GetPawnMobility(startingCoordinate, board, pieceColor);
                default:
                    throw new NotImplementedException();
            }
        }

        internal static IEnumerable<SquareCoordinate> GetPawnFrontSquares(SquareCoordinate square, Color color)
        {
            var ret = new List<SquareCoordinate>();

            //Left
            if (IsValidPosition(square.Rank + color.MovingDirection, square.File - 1))
                ret.Add(new SquareCoordinate(square.Rank + color.MovingDirection, square.File - 1));

            //In front
            if (IsValidPosition(square.Rank + color.MovingDirection, square.File))
                ret.Add(new SquareCoordinate(square.Rank + color.MovingDirection, square.File));

            //Right
            if (IsValidPosition(square.Rank + color.MovingDirection, square.File + 1))
                ret.Add(new SquareCoordinate(square.Rank + color.MovingDirection, square.File + 1));

            return ret;
        }

        #region Pawn control

        private static IEnumerable<SquareCoordinate> GetPawnControl(SquareCoordinate startingCoordinate, Board board, Color color)
        {
            var ret = new List<SquareCoordinate>();

            var captureRank = startingCoordinate.Rank + color.MovingDirection;
            var leftCaptureFile = startingCoordinate.File - 1;
            var rightCaptureFile = startingCoordinate.File + 1;

            if (IsValidPosition(captureRank, leftCaptureFile))
                ret.Add(new SquareCoordinate(captureRank, leftCaptureFile));

            if (IsValidPosition(captureRank, rightCaptureFile))
                ret.Add(new SquareCoordinate(captureRank, rightCaptureFile));

            return ret;
        }

        #endregion

        #region Pawn mobility

        private static IEnumerable<SquareCoordinate> GetPawnMobility(SquareCoordinate square, Board board, Color pieceColor)
        {
            var ret = new List<SquareCoordinate>();

            SquareCoordinate? allowedSquare = null;

            if (DoubleForwardMoveAllowed(board, pieceColor, square, out allowedSquare))
                ret.Add(allowedSquare.Value);

            if (SingleForwardMoveAllowed(board, pieceColor, square, out allowedSquare))
                ret.Add(allowedSquare.Value);

            if (IsPawnCaptureAllowed(board, pieceColor, square, square.File - 1, out allowedSquare))
                ret.Add(allowedSquare.Value);

            if (IsPawnCaptureAllowed(board, pieceColor, square, square.File + 1, out allowedSquare))
                ret.Add(allowedSquare.Value);

            return ret;
        }

        private static bool EnPassantAllowed(Board board, Color color, SquareCoordinate square, out SquareCoordinate? allowedSquare)
        {
            if (square.Rank == color.EnPassantStartingRank)
            {
                var lastMove = board.GetLastMove();
                if (lastMove != null)
                {
                    var lastMoveEnPassantInfo = lastMove.GetAllowEnPassantOnNextMoveInfo();
                    if (lastMoveEnPassantInfo.AllowEnPassant)
                    {
                        var leftCaptureFile = square.File - 1;
                        var rightCaptureFile = square.File + 1;

                        if (lastMoveEnPassantInfo.IsEnPassantFile(leftCaptureFile))
                        {
                            allowedSquare = new SquareCoordinate(color.EnPassantStartingRank, leftCaptureFile);
                            return true;
                        }
                        else if (lastMoveEnPassantInfo.IsEnPassantFile(rightCaptureFile))
                        {
                            allowedSquare = new SquareCoordinate(color.EnPassantStartingRank, rightCaptureFile);
                            return true;
                        }
                    }
                }
            }

            allowedSquare = null;
            return false;
        }

        private static bool IsPawnCaptureAllowed(Board board, Color color, SquareCoordinate square, int captureFile, out SquareCoordinate? allowedSquare)
        {
            var endingRank = square.Rank + color.MovingDirection;

            var isAllowed = IsValidPosition(square.Rank, captureFile)
                         && board.IsAnyOpponentPieceInSquare(new SquareCoordinate(endingRank, captureFile), color);

            allowedSquare = isAllowed
                ? new SquareCoordinate(endingRank, captureFile)
                : (SquareCoordinate?)null;

            return isAllowed;
        }

        private static bool DoubleForwardMoveAllowed(Board board, Color color, SquareCoordinate square, out SquareCoordinate? allowedSquare)
        {
            var singleRankMove = square.Rank + color.MovingDirection;
            var doubleRankMove = square.Rank + color.MovingDirection * 2;

            var isAllowed = square.Rank == color.PawnFirstRank
                && !board.IsAnyPieceInSquare(singleRankMove, square.File)
                && !board.IsAnyPieceInSquare(doubleRankMove, square.File);

            allowedSquare = isAllowed
                ? new SquareCoordinate(doubleRankMove, square.File)
                : (SquareCoordinate?)null;

            return isAllowed;
        }

        private static bool SingleForwardMoveAllowed(Board board, Color color, SquareCoordinate square, out SquareCoordinate? allowedSquare)
        {
            var singleRankMove = square.Rank + color.MovingDirection;

            var isAllowed = !board.IsAnyPieceInSquare(singleRankMove, square.File)
                         && IsValidPosition(singleRankMove, square.File);

            allowedSquare = isAllowed
                ? new SquareCoordinate(singleRankMove, square.File)
                : (SquareCoordinate?)null;

            return isAllowed;
        }

        #endregion

        #endregion

        private static bool TryAddPieceGetIsLast(List<SquareCoordinate> ret,
                                                 SquareCoordinate startingSquare,
                                                 int newRank,
                                                 int newFile,
                                                 Board board,
                                                 Color pieceColor,
                                                 SquareInfluenceType availabilityType)
        {
            if (!IsValidPosition(newRank, newFile))
                return true;

            TryAddSquare(ret, startingSquare, newRank, newFile, board, pieceColor, availabilityType);

            return board.IsAnyPieceInSquare(new SquareCoordinate(newRank, newFile));
        }

        private static void TryAddSquare(List<SquareCoordinate> ret,
                                         SquareCoordinate startingSquare,
                                         int newRank,
                                         int newFile,
                                         Board board,
                                         Color pieceColor,
                                         SquareInfluenceType availabilityType)
        {
            if (!IsValidPosition(newRank, newFile))
                return;

            var newSquare = new SquareCoordinate(newRank, newFile);

            switch (availabilityType)
            {
                case SquareInfluenceType.Control:
                    ret.Add(newSquare);
                    break;
                case SquareInfluenceType.Mobility:
                    if (IsInPieceMobility(board, startingSquare, newSquare, pieceColor))
                        ret.Add(newSquare);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private static bool IsInPieceMobility(Board board, SquareCoordinate startingSquare, SquareCoordinate endingSquare, Color color)
        {
            return board.CanPieceOfColorGoToSquare(endingSquare, color);
        }

        #endregion

        //TODO repository factory logic?
        internal static bool IsValidPosition(int rank, int file)
        {
            return rank >= 1
                && rank <= 8
                && file >= 1
                && file <= 8;
        }

        internal static string GetFileFromInt(int rank)
        {
            switch (rank)
            {
                case 1:
                    return "a";
                case 2:
                    return "b";
                case 3:
                    return "c";
                case 4:
                    return "d";
                case 5:
                    return "e";
                case 6:
                    return "f";
                case 7:
                    return "g";
                case 8:
                    return "h";
                default:
                    throw new ArgumentException();
            }
        }
    }

    internal enum SquareInfluenceType
    {
        Control,
        Mobility
    }
}
