using System;
using System.Net;
using System.Text;

namespace HueLib2
{
    public enum WebRequestType { PUT, GET, POST, DELETE };
    public static class Communication
    {
        private static int _timeout = 3000;
        public static bool DetectProxy = false;
        public static string lastjson { private set; get; }

        public static int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        public static CommResult SendRequest(Uri url, WebRequestType type, string data = "")
        {
            CommResult result = new CommResult();

            try
            {
                WebClientTimeout wc = new WebClientTimeout {Proxy = null, Timeout = _timeout};
                string Method = string.Empty;
                string received = string.Empty;
                switch (type)
                {
                    case WebRequestType.PUT:
                        Method = "PUT";
                        received = wc.UploadString(url, Method, data);
                        break;
                    case WebRequestType.GET:
                        Method = "GET";
                        var bytes = wc.DownloadData(url);
                        UTF8Encoding utf8 = new UTF8Encoding();
                        received = utf8.GetString(bytes);
                        break;
                    case WebRequestType.POST:
                        Method = "POST";
                        received = wc.UploadString(url, Method, data);
                        break;
                    case WebRequestType.DELETE:
                        Method = "DELETE";
                        received = wc.UploadString(url, Method, data);
                        break;
                }

                if (received != string.Empty)
                {
                    result.status = WebExceptionStatus.Success;
                    result.data = received;
                }

                lastjson = received;
            }
            catch (WebException ex)
            {
                result.status = ex.Status;
                result.data = "{}";
            }
            catch(Exception)
            {
                lastjson = null;
            }
            return result;
        }
    }

    public class CommResult
    {
        public WebExceptionStatus status ;
        public string data;
    }
}
