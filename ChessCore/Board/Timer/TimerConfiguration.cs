using System.Collections.Generic;

namespace ChessCore
{
    public class TimerConfiguration
    {
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int AfterMoveSecondsIncrement { get; set; }
        public IDictionary<int, int> AfterMoveNumSecondsIncrement { get; set; }
    }
}
