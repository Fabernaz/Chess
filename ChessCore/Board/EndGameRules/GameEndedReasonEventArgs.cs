using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCore
{
    public class GameEndedReasonEventArgs : EventArgs
    {
        public GameEndedReason Reason { get; }
        public GameOutcome Outcome { get; }

        public GameEndedReasonEventArgs(GameEndedReason reason, Color winningColor)
        {
            Reason = reason;
            Outcome = winningColor is null ? GameOutcome.Draw : GetOutcome(winningColor);
        }

        private GameOutcome GetOutcome(Color winningColor)
        {
            return winningColor == Color.White ? GameOutcome.WhiteWins : GameOutcome.BlackWins;
        }

        public GameEndedReasonEventArgs(GameEndedReason reason)
            : this(reason, null)
        { }
    }
}
