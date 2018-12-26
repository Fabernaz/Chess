using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace ChessCore
{
    internal class PiecesMobilityManager
    {
        #region Fields

        private readonly IDictionary<Pawn, ISet<SquareCoordinate>> _pawnsFrontSquaresDict;
        private readonly IDictionary<SquareCoordinate, ISet<Pawn>> _squaresControlledByPawnDict;
        private readonly Board _board;
        private readonly ISet<Square> _allSquares;
        private readonly IDictionary<Square, ISet<Piece>> _piecesControlDict;

        #endregion

        #region Constructors

        public PiecesMobilityManager(Board board,
                                     IDictionary<Square, ISet<Piece>> pieceControlDict,
                                     ISet<Square> allSquares)
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

        private IDictionary<Pawn, ISet<SquareCoordinate>> InitPawnFrontSquaresDict()
        {
            return new Dictionary<Pawn, ISet<SquareCoordinate>>();
        }

        private IDictionary<SquareCoordinate, ISet<Pawn>> InitSquareControlledByPawnDict()
        {
            var ret = new Dictionary<SquareCoordinate, ISet<Pawn>>();

            foreach (var square in _allSquares)
                ret.Add(square.Coordinate, new HashSet<Pawn>());

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
                _pawnsFrontSquaresDict.Add(addedPiece as Pawn, new HashSet<SquareCoordinate>());
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
                                    move.StartingSquare,
                                    move.EndingSquare);
        }

        #endregion

        #region Mobility

        internal bool HasAnyAvailableMove(Color color)
        {
            var allColorPieces = _board.GetAllPiecesOfColor(color);

            foreach (var piece in allColorPieces)
                if (piece.GetAvailableMoves().Any())
                    return true;

            return false;
        }

        private void ResetPiecesMobility(Piece movedPiece, Square startingSquare, Square endingSquare)
        {
            HandlePawnMobility(movedPiece, startingSquare.Coordinate, endingSquare.Coordinate);
            ResetKingMobility();
            ResetPieceMobility(movedPiece, startingSquare, endingSquare);
        }

        private void ResetKingMobility()
        {
            _board.GetKing(Color.White).ResetAvailableMoves(_board);
            _board.GetKing(Color.Black).ResetAvailableMoves(_board);
        }

        private void ResetPieceMobility(Piece movedPiece, Square startingSquare, Square endingSquare)
        {
            var allInfluencedPieces = new List<Piece>();

            var previouslyAvailableMoves = movedPiece.GetAvailableMoves();
            movedPiece.ResetAvailableMoves(_board);
            var newAvailableMoves = movedPiece.GetAvailableMoves();

            var piecesControllingMovingSquares = _piecesControlDict[startingSquare]
                .Concat(_piecesControlDict[endingSquare])
                .Where(p => !p.Equals(movedPiece));
            allInfluencedPieces.AddRange(piecesControllingMovingSquares);

            var attackedPieces = newAvailableMoves.Select(coordinate => _board[coordinate])
                                           .Where(p => p.ContainsPiece())
                                           .Where(p => p.Piece.Color.IsOpponentColor(movedPiece.Color))
                                           .Select(p => p.Piece);
            allInfluencedPieces.AddRange(attackedPieces);

            var previouslyAttackedPieces = previouslyAvailableMoves.Select(coordinate => _board[coordinate])
                                           .Where(p => p.ContainsPiece())
                                           .Where(p => p.Piece.Color.IsOpponentColor(movedPiece.Color))
                                           .Select(p => p.Piece);
            allInfluencedPieces.AddRange(previouslyAttackedPieces);

            foreach (var piece in allInfluencedPieces.Where(p => !p.Captured))
                piece.ResetAvailableMoves(_board);
        }

        private void HandlePawnMobility(Piece piece, params SquareCoordinate[] squares)
        {
            ResetPawnFrontSquares(piece);
            ResetPawnMobility(squares);
        }

        private void ResetPawnMobility(params SquareCoordinate[] squares)
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
