using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCore
{
    public interface IBoardFactory
    {
        Board GetBoard(IStartingPiecesSetFactory pieceFactory);
    }

    public class BoardFactory : IBoardFactory
    {
        public Board GetBoard(IStartingPiecesSetFactory pieceFactory)
        {
            var board = Board.GetInstance();
            board.InitPieces(pieceFactory.GetPiecesInStartingPosition());
            return board;
        }
    }
}
