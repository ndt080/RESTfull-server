using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace test_client
{
    internal static class HttpTestClient
    {
        private const string Url = "http://localhost:5050";
        private static readonly string RequestUrl = Url + "/contacts";

        private static bool Testing(HttpStatusCode respStatusCode, int statusCode)
        {
            return respStatusCode == (HttpStatusCode) statusCode;
        }

        private static HttpRequestMessage CreateRequestMessage(string url, string method, IEnumerable<string[]> headers)
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = new Uri(url);
            method = method.ToLower();
            request.Method = method switch
            {
                "get" => HttpMethod.Get,
                "post" => HttpMethod.Post,
                "put" => HttpMethod.Put,
                "delete" => HttpMethod.Delete,
                _ => request.Method
            };

            foreach (var h in headers)
            {
                request.Headers.Add(h[0].ToLower(), h[1]);
            }

            return request;
        }

        private static void WriteHttpResponseInfo(HttpResponseMessage respMsg, string message)
        {
            Console.WriteLine("log>\tMethod: {0}\n" +
                              "\tURL: {1}\n" +
                              "\tStatusCode: {2}\n" +
                              "\t{3}",
                respMsg.RequestMessage.Method, respMsg.RequestMessage.RequestUri, (int) respMsg.StatusCode, message);
        }

        static async Task Main(string[] args)
        {
            try
            {
                using var client = new HttpClient();
                Console.WriteLine("log>\tRunning the integration test for HTTP RESTfull server");

                /*################################VALID GET REQUEST################################*/
                Console.WriteLine("log>\tSending a valid GET request without parameters...\n" +
                                  "\tURL: {0}", RequestUrl);
                var result = await client.GetAsync(RequestUrl);
                WriteHttpResponseInfo(result, Testing(result.StatusCode, 200) ? "Request success!" : "Request Error!");

                /*##############################INVALID GET REQUEST################################*/
                var invalidReqUrl = Url + "/";
                Console.WriteLine("log>\tSending a invalid GET request without parameters...\n" +
                                  "\tURL: {0}", invalidReqUrl);
                result = await client.GetAsync(invalidReqUrl);
                WriteHttpResponseInfo(result, Testing(result.StatusCode, 200) ? "Request success!" : "Request Error!");

                /*################################VALID GET REQUEST################################*/
                var headers = new List<string[]>()
                {
                    new string[] {"id", "155e4b37-2cc1-40bd-ac27-a028dbe6d30f"}
                };
                Console.WriteLine("log>\tSending a valid GET request with the ID parameter...\n" +
                                  "\tURL: {0}", RequestUrl);
                foreach (var h in headers)
                {
                    Console.WriteLine("\t{0}: {1}", h[0].ToLower(), h[1]);
                }
                result = await client.SendAsync(CreateRequestMessage(RequestUrl, "GET", headers));
                WriteHttpResponseInfo(result, Testing(result.StatusCode, 200) ? "Request success!" : "Request Error!");
                headers.Clear();

                /*################################VALID POST REQUEST################################*/
                headers = new List<string[]>()
                {
                    new string[] {"LastName", "Scherbenok"},
                    new string[] {"FirstName", "Andrei"},
                    new string[] {"NumberPhone", "375337561513"},
                };
                Console.WriteLine("log>\tSending a valid POST request with the LastName, FirstName, NumberPhone" +
                                  " parameters...\n" +
                                  "\tURL: {0}", RequestUrl);
                foreach (var h in headers)
                {
                    Console.WriteLine("\t{0}: {1}", h[0].ToLower(), h[1]);
                }
                result = await client.SendAsync(CreateRequestMessage(RequestUrl, "POST", headers));
                WriteHttpResponseInfo(result, Testing(result.StatusCode, 200) ? "Request success!" : "Request Error!");
                headers.Clear();
                
                /*################################INVALID POST REQUEST################################*/
                headers = new List<string[]>()
                {
                    new string[] {"LastName", "Scherbenok"},
                    new string[] {"FirstName", "Andrei"},
                };
                Console.WriteLine("log>\tSending a invalid POST request with the LastName, FirstName" +
                                  " parameters...\n" +
                                  "\tURL: {0}", RequestUrl);
                foreach (var h in headers)
                {
                    Console.WriteLine("\t{0}: {1}", h[0].ToLower(), h[1]);
                }
                result = await client.SendAsync(CreateRequestMessage(RequestUrl, "POST", headers));
                WriteHttpResponseInfo(result, Testing(result.StatusCode, 200) ? "Request success!" : "Request Error!");
                headers.Clear();

                /*################################VALID PUT REQUEST################################*/
                headers = new List<string[]>()
                {
                    new string[] {"id", "f786c43f-f8bb-4336-bad0-5a7b304365e1"},
                    new string[] {"LastName", "Scherbenok"},
                    new string[] {"FirstName", "Andrei"},
                    new string[] {"NumberPhone", "375337561513"},
                };
                Console.WriteLine("log>\tSending a valid PUT request with the id, LastName, FirstName, NumberPhone" +
                                  " parameters...\n" +
                                  "\tURL: {0}", RequestUrl);
                foreach (var h in headers)
                {
                    Console.WriteLine("\t{0}: {1}", h[0].ToLower(), h[1]);
                }
                result = await client.SendAsync(CreateRequestMessage(RequestUrl, "PUT", headers));
                WriteHttpResponseInfo(result, Testing(result.StatusCode, 200) ? "Request success!" : "Request Error!");
                headers.Clear();

                /*################################VALID DELETE REQUEST################################*/
                headers = new List<string[]>()
                {
                    new string[] {"id", "f786c43f-f8bb-4336-bad0-5a7b304365e1"},
                };
                Console.WriteLine("log>\tSending a valid DELETE request with the id parameter...\n" +
                                  "\tURL: {0}", RequestUrl);
                foreach (var h in headers)
                {
                    Console.WriteLine("\t{0}: {1}", h[0].ToLower(), h[1]);
                }

                result = await client.SendAsync(CreateRequestMessage(RequestUrl, "DELETE", headers));
                WriteHttpResponseInfo(result, Testing(result.StatusCode, 200) ? "Request success!" : "Request Error!");
            }
            catch (Exception ex)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    System.Diagnostics.Debugger.Break();
                }
                Console.WriteLine(ex.Message.ToString());
            }
        }
    }

}
