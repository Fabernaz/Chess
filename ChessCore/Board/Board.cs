using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCore
{
    //TODO: event dispose to avoid memory leaks: is necessary?

    public class Board : Model
    {
        #region Fields

        private readonly ValidMoveFactory _moveFactory;
        private IDictionary<Position, BoardCell> _board;
        private readonly IDictionary<Color, IList<Piece>> _capturedPieces;
        private readonly IDictionary<Color, IList<Piece>> _playingPieces;
        private MovesSet _movesRegister;
        private readonly PiecesInfluenceManager _piecesInflueceManager;
        private readonly IDictionary<Color, King> _kingsDict;
        private readonly IDictionary<Color, ISet<Piece>> _pinningPieces;

        #endregion

        #region Properties

        public Color NextMoveTurn { get; private set; }

        #endregion

        #region Constructors

        private static Board _instance;

        internal static Board GetInstance()
        {
            if (_instance == null)
                _instance = new Board();
            return _instance;
        }

        private Board()
        {
            _capturedPieces = new Dictionary<Color, IList<Piece>>
            {
                { Color.Black, new List<Piece>() },
                { Color.White, new List<Piece>() }
            };
            _playingPieces = new Dictionary<Color, IList<Piece>>
            {
                { Color.Black, new List<Piece>() },
                { Color.White, new List<Piece>() }
            };
            _pinningPieces = new Dictionary<Color, ISet<Piece>>
            {
                { Color.Black, new HashSet<Piece>() },
                { Color.White, new HashSet<Piece>() }
            };

            _movesRegister = new MovesSet();
            _kingsDict = new Dictionary<Color, King>();

            NextMoveTurn = Color.White;
            _moveFactory = new ValidMoveFactory();
            _board = CreateBoard();

            _piecesInflueceManager = new PiecesInfluenceManager(this);
        }

        #endregion

        #region Init

        private IDictionary<Position, BoardCell> CreateBoard()
        {
            var ret = new Dictionary<Position, BoardCell>();

            for (int file = 1; file < 9; file++)
                for (int rank = 1; rank < 9; rank++)
                    ret.Add(CreateCell(rank, file));

            return ret;
        }

        private KeyValuePair<Position, BoardCell> CreateCell(int rank, int file)
        {
            var position = new Position(rank, file);
            return new KeyValuePair<Position, BoardCell>(position, CreateCell(position));
        }

        private BoardCell CreateCell(Position boardPosition)
        {
            return new BoardCell(boardPosition);
        }

        #endregion

        #region Setup

        internal void InitPieces(IEnumerable<Piece> pieces)
        {
            if (!IsValidPiecesSet(pieces))
                throw new InvalidOperationException();

            _kingsDict[Color.White] = pieces.Single(p => p is King && p.Color.IsWhite) as King;
            _kingsDict[Color.Black] = pieces.Single(p => p is King && p.Color.IsBlack) as King;

            foreach (var piece in pieces)
                AddPiece(piece);

            _piecesInflueceManager.AddPieces(pieces);
        }

        private bool IsValidPiecesSet(IEnumerable<Piece> pieces)
        {
            return pieces.Count(p => p is King && p.Color.IsWhite) == 1
                && pieces.Count(p => p is King && p.Color.IsBlack) == 1;
        }

        #endregion

        #region Utils

        internal IDisposable PlayTemporaryMove(BoardCell startingCell, BoardCell endingCell)
        {
            var ret = new TemporaryMoveDisposable(startingCell, endingCell);
            _piecesInflueceManager.PlayTemporaryMove(ret);
            return ret;
        }

        internal MoveBase GetLastMove()
        {
            return _movesRegister.GetLastMove();
        }

        internal Piece GetLastMovedPiece()
        {
            return GetLastMove()?.GetAffectedPieces().MovedPiece;
        }

        internal bool CanPieceOfColorGoToPosition(Position position, Color color)
        {
            return _board[position].Piece == null
                || _board[position].Piece.Color.IsOpponentColor(color);
        }

        internal bool IsAnyPieceInPosition(Position position)
        {
            return _board[position].ContainsPiece();
        }

        internal bool IsAnyPieceInPosition(int rank, int file)
        {
            return _board[new Position(rank, file)].ContainsPiece();
        }

        internal bool IsAnyPieceOfDifferentColorInPosition(Position position, Color color)
        {
            return _board[position].ContainsPieceOfDifferentColor(color);
        }

        public IEnumerable<BoardCell> GetBoardCells()
        {
            return _board.Select(x => x.Value);
        }

        private void AddPiece(Piece piece)
        {
            if (piece == null || piece.Position == null)
                return;

            _playingPieces[piece.Color].Add(piece);
            _board[piece.Position.Value].Piece = piece;
            if (piece.CanPin)
                _pinningPieces[piece.Color].Add(piece);
        }

        internal BoardCell GetCell(Position position)
        {
            return _board[position];
        }

        internal BoardCell GetCell(int rank, int file)
        {
            return GetCell(new Position(rank, file));
        }

        #endregion

        #region Move

        public void TryPlayMove(BoardCell startingCell, BoardCell endingCell)
        {
            if (_moveFactory.TryCreateValidMove(startingCell, endingCell, this, out MoveBase move))
                MakeMove(move);
        }

        private void MakeMove(MoveBase move)
        {
            var operations = move.GetMoveOperations();
            PerformMoveOperations(operations);

            _movesRegister.OnMovePlayed(move);
            move.OnMovePlayed();
            _piecesInflueceManager.OnMovePlayed(operations);

            FlipNextMoveTurn();
        }

        private void PerformMoveOperations(MoveOperations operations)
        {
            foreach (var piece in operations.CapturedPieces)
                RemovePiece(piece);
            foreach (var operation in operations.MovedPieces)
                MovePiece(operation);
        }

        private void MovePiece(PieceMove move)
        {
            _board[move.EndingPosition].Piece = move.MovedPiece;
            _board[move.StartingPosition].Piece = null;
        }

        private void RemovePiece(Piece piece)
        {
            _board[piece.Position.Value].Piece = null;
            _capturedPieces[piece.Color].Add(piece);
            _playingPieces[piece.Color].Remove(piece);
            if (piece.CanPin)
                _pinningPieces[piece.Color].Remove(piece);
        }

        private void FlipNextMoveTurn()
        {
            NextMoveTurn = this.NextMoveTurn == Color.Black ?
                Color.White :
                Color.Black;
        }

        #endregion

        internal bool IsAnyPieceInBetween(Position position1, Position position2)
        {
            var inBetweenPositions = position1.GetAllInBetweenPositions(position2);
            return inBetweenPositions.Any(p => IsAnyPieceInPosition(p));
        }

        internal bool AnyPlayingPieceOfSameTypeAndColor(Piece piece)
        {
            return _playingPieces[piece.Color].Any(p => p.GetType() == piece.GetType());
        }

        internal IEnumerable<Piece> GetAllPlayingPiecesOfSomeColor(Color color)
        {
            return _board.Select(x => x.Value.Piece)
                         .Where(piece => piece.Color == color);
        }

        internal IEnumerable<Piece> GetAllPlayingPieces()
        {
            return _board.Select(x => x.Value.Piece);
        }

        internal IEnumerable<BoardCell> GetAllBoardSquares()
        {
            return _board.Select(x => x.Value);
        }

        internal bool IsControlledByOppositeColor(Position position, Color color)
        {
            return _piecesInflueceManager.IsControlledByOppositeColor(GetCell(position), color);
        }

        internal bool IsInOpponentControlAfterMove(BoardCell startingPosition, BoardCell endingPosition, BoardCell position)
        {
            var opponentControlAfterMove = _piecesInflueceManager.GetOpponentControlAfterMove(startingPosition, endingPosition, startingPosition.Piece.Color);
            return opponentControlAfterMove.Contains(position.Position);
        }

        internal bool WouldBeInCheckAfterMove(BoardCell startingSquare, BoardCell endingSquare)
        {
            var color = startingSquare.Piece.Color;

            using (PlayTemporaryMove(startingSquare, endingSquare))
            {
                var kingPosition = GetKingCell(color);
                var controlledSquares = _piecesInflueceManager.GetOpponentControlOnPosition(kingPosition, color);
                return controlledSquares.Contains(kingPosition.Position);
            }
        }

        internal ISet<Position> FilterAvailableMovesForChecks(BoardCell startingSquare, BoardCell endingSquare)
        {
            return null;
        }

        internal bool IsInOpponentControl(BoardCell square, Color color)
        {
            return _piecesInflueceManager.IsColorControllingSquare(square, color.OpponentColor);
        }

        internal BoardCell GetKingCell(Color color)
        {
            return GetCell(GetKing(color).Position.Value);
        }

        internal King GetKing(Color color)
        {
            return _kingsDict[color];
        }
    }

    internal class TemporaryMoveDisposable : IDisposable
    {
        internal readonly BoardCell _startingCell;
        internal readonly BoardCell _endingCell;
        internal readonly Piece _movedPiece;
        private readonly Piece _capturedPiece;
        private readonly Position? _movedPiecePosition;

        internal event EventHandler Disposing;

        public TemporaryMoveDisposable(BoardCell startingCell, BoardCell endingCell)
        {
            Guard.ArgumentNotNull(startingCell, nameof(startingCell));
            _startingCell = startingCell;
            _movedPiece = startingCell.Piece;
            _movedPiecePosition = _movedPiece.Position;
            Guard.ArgumentNotNull(endingCell, nameof(endingCell));
            _endingCell = endingCell;
            _capturedPiece = endingCell.Piece;
            _movedPiecePosition = _capturedPiece?.Position;

            PlayMove();
        }

        private void PlayMove()
        {
            _startingCell.Piece = null;
            _endingCell.Piece = _movedPiece;

            _movedPiece.Position = _endingCell.Position;
            if (_capturedPiece != null)
                _capturedPiece.Position = null;
        }

        private void RevertMove()
        {
            _startingCell.Piece = _movedPiece;
            _endingCell.Piece = _capturedPiece;

            _movedPiece.Position = _startingCell.Position;
            if (_capturedPiece != null)
                _capturedPiece.Position = _endingCell.Position;
        }

        public void Dispose()
        {
            Disposing?.Invoke(this, new EventArgs());
            RevertMove();
        }
    }

}
