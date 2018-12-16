using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCore
{
    public class TestStartingPiecesFactory : IStartingPiecesSetFactory
    {
        public IEnumerable<Piece> GetPiecesInStartingPosition()
        {
            var ret = new List<Piece>();

            ret.Add(new Pawn(Color.White, new SquareCoordinate(2, 2)));
            ret.Add(new Pawn(Color.Black, new SquareCoordinate(7, 2)));
            ret.Add(new Knight(Color.Black, new SquareCoordinate(7, 4)));
            ret.Add(new King(Color.White, new SquareCoordinate(5, 5)));
            ret.Add(new King(Color.Black, new SquareCoordinate(7, 7)));

            return ret;
        }
    }
}

