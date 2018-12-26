using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace ChessCore
{
    internal class PlayerTimer
    {
        #region Fields

        private const int INTERVAL = 100;
        private readonly TimerConfiguration _config;
        private readonly Timer _timer;
        private int _millisecondsLeft;
        private int _moveNumber;

        #endregion

        #region Properties

        internal string TimeLeft { get; private set; }

        internal event EventHandler TimeChanged;
        internal event EventHandler TimesUp;

        #endregion

        #region Constructors

        internal PlayerTimer(TimerConfiguration config)
        {
            _config = config;
            _timer = new Timer(INTERVAL);
            _timer.Elapsed += OnIntervalElapsed;
            _millisecondsLeft = config.Hours * 60 * 60 * 1000 + config.Minutes * 60 * 1000;
            ResetTimeLeft();
        }

        private void OnIntervalElapsed(object sender, ElapsedEventArgs e)
        {
            _millisecondsLeft -= INTERVAL;
            CheckForTimesUp();
            ResetTimeLeft();
        }

        private void CheckForTimesUp()
        {
            if(_millisecondsLeft <= 0)
            {
                _timer.Stop();
                TimesUp?.Invoke(this, new EventArgs());
                _millisecondsLeft = 0;
            }
        }

        private void ResetTimeLeft()
        {
            TimeSpan t = TimeSpan.FromMilliseconds(_millisecondsLeft);

            TimeLeft = string.Empty;
            TimeLeft = t.Hours > 0 || t.Minutes > 0 ?
                    string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds) :
                    string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D1}", t.Hours, t.Minutes, t.Seconds, t.Milliseconds);

            TimeChanged?.Invoke(this, new EventArgs());
        }

        #endregion

        #region Methods

        internal void Start()
        {
            AlternateActivity();
        }

        internal void Stop()
        {
            _timer.Stop();
        }

        internal void OnMovePlayed()
        {
            AlternateActivity();

            if (!_timer.Enabled)
            {
                _moveNumber++;
                IncrementOnMove();
                ResetTimeLeft();
            }
        }

        internal void FirstGameMovePlayed()
        {
            _moveNumber++;
        }

        private void IncrementOnMove()
        {
            _millisecondsLeft += _config.AfterMoveSecondsIncrement * 1000;
            if (_config.AfterMoveNumSecondsIncrement.ContainsKey(_moveNumber))
                _millisecondsLeft += _config.AfterMoveNumSecondsIncrement[_moveNumber] * 1000;
        }

        private void AlternateActivity()
        {
            _timer.Enabled = !_timer.Enabled;
            if (!_timer.Enabled)
                _timer.Stop();
            else
                _timer.Start();
        }

        #endregion
    }
}
