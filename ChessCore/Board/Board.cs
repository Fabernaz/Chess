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
        private readonly IDictionary<SquareCoordinate, Square> _position;
        private readonly IDictionary<Color, IList<Piece>> _playingPieces;
        private readonly ValidMoveFactory _moveFactory;
        private readonly TimerManager _timerManager;
        private readonly MovesSet _movesSet;
        private readonly PiecesInfluenceManager _piecesInfluenceManager;
        private readonly InfoAsker<PromoteTo> _infoAsker;
        private EndGameRules _endGameRules;
        private bool _gameEnded;

        #endregion

        #region Indexers

        public Square this[SquareCoordinate coordinate]
        {
            get { return _position[coordinate]; }
        }

        public Square this[int rank, int file]
        {
            get { return _position[new SquareCoordinate(rank, file)]; }
        }

        #endregion

        #region Properties

        public Color NextMoveTurn { get; private set; }

        #endregion

        #region Events

        public event EventHandler MovePlayed;
        public event EventHandler BlackTimerChanged;
        public event EventHandler WhiteTimerChanged;
        public event EventHandler AskForPieceEvent;
        public event EventHandler<GameEndedReasonEventArgs> GameEndedEvent;

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
            _playingPieces = new Dictionary<Color, IList<Piece>>
            {
                { Color.Black, new List<Piece>() },
                { Color.White, new List<Piece>() }
            };

            _movesSet = new MovesSet();
            _kingsDict = new Dictionary<Color, King>();

            NextMoveTurn = Color.White;
            _infoAsker = GetPieceAsker();
            _moveFactory = new ValidMoveFactory(_infoAsker);
            _position = CreatePosition();
            _timerManager = InitTimeManager();
            _piecesInfluenceManager = new PiecesInfluenceManager(this);
        }

        #endregion

        #region Init

        private EndGameRules InitEndGameRules()
        {
            var ret = new EndGameRules(this, _timerManager, _movesSet, _piecesInfluenceManager);
            ret.GameEndedEvent += (sender, e) =>
            {
                _gameEnded = true;
                GameEndedEvent?.Invoke(this, e);
            };
            return ret;
        }

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

        private IDictionary<SquareCoordinate, Square> CreatePosition()
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
            _endGameRules = InitEndGameRules();
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
            return _movesSet.GetLastMove();
        }

        internal Piece GetLastMovedPiece()
        {
            return GetLastMove()?.MovedPiece;
        }

        internal bool CanPieceOfColorGoToSquare(SquareCoordinate coordinate, Color color)
        {
            return _position[coordinate].Piece == null
                || _position[coordinate].Piece.Color.IsOpponentColor(color);
        }

        internal bool IsAnyPieceInSquare(SquareCoordinate coordinate)
        {
            return _position[coordinate].ContainsPiece();
        }

        internal bool IsAnyPieceInSquare(int rank, int file)
        {
            return _position[new SquareCoordinate(rank, file)].ContainsPiece();
        }

        internal bool IsAnyOpponentPieceInSquare(SquareCoordinate coordinate, Color color)
        {
            return _position[coordinate].ContainsOpponentPiece(color);
        }

        public IEnumerable<Square> GetSquares()
        {
            return _position.Select(x => x.Value);
        }

        private void AddPiece(PieceCoordinatePair pair)
        {
            if (pair.Piece == null)
                return;

            pair.Piece.CurrentSquare = this[pair.Coordinate];

            _playingPieces[pair.Piece.Color].Add(pair.Piece);
            _position[pair.Piece.CurrentSquare.Coordinate].Piece = pair.Piece;
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
            _movesSet.OnMovePlayed(move);
            _timerManager.OnMovePlayed();

            _endGameRules.OnMovePlayed(move);
            if (!_gameEnded)
                FlipNextMoveTurn();

            UpdateMoveWithCheckInformation(move);

            MovePlayed?.Invoke(this, new EventArgs());
        }

        private void UpdateMoveWithCheckInformation(Move move)
        {
            var operations = move.GetMoveOperations();
            if (IsInCheck(operations.MovingColor.OpponentColor))
            {
                if (_gameEnded)
                    move.IsCheckMate = true;
                else
                    move.IsCheck = true;
            };
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
            _position[move.EndingSquare.Coordinate].Piece = move.MovedPiece;
            _position[move.StartingSquare.Coordinate].Piece = null;
        }

        private void RemovePiece(Piece piece)
        {
            _position[piece.CurrentSquare.Coordinate].Piece = null;
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
            return _position.Select(x => x.Value.Piece)
                         .Where(p => p != null)
                         .Where(p => p.GetType() == pieceType)
                         .Where(piece => piece.Color == color);
        }

        internal IEnumerable<Piece> GetAllPiecesOfColor(Color color)
        {
            return _position.Select(x => x.Value.Piece)
                         .Where(p => p != null)
                         .Where(piece => piece.Color == color);
        }

        internal IEnumerable<Piece> GetAllPieces()
        {
            return _position.Select(x => x.Value.Piece)
                            .Where(p => p != null);
        }

        internal IEnumerable<Square> GetAllBoardSquares()
        {
            return _position.Select(x => x.Value);
        }

        internal bool IsControlledByColor(SquareCoordinate coordinate, Color color)
        {
            return _piecesInfluenceManager.IsControlledByColor(this[coordinate], color);
        }

        internal bool IsInCheck(Color color)
        {
            return IsControlledByColor(GetKingSquare(color).Coordinate, color.OpponentColor);
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
            return this[GetKing(color).CurrentSquare.Coordinate];
        }

        internal King GetKing(Color color)
        {
            return _kingsDict[color];
        }

        public IEnumerable<MovePair> GetAllMovesPlayed()
        {
            return _movesSet.GetAllMoves();
        }
    }
}
