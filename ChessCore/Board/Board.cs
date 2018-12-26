using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace ChessCore
{
    //TODO: event dispose to avoid memory leaks: is necessary?

    public class Board
    {
        #region Fields

        private readonly IDictionary<Color, King> _kingsDict;
        private readonly IDictionary<SquareCoordinate, Square> _board;
        private readonly IDictionary<Color, IList<Piece>> _capturedPieces;
        private readonly IDictionary<Color, IList<Piece>> _playingPieces;

        private readonly ValidMoveFactory _moveFactory;
        private readonly TimerManager _timerManager;
        private readonly MovesSet _movesRegister;
        private readonly PiecesInfluenceManager _piecesInfluenceManager;
        private readonly InfoAsker<PromoteTo> _infoAsker;

        #endregion

        #region Properties

        public Color NextMoveTurn { get; private set; }

        #endregion

        #region Events

        public event EventHandler MovePlayed;
        public event EventHandler BlackTimerChanged;
        public event EventHandler WhiteTimerChanged;
        public event EventHandler AskForPieceEvent;

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

            _movesRegister = new MovesSet();
            _kingsDict = new Dictionary<Color, King>();

            NextMoveTurn = Color.White;
            _infoAsker = GetPieceAsker();
            _moveFactory = new ValidMoveFactory(_infoAsker);
            _board = CreateBoard();
            _timerManager = InitTimeManager();
            _piecesInfluenceManager = new PiecesInfluenceManager(this);
        }

        #endregion

        #region Init

        private InfoAsker<PromoteTo> GetPieceAsker()
        {
            var ret = new InfoAsker<PromoteTo>();
            ret.AskingInfoEvent += (sender, e) => { AskForPieceEvent?.Invoke(this, new EventArgs()); };
            return ret;
        }

        public void ProvideRequestedPiece(PromoteTo promoteTo)
        {
            _infoAsker.ProvideRequestedInfo(promoteTo);
        }

        private TimerManager InitTimeManager()
        {
            var ret = new TimerManager(new TimerConfiguration
            {
                Hours = 0,
                Minutes = 1,
                AfterMoveNumSecondsIncrement = new Dictionary<int, int> { { 5, 20 } },
                AfterMoveSecondsIncrement = 3
            });

            ret.WhiteTimerChanged += (sender, e) => { WhiteTimerChanged?.Invoke(this, new EventArgs()); };
            ret.BlackTimerChanged += (sender, e) => { BlackTimerChanged?.Invoke(this, new EventArgs()); };

            return ret;
        }

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

        internal void InitPieces(IEnumerable<PieceCoordinatePair> pieceCoordinatePairs)
        {
            var allPieces = pieceCoordinatePairs.Select(kvp => kvp.Piece);
            if (!IsValidPiecesSet(allPieces))
                throw new InvalidOperationException();

            _kingsDict[Color.White] = allPieces.Single(p => p is King && p.Color.IsWhite) as King;
            _kingsDict[Color.Black] = allPieces.Single(p => p is King && p.Color.IsBlack) as King;

            foreach (var pair in pieceCoordinatePairs)
                AddPiece(pair);

            _piecesInfluenceManager.AddPieces(allPieces);
        }

        private bool IsValidPiecesSet(IEnumerable<Piece> pieces)
        {
            return pieces.Count(p => p is King && p.Color.IsWhite) == 1
                && pieces.Count(p => p is King && p.Color.IsBlack) == 1;
        }

        #endregion

        public string GetBlackTime()
        {
            return _timerManager.GetBlackTime();
        }

        public string GetWhiteTime()
        {
            return _timerManager.GetWhiteTime();
        }

        #region Utils

        internal IDisposable PlayTemporaryMove(Square startingSquare, Square endingSquare)
        {
            var ret = new TemporaryMoveDisposable(startingSquare, endingSquare);
            _piecesInfluenceManager.PlayTemporaryMove(ret);
            return ret;
        }

        internal Move GetLastMove()
        {
            return _movesRegister.GetLastMove();
        }

        internal Piece GetLastMovedPiece()
        {
            return GetLastMove()?.MovedPiece;
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

        private void AddPiece(PieceCoordinatePair pair)
        {
            if (pair.Piece == null)
                return;

            pair.Piece.CurrentSquare = GetSquare(pair.Coordinate);

            _playingPieces[pair.Piece.Color].Add(pair.Piece);
            _board[pair.Piece.CurrentSquare.Coordinate].Piece = pair.Piece;
        }

        internal Square GetSquare(SquareCoordinate coordinate)
        {
            return _board[coordinate];
        }

        #endregion

        #region Move

        public void TryPlayMove(Square startingSquare, Square endingSquare)
        {
            if (_moveFactory.TryCreateValidMove(startingSquare, endingSquare, this, out Move move))
                MakeMove(move);
        }

        private void MakeMove(Move move)
        {
            var operations = move.GetMoveOperations();
            PerformMoveOperations(operations);

            move.PlayMove();
            _piecesInfluenceManager.OnMovePlayed(operations);
            _movesRegister.OnMovePlayed(move);
            _timerManager.OnMovePlayed();

            OpponentInCheck(move);
            FlipNextMoveTurn();

            MovePlayed?.Invoke(this, new EventArgs());
        }

        private void OpponentInCheck(Move move)
        {
            var operations = move.GetMoveOperations();
            var opponentColor = operations.MovingColor.OpponentColor;
            move.IsCheck = _piecesInfluenceManager.IsControlledByOppositeColor(GetKingSquare(opponentColor), opponentColor);
        }

        private void PerformMoveOperations(MoveOperations operations)
        {
            foreach (var pair in operations.AddedPieces)
                AddPiece(pair);
            _piecesInfluenceManager.AddPieces(operations.AddedPieces.Select(p => p.Piece));

            foreach (var piece in operations.CapturedPieces)
                RemovePiece(piece);

            foreach (var move in operations.MovedPieces)
                MovePiece(move);
        }

        private void MovePiece(PieceMoveInfo move)
        {
            _board[move.EndingSquare.Coordinate].Piece = move.MovedPiece;
            _board[move.StartingSquare.Coordinate].Piece = null;
        }

        private void RemovePiece(Piece piece)
        {
            _board[piece.CurrentSquare.Coordinate].Piece = null;
            _capturedPieces[piece.Color].Add(piece);
            _playingPieces[piece.Color].Remove(piece);
        }

        private void FlipNextMoveTurn()
        {
            NextMoveTurn = this.NextMoveTurn == Color.Black ?
                Color.White :
                Color.Black;
        }

        #endregion

        internal bool IsAnyPieceInBetween(Square square1, Square square2)
        {
            var inBetweenSquare = square1.Coordinate.GetAllInBetweenSquares(square2.Coordinate);
            return inBetweenSquare.Any(p => IsAnyPieceInSquare(p));
        }

        internal IEnumerable<Piece> GetAllPlayingPiecesOfSameTypeAndColor(Type pieceType, Color color)
        {
            return _board.Select(x => x.Value.Piece)
                         .Where(p => p != null)
                         .Where(p => p.GetType() == pieceType)
                         .Where(piece => piece.Color == color);
        }

        internal IEnumerable<Square> GetAllBoardSquares()
        {
            return _board.Select(x => x.Value);
        }

        internal bool IsControlledByOppositeColor(SquareCoordinate coordinate, Color color)
        {
            return _piecesInfluenceManager.IsControlledByOppositeColor(GetSquare(coordinate), color);
        }

        internal bool IsInOpponentControlAfterMove(Square startingCoordinate, Square endingCoordinate, Square square)
        {
            var opponentControlAfterMove = _piecesInfluenceManager.GetOpponentControlAfterMove(startingCoordinate, endingCoordinate, startingCoordinate.Piece.Color);
            return opponentControlAfterMove.Contains(square.Coordinate);
        }

        internal bool WouldBeInCheckAfterMove(Square startingSquare, Square endingSquare)
        {
            var color = startingSquare.Piece.Color;

            using (PlayTemporaryMove(startingSquare, endingSquare))
            {
                var kigSquare = GetKingSquare(color);
                var controlledSquares = _piecesInfluenceManager.GetOpponentControlOnSquare(kigSquare, color);
                return controlledSquares.Contains(kigSquare.Coordinate);
            }
        }

        internal bool IsInOpponentControl(Square square, Color color)
        {
            return _piecesInfluenceManager.IsColorControllingSquare(square, color.OpponentColor);
        }

        internal Square GetKingSquare(Color color)
        {
            return GetSquare(GetKing(color).CurrentSquare.Coordinate);
        }

        internal King GetKing(Color color)
        {
            return _kingsDict[color];
        }

        public IEnumerable<MovePair> GetAllMovesPlayed()
        {
            return _movesRegister.GetAllMoves();
        }
    }
}
