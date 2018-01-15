
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Trader
{
    public class WebApiUtils
    {

        public static string Send(HttpMethod method, string url, string json=null, string token = null, TimeSpan? timeout = null)
        {
            LogUtils.Debug($"calling url {method} {url}");

            var headers =
                new Dictionary<string, string>() { { "accept", "application/json" } };
            if (!string.IsNullOrEmpty(token))
            {
                if (!token.Contains(' '))
                    token = "Bearer " + token;
                headers.Add("authorization", token);
            }
            LogUtils.Debug($"webapi.Send {method} {url} {json}");
            var response = Task.Run(() => SendAsync(method, url, json, headers, timeout, "application/json")).Result;

            var res= response.Content.ReadAsStringAsync().Result;
            LogUtils.Debug($"webapi.Send received {method} {url} {response.StatusCode}  {res}");
            return res;

        }
        public static T Send<T>(HttpMethod method, string url, object data=null, string token = null, TimeSpan? timeout = null)
        {
            string json = null;
            if (data != null)
                json = JsonUtils.Serialize(data);

            return JsonUtils.Decode<T>(Send(method, url, json, token, timeout));
        }


        public static async Task<HttpResponseMessage> SendAsync(HttpMethod method, string url,
            string data,
            Dictionary<string, string> headers,
            TimeSpan? timeout = null,
            string contentType = "application/json")
        {

                using (var client = new HttpClient())
                {
                    var httpContent = new HttpRequestMessage(method, url);
                    foreach (var header in headers)
                        httpContent.Headers.TryAddWithoutValidation(header.Key, header.Value);

                    if (data != null)
                        httpContent.Content = new StringContent(data, Encoding.UTF8, contentType);
                    var cancelToken = new System.Threading.CancellationToken();
                    var response = client.SendAsync(httpContent, cancelToken);
                    if (timeout == null)
                    {
                        return await response;
                    }
                    else if (await Task.WhenAny(response, Task.Delay(timeout.Value, cancelToken)) == response)
                    {
                        // Task completed within timeout.
                        return await response;
                    }
                    else
                    {
                        // timeout/cancellation logic
                        throw new TimeoutException($"after {timeout.Value} calling to {url}");
                    }
            }
        }

    }
}