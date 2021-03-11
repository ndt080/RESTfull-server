using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using rest_server.Controllers;

namespace rest_server.Models
{
    public class API
    {
        private static HttpListenerContext Context { get; set; } 
        private String AbsolutePath { get; set; }

        public API(HttpListenerContext context, String url)
        {
            API.Context = context;
            this.AbsolutePath = url.ToLower();
        }
        
        public async void GET()
        {
            var url = Context.Request.Url.AbsolutePath.ToLower();
            byte[] data = new byte[] { };
            byte[] error = new byte[] { };
            if (url == AbsolutePath)
            {
                if (!string.IsNullOrEmpty(Context.Request.Headers["id"]))
                {
                    var obj = APIController.GetByID(Context.Request.Headers["id"]);
                    if (obj != null)
                    {
                        data = await JsonSerialization(obj);
                        await OutputData(data, "application/json", HttpStatusCode.OK);  
                    }
                    else
                    {
                        await OutputData(error, "application/json", HttpStatusCode.BadRequest);  
                    }
                }
                else
                { 
                    data = await JsonSerialization(APIController.Get());
                    await OutputData(data, "application/json", HttpStatusCode.OK);
                }
            }
            else
            {
                await OutputData(error, "application/json", HttpStatusCode.NotFound);
            }
            
        }

        public async void POST()
        {
            var url = Context.Request.Url.AbsolutePath.ToLower();
            byte[] error = new byte[] { };
            byte[] data = new byte[] { };
            if (url == AbsolutePath)
            {
                if (!string.IsNullOrEmpty(Context.Request.Headers["lastname"]) && 
                    !string.IsNullOrEmpty(Context.Request.Headers["firstName"]) &&
                    !string.IsNullOrEmpty(Context.Request.Headers["numberPhone"]))
                {
                    APIController.Add(
                        Context.Request.Headers["lastname"], 
                        Context.Request.Headers["firstName"], 
                        Context.Request.Headers["numberPhone"]
                        );
                    await OutputData(data, "application/json", HttpStatusCode.OK);
                }
                else
                {
                    await OutputData(error, "application/json", HttpStatusCode.BadRequest);
                }
            }
            else
            {
                await OutputData(error, "application/json", HttpStatusCode.NotFound);
            }
        }

        public async void PUT()
        {
            var url = Context.Request.Url.AbsolutePath.ToLower();
            byte[] error = new byte[] { };
            byte[] data = new byte[] { };
            if (url == AbsolutePath)
            {
                if (!string.IsNullOrEmpty(Context.Request.Headers["id"]) && 
                    !string.IsNullOrEmpty(Context.Request.Headers["lastname"]) && 
                    !string.IsNullOrEmpty(Context.Request.Headers["firstName"]) &&
                    !string.IsNullOrEmpty(Context.Request.Headers["numberPhone"]))
                {
                    APIController.Update(
                        Context.Request.Headers["id"],
                        Context.Request.Headers["lastname"], 
                        Context.Request.Headers["firstName"], 
                        Context.Request.Headers["numberPhone"]
                    );
                    await OutputData(data, "application/json", HttpStatusCode.OK);
                }
                else
                {
                    await OutputData(error, "application/json", HttpStatusCode.BadRequest);
                }
            }
            else
            {
                await OutputData(error, "application/json", HttpStatusCode.NotFound);
            }
        }

        public async void DELETE()
        {
            var url = Context.Request.Url.AbsolutePath.ToLower();
            byte[] error = new byte[] { };
            byte[] data = new byte[] { };
            if (url == AbsolutePath)
            {

                if (!string.IsNullOrEmpty(Context.Request.Headers["id"]))
                {
                    APIController.Remove(Context.Request.Headers["id"]);
                    await OutputData(data, "application/json", HttpStatusCode.OK);
                }
                else
                {
                    await OutputData(error, "application/json", HttpStatusCode.BadRequest);
                }
            }
            else
            {
                await OutputData(error, "application/json", HttpStatusCode.NotFound);
            }
        }

        private static async Task OutputData(byte[] data, string contentType, HttpStatusCode status)
        {
            var request = Context.Request;         
            var response = Context.Response;
            
            response.StatusCode = (int)status;   
            response.ContentType = contentType;
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = data.LongLength;
            await response.OutputStream.WriteAsync(data, 0, data.Length);
            
            Console.WriteLine("log> {0} запрос был пойман: {1}",
                request.HttpMethod, request.Url);
            Console.WriteLine("     Статус: {0}", response.StatusCode);
        }
        private static async Task<byte[]> JsonSerialization(Object obj)
        {
           var json = JsonConvert.SerializeObject(obj);
           return await Task.Run(() => Encoding.UTF8.GetBytes(json));
        }
    }
}