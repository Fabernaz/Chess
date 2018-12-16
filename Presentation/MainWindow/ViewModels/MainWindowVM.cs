using ChessCore;

namespace Presentation
{
    public class MainWindowVM
    {
        public BoardVM Board { get; }

        public MainWindowVM(IBoardFactory boardFactory,
                            IStartingPiecesSetFactory pieceFactory)
        {
            Board = new BoardVM(boardFactory.GetBoard(pieceFactory));
        }

    }
}
