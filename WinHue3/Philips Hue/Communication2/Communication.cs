using System;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using WinHue3.Functions.Application_Settings.Settings;
using System.Threading;

namespace WinHue3.Philips_Hue.Communication2
{
    public static class HueHttpClient
    {

        private static HttpClient client;



        public static int Timeout
        {
            get => client.Timeout.Milliseconds;
            set {
                Init(value);
            }
        }

        public static string LastJson;

        static HueHttpClient()
        {
            Init();
        }

        private static void Init(int? timeout = null)
        {
  
            if(client != null) { client.Dispose(); }
            client = new HttpClient
            {
                Timeout = new TimeSpan(0, 0, 0, timeout ?? WinHueSettings.settings.Timeout),

            };

            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        }

        public static async Task<HttpResult> SendRequestAsyncTask(Uri url, WebRequestType type, string data = "")
        {
            HttpResult httpres = new HttpResult();
            LastJson = data;
            try
            {
                HttpResponseMessage httpr;
                switch (type)
                {
                    case WebRequestType.Put:
                        httpr = await client.PutAsync(url, new StringContent(data));
                        break;
                    case WebRequestType.Get:
                        httpr = await client.GetAsync(url);
                        break;
                    case WebRequestType.Post:
                        httpr = await client.PostAsync(url, new StringContent(data));
                        break;
                    case WebRequestType.Delete:
                        httpr = await client.DeleteAsync(url);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, $"{type} is not a recognized value.");
                }

                httpres.Success = httpr.IsSuccessStatusCode;
                httpres.StatusCode = httpr.StatusCode;
                if (httpr.IsSuccessStatusCode)
                {
                    httpres.Data = httpr.Content.ReadAsStringAsync().Result;
                }

            }
            catch (TimeoutException)
            {
                OnCommunicationTimeOut?.Invoke(client,new TimeOutEventArgs(url, type));
            }
            catch (Exception ex)
            {
                httpres.Success = false;
                httpres.Data = ex.Message;
            }

            return httpres;
        }

        public static HttpResult SendRequest(Uri url, WebRequestType type, string data = "")
        {
            HttpResult httpres = new HttpResult();

            try
            {
                HttpResponseMessage httpr;

                switch (type)
                {
                    case WebRequestType.Put:
                        httpr = client.PutAsync(url, new StringContent(data)).Result;
                        break;
                    case WebRequestType.Get:
                        httpr = client.GetAsync(url).Result;
                        break;
                    case WebRequestType.Post:
                        httpr = client.PostAsync(url, new StringContent(data)).Result;
                        break;
                    case WebRequestType.Delete:
                        httpr = client.DeleteAsync(url).Result;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, $"{type} is not a recognized value.");
                }

                httpres.StatusCode = httpr.StatusCode;
                httpres.Success = httpr.IsSuccessStatusCode;
                if (httpr.IsSuccessStatusCode)
                {
                    httpres.Data = httpr.Content.ReadAsStringAsync().Result;

                }
            }
            catch (TimeoutException)
            {
                OnCommunicationTimeOut?.Invoke(client,new TimeOutEventArgs(url, type));
            }
            catch (Exception ex)
            {
                httpres.Success = false;
                httpres.Data = ex.Message;
            }

            return httpres;
        }

        public static event CommunicationTimedOutEvent OnCommunicationTimeOut;
        public delegate void CommunicationTimedOutEvent(object sender, TimeOutEventArgs e);


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


    public class HttpResult
    {
        public HttpResult()
        {
            Success = false;
        }
        public bool Success;
        public string Data;
        public HttpStatusCode StatusCode;
    }

}
