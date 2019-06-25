using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.Philips_Hue.Communication
{
 
    [Obsolete]
    public static class Comm
    {
        private static int _timeout = 3000;
        public static bool DetectProxy = false;
        private static string _lastjson = string.Empty;
        private static readonly string _lastUrl = string.Empty;
        private static Action _timeoutcallback;

        public static int Timeout
        {
            get => _timeout;
            set => _timeout = value;
        }

        public static string Lastjson => _lastjson;

        public static string LastUrl => _lastUrl;

        public static Action Timeoutcallback { get => _timeoutcallback; set => _timeoutcallback = value; }

        public static CommResult SendRequest(Uri url, WebRequestType type, string data = "")
        {
            CommResult result = new CommResult();
            result.Url = url;
            try
            {
                WebHeaderCollection whc = new WebHeaderCollection {{HttpRequestHeader.ContentType, "application/json"}};
                WebClientTimeout wc = new WebClientTimeout {Proxy = null, Timeout = _timeout, Headers = whc};

                string Method = string.Empty;
                string received = string.Empty;
                switch (type)
                {
                    case WebRequestType.Put:
                        Method = "PUT";
                        received = wc.UploadString(url, Method, data);
                        break;
                    case WebRequestType.Get:
                        Method = "GET";
                        var bytes = wc.DownloadData(url);
                        UTF8Encoding utf8 = new UTF8Encoding();
                        received = utf8.GetString(bytes);
                        break;
                    case WebRequestType.Post:
                        Method = "POST";
                        received = wc.UploadString(url, Method, data);
                        break;
                    case WebRequestType.Delete:
                        Method = "DELETE";
                        received = wc.UploadString(url, Method, data);
                        break;
                }

                if (received != string.Empty)
                {
                    result.Status = WebExceptionStatus.Success;
                    result.Data = received;
                }

                _lastjson = received;
            }
            catch (TimeoutException)
            {               
                OnCommunicationTimeOut(new TimeOutEventArgs(url, type));
                _timeoutcallback?.Invoke();
                result = null;
            }
            catch (WebException ex)
            {
                result.Status = ex.Status;
                result.Data = "{}";
                result.Ex = ex;
            }

            catch(Exception ex)
            {
                result.Status = WebExceptionStatus.UnknownError;
                result.Data = "{}";
                result.Ex = ex;
                _lastjson = null;
            }
            return result;
        }

        public static async Task<CommResult> SendRequestAsyncTask(Uri url, WebRequestType type, string data = "")
        {
            CommResult result = new CommResult {Url = url};

            try
            {
                WebClientTimeout wc = new WebClientTimeout { Proxy = null, Timeout = _timeout };
                string Method = string.Empty;
                string received = string.Empty;
                switch (type)
                {
                    case WebRequestType.Put:
                        Method = "PUT";
                        received = await wc.UploadStringTaskAsync(url, Method, data);
                        break;
                    case WebRequestType.Get:
                        Method = "GET";
                        byte[] bytes = await wc.DownloadDataTaskAsync(url);
                        UTF8Encoding utf8 = new UTF8Encoding();
                        received = utf8.GetString(bytes);
                        break;
                    case WebRequestType.Post:
                        Method = "POST";
                        received = await wc.UploadStringTaskAsync(url, Method, data);
                        break;
                    case WebRequestType.Delete:
                        Method = "DELETE";
                        received = await wc.UploadStringTaskAsync(url, Method, data);
                        break;
                }

                if (received != string.Empty)
                {
                    result.Status = WebExceptionStatus.Success;
                    result.Data = received;
                }

                _lastjson = received;
            }
            catch (TimeoutException ex)
            {
                OnCommunicationTimeOut(new TimeOutEventArgs(url, type));
                _timeoutcallback?.Invoke();
                result.Status = WebExceptionStatus.Timeout;
                result.Data = null;
                result.Ex = ex;
            }
            catch (WebException ex)
            {
                result.Status = ex.Status;
                result.Data = "{}";
                result.Ex = ex;
            }
            catch (Exception ex)
            {
                result.Status = WebExceptionStatus.UnknownError;
                result.Data = "{}";
                result.Ex = ex;
                _lastjson = null;
            }
            return result;
        }

        public static event EventHandler CommunicationTimedOut;

        private static void OnCommunicationTimeOut(TimeOutEventArgs e)
        {
            CommunicationTimedOut?.Invoke(null, e);
        }

    }

    public class TimeOutEventArgs : EventArgs
    {
        private Uri _url;
        private WebRequestType _type;

        public TimeOutEventArgs(Uri url, WebRequestType type)
        {
            _url = url;
            _type = type;
        }

        public Uri Url => _url;

        public WebRequestType Type => _type;
    }

    public class CommResult
    {
        public WebExceptionStatus Status ;
        public string Data;
        public Exception Ex;
        public Uri Url;
    }
}
