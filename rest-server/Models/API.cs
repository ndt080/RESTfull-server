using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using rest_server.Controllers;

namespace rest_server.Models
{
    public class API<TClass>
    {
        private static HttpListenerContext Context { get; set; } 
        private String AbsolutePath { get; set; }

        public API(HttpListenerContext context, String url)
        {
            Context = context;
            AbsolutePath = url.ToLower();
        }
        
        public async Task Get(IBaseController<string, TClass> controller)
        {
            var url = Context.Request.Url.AbsolutePath.ToLower();
            byte[] data = { };
            byte[] error = { };
            if (url == AbsolutePath)
            {
                if (!string.IsNullOrEmpty(Context.Request.Headers["id"]))
                {
                    var obj = controller.Get(Context.Request.Headers["id"]);
                    
                    if (obj != null)
                    {
                        data = await JsonSerialization(obj);
                        await SendResponse(data, "application/json", HttpStatusCode.OK);  
                    }
                    else
                    {
                        await SendResponse(error, "application/json", HttpStatusCode.BadRequest);  
                    }
                }
                else
                { 
                    data = await JsonSerialization(controller.GetAll());
                    await SendResponse(data, "application/json", HttpStatusCode.OK);
                }
            }
            else
            {
                await SendResponse(error, "application/json", HttpStatusCode.NotFound);
            }
            
        }

        public async Task Post(IBaseController<string, TClass> controller)
        {
            var url = Context.Request.Url.AbsolutePath.ToLower();
            byte[] error = { };
            byte[] data = { };
            if (url == AbsolutePath)
            {
                if (!string.IsNullOrEmpty(Context.Request.Headers["lastname"]) && 
                    !string.IsNullOrEmpty(Context.Request.Headers["firstName"]) &&
                    !string.IsNullOrEmpty(Context.Request.Headers["numberPhone"]))
                {
                    controller.Save(
                        new [] {
                            Context.Request.Headers["lastname"], 
                            Context.Request.Headers["firstName"], 
                            Context.Request.Headers["numberPhone"]
                        }
                    );
                    await SendResponse(data, "application/json", HttpStatusCode.OK);
                }
                else
                {
                    await SendResponse(error, "application/json", HttpStatusCode.BadRequest);
                }
            }
            else
            {
                await SendResponse(error, "application/json", HttpStatusCode.NotFound);
            }
        }

        public async Task Put(IBaseController<string, TClass> controller)
        {
            var url = Context.Request.Url.AbsolutePath.ToLower();
            byte[] error = { };
            byte[] data = { };
            if (url == AbsolutePath)
            {
                if (!string.IsNullOrEmpty(Context.Request.Headers["id"]) && 
                    !string.IsNullOrEmpty(Context.Request.Headers["lastname"]) && 
                    !string.IsNullOrEmpty(Context.Request.Headers["firstName"]) &&
                    !string.IsNullOrEmpty(Context.Request.Headers["numberPhone"]))
                {
                    controller.Update(
                        Context.Request.Headers["id"],
                        new[] {
                            Context.Request.Headers["lastname"], 
                            Context.Request.Headers["firstName"], 
                            Context.Request.Headers["numberPhone"]
                        }
                    );
                    await SendResponse(data, "application/json", HttpStatusCode.OK);
                }
                else
                {
                    await SendResponse(error, "application/json", HttpStatusCode.BadRequest);
                }
            }
            else
            {
                await SendResponse(error, "application/json", HttpStatusCode.NotFound);
            }
        }

        public async Task Delete(IBaseController<string, TClass> controller)
        {
            var url = Context.Request.Url.AbsolutePath.ToLower();
            byte[] error = { };
            byte[] data = { };
            if (url == AbsolutePath)
            {

                if (!string.IsNullOrEmpty(Context.Request.Headers["id"]))
                {
                    controller.Delete(Context.Request.Headers["id"]);
                    await SendResponse(data, "application/json", HttpStatusCode.OK);
                }
                else
                {
                    await SendResponse(error, "application/json", HttpStatusCode.BadRequest);
                }
            }
            else
            {
                await SendResponse(error, "application/json", HttpStatusCode.NotFound);
            }
        }

        public static async Task SendResponse(byte[] data, string contentType, HttpStatusCode status)
        {
            var response = Context.Response;
            response.StatusCode = (int)status;   
            response.ContentType = contentType;
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = data.LongLength;
            await response.OutputStream.WriteAsync(data, 0, data.Length);
        }
        private static async Task<byte[]> JsonSerialization(Object obj)
        {
           var json = JsonConvert.SerializeObject(obj);
           return await Task.Run(() => Encoding.UTF8.GetBytes(json));
        }
    }
}