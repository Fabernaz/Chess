using System;
using System.Threading;

namespace Common
{
    public class InfoAsker<TInfo>
    {
        private AutoResetEvent _autoReset;
        private TInfo _information;

        public event EventHandler AskingInfoEvent;

        public InfoAsker()
        {
            _autoReset = new AutoResetEvent(false);
        }

        public TInfo AskAndWaitForInfo()
        {
            Thread askForInfo = new Thread(new ThreadStart(AskInfo));
            askForInfo.Start();
            _autoReset.WaitOne();
            return _information;
        }

        private void AskInfo()
        {
            if (AskingInfoEvent == null)
            {
                _autoReset.Set();
                return;
            }
            else
                AskingInfoEvent.Invoke(this, new EventArgs());
        }

        public void ProvideRequestedInfo(TInfo information)
        {
            _information = information;
            _autoReset.Set();
        }
    }

    public class InfoEventArgs<TInfo> : EventArgs
    {
        public TInfo Info { get; set; }

        public InfoEventArgs(TInfo info)
        {
            Info = info;
        }
    }
}
