namespace HudsonLedSygnalizer
{
    #region Using
    using System;
    using System.Net;
    #endregion
    internal class MyWebClient : WebClient
    {
        #region Private variables
        private CookieContainer m_container = new CookieContainer();
        #endregion

        #region Overrides
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).CookieContainer = m_container;
                (request as HttpWebRequest).Timeout = 6000;
            }
            return request;
        }
        #endregion
    }
}
