using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace ChessCore
{
    internal class EndGameRules
    {
        private readonly TimerManager _timerManager;
        private readonly MovesSet _movesSet;
        private readonly PiecesInfluenceManager _piecesInfluenceManager;
        private readonly Board _board;
        private readonly List<ISet<ThreefoldRepetitionPositionUnit>> _positionsToCheckForThreefoldRepetition;
        private bool _gameEnded;

        internal event EventHandler<GameEndedReasonEventArgs> GameEndedEvent;

        internal EndGameRules(Board board,
                              TimerManager timerManager,
                              MovesSet movesSet,
                              PiecesInfluenceManager piecesInfluenceManager)
        {
            Guard.ArgumentNotNull(board, nameof(board));
            _board = board;
            Guard.ArgumentNotNull(timerManager, nameof(timerManager));
            _timerManager = timerManager;
            Guard.ArgumentNotNull(movesSet, nameof(movesSet));
            _movesSet = movesSet;
            Guard.ArgumentNotNull(piecesInfluenceManager, nameof(piecesInfluenceManager));
            _piecesInfluenceManager = piecesInfluenceManager;

            _positionsToCheckForThreefoldRepetition = new List<ISet<ThreefoldRepetitionPositionUnit>>();
            _positionsToCheckForThreefoldRepetition.Add(GetPositionForThreefoldRepetition(_board.NextMoveTurn.OpponentColor));

            GameEndedEvent += OnGameEnded;

            InitTimeUpRule();
        }

        private void OnGameEnded(object sender, GameEndedReasonEventArgs e)
        {
            _gameEnded = true;
            _timerManager.OnGameEnded();
        }

        internal void OnMovePlayed(Move move)
        {
            if (!_gameEnded)
                CheckForMates(move.MovingColor);
            if (!_gameEnded)
                CheckFor50MovesRule();
            if (!_gameEnded)
                CheckForInsufficientMaterial();
            if (!_gameEnded)
                CheckForThreefoldRepetition(move);
        }

        #region Insufficient material

        private bool CheckForInsufficientMaterial()
        {
            var allPiecesExceptKings = _board.GetAllPieces().Where(p => !(p is King));

            return IsKingsDraw(allPiecesExceptKings)
                || IsKingsAndBishopsDraw(allPiecesExceptKings)
                || IsKingsAndKnightDraw(allPiecesExceptKings);
        }

        private bool IsKingsAndKnightDraw(IEnumerable<Piece> allPiecesExceptKings)
        {
            return allPiecesExceptKings.Count() == 1
                && allPiecesExceptKings.First() is Knight;
        }

        private bool IsKingsDraw(IEnumerable<Piece> allPiecesExceptKings)
        {
            return !allPiecesExceptKings.Any();
        }

        private bool IsKingsAndBishopsDraw(IEnumerable<Piece> allPiecesExceptKings)
        {
            if (allPiecesExceptKings.Any(p => !(p is Bishop)))
                return false;

            return allPiecesExceptKings.Any(p => p.Color != allPiecesExceptKings.First().Color);
        }

        #endregion

        #region Threefold repetition

        private void CheckForThreefoldRepetition(Move move)
        {
            var currentPosition = GetPositionForThreefoldRepetition(move.MovingColor);

            if (IsThreefoldRepetition(currentPosition))
                GameEndedEvent?.Invoke(this, new GameEndedReasonEventArgs(GameEndedReason.ThreefoldRepetition));
            else
                UpdateThreefoldRepetitionInformation(move, currentPosition);
        }

        private void UpdateThreefoldRepetitionInformation(Move move, ISet<ThreefoldRepetitionPositionUnit> currentPosition)
        {
            if (MoveBreaksThreefoldRepetitionRule(move))
                _positionsToCheckForThreefoldRepetition.Clear();
            else
                _positionsToCheckForThreefoldRepetition.Add(currentPosition);
        }

        private bool MoveBreaksThreefoldRepetitionRule(Move move)
        {
            return move is Castle
                || move is PawnPromotion
                || move.MovedPiece is Pawn
                || move.IsCapture;
        }

        private ISet<ThreefoldRepetitionPositionUnit> GetPositionForThreefoldRepetition(Color movingColor)
        {
            var ret = new HashSet<ThreefoldRepetitionPositionUnit>();

            var pieces = _board.GetAllPieces();
            foreach (var piece in pieces)
                ret.AddRange(piece.GetAvailableMoves()
                   .Select(endingSquare => GetThreefoldRepetitionPositionUnit(piece, endingSquare, movingColor)));

            return ret;
        }

        private ThreefoldRepetitionPositionUnit GetThreefoldRepetitionPositionUnit(Piece piece, SquareCoordinate endingSquare, Color movingColor)
        {
            return new ThreefoldRepetitionPositionUnit(piece.CurrentSquare.Coordinate, endingSquare, piece, movingColor.OpponentColor);
        }

        private bool IsThreefoldRepetition(ISet<ThreefoldRepetitionPositionUnit> currentPosition)
        {
            int equalPositions = 0;
            foreach (var u in _positionsToCheckForThreefoldRepetition)
                if (currentPosition.SetEquals(u) && ++equalPositions == 3)
                    return true;
            return false;
        }

        #endregion

        #region 50-moves rule

        private void CheckFor50MovesRule()
        {
            if (_movesSet.NoPawnOrCaptureMovesNumber >= 50)
                GameEndedEvent.Invoke(this, new GameEndedReasonEventArgs(GameEndedReason.FiftyRule));
        }

        #endregion

        #region Mates

        private void CheckForMates(Color movingColor)
        {
            var hasAnyAvailableMove = _piecesInfluenceManager.HasAnyAvailableMove(movingColor.OpponentColor);
            if (hasAnyAvailableMove)
                return;

            if (_board.IsInCheck(movingColor.OpponentColor))
                GameEndedEvent.Invoke(this, new GameEndedReasonEventArgs(GameEndedReason.CheckMate, movingColor));
            else
                GameEndedEvent.Invoke(this, new GameEndedReasonEventArgs(GameEndedReason.StaleMate));
        }

        #endregion

        #region Time up rule

        private void InitTimeUpRule()
        {
            _timerManager.BlackTimesUp += (sender, e) => { GameEndedEvent?.Invoke(this, new GameEndedReasonEventArgs(GameEndedReason.TimeUp, Color.White)); };
            _timerManager.WhiteTimesUp += (sender, e) => { GameEndedEvent?.Invoke(this, new GameEndedReasonEventArgs(GameEndedReason.TimeUp, Color.Black)); };
        }

        #endregion
    }


}
