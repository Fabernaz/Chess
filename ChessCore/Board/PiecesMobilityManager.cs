using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCore
{
    internal class PiecesMobilityManager
    {
        #region Fields

        private readonly IDictionary<Pawn, ISet<Position>> _pawnsFrontSquaresDict;
        private readonly IDictionary<Position, ISet<Pawn>> _squaresControlledByPawnDict;
        private readonly Board _board;
        private readonly ISet<BoardCell> _allSquares;
        private readonly IDictionary<BoardCell, ISet<Piece>> _piecesControlDict;

        #endregion

        #region Constructors

        public PiecesMobilityManager(Board board,
                                     IDictionary<BoardCell, ISet<Piece>> pieceControlDict,
                                     ISet<BoardCell> allSquares)
        {
            Guard.ArgumentNotNull(board, nameof(board));
            _board = board;
            Guard.ArgumentNotNull(allSquares, nameof(allSquares));
            _allSquares = allSquares;
            Guard.ArgumentNotNull(pieceControlDict, nameof(pieceControlDict));
            _piecesControlDict = pieceControlDict;

            _pawnsFrontSquaresDict = InitPawnFrontSquaresDict();
            _squaresControlledByPawnDict = InitSquareControlledByPawnDict();
        }

        #endregion

        #region Init

        private IDictionary<Pawn, ISet<Position>> InitPawnFrontSquaresDict()
        {
            return new Dictionary<Pawn, ISet<Position>>();
        }

        private IDictionary<Position, ISet<Pawn>> InitSquareControlledByPawnDict()
        {
            var ret = new Dictionary<Position, ISet<Pawn>>();

            foreach (var square in _allSquares)
                ret.Add(square.Position, new HashSet<Pawn>());

            return ret;
        }

        #endregion

        #region Pieces adding

        internal void AddPieces(IEnumerable<Piece> pieces)
        {
            foreach (var piece in pieces)
                OnPieceAdded(piece);

            SetAllPiecesMobility(pieces);
        }

        private void OnPieceAdded(Piece addedPiece)
        {
            if (addedPiece is Pawn)
                _pawnsFrontSquaresDict.Add(addedPiece as Pawn, new HashSet<Position>());
        }

        private void SetAllPiecesMobility(IEnumerable<Piece> pieces)
        {
            foreach (var piece in pieces)
                piece.ResetAvailableMoves(_board);
        }

        #endregion

        #region Move played

        internal void OnMovePlayed(MoveOperations moveOperations)
        {
            foreach (var move in moveOperations.MovedPieces)
                ResetPiecesMobility(move.MovedPiece,
                                    _board.GetCell(move.StartingPosition),
                                    _board.GetCell(move.EndingPosition));
        }

        #endregion

        #region Mobility

        private void ResetPiecesMobility(Piece movedPiece, BoardCell startingSquare, BoardCell endingSquare)
        {
            HandlePawnMobility(movedPiece, startingSquare.Position, endingSquare.Position);
            ResetKingMobility();
            ResetPieceMobility(movedPiece, startingSquare, endingSquare);
        }

        private void ResetKingMobility()
        {
            _board.GetKing(Color.White).ResetAvailableMoves(_board);
            _board.GetKing(Color.Black).ResetAvailableMoves(_board);
        }

        private void ResetPieceMobility(Piece movedPiece, BoardCell startingSquare, BoardCell endingSquare)
        {
            var allInfluencedPieces = new List<Piece>();

            var previouslyAvailableMoves = movedPiece.GetAvailableMoves();
            movedPiece.ResetAvailableMoves(_board);
            var newAvailableMoves = movedPiece.GetAvailableMoves();

            var piecesControllingMovingSquares = _piecesControlDict[startingSquare]
                .Concat(_piecesControlDict[endingSquare])
                .Where(p => !p.Equals(movedPiece));
            allInfluencedPieces.AddRange(piecesControllingMovingSquares);

            var attackedPieces = newAvailableMoves.Select(p => _board.GetCell(p))
                                           .Where(p => p.ContainsPiece())
                                           .Where(p => p.Piece.Color.IsOpponentColor(movedPiece.Color))
                                           .Select(p => p.Piece);
            allInfluencedPieces.AddRange(attackedPieces);

            var previouslyAttackedPieces = previouslyAvailableMoves.Select(p => _board.GetCell(p))
                                           .Where(p => p.ContainsPiece())
                                           .Where(p => p.Piece.Color.IsOpponentColor(movedPiece.Color))
                                           .Select(p => p.Piece);
            allInfluencedPieces.AddRange(previouslyAttackedPieces);

            foreach (var attackedPiece in allInfluencedPieces)
                attackedPiece.ResetAvailableMoves(_board);
        }

        private void HandlePawnMobility(Piece piece, params Position[] squares)
        {
            ResetPawnFrontSquares(piece);
            ResetPawnMobility(squares);
        }

        private void ResetPawnMobility(params Position[] squares)
        {
            var influencedPawns = new List<Pawn>();
            foreach (var square in squares)
                influencedPawns.AddRange(_squaresControlledByPawnDict[square]);

            foreach (var pawn in influencedPawns)
                pawn.ResetAvailableMoves(_board);
        }

        private void ResetPawnFrontSquares(Piece piece)
        {
            if (!(piece is Pawn))
                return;

            var pawn = piece as Pawn;

            var previousFrontSquares = _pawnsFrontSquaresDict[pawn];
            var newFrontSquares = pawn.GetFrontSquares();

            foreach (var square in previousFrontSquares)
                _squaresControlledByPawnDict[square].Remove(pawn);

            _pawnsFrontSquaresDict[pawn].Clear();
            foreach (var square in previousFrontSquares)
                _squaresControlledByPawnDict[square].Remove(pawn);

            _pawnsFrontSquaresDict[pawn].AddRange(newFrontSquares);
            foreach (var square in newFrontSquares)
                _squaresControlledByPawnDict[square].Add(pawn);
        }

        #endregion
    }
}
