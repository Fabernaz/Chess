using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCore
{
    internal class PiecesInfluenceManager
    {
        private readonly PiecesControlManager _controlManager;
        private readonly PiecesMobilityManager _mobilityManager;

        #region Constructors

        public PiecesInfluenceManager(Board board)
        {
            var allSquares = new HashSet<BoardCell>(board.GetAllBoardSquares());
            var pieceControlDict = GetPieceControlDict(allSquares);

            _controlManager = new PiecesControlManager(board, pieceControlDict, allSquares);
            _mobilityManager = new PiecesMobilityManager(board, pieceControlDict, allSquares);
        }

        #endregion

        #region Init

        private IDictionary<BoardCell, ISet<Piece>> GetPieceControlDict(ISet<BoardCell> allSquares)
        {
            var ret = new Dictionary<BoardCell, ISet<Piece>>();

            foreach (var square in allSquares)
                ret.Add(square, new HashSet<Piece>());

            return ret;
        }

        #endregion

        #region Add pieces

        internal void AddPieces(IEnumerable<Piece> pieces)
        {
            _controlManager.AddPieces(pieces);
            _mobilityManager.AddPieces(pieces);
        }

        #endregion

        #region Move played

        internal void OnMovePlayed(MoveOperations moveOperations)
        {
            _controlManager.OnMovePlayed(moveOperations);
            _mobilityManager.OnMovePlayed(moveOperations);
        }

        #endregion

        #region Control methods

        internal bool IsControlledByOppositeColor(BoardCell position, Color color)
        {
            return _controlManager.IsControlledByOppositeColor(position, color);
        }

        internal ISet<Position> GetOpponentControlAfterMove(BoardCell startingPosition, BoardCell endingPosition, Color color)
        {
            return _controlManager.GetOpponentControlAfterMove(startingPosition, endingPosition, color);
        }

        internal ISet<Position> GetOpponentControlOnPosition(BoardCell position, Color color)
        {
            return _controlManager.GetOpponentControlOnPosition(position, color);
        }

        internal bool IsColorControllingSquare(BoardCell square, Color color)
        {
            return _controlManager.IsColorControllingSquare(square, color);
        }

        #endregion

        internal void PlayTemporaryMove(TemporaryMoveDisposable disp)
        {
            _controlManager.PlayTemporaryMove(disp);
        }
    }
}
