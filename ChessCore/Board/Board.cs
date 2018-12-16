using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCore
{
    //TODO: event dispose to avoid memory leaks: is necessary?

    public class Board
    {
        #region Fields

        private readonly ValidMoveFactory _moveFactory;
        private IDictionary<SquareCoordinate, Square> _board;
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

        private IDictionary<SquareCoordinate, Square> CreateBoard()
        {
            var ret = new Dictionary<SquareCoordinate, Square>();

            for (int file = 1; file < 9; file++)
                for (int rank = 1; rank < 9; rank++)
                    ret.Add(CreateSquare(rank, file));

            return ret;
        }

        private KeyValuePair<SquareCoordinate, Square> CreateSquare(int rank, int file)
        {
            var coordinate = new SquareCoordinate(rank, file);
            return new KeyValuePair<SquareCoordinate, Square>(coordinate, CreateSquare(coordinate));
        }

        private Square CreateSquare(SquareCoordinate coordinate)
        {
            return new Square(coordinate);
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

        internal IDisposable PlayTemporaryMove(Square startingSquare, Square endingSquare)
        {
            var ret = new TemporaryMoveDisposable(startingSquare, endingSquare);
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

        internal bool CanPieceOfColorGoToSquare(SquareCoordinate coordinate, Color color)
        {
            return _board[coordinate].Piece == null
                || _board[coordinate].Piece.Color.IsOpponentColor(color);
        }

        internal bool IsAnyPieceInSquare(SquareCoordinate coordinate)
        {
            return _board[coordinate].ContainsPiece();
        }

        internal bool IsAnyPieceInSquare(int rank, int file)
        {
            return _board[new SquareCoordinate(rank, file)].ContainsPiece();
        }

        internal bool IsAnyOpponentPieceInSquare(SquareCoordinate coordinate, Color color)
        {
            return _board[coordinate].ContainsOpponentPiece(color);
        }

        public IEnumerable<Square> GetSquares()
        {
            return _board.Select(x => x.Value);
        }

        private void AddPiece(Piece piece)
        {
            if (piece == null || piece.CurrentCoordinate == null)
                return;

            _playingPieces[piece.Color].Add(piece);
            _board[piece.CurrentCoordinate.Value].Piece = piece;
            if (piece.CanPin)
                _pinningPieces[piece.Color].Add(piece);
        }

        internal Square GetSquare(SquareCoordinate coordinate)
        {
            return _board[coordinate];
        }

        internal Square GetSquare(int rank, int file)
        {
            return GetSquare(new SquareCoordinate(rank, file));
        }

        #endregion

        #region Move

        public void TryPlayMove(Square startingSquare, Square endingSquare)
        {
            if (_moveFactory.TryCreateValidMove(startingSquare, endingSquare, this, out MoveBase move))
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
            _board[move.EndingSquare].Piece = move.MovedPiece;
            _board[move.StartingSquare].Piece = null;
        }

        private void RemovePiece(Piece piece)
        {
            _board[piece.CurrentCoordinate.Value].Piece = null;
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

        internal bool IsAnyPieceInBetween(SquareCoordinate coordinate1, SquareCoordinate coordinate2)
        {
            var inBetweenSquare = coordinate1.GetAllInBetweenSquares(coordinate2);
            return inBetweenSquare.Any(p => IsAnyPieceInSquare(p));
        }

        internal bool ExistsPlayingPieceOfSameTypeAndColor(Piece piece)
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

        internal IEnumerable<Square> GetAllBoardSquares()
        {
            return _board.Select(x => x.Value);
        }

        internal bool IsControlledByOppositeColor(SquareCoordinate coordinate, Color color)
        {
            return _piecesInflueceManager.IsControlledByOppositeColor(GetSquare(coordinate), color);
        }

        internal bool IsInOpponentControlAfterMove(Square startingCoordinate, Square endingCoordinate, Square square)
        {
            var opponentControlAfterMove = _piecesInflueceManager.GetOpponentControlAfterMove(startingCoordinate, endingCoordinate, startingCoordinate.Piece.Color);
            return opponentControlAfterMove.Contains(square.Coordinate);
        }

        internal bool WouldBeInCheckAfterMove(Square startingSquare, Square endingSquare)
        {
            var color = startingSquare.Piece.Color;

            using (PlayTemporaryMove(startingSquare, endingSquare))
            {
                var kigSquare = GetKingSquare(color);
                var controlledSquares = _piecesInflueceManager.GetOpponentControlOnSquare(kigSquare, color);
                return controlledSquares.Contains(kigSquare.Coordinate);
            }
        }

        internal ISet<SquareCoordinate> FilterAvailableMovesForChecks(Square startingSquare, Square endingSquare)
        {
            return null;
        }

        internal bool IsInOpponentControl(Square square, Color color)
        {
            return _piecesInflueceManager.IsColorControllingSquare(square, color.OpponentColor);
        }

        internal Square GetKingSquare(Color color)
        {
            return GetSquare(GetKing(color).CurrentCoordinate.Value);
        }

        internal King GetKing(Color color)
        {
            return _kingsDict[color];
        }
    }

    internal class TemporaryMoveDisposable : IDisposable
    {
        internal readonly Square _startingSquare;
        internal readonly Square _endingSquare;
        internal readonly Piece _movedPiece;
        private readonly Piece _capturedPiece;
        private readonly SquareCoordinate? _movedPieceSquare;

        internal event EventHandler Disposing;

        public TemporaryMoveDisposable(Square startingSquare, Square endingSquare)
        {
            Guard.ArgumentNotNull(startingSquare, nameof(startingSquare));
            _startingSquare = startingSquare;
            _movedPiece = startingSquare.Piece;
            _movedPieceSquare = _movedPiece.CurrentCoordinate;
            Guard.ArgumentNotNull(endingSquare, nameof(endingSquare));
            _endingSquare = endingSquare;
            _capturedPiece = endingSquare.Piece;
            _movedPieceSquare = _capturedPiece?.CurrentCoordinate;

            PlayMove();
        }

        private void PlayMove()
        {
            _startingSquare.Piece = null;
            _endingSquare.Piece = _movedPiece;

            _movedPiece.CurrentCoordinate = _endingSquare.Coordinate;
            if (_capturedPiece != null)
                _capturedPiece.CurrentCoordinate = null;
        }

        private void RevertMove()
        {
            _startingSquare.Piece = _movedPiece;
            _endingSquare.Piece = _capturedPiece;

            _movedPiece.CurrentCoordinate = _startingSquare.Coordinate;
            if (_capturedPiece != null)
                _capturedPiece.CurrentCoordinate = _endingSquare.Coordinate;
        }

        public void Dispose()
        {
            Disposing?.Invoke(this, new EventArgs());
            RevertMove();
        }
    }

}
