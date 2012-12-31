namespace HudsonLedSygnalizer
{
    #region Using
    using System;
    using System.Timers;
    #endregion

    internal class HudsonLedSygnalizer : IDisposable
    {
        #region Private variabels
        private static string _HudsonUrl = HudsonLedSygnalizerSettings.Default.HudsonUrl;
        private static string _HudsonUserName = HudsonLedSygnalizerSettings.Default.HudsonUserName;
        private static string _HudsonUserPassword = HudsonLedSygnalizerSettings.Default.HudsonUserPassword;
        private static LedNotifier _LedNotifier;
        private static Timer _Timer;
        private static HudsonClient _HudsonClient;
        #endregion

        #region Constructors
        internal HudsonLedSygnalizer()
        {
            // Init led notifier
            _LedNotifier = new LedNotifier();
            _HudsonClient = new HudsonClient(new Uri(_HudsonUrl), _HudsonUserName, _HudsonUserPassword);

            // Init main timer
            _Timer = new Timer();
            _Timer.Interval = HudsonLedSygnalizerSettings.Default.MainInterval;
            _Timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            _Timer.Enabled = false;

            
        }
        #endregion

        #region Methods
        internal void Start()
        {
            _Timer.Enabled = true;
        }

        internal void Stop()
        {
            _Timer.Enabled = false;
        }
        #endregion

        #region Private methods
        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Environment.UserInteractive)
            {
                Console.Write(".");
            }

            if (MyCheckHudson())
            {
                _LedNotifier.Disable();
            }
            else
            {
                _LedNotifier.Enable();
            }
        }

        private static bool MyCheckHudson()
        {
            return _HudsonClient.IsOk;
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_LedNotifier != null)
            {
                _LedNotifier.Dispose();
            }
        }

        #endregion
    }
}