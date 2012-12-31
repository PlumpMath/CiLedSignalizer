namespace HudsonLedSygnalizer
{
    #region Using
    using System.ServiceProcess;
    #endregion

    partial class HudsonLedSygnalizerService : ServiceBase
    {
        #region Private variables
        private HudsonLedSygnalizer _HudsonLedSygnalizer;
        #endregion

        #region Constructors
        public HudsonLedSygnalizerService()
        {
            InitializeComponent();

            _HudsonLedSygnalizer = new HudsonLedSygnalizer();
        }
        #endregion

        #region Overrides
        protected override void OnStart(string[] args)
        {
            _HudsonLedSygnalizer.Start();
        }

        protected override void OnStop()
        {
            _HudsonLedSygnalizer.Stop();
        }
        #endregion
    }
}
