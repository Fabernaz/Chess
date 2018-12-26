using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCore
{
    internal class PawnPromotion : Move
    { 

        public PawnPromotion(Square startingSquare, Square endingSquare, Piece promotedTo, Square ambiguousMoveStartingSquare)
            : base(endingSquare.Piece != null,
                   promotedTo,
                   startingSquare.Piece,
                   startingSquare,
                   endingSquare,
                   ambiguousMoveStartingSquare)
        { }

        internal override MoveOperations GetMoveOperations()
        {
            var ret = new MoveOperations(MovedPiece.Color);

            ret.CapturedPieces.Add(CapturedPiece);
            ret.AddedPieces.Add(new PieceCoordinatePair(MovedPiece, EndingSquare.Coordinate));

            return ret;
        }

        public override string ToString()
        {
            var piece = CapturedPiece.GetNotation();
            var endingFile = MoveUtilities.GetFileFromInt(EndingSquare.Coordinate.File);
            var rank = EndingSquare.Coordinate.Rank;
            var isCapture = GetCaptureNotation(IsCapture);
            var check = GetCheckNotation();
            var disambiguating = GetDisambiguating();
            var promotion = GetPromotionNotation();

            return string.Format("{0}{1}{2}{3}{4}{5}{6}", piece,
                                                          disambiguating,
                                                          isCapture,
                                                          endingFile,
                                                          rank,
                                                          promotion,
                                                          check);
        }

        private string GetPromotionNotation()
        {
            return "=" + MovedPiece.GetNotation();
        }
    }
}
