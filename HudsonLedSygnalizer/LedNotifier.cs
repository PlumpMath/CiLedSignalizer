namespace HudsonLedSygnalizer
{
    #region Using
    using System;
    using System.Timers;
    using HIDLibrary;
    #endregion

    internal class LedNotifier : IDisposable
    {
        #region Private variables
        Timer _Timer;
        HidDevice _MessageBoard = null;

        // Led Messageboard from Dreamcheeky - http://www.dreamcheeky.com/led-message-board
        private int _VentodId = 0x1d34;
        private int _ProductId = 0x0013;

        byte[] Packet0 = new byte[] { 0x00, 0x00, 0x00, 0xFF, 0xFC, 0x7F, 0xFF, 0xF8, 0x3F };
        byte[] Packet1 = new byte[] { 0x00, 0x00, 0x02, 0xFF, 0xF0, 0x1F, 0xFF, 0xE0, 0x0F };
        byte[] Packet2 = new byte[] { 0x00, 0x00, 0x04, 0xFF, 0xF0, 0x1F, 0xFF, 0xF8, 0x3F };
        byte[] Packet3 = new byte[] { 0x00, 0x00, 0x06, 0xFF, 0xFC, 0x7F, 0xFF, 0xFF, 0xFF };

        byte[] Packet4 = new byte[] { 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, };
        byte[] Packet5 = new byte[] { 0x00, 0x00, 0x02, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, };
        byte[] Packet6 = new byte[] { 0x00, 0x00, 0x04, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, };
        byte[] Packet7 = new byte[] { 0x00, 0x00, 0x06, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, };

        // Logger
        private Codefusion.S000.Basic.Logging.LogProvider _LogProvider = new Codefusion.S000.Basic.Logging.LogProvider("LedNotifier");
        #endregion

        #region Constructors
        internal LedNotifier()
        {
            // Init timer
            _Timer = new Timer();
            _Timer.Enabled = false;
            _Timer.Interval = 1000;
            _Timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);

            // Init message board
            MyDetectMessageBoard();

        }
        #endregion

        #region Metods
        internal void Enable()
        {
            _Timer.Enabled = true;
        }

        internal void Disable()
        {
            _Timer.Enabled = false;
        }

        #endregion

        #region Private variabels
        private void MyDetectMessageBoard()
        {
            HidDevice[] HidDeviceList;

            try
            {

                HidDeviceList = HidDevices.Enumerate(_VentodId, _ProductId);

                if (HidDeviceList.Length > 0)
                {
                    _MessageBoard = HidDeviceList[0];

                    _MessageBoard.Open();
                }
            }
            catch (Exception ex)
            {
                _LogProvider.Error(ex);
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Try to detect message board
            if (_MessageBoard == null)
            {
                MyDetectMessageBoard();
            }

            // Try to send the signal
            if (_MessageBoard != null)
            {
                _MessageBoard.Write(Packet0);
                _MessageBoard.Write(Packet1);
                _MessageBoard.Write(Packet2);
                _MessageBoard.Write(Packet3);
            }
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_MessageBoard != null)
            {
                try
                {
                    _MessageBoard.Close();
                }
                catch (Exception ex)
                {
                    _LogProvider.Error(ex);
                }
            }
        }

        #endregion
    }
}
