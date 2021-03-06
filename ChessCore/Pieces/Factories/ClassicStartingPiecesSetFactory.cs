﻿using System.Collections.Generic;

namespace ChessCore
{
    public class ClassicStartingPiecesSetFactory : IStartingPiecesSetFactory
    {
        public IEnumerable<PieceCoordinatePair> GetPiecesInStartingPosition()
        {
            var ret = new List<PieceCoordinatePair>();

            //White queen
            ret.Add(new PieceCoordinatePair(new Queen(Color.White), 
                                            new SquareCoordinate(1, 4)));
            //Black queen
            ret.Add(new PieceCoordinatePair(new Queen(Color.Black),
                                            new SquareCoordinate(8, 4)));

            //White king side rook
            ret.Add(new PieceCoordinatePair(new Rook(Color.White),
                                            new SquareCoordinate(1, 1)));
            //White queen side rook
            ret.Add(new PieceCoordinatePair(new Rook(Color.White), 
                                            new SquareCoordinate(1, 8)));
            //Black queen side rook
            ret.Add(new PieceCoordinatePair(new Rook(Color.Black),
                                            new SquareCoordinate(8, 8)));
            //Black king side rook
            ret.Add(new PieceCoordinatePair(new Rook(Color.Black),
                                            new SquareCoordinate(8, 1)));



            //White king side knight
            ret.Add(new PieceCoordinatePair(new Knight(Color.White), 
                                            new SquareCoordinate(1, 2)));
            //White queen side knight
            ret.Add(new PieceCoordinatePair(new Knight(Color.White), 
                                            new SquareCoordinate(1, 7)));
            //Black king side knight
            ret.Add(new PieceCoordinatePair(new Knight(Color.Black), 
                                            new SquareCoordinate(8, 2)));
            //Black queen side knight
            ret.Add(new PieceCoordinatePair(new Knight(Color.Black), 
                                            new SquareCoordinate(8, 7)));

            //White king side bishop
            ret.Add(new PieceCoordinatePair(new Bishop(Color.White), 
                                            new SquareCoordinate(1, 3)));
            //White queen side bishop
            ret.Add(new PieceCoordinatePair(new Bishop(Color.White), 
                                            new SquareCoordinate(1, 6)));
            //Black king side bishop
            ret.Add(new PieceCoordinatePair(new Bishop(Color.Black), 
                                            new SquareCoordinate(8, 3)));
            //Black queen side bishop
            ret.Add(new PieceCoordinatePair(new Bishop(Color.Black), 
                                            new SquareCoordinate(8, 6)));

            //White pawns
            for (int i = 1; i < 9; i++)
                ret.Add(new PieceCoordinatePair(new Pawn(Color.White), 
                                                new SquareCoordinate(2, i)));
            //Black pawns
            for (int i = 1; i < 9; i++)
                ret.Add(new PieceCoordinatePair(new Pawn(Color.Black), 
                                                new SquareCoordinate(7, i)));

            //White king
            ret.Add(new PieceCoordinatePair(new King(Color.White), 
                                            new SquareCoordinate(1, 5)));
            //Black king
            ret.Add(new PieceCoordinatePair(new King(Color.Black), 
                                            new SquareCoordinate(8, 5)));

            return ret;
        }
    }
}
