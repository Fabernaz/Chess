using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessCore
{
    internal class PiecesControlManager
    {
        #region Fields

        private readonly Board _board;
        private readonly ISet<Square> _allSquares;
        private readonly IDictionary<Color, IDictionary<Piece, ISet<Square>>> _control;
        private readonly IDictionary<Color, ISet<Square>> _controlledSquares;
        private readonly IDictionary<Color, IList<Square>> _controlCardinality;
        private readonly IDictionary<Square, ISet<Piece>> _piecesControlDict;

        #endregion

        #region Constructors

        internal PiecesControlManager(Board board,
                                      IDictionary<Square, ISet<Piece>> pieceControlDict,
                                      ISet<Square> allSquares)
        {
            Guard.ArgumentNotNull(board, nameof(board));
            _board = board;
            Guard.ArgumentNotNull(allSquares, nameof(allSquares));
            _allSquares = allSquares;
            Guard.ArgumentNotNull(pieceControlDict, nameof(pieceControlDict));
            _piecesControlDict = pieceControlDict;

            _control = InitControlDict();
            _controlledSquares = InitControlledSquaresDict();
            _controlCardinality = GetControlCardinality();
        }

        internal void AddPieces(IEnumerable<Piece> pieces)
        {
            foreach (var piece in pieces)
                AddPiece(piece);
        }

        private void AddPiece(Piece addedPiece)
        {
            var controlledSquares = addedPiece.GetControlledSquares(_board)
                                              .Select(p => _board.GetSquare(p));

            _control[addedPiece.Color].Add(addedPiece, new HashSet<Square>());

            AddNewControlledSquares(addedPiece, controlledSquares);
            ResetSquaresControl();
        }

        private IDictionary<Color, IList<Square>> GetControlCardinality()
        {
            return new Dictionary<Color, IList<Square>>
            {
                { Color.White, new List<Square>() },
                { Color.Black, new List<Square>() }
            };
        }

        private IDictionary<Color, IDictionary<Piece, ISet<Square>>> InitControlDict()
        {
            return new Dictionary<Color, IDictionary<Piece, ISet<Square>>>
            {
                { Color.White, new Dictionary<Piece, ISet<Square>>() },
                { Color.Black, new Dictionary<Piece, ISet<Square>>() }
            };
        }

        private IDictionary<Color, ISet<Square>> InitControlledSquaresDict()
        {
            return new Dictionary<Color, ISet<Square>>
            {
                { Color.White, new HashSet<Square>() },
                { Color.Black, new HashSet<Square>() }
            };
        }

        #endregion

        internal void OnMovePlayed(MoveOperations moveOperations)
        {
            foreach (var pieceMove in moveOperations.MovedPieces)
                OnPieceMoved(pieceMove);

            foreach (var capturedPiece in moveOperations.CapturedPieces)
                OnPieceCaptured(capturedPiece);
        }

        #region Moved piece

        internal ISet<SquareCoordinate> GetOpponentControlAfterMove(Square startingCoordinate, Square endingCoordinate, Color color)
        {
            var ret = new HashSet<SquareCoordinate>();

            var opponentControllingPieces = _piecesControlDict[startingCoordinate]
                                                .Where(piece => piece.Color.IsOpponentColor(color));

            using (_board.PlayTemporaryMove(startingCoordinate, endingCoordinate))
            {
                foreach (var piece in opponentControllingPieces)
                    ret.AddRange(piece.GetControlledSquares(_board));
            };

            return ret;
        }

        internal ISet<SquareCoordinate> GetOpponentControlOnPosition(Square position, Color color)
        {
            var ret = new HashSet<SquareCoordinate>();
            
            var opponentControllingPieces = _piecesControlDict[position]
                                                .Where(piece => piece.Color.IsOpponentColor(color));

            foreach (var piece in opponentControllingPieces)
                ret.AddRange(piece.GetControlledSquares(_board));

            return ret;
        }

        internal bool IsColorControllingSquare(Square square, Color color)
        {
            return _controlledSquares[color].Contains(square);
        }

        private void OnPieceMoved(PieceMove move)
        {
            var colorControlList = _controlCardinality[move.MovedPiece.Color];
            var previouslyControlledSquares = _control[move.MovedPiece.Color][move.MovedPiece];
            var newControlledSquares = move.MovedPiece.GetControlledSquares(_board)
                                                      .Select(p => _board.GetSquare(p));

            RemovePreviousControlledSquares(move.MovedPiece, previouslyControlledSquares);
            AddNewControlledSquares(move.MovedPiece, newControlledSquares);
            if (move.StartingSquare != move.EndingSquare)
                RecalculateControlOnInterfearingPieceMoved(move);

            ResetSquaresControl();
        }

        private void RecalculateControlOnInterfearingPieceMoved(PieceMove move)
        {
            var controllingPieces = _piecesControlDict[move.StartingSquare];
            foreach (var controllingPiece in controllingPieces.Where(p => !p.Captured).ToList())
                OnPieceMoved(new PieceMove(move.StartingSquare, move.StartingSquare, controllingPiece, null));

            if (!move.IsCapture)
            {
                controllingPieces = _piecesControlDict[move.EndingSquare];
                foreach (var controllingPiece in controllingPieces.Where(p => !p.Captured).ToList())
                    OnPieceMoved(new PieceMove(move.EndingSquare, move.EndingSquare, controllingPiece, null));
            }
        }

        private void AddNewControlledSquares(Piece piece, IEnumerable<Square> controlledSquares)
        {
            _control[piece.Color][piece].AddRange(controlledSquares);
            _controlCardinality[piece.Color].AddRange(controlledSquares);
            _controlledSquares[piece.Color].AddRange(controlledSquares);
            foreach (var square in controlledSquares)
                _piecesControlDict[square].Add(piece);
        }

        private void RemovePreviousControlledSquares(Piece piece, IEnumerable<Square> previouslyControlledSquares)
        {
            _controlCardinality[piece.Color].RemoveRange(previouslyControlledSquares);
            _control[piece.Color][piece] = new HashSet<Square>();
            foreach (var square in previouslyControlledSquares)
                _piecesControlDict[square].Remove(piece);

            foreach (var square in previouslyControlledSquares)
                if (!_controlCardinality[piece.Color].Contains(square))
                    _controlledSquares[piece.Color].Remove(square);
        }

        private void ResetSquaresControl()
        {
            foreach (var square in _allSquares)
            {
                square.IsControlledByWhite = false;
                square.IsControlledByBlack = false;
            }

            var controlledSquares = _controlledSquares[Color.White];
            foreach (var square in controlledSquares)
                square.IsControlledByWhite = true;

            controlledSquares = _controlledSquares[Color.Black];
            foreach (var square in controlledSquares)
                square.IsControlledByBlack = true;
        }

        #endregion

        private void OnPieceCaptured(Piece capturedPiece)
        {
            var previouslyControlledSquares = _control[capturedPiece.Color][capturedPiece];

            RemovePreviousControlledSquares(capturedPiece, previouslyControlledSquares);
            _control[capturedPiece.Color].Remove(capturedPiece);
            _piecesControlDict[capturedPiece.CurrentSquare].Remove(capturedPiece);
            ResetSquaresControl();
        }

        internal bool IsControlledByOppositeColor(Square position, Color color)
        {
            return _controlledSquares[color.OpponentColor].Contains(position);
        }

        internal void PlayTemporaryMove(TemporaryMoveDisposable disp)
        {
            var attackingOpponentPieces = _piecesControlDict[disp.StartingSquare];
            attackingOpponentPieces.Add(disp.MovedPiece);

            foreach (var attackingPiece in attackingOpponentPieces.Where(p => !p.Captured).ToList())
            {
                var newControlledSquares = attackingPiece.GetControlledSquares(_board)
                                             .Select(p => _board.GetSquare(p));
                var previouslyControlledSquares = _control[attackingPiece.Color][attackingPiece];

                ReplaceSquareInPiecesControlDict(attackingPiece, newControlledSquares, previouslyControlledSquares);

                disp.Disposing += (sender, e) =>
                {
                    ReplaceSquareInPiecesControlDict(attackingPiece, previouslyControlledSquares, newControlledSquares);
                };
            }
        }

        private void ReplaceSquareInPiecesControlDict(Piece piece, IEnumerable<Square> newControlledSquares, IEnumerable<Square> previouslyControlledSquares)
        {
            foreach (var square in previouslyControlledSquares)
                _piecesControlDict[square].Remove(piece);

            foreach (var square in newControlledSquares)
                _piecesControlDict[square].Add(piece);
        }
    }
}

