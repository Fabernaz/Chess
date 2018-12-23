using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCore
{
    public class MovePair
    {
        public int MoveNumber { get; }
        public MoveBase WhiteMove { get; set; }
        public MoveBase BlackMove { get; set; }

        public MovePair(int moveNumber, MoveBase whiteMove)
        {
            if (moveNumber < 1)
                throw new ArgumentException();
            MoveNumber = moveNumber;
            WhiteMove = whiteMove;
        }

        public MovePair(int moveNumber, MoveBase whiteMove, MoveBase blackMove)
            : this(moveNumber, whiteMove)
        {
            BlackMove = blackMove;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is MovePair))
                return false;

            var other = obj as MovePair;
            return other.MoveNumber == MoveNumber;
        }

        public override int GetHashCode()
        {
            return MoveNumber.GetHashCode();
        }

        public static bool operator ==(MovePair obj1, MovePair obj2)
        {
            return obj1.Equals(obj2);
        }

        public static bool operator !=(MovePair obj1, MovePair obj2)
        {
            return !obj1.Equals(obj2);
        }

        public override string ToString()
        {
            return string.Format("{0}. {1} {2}", MoveNumber, 
                                                 WhiteMove, 
                                                 BlackMove?.ToString() ?? "...");
        }

        public IEnumerable<string> GetStringExplodedRepresentation()
        {
            var ret = new List<string>();
            ret.Add(MoveNumber + ".");
            ret.Add(WhiteMove.ToString());
            ret.Add(BlackMove?.ToString() ?? "...");
            return ret;
        }
    }
}
