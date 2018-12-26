using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCore
{
    internal class PiecesInfluenceManager
    {
        #region Fields

        private readonly PiecesControlManager _controlManager;
        private readonly PiecesMobilityManager _mobilityManager;

        #endregion

        #region Constructors

        public PiecesInfluenceManager(Board board)
        {
            var allSquares = new HashSet<Square>(board.GetAllBoardSquares());
            var pieceControlDict = GetPieceControlDict(allSquares);

            _controlManager = new PiecesControlManager(board, pieceControlDict, allSquares);
            _mobilityManager = new PiecesMobilityManager(board, pieceControlDict, allSquares);
        }

        #endregion

        #region Init

        private IDictionary<Square, ISet<Piece>> GetPieceControlDict(ISet<Square> allSquares)
        {
            var ret = new Dictionary<Square, ISet<Piece>>();

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

        internal bool IsControlledByColor(Square position, Color color)
        {
            return _controlManager.IsControlledByColor(position, color);
        }

        internal ISet<SquareCoordinate> GetOpponentControlAfterMove(Square startingCoordinate, Square endingCoordinate, Color color)
        {
            return _controlManager.GetOpponentControlAfterMove(startingCoordinate, endingCoordinate, color);
        }

        internal ISet<SquareCoordinate> GetOpponentControlOnSquare(Square position, Color color)
        {
            return _controlManager.GetOpponentControlOnPosition(position, color);
        }

        internal bool IsColorControllingSquare(Square square, Color color)
        {
            return _controlManager.IsColorControllingSquare(square, color);
        }

        #endregion

        #region Mobility methods

        internal bool HasAnyAvailableMove(Color color)
        {
            return _mobilityManager.HasAnyAvailableMove(color);
        }

        #endregion

        #region Temporary move

        internal void PlayTemporaryMove(TemporaryMoveDisposable disp)
        {
            _controlManager.PlayTemporaryMove(disp);
        }

        #endregion
    }
}
