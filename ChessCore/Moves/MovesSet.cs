using System;
using System.Collections.Generic;

namespace ChessCore
{
    public class MovesSet
    {
        private readonly List<MovePair> _moves;
        private Color _currentPieceMoveColor;
        private MovePair _currentMove;
        private Move _lastMoveAdded;

        internal MovesSet()
        {
            _moves = new List<MovePair>();
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
                AddNewMovePair(move);
            else if (_currentPieceMoveColor.IsBlack)
                UpdateBlackInLastMove(move);
            else
                throw new NotImplementedException();

            _lastMoveAdded = move;
        }

        private void UpdateBlackInLastMove(Move move)
        {
            _currentMove.BlackMove = move;
            _currentMove = null;
        }

        private void AddNewMovePair(Move move)
        {
            _currentMove = new MovePair(_moves.Count + 1, move);
            _moves.Add(_currentMove);
        }

        private void FlipColor()
        {
            _currentPieceMoveColor = _currentPieceMoveColor.OpponentColor;
        }

        internal IEnumerable<MovePair> GetAllMoves()
        {
            return _moves;
        }
    }
}
