using System;

namespace ChessCore
{
    internal class EnPassant : Move
    {

        #region Constructors

        internal EnPassant(Square startingMovedPawnSquare,
                           Square endingMovedPawnSquare,
                           Square capturedPawnSquare,
                           Square ambiguousMoveStartingSquare)
            : base(true, 
                   startingMovedPawnSquare.Piece, 
                   capturedPawnSquare.Piece,
                   startingMovedPawnSquare, 
                   endingMovedPawnSquare,
                   ambiguousMoveStartingSquare)
        { }

        #endregion

        #region Notation

        public override string ToString()
        {
            return string.Format("{0}x{1}{2}e.p.{3}", MoveUtilities.GetFileFromInt(StartingSquare.Coordinate.File),
                                                      MoveUtilities.GetFileFromInt(EndingSquare.Coordinate.File),
                                                      EndingSquare.Coordinate.Rank,
                                                      GetCheckNotation());
        }

        #endregion
    }
}
