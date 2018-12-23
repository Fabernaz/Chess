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

            return ret;
        }
    }
}


