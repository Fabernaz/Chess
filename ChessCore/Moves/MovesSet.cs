using System;
using System.Collections.Generic;

namespace ChessCore
{
    public class MovesSet
    {
        private readonly ISet<MovePair> _moves;
        private Color _currentPieceMoveColor;
        private MovePair _currentMove;
        private MoveBase _lastMoveAdded;

        internal MovesSet()
        {
            _moves = new HashSet<MovePair>();
            _currentPieceMoveColor = Color.White;
        }

        internal MoveBase GetLastMove()
        {
            return _lastMoveAdded;
        }

        internal void OnMovePlayed(MoveBase move)
        {
            RegisterMove(move);
            FlipColor();
        }

        private void RegisterMove(MoveBase move)
        {
            if (_currentPieceMoveColor.IsWhite)
                AddNewMovePair(move);
            else if (_currentPieceMoveColor.IsBlack)
                UpdateBlackInLastMove(move);
            else
                throw new NotImplementedException();

            _lastMoveAdded = move;
        }

        private void UpdateBlackInLastMove(MoveBase move)
        {
            _currentMove.BlackMove = move;
            _currentMove = null;
        }

        private void AddNewMovePair(MoveBase move)
        {
            _currentMove = new MovePair(_moves.Count + 1, move);
            _moves.Add(_currentMove);
        }

        private void FlipColor()
        {
            _currentPieceMoveColor = _currentPieceMoveColor.OpponentColor;
        }
    }
}
