using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;

namespace Brierley.ClientDevUtilities.Net
{
    public class CataboomUtility : ICataboomUtility
    {
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
        private const string _className = "CataboomUtilities";

        public static CataboomUtility Instance { get; private set; }

        private static HttpClient _httpClient;
        private static object _httpClientLock = new object();
        private static HttpClient HttpClient
        {
            get
            {
                if (_httpClient == null)
                {
                    lock (_httpClientLock)
                    {
                        if (_httpClient == null)
                        {
                            var client = new HttpClient();
                            client.DefaultRequestHeaders.Accept.Clear();
                            _httpClient = client;
                        }
                    }
                }

                return _httpClient;
            }
        }

        static CataboomUtility()
        {
            Instance = new CataboomUtility();
        }

        public async Task<string> GetCataboomGameURLAsync(string requestURI)
        {
            string methodName = "GetCataboomGameURL";
            try
            {
                return await SendRequestAsync<string>(HttpMethod.Get, requestURI);
            }
            catch (Exception ex)
            {
                string error = "Unexpected error getting game URL from cataboom Message : " + ex.Message;
                _logger.Error(_className, methodName, error);
                throw new LWOperationInvocationException(error, ex) { ErrorCode = 1};
            }
        }

        private async Task<T> SendRequestAsync<T>(HttpMethod method, string requestUri)
        {
            const string METHOD_NAME = "SendRequestAsync";

            using (var request = new HttpRequestMessage())
            {
                request.Method = method;
                request.RequestUri = new Uri(requestUri, UriKind.RelativeOrAbsolute);

                using (var response = await HttpClient.SendAsync(request))
                {
                    switch (response.StatusCode)
                    {
                        //response code on success
                        case HttpStatusCode.OK:
                            {
                                //Cataboom may return a pretty Not found web page in the response with a status code of 200
                                //Current cataboom setup seems to return a URL of length 71+ depending on the length of the actual game name ect 'chevron-game1'
                                if (response.Content.Headers.ContentLength > 200)
                                {
                                    string errorMessage = String.Format("Calling the URI {0} we got a response back that exceeds a reasonible url length from cataboom, it may be the html webpage indicating a 404", requestUri);
                                    throw new LWException(errorMessage);
                                }
                                break;
                            }
                        //common Cataboom errors
                        case HttpStatusCode.Forbidden:
                            {
                                string errorMessage = String.Format("Got an Unauthorized exception calling Cataboom with this URL {0} ErrorMessage = {1}", requestUri, await response.Content.ReadAsStringAsync());
                                _logger.Error(_className, METHOD_NAME, errorMessage);
                                break;
                            }
                        default:
                            {
                                string errorMessage = await response.Content.ReadAsStringAsync();
                                _logger.Error(_className, METHOD_NAME, String.Format("Unexpected response code from cataboom: {0} with Content: {1}", response.StatusCode.ToString(),!String.IsNullOrEmpty(errorMessage) ? errorMessage : "Null error message"));
                                break;
                            }

                    }

                    response.EnsureSuccessStatusCode();

                    T payload = default(T);
                    if (typeof(T) == typeof(string))
                    {
                        string gameUrlraw = await response.Content.ReadAsStringAsync();
                        if (!gameUrlraw.StartsWith("http"))
                        {
                            string errorMessage = String.Format("Returned response does not start with 'http' Request: {0} Response: {1}", requestUri, gameUrlraw);
                            throw new LWException(errorMessage);
                        }
                        payload = (T)Convert.ChangeType(gameUrlraw, typeof(T));
                    }

                    return payload;
                }
            }
        }
    }
}
