using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessCore
{
    public class MovesSet
    {
        private readonly List<MovePair> _movePairs;
        private Color _currentPieceMoveColor;
        private MovePair _currentMovePair;
        private Move _lastMoveAdded;

        public int NoPawnOrCaptureMovesNumber { get; private set; }

        internal MovesSet()
        {
            _movePairs = new List<MovePair>();
            _currentPieceMoveColor = Color.White;
        }

        internal Move GetLastMove()
        {
            return _lastMoveAdded;
        }

        internal void OnMovePlayed(Move move)
        {
            RegisterMove(move);
            FlipColor();
        }

        private void RegisterMove(Move move)
        {
            if (_currentPieceMoveColor.IsWhite)
                OnWhiteMoved(move);
            else
                OnBlackMoved(move);

            _lastMoveAdded = move;
        }

        private void OnWhiteMoved(Move move)
        {
            _currentMovePair = new MovePair(_movePairs.Count + 1, move);
            _movePairs.Add(_currentMovePair);
        }

        private void OnBlackMoved(Move move)
        {
            UpdateBlackInLastMove(move);
            RegisterLastMovePairFor50Rule();
        }

        private void UpdateBlackInLastMove(Move move)
        {
            _currentMovePair.BlackMove = move;
        }

        private void RegisterLastMovePairFor50Rule()
        {
            if (_currentPieceMoveColor == Color.White)
                return;

            if (MovePairBreaks50Rule(_currentMovePair))
                NoPawnOrCaptureMovesNumber = 0;
            else
                NoPawnOrCaptureMovesNumber++;
        }

        private bool MovePairBreaks50Rule(MovePair movePair)
        {
            return MoveBreaks50Rule(_currentMovePair.WhiteMove)
                || MoveBreaks50Rule(_currentMovePair.BlackMove);
        }

        private bool MoveBreaks50Rule(Move move)
        {
            return move.MovedPiece is Pawn
                || move.IsCapture;
        }

        private void FlipColor()
        {
            _currentPieceMoveColor = _currentPieceMoveColor.OpponentColor;
        }

        internal IEnumerable<MovePair> GetAllMoves()
        {
            return _movePairs;
        }
    }
}
