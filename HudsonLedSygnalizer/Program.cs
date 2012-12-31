namespace HudsonLedSygnalizer
{
    #region Using
    using System;
    using System.Timers;
    using System.ServiceProcess;
    #endregion

    class Program
    {
        #region Public methods
        static void Main(string[] args)
        {
            // Check if the program is ran as service or not
            if (Environment.UserInteractive)
            {
                using (HudsonLedSygnalizer Sygnalizer = new HudsonLedSygnalizer())
                {
                    Sygnalizer.Start();

                    Console.ReadKey();

                    Sygnalizer.Stop();

                }
            }
            else
            {
                // Run as service
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
				{ 
					new HudsonLedSygnalizerService() 
				};
                ServiceBase.Run(ServicesToRun);
            }
        }
        #endregion

 
    }
}
