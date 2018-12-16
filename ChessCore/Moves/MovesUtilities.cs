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

        internal static bool IsOnTwoOneL(this Position startingPosition, Position endingPosition)
        {
            var rankIncrement = Math.Abs(startingPosition.Rank - endingPosition.Rank);
            var fileIncrement = Math.Abs(startingPosition.File - endingPosition.File);
            return IsATwoOneL(rankIncrement, fileIncrement)
                || IsATwoOneL(fileIncrement, rankIncrement);
        }

        private static bool IsATwoOneL(int side1Increment, int side2Increment)
        {
            return side1Increment == 2 && side2Increment == 1;
        }

        #endregion

        #region Rook moves

        internal static bool IsOnRankOrFile(this Position startingPosition, Position endingPosition)
        {
            return startingPosition.File == endingPosition.File
                || startingPosition.Rank == endingPosition.Rank;
        }

        #endregion

        #region Queen moves

        internal static bool IsOnRankFileOrDiagonal(this Position startingPosition, Position endingPosition)
        {
            return startingPosition.IsOnRankOrFile(endingPosition)
                || startingPosition.IsOnDiagonal(endingPosition);
        }

        #endregion

        #region Bishop moves

        internal static bool IsOnDiagonal(this Position startingPosition, Position endingPosition)
        {
            var rankIncrement = startingPosition.Rank - endingPosition.Rank;
            var fileIncrement = startingPosition.File - endingPosition.File;

            if (fileIncrement == 0)
                return false;

            return fileIncrement != 0
                && Math.Abs((double)rankIncrement / fileIncrement) == 1.00;
        }

        #endregion

        #region King moves

        internal static bool IsKingValidMove(Position startingPosition, Position endingPosition)
        {
            var rankDiff = Math.Abs(startingPosition.Rank - endingPosition.Rank);
            var fileDiff = Math.Abs(startingPosition.File - endingPosition.File);

            return (rankDiff != 0 || fileDiff != 0)
                && (rankDiff == 1 || rankDiff == 0)
                && (fileDiff == 1 || fileDiff == 0);
        }

        #endregion

        #region Pawn moves

        internal static bool IsOnSameFile(this Position startingPosition, Position endingPosition)
        {
            return startingPosition.File == endingPosition.File;
        }

        internal static int GetRankDifference(this Position startingPosition, Position endingPosition)
        {
            return endingPosition.Rank - startingPosition.Rank;
        }

        internal static int GetAbsRankDifference(this Position startingPosition, Position endingPosition)
        {
            return Math.Abs(startingPosition.GetRankDifference(endingPosition));
        }

        internal static int GetFileDifference(this Position startingPosition, Position endingPosition)
        {
            return endingPosition.File - startingPosition.File;
        }

        internal static bool IsOnDiagonalAdjacentSquare(this Position startingPosition, Position endingPosition)
        {
            return Math.Abs(startingPosition.GetRankDifference(endingPosition)) == 1
                && Math.Abs(startingPosition.GetFileDifference(endingPosition)) == 1;
        }

        #endregion

        #region In between positions

        internal static IEnumerable<Position> GetAllInBetweenPositions(this Position startingPosition, Position endingPosition)
        {
            var ret = new List<Position>();

            if (startingPosition.IsOnDiagonal(endingPosition))
                Extensions.AddRange(ret, GetDiagonalInBetweenPositions(startingPosition, endingPosition));
            if (startingPosition.IsOnRankOrFile(endingPosition))
                Extensions.AddRange(ret, GetStraightInBetweenPosition(startingPosition, endingPosition));

            return ret;
        }

        private static IEnumerable<Position> GetDiagonalInBetweenPositions(Position startingPosition, Position endingPosition)
        {
            var ret = new List<Position>();

            var shift = Math.Abs(endingPosition.Rank - startingPosition.Rank);
            var fileDirection = Math.Sign(endingPosition.File - startingPosition.File);
            var rankDirection = Math.Sign(endingPosition.Rank - startingPosition.Rank);

            for (int i = 1; i < shift; i++)
            {
                var rank = startingPosition.Rank + (rankDirection * i);
                var file = startingPosition.File + (fileDirection * i);
                ret.Add(new Position(rank, file));
            }

            return ret;
        }

        private static IEnumerable<Position> GetStraightInBetweenPosition(Position startingPosition, Position endingPosition)
        {
            var ret = new List<Position>();

            var fileShift = endingPosition.File - startingPosition.File;
            if (fileShift != 0)
            {
                var shiftDirection = Math.Sign(fileShift);
                var shiftAbs = Math.Abs(fileShift);
                for (int i = 1; i < shiftAbs; i++)
                {
                    var newFile = startingPosition.File + (shiftDirection * i);
                    ret.Add(new Position(startingPosition.Rank, newFile));
                }
            }
            else
            {
                var rankShift = endingPosition.Rank - startingPosition.Rank;
                var shiftDirection = Math.Sign(rankShift);
                var shiftAbs = Math.Abs(rankShift);
                for (int i = 1; i < shiftAbs; i++)
                {
                    var newRank = startingPosition.Rank + (shiftDirection * i);
                    ret.Add(new Position(newRank, startingPosition.File));
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
                if (board.IsAnyPieceInPosition(square))
                    return false;
            return true;
        }

        private static bool CastlePiecesHaveNotBeenMoved(King king, Board board, CastleType castleType)
        {
            if (king.HasBeenMoved)
                return false;

            var rookSquare = board.GetCell(king.GetCastleRookStartingSquare(castleType));
            return rookSquare.Piece != null
                && !rookSquare.Piece.HasBeenMoved;
        }

        private static bool CastlingSquaresAreNotControlledByOpponent(King king, Board board, CastleType castleType)
        {
            foreach (var square in king.GetCastlingSquares(castleType))
                if (board.IsInOpponentControl(board.GetCell(square), king.Color))
                    return false;

            return true;
        }

        #endregion

        internal static IEnumerable<Position> GetDiagonalAvailability(Position startingPosition, Board board, Color pieceColor, SquareInfluenceType availabilityType)
        {
            var ret = new List<Position>();

            //Upper right diagonal
            for (int i = 1; i < 8; i++)
            {
                var rank = startingPosition.Rank + i;
                var file = startingPosition.File + i;
                if (TryAddPieceGetIsLast(ret, startingPosition, rank, file, board, pieceColor, availabilityType))
                    break;
            }
            //Upper left diagonal
            for (int i = 1; i < 8; i++)
            {
                var rank = startingPosition.Rank + i;
                var file = startingPosition.File - i;
                if (TryAddPieceGetIsLast(ret, startingPosition, rank, file, board, pieceColor, availabilityType))
                    break;
            }
            //Bottom right diagonal
            for (int i = 1; i < 8; i++)
            {
                var rank = startingPosition.Rank - i;
                var file = startingPosition.File + i;
                if (TryAddPieceGetIsLast(ret, startingPosition, rank, file, board, pieceColor, availabilityType))
                    break;
            }
            //Bottom left diagonal
            for (int i = 1; i < 8; i++)
            {
                var rank = startingPosition.Rank - i;
                var file = startingPosition.File - i;
                if (TryAddPieceGetIsLast(ret, startingPosition, rank, file, board, pieceColor, availabilityType))
                    break;
            }

            return ret;
        }

        internal static IEnumerable<Position> GetLineAvailability(Position startingPosition, Board board, Color pieceColor, SquareInfluenceType availabilityType)
        {
            var ret = new List<Position>();

            //Upper line
            for (int i = 1; i < 8; i++)
            {
                var rank = startingPosition.Rank + i;
                var file = startingPosition.File;
                if (TryAddPieceGetIsLast(ret, startingPosition, rank, file, board, pieceColor, availabilityType))
                    break;
            }
            //Bottom line
            for (int i = 1; i < 8; i++)
            {
                var rank = startingPosition.Rank - i;
                var file = startingPosition.File;
                if (TryAddPieceGetIsLast(ret, startingPosition, rank, file, board, pieceColor, availabilityType))
                    break;
            }
            //Right line
            for (int i = 1; i < 8; i++)
            {
                var rank = startingPosition.Rank;
                var file = startingPosition.File + i;
                if (TryAddPieceGetIsLast(ret, startingPosition, rank, file, board, pieceColor, availabilityType))
                    break;
            }
            //Left line
            for (int i = 1; i < 8; i++)
            {
                var rank = startingPosition.Rank;
                var file = startingPosition.File - i;
                if (TryAddPieceGetIsLast(ret, startingPosition, rank, file, board, pieceColor, availabilityType))
                    break;
            }

            return ret;
        }

        internal static IEnumerable<Position> GetKingAvailability(King king, Position startingPosition, Board board, SquareInfluenceType availabilityType)
        {
            switch (availabilityType)
            {
                case SquareInfluenceType.Control:
                    return GetKingControl(startingPosition);
                case SquareInfluenceType.Mobility:
                    return GetKingMobility(king, startingPosition, board);
                default:
                    throw new NotImplementedException();
            }
        }

        internal static IEnumerable<Position> GetKingMobility(King king, Position startingPosition, Board board)
        {
            var ret = new List<Position>();

            Position? newPos = null;

            //Upper left
            if (TryGetNewPosition(startingPosition.Rank + 1, startingPosition.File - 1, out newPos))
                if (!board.IsControlledByOppositeColor(newPos.Value, king.Color)
                    && board.CanPieceOfColorGoToPosition(newPos.Value, king.Color))
                    ret.Add(newPos.Value);
            //Upper middle
            if (TryGetNewPosition(startingPosition.Rank + 1, startingPosition.File, out newPos))
                if (!board.IsControlledByOppositeColor(newPos.Value, king.Color)
                    && board.CanPieceOfColorGoToPosition(newPos.Value, king.Color))
                    ret.Add(newPos.Value);
            //Upper right
            if (TryGetNewPosition(startingPosition.Rank + 1, startingPosition.File + 1, out newPos))
                if (!board.IsControlledByOppositeColor(newPos.Value, king.Color)
                    && board.CanPieceOfColorGoToPosition(newPos.Value, king.Color))
                    ret.Add(newPos.Value);
            //Middle right
            if (TryGetNewPosition(startingPosition.Rank, startingPosition.File + 1, out newPos))
                if (!board.IsControlledByOppositeColor(newPos.Value, king.Color)
                    && board.CanPieceOfColorGoToPosition(newPos.Value, king.Color))
                    ret.Add(newPos.Value);
            //Middle left
            if (TryGetNewPosition(startingPosition.Rank, startingPosition.File - 1, out newPos))
                if (!board.IsControlledByOppositeColor(newPos.Value, king.Color)
                    && board.CanPieceOfColorGoToPosition(newPos.Value, king.Color))
                    ret.Add(newPos.Value);
            //Bottom right
            if (TryGetNewPosition(startingPosition.Rank - 1, startingPosition.File + 1, out newPos))
                if (!board.IsControlledByOppositeColor(newPos.Value, king.Color)
                    && board.CanPieceOfColorGoToPosition(newPos.Value, king.Color))
                    ret.Add(newPos.Value);
            //Bottom middle
            if (TryGetNewPosition(startingPosition.Rank - 1, startingPosition.File, out newPos))
                if (!board.IsControlledByOppositeColor(newPos.Value, king.Color)
                    && board.CanPieceOfColorGoToPosition(newPos.Value, king.Color))
                    ret.Add(newPos.Value);
            //Bottom left
            if (TryGetNewPosition(startingPosition.Rank - 1, startingPosition.File - 1, out newPos))
                if (!board.IsControlledByOppositeColor(newPos.Value, king.Color)
                    && board.CanPieceOfColorGoToPosition(newPos.Value, king.Color))
                    ret.Add(newPos.Value);

            //King side castle
            if (CanCastle(king, board, CastleType.KingSide))
                ret.Add(king.GetCastleEndingPosition(CastleType.KingSide));
            //Queen side castle
            if (CanCastle(king, board, CastleType.QueenSide))
                ret.Add(king.GetCastleEndingPosition(CastleType.QueenSide));

            return ret;
        }

        internal static IEnumerable<Position> GetKingControl(Position startingPosition)
        {
            var ret = new List<Position>();

            Position? newPos = null;

            //Old square
            ret.Add(startingPosition);
            //Upper left
            if (TryGetNewPosition(startingPosition.Rank + 1, startingPosition.File - 1, out newPos))
                ret.Add(newPos.Value);
            //Upper middle
            if (TryGetNewPosition(startingPosition.Rank + 1, startingPosition.File, out newPos))
                ret.Add(newPos.Value);
            //Upper right
            if (TryGetNewPosition(startingPosition.Rank + 1, startingPosition.File + 1, out newPos))
                ret.Add(newPos.Value);
            //Middle right
            if (TryGetNewPosition(startingPosition.Rank, startingPosition.File + 1, out newPos))
                ret.Add(newPos.Value);
            //Middle left
            if (TryGetNewPosition(startingPosition.Rank, startingPosition.File - 1, out newPos))
                ret.Add(newPos.Value);
            //Bottom right
            if (TryGetNewPosition(startingPosition.Rank - 1, startingPosition.File + 1, out newPos))
                ret.Add(newPos.Value);
            //Bottom middle
            if (TryGetNewPosition(startingPosition.Rank - 1, startingPosition.File, out newPos))
                ret.Add(newPos.Value);
            //Bottom left
            if (TryGetNewPosition(startingPosition.Rank - 1, startingPosition.File - 1, out newPos))
                ret.Add(newPos.Value);
            return ret;
        }

        //TODO position factory?
        internal static bool TryGetNewPosition(int rank, int file, out Position? position)
        {
            var isValidPos = IsValidPosition(rank, file);
            position = isValidPos ? new Position(rank, file) : (Position?)null;
            return isValidPos;
        }

        internal static IEnumerable<Position> GetKnightAvailability(Position startingPosition, Board board, Color pieceColor, SquareInfluenceType availabilityType)
        {
            var ret = new List<Position>();

            //Upper right, up
            var rank = startingPosition.Rank + 2;
            var file = startingPosition.File + 1;
            TryAddSquare(ret, startingPosition, rank, file, board, pieceColor, availabilityType);
            //Upper right, right
            rank = startingPosition.Rank + 1;
            file = startingPosition.File + 2;
            TryAddSquare(ret, startingPosition, rank, file, board, pieceColor, availabilityType);

            //Upper left, up
            rank = startingPosition.Rank + 2;
            file = startingPosition.File - 1;
            TryAddSquare(ret, startingPosition, rank, file, board, pieceColor, availabilityType);
            //Upper left, right
            rank = startingPosition.Rank + 1;
            file = startingPosition.File - 2;
            TryAddSquare(ret, startingPosition, rank, file, board, pieceColor, availabilityType);

            //Bottom right, bottom
            rank = startingPosition.Rank - 2;
            file = startingPosition.File + 1;
            TryAddSquare(ret, startingPosition, rank, file, board, pieceColor, availabilityType);
            //Bottom right, right
            rank = startingPosition.Rank - 1;
            file = startingPosition.File + 2;
            TryAddSquare(ret, startingPosition, rank, file, board, pieceColor, availabilityType);

            //Bottom left, bottom
            rank = startingPosition.Rank - 2;
            file = startingPosition.File - 1;
            TryAddSquare(ret, startingPosition, rank, file, board, pieceColor, availabilityType);
            //Bottom left, right
            rank = startingPosition.Rank - 1;
            file = startingPosition.File - 2;
            TryAddSquare(ret, startingPosition, rank, file, board, pieceColor, availabilityType);

            return ret;
        }

        #region  Pawn availability

        internal static IEnumerable<Position> GetPawnAvailability(Position startingPosition, Board board, Color pieceColor, SquareInfluenceType availabilityType)
        {
            switch (availabilityType)
            {
                case SquareInfluenceType.Control:
                    return GetPawnControl(startingPosition, board, pieceColor);
                case SquareInfluenceType.Mobility:
                    return GetPawnMobility(startingPosition, board, pieceColor);
                default:
                    throw new NotImplementedException();
            }
        }

        internal static IEnumerable<Position> GetPawnFrontSquares(Position square, Color color)
        {
            var ret = new List<Position>();

            //Left
            if (IsValidPosition(square.Rank + color.MovingDirection, square.File - 1))
                ret.Add(new Position(square.Rank + color.MovingDirection, square.File - 1));

            //In front
            if (IsValidPosition(square.Rank + color.MovingDirection, square.File))
                ret.Add(new Position(square.Rank + color.MovingDirection, square.File));

            //Right
            if (IsValidPosition(square.Rank + color.MovingDirection, square.File + 1))
                ret.Add(new Position(square.Rank + color.MovingDirection, square.File + 1));

            return ret;
        }

        #region Pawn control

        private static IEnumerable<Position> GetPawnControl(Position startingPosition, Board board, Color color)
        {
            var ret = new List<Position>();

            var captureRank = startingPosition.Rank + color.MovingDirection;
            var leftCaptureFile = startingPosition.File - 1;
            var rightCaptureFile = startingPosition.File + 1;

            if (IsValidPosition(captureRank, leftCaptureFile))
                ret.Add(new Position(captureRank, leftCaptureFile));

            if (IsValidPosition(captureRank, rightCaptureFile))
                ret.Add(new Position(captureRank, rightCaptureFile));

            return ret;
        }

        #endregion

        #region Pawn mobility

        private static IEnumerable<Position> GetPawnMobility(Position square, Board board, Color pieceColor)
        {
            var ret = new List<Position>();

            Position? allowedSquare = null;

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

        private static bool EnPassantAllowed(Board board, Color color, Position square, out Position? allowedSquare)
        {
            if (square.Rank == color.EnPassantStartingRank)
            {
                var lastMove = board.GetLastMove();
                if (lastMove != null)
                {
                    var lastMoveEnPassantInfo = lastMove.GetAllowEnPassantOnNextMoveInfo();
                    if (lastMoveEnPassantInfo.IsAllowed)
                    {
                        var leftCaptureFile = square.File - 1;
                        var rightCaptureFile = square.File + 1;

                        if (lastMoveEnPassantInfo.IsEnPassantFile(leftCaptureFile))
                        {
                            allowedSquare = new Position(color.EnPassantStartingRank, leftCaptureFile);
                            return true;
                        }
                        else if (lastMoveEnPassantInfo.IsEnPassantFile(rightCaptureFile))
                        {
                            allowedSquare = new Position(color.EnPassantStartingRank, rightCaptureFile);
                            return true;
                        }
                    }
                }
            }

            allowedSquare = null;
            return false;
        }

        private static bool IsPawnCaptureAllowed(Board board, Color color, Position square, int captureFile, out Position? allowedSquare)
        {
            var endingRank = square.Rank + color.MovingDirection;

            var isAllowed = IsValidPosition(square.Rank, captureFile)
                         && board.IsAnyPieceOfDifferentColorInPosition(new Position(endingRank, captureFile), color);

            allowedSquare = isAllowed
                ? new Position(endingRank, captureFile)
                : (Position?)null;

            return isAllowed;
        }

        private static bool DoubleForwardMoveAllowed(Board board, Color color, Position square, out Position? allowedSquare)
        {
            var singleRankMove = square.Rank + color.MovingDirection;
            var doubleRankMove = square.Rank + color.MovingDirection * 2;

            var isAllowed = square.Rank == color.PawnFirstRank
                && !board.IsAnyPieceInPosition(singleRankMove, square.File)
                && !board.IsAnyPieceInPosition(doubleRankMove, square.File);

            allowedSquare = isAllowed
                ? new Position(doubleRankMove, square.File)
                : (Position?)null;

            return isAllowed;
        }

        private static bool SingleForwardMoveAllowed(Board board, Color color, Position square, out Position? allowedSquare)
        {
            var singleRankMove = square.Rank + color.MovingDirection;

            var isAllowed = !board.IsAnyPieceInPosition(singleRankMove, square.File)
                         && IsValidPosition(singleRankMove, square.File);

            allowedSquare = isAllowed
                ? new Position(singleRankMove, square.File)
                : (Position?)null;

            return isAllowed;
        }

        #endregion

        #endregion

        private static bool TryAddPieceGetIsLast(List<Position> ret,
                                                 Position startingSquare,
                                                 int newRank,
                                                 int newFile,
                                                 Board board,
                                                 Color pieceColor,
                                                 SquareInfluenceType availabilityType)
        {
            if (!IsValidPosition(newRank, newFile))
                return true;

            TryAddSquare(ret, startingSquare, newRank, newFile, board, pieceColor, availabilityType);

            return board.IsAnyPieceInPosition(new Position(newRank, newFile));
        }

        private static void TryAddSquare(List<Position> ret,
                                         Position startingSquare,
                                         int newRank,
                                         int newFile,
                                         Board board,
                                         Color pieceColor,
                                         SquareInfluenceType availabilityType)
        {
            if (!IsValidPosition(newRank, newFile))
                return;

            var newSquare = new Position(newRank, newFile);

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

        private static bool IsInPieceMobility(Board board, Position startingSquare, Position endingSquare, Color color)
        {
            return board.CanPieceOfColorGoToPosition(endingSquare, color);
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
