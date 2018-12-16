using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessCore
{
    internal class PiecesControlManager
    {
        #region Fields

        private readonly Board _board;
        private readonly ISet<BoardCell> _allSquares;
        private readonly IDictionary<Color, IDictionary<Piece, ISet<BoardCell>>> _control;
        private readonly IDictionary<Color, ISet<BoardCell>> _controlledSquares;
        private readonly IDictionary<Color, IList<BoardCell>> _controlCardinality;
        private readonly IDictionary<BoardCell, ISet<Piece>> _piecesControlDict;

        #endregion

        #region Constructors

        internal PiecesControlManager(Board board, 
                                      IDictionary<BoardCell, ISet<Piece>> pieceControlDict,
                                      ISet<BoardCell> allSquares)
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
                                              .Select(p => _board.GetCell(p));

            _control[addedPiece.Color].Add(addedPiece, new HashSet<BoardCell>());

            AddNewControlledSquares(addedPiece, controlledSquares);
            ResetSquaresControl();
        }

        private IDictionary<Color, IList<BoardCell>> GetControlCardinality()
        {
            return new Dictionary<Color, IList<BoardCell>>
            {
                { Color.White, new List<BoardCell>() },
                { Color.Black, new List<BoardCell>() }
            };
        }

        private IDictionary<Color, IDictionary<Piece, ISet<BoardCell>>> InitControlDict()
        {
            return new Dictionary<Color, IDictionary<Piece, ISet<BoardCell>>>
            {
                { Color.White, new Dictionary<Piece, ISet<BoardCell>>() },
                { Color.Black, new Dictionary<Piece, ISet<BoardCell>>() }
            };
        }

        private IDictionary<Color, ISet<BoardCell>> InitControlledSquaresDict()
        {
            return new Dictionary<Color, ISet<BoardCell>>
            {
                { Color.White, new HashSet<BoardCell>() },
                { Color.Black, new HashSet<BoardCell>() }
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

        internal ISet<Position> GetOpponentControlAfterMove(BoardCell startingPosition, BoardCell endingPosition, Color color)
        {
            var ret = new HashSet<Position>();

            var opponentControllingPieces = _piecesControlDict[startingPosition]
                                                .Where(piece => piece.Color.IsOpponentColor(color));

            using (_board.PlayTemporaryMove(startingPosition, endingPosition))
            {
                foreach (var piece in opponentControllingPieces)
                    ret.AddRange(piece.GetControlledSquares(_board));
            };

            return ret;
        }

        internal ISet<Position> GetOpponentControlOnPosition(BoardCell position, Color color)
        {
            var ret = new HashSet<Position>();

            var allPieces = _piecesControlDict[position];
            var opponentPiecesss = allPieces.Where(piece => piece.Color.IsOpponentColor(color));

            var opponentControllingPieces = _piecesControlDict[position]
                                                .Where(piece => piece.Color.IsOpponentColor(color));

            foreach (var piece in opponentControllingPieces)
                ret.AddRange(piece.GetControlledSquares(_board));

            return ret;
        }

        internal bool IsColorControllingSquare(BoardCell square, Color color)
        {
            return _controlledSquares[color].Contains(square);
        }

        private void OnPieceMoved(PieceMove move)
        {
            var colorControlList = _controlCardinality[move.MovedPiece.Color];
            var previouslyControlledSquares = _control[move.MovedPiece.Color][move.MovedPiece];
            var newControlledSquares = move.MovedPiece.GetControlledSquares(_board)
                                                      .Select(p => _board.GetCell(p));

            RemovePreviousControlledSquares(move.MovedPiece, previouslyControlledSquares);
            AddNewControlledSquares(move.MovedPiece, newControlledSquares);
            if (move.StartingPosition != move.EndingPosition)
                RecalculateControlOnInterfearingPieceMoved(move);

            ResetSquaresControl();
            
        }

        private void RecalculateControlOnInterfearingPieceMoved(PieceMove move)
        {
            var startingCell = _board.GetCell(move.StartingPosition);
            var endingCell = _board.GetCell(move.EndingPosition);

            var controllingPieces = _piecesControlDict[startingCell];
            foreach (var controllingPiece in controllingPieces.ToList())
                OnPieceMoved(new PieceMove(startingCell.Position, startingCell.Position, controllingPiece, null));

            if (!move.IsCapture)
            {
                controllingPieces = _piecesControlDict[endingCell];
                foreach (var controllingPiece in controllingPieces.ToList())
                    OnPieceMoved(new PieceMove(endingCell.Position, endingCell.Position, controllingPiece, null));
            }
        }

        private void AddNewControlledSquares(Piece piece, IEnumerable<BoardCell> controlledSquares)
        {
            _control[piece.Color][piece].AddRange(controlledSquares);
            _controlCardinality[piece.Color].AddRange(controlledSquares);
            _controlledSquares[piece.Color].AddRange(controlledSquares);
            foreach (var square in controlledSquares)
                _piecesControlDict[square].Add(piece);
        }

        private void RemovePreviousControlledSquares(Piece piece, IEnumerable<BoardCell> previouslyControlledSquares)
        {
            _controlCardinality[piece.Color].RemoveRange(previouslyControlledSquares);
            _control[piece.Color][piece] = new HashSet<BoardCell>();
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
            ResetSquaresControl();
        }

        internal bool IsControlledByOppositeColor(BoardCell position, Color color)
        {
            return _controlledSquares[color.OpponentColor].Contains(position);
        }

        internal void PlayTemporaryMove(TemporaryMoveDisposable disp)
        {
            var attackingOpponentPieces = _piecesControlDict[disp._startingCell]
                .Where(p => p.Color.IsOpponentColor(disp._movedPiece.Color));

            foreach(var attackingPiece in attackingOpponentPieces.ToList())
            {
                var newControlledSquares = attackingPiece.GetControlledSquares(_board)
                                             .Select(p => _board.GetCell(p));
                var previouslyControlledSquares = _control[attackingPiece.Color][attackingPiece];

                ReplateSquareInPiecesControlDict(attackingPiece, newControlledSquares, previouslyControlledSquares);

                disp.Disposing += (sender, e) =>
                {
                    ReplateSquareInPiecesControlDict(attackingPiece, previouslyControlledSquares, newControlledSquares);
                };
            }
        }

        private void ReplateSquareInPiecesControlDict(Piece piece, IEnumerable<BoardCell> newControlledSquares, IEnumerable<BoardCell> previouslyControlledSquares)
        {
            foreach (var square in previouslyControlledSquares)
                _piecesControlDict[square].Remove(piece);

            foreach (var square in newControlledSquares)
                _piecesControlDict[square].Add(piece);
        }
    }
}

