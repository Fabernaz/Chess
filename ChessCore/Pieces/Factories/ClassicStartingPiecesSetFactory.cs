using System.Collections.Generic;

namespace ChessCore
{
    public class ClassicStartingPiecesSetFactory : IStartingPiecesSetFactory
    {
        public IEnumerable<Piece> GetPiecesInStartingPosition()
        {
            var ret = new List<Piece>();

            //White queen
            ret.Add(new Queen(Color.White, new Position(1, 4)));
            //Black queen
            ret.Add(new Queen(Color.Black, new Position(8, 4)));

            //White king side rook
            ret.Add(new Rook(Color.White, new Position(1, 1)));
            //White queen side rook
            ret.Add(new Rook(Color.White, new Position(1, 8)));
            //White queen side rook
            ret.Add(new Rook(Color.Black, new Position(8, 1)));
            //Black queen side rook
            ret.Add(new Rook(Color.Black, new Position(8, 8)));

            //White king side knight
            ret.Add(new Knight(Color.White, new Position(1, 2)));
            //White queen side knight
            ret.Add(new Knight(Color.White, new Position(1, 7)));
            //White queen side knight
            ret.Add(new Knight(Color.Black, new Position(8, 2)));
            //Black queen side knight
            ret.Add(new Knight(Color.Black, new Position(8, 7)));

            //White king side bishop
            ret.Add(new Bishop(Color.White, new Position(1, 3)));
            //White queen side bishop
            ret.Add(new Bishop(Color.White, new Position(1, 6)));
            //White queen side bishop
            ret.Add(new Bishop(Color.Black, new Position(8, 3)));
            //Black queen side bishop
            ret.Add(new Bishop(Color.Black, new Position(8, 6)));

            //White pawns
            for (int i = 1; i < 9; i++)
                ret.Add(new Pawn(Color.White, new Position(2, i)));
            //Black pawns
            for (int i = 1; i < 9; i++)
                ret.Add(new Pawn(Color.Black, new Position(7, i)));

            //White king
            ret.Add(new King(Color.White, new Position(1, 5)));
            //Black king
            ret.Add(new King(Color.Black, new Position(8, 5)));

            return ret;
        }
    }
}
