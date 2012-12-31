namespace HudsonLedSygnalizer
{
    #region Using
    using System;
    using System.Web;
    using System.Net;
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;
    using System.Text;
    using System.Threading;
    #endregion

    internal class HudsonClient
    {
        #region Private variables
        bool _IsOk = true;

        private Uri _Uri;
        private string _UserName;
        private string _Password;

        private string _HudsonFiler = "api/xml?tree=jobs[name,lastBuild[result]]";

        // Locking object
        private object _TimerLocker = new object();

        // Logging object
        private Codefusion.S000.Basic.Logging.LogProvider _LogProvider = new Codefusion.S000.Basic.Logging.LogProvider("HudsonClient");
        #endregion

        #region Constructors
        internal HudsonClient(Uri uri, string userName, string password)
        {
            if (uri == null)
            {
                throw new System.ArgumentNullException("uri");
            }

            _Uri = uri;
            _UserName = userName;
            _Password = password;
        }
        #endregion

        #region Public properties
        internal bool IsOk
        {
            get
            {
                return MyGetIsOk();
            }
        }
        #endregion

        #region Private methods
        private bool MyGetIsOk()
        {
            // Non blocking lock
            if (Monitor.TryEnter(_TimerLocker))
            {
                // Create web client instance
                MyWebClient WebClient = null;

                try
                {
                    WebClient = new MyWebClient();

                    // Log-in in if neccessary
                    if (!String.IsNullOrEmpty(_UserName) && !String.IsNullOrEmpty(_Password))
                    {
                        // Readc login page and dump result to dummy string
                        string Dump = (new StreamReader(WebClient.OpenRead(_Uri.ToString() + "loginEntry"))).ReadToEnd();

                        System.Collections.Specialized.NameValueCollection Variables = new System.Collections.Specialized.NameValueCollection();
                        Variables.Add("j_username", _UserName);
                        Variables.Add("j_password", _Password);
                        Variables.Add("from", "/");
                        Variables.Add("Submit", "log in");

                        WebClient.UploadValues(_Uri.ToString() + "j_acegi_security_check", "POST", Variables);
                    }

                    StreamReader RequestReader = new StreamReader(WebClient.OpenRead(_Uri.ToString() + _HudsonFiler));
                    string ResponseFromServer = RequestReader.ReadToEnd();

                    XElement XElement = XElement.Parse(ResponseFromServer);

                    foreach (XElement Element in XElement.Elements())
                    {
                        // TODO mk from mk; meybe add project filtering
                        //Element.Element("name");
                        XElement LastBuildElement = Element.Element("lastBuild");

                        if (LastBuildElement != null)
                        {
                            XElement IsSuccessResult = LastBuildElement.Element("result");

                            if (IsSuccessResult != null && IsSuccessResult.Value.Equals("FAILURE"))
                            {
                                _IsOk = false;
                                break;
                            }
                        }

                        _IsOk = true;
                    }
                }
                catch (Exception ex)
                {
                    _LogProvider.Error(ex);
                }
                finally
                {
                    WebClient.Dispose();
                    
                    // Exit from critical section
                    Monitor.Exit(_TimerLocker);
                }
            }
            return _IsOk;
        }
        #endregion
    }
}
