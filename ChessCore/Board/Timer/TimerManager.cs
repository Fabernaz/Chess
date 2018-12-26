using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessCore
{
    internal class TimerManager
    {
        private readonly TimerConfiguration _config;
        private readonly PlayerTimer _whiteTimer;
        private readonly PlayerTimer _blackTimer;
        private int moveNumber;

        internal event EventHandler BlackTimerChanged;
        internal event EventHandler WhiteTimerChanged;
        internal event EventHandler BlackTimesUp;
        internal event EventHandler WhiteTimesUp;

        internal TimerManager(TimerConfiguration config)
        {
            _config = config;

            _whiteTimer = new PlayerTimer(config);
            _whiteTimer.TimeChanged += (sender, e) => { WhiteTimerChanged?.Invoke(this, e); };
            _whiteTimer.TimesUp += (sender, e) => { WhiteTimesUp?.Invoke(this, e); };
            _blackTimer = new PlayerTimer(config);
            _blackTimer.TimeChanged += (sender, e) => { BlackTimerChanged?.Invoke(this, e); };
            _blackTimer.TimesUp += (sender, e) => { BlackTimesUp?.Invoke(this, e); };
        }

        internal void OnMovePlayed()
        {
            if (moveNumber == 0)
            {
                _whiteTimer.FirstGameMovePlayed();
                _blackTimer.OnMovePlayed();
            }
            else
            {
                _blackTimer.OnMovePlayed();
                _whiteTimer.OnMovePlayed();
            }

            moveNumber++;
        }

        internal string GetBlackTime()
        {
            return _blackTimer.TimeLeft;
        }

        internal void OnGameEnded()
        {
            _blackTimer.Stop();
            _whiteTimer.Stop();
        }

        internal string GetWhiteTime()
        {
            return _whiteTimer.TimeLeft;
        }
    }
}