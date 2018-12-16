using System;

namespace ChessCore
{
    public struct Position
    {
        public int Rank { get; }
        public int File { get; }

        public Position(int rank, int file)
        {
            if (!MoveUtilities.IsValidPosition(rank, file))
                throw new ArgumentException();

            File = file;
            Rank = rank;
        }

        public override bool Equals(Object obj)
        {
            return obj is Position && this == (Position)obj;
        }
        public override int GetHashCode()
        {
            return Rank.GetHashCode() ^ File.GetHashCode();
        }
        public static bool operator ==(Position x, Position y)
        {
            return x.Rank == y.Rank && x.File == y.File;
        }
        public static bool operator !=(Position x, Position y)
        {
            return !(x == y);
        }

        public override string ToString()
        {
            var file = MoveUtilities.GetFileFromInt(File);
            return file.ToString() + Rank;
        }

        public Position AddRank(int rank)
        {
            return new Position(Rank + rank, File);
        }

        public Position AddFile(int file)
        {
            return new Position(Rank, File + file);
        }

        public Position AddRankAndFile(int rank, int file)
        {
            return new Position(Rank + rank, File + file);
        }
    }
}
