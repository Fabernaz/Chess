using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCore
{
    public class TestStartingPiecesFactory : IStartingPiecesSetFactory
    {
        public IEnumerable<PieceCoordinatePair> GetPiecesInStartingPosition()
        {
            var ret = new List<PieceCoordinatePair>();

            
            ret.Add(new PieceCoordinatePair(new Pawn(Color.White),
                                                new SquareCoordinate(7, 2)));

            //White king
            ret.Add(new PieceCoordinatePair(new King(Color.White),
                                            new SquareCoordinate(1, 5)));
            //Black king
            ret.Add(new PieceCoordinatePair(new King(Color.Black),
                                            new SquareCoordinate(8, 7)));
            ret.Add(new PieceCoordinatePair(new Pawn(Color.Black),
                                                new SquareCoordinate(7, 8)));

            return ret;
        }
    }
}


