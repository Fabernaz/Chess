using System;

namespace ChessCore
{
    public struct SquareCoordinate
    {
        public int Rank { get; }
        public int File { get; }

        public SquareCoordinate(int rank, int file)
        {
            if (!MoveUtilities.IsValidPosition(rank, file))
                throw new ArgumentException();

            File = file;
            Rank = rank;
        }

        public override bool Equals(Object obj)
        {
            return obj is SquareCoordinate && this == (SquareCoordinate)obj;
        }
        public override int GetHashCode()
        {
            return Rank.GetHashCode() ^ File.GetHashCode();
        }
        public static bool operator ==(SquareCoordinate x, SquareCoordinate y)
        {
            return x.Rank == y.Rank && x.File == y.File;
        }
        public static bool operator !=(SquareCoordinate x, SquareCoordinate y)
        {
            return !(x == y);
        }

        public override string ToString()
        {
            var file = MoveUtilities.GetFileFromInt(File);
            return file.ToString() + Rank;
        }

        public SquareCoordinate AddRank(int rank)
        {
            return new SquareCoordinate(Rank + rank, File);
        }

        public SquareCoordinate AddFile(int file)
        {
            return new SquareCoordinate(Rank, File + file);
        }

        public SquareCoordinate AddRankAndFile(int rank, int file)
        {
            return new SquareCoordinate(Rank + rank, File + file);
        }
    }
}
