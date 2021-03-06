using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using rest_server.Controllers;

namespace rest_server.Models
{
    public class Api<TClass>
    {
        private static HttpListenerContext Context { get; set; } 
        private String AbsolutePath { get; set; }
        private IBaseController<string, TClass> Controller { get; set; }

        public Api(HttpListenerContext context,IBaseController<string, TClass> ctrl, string url)
        {
            Context = context;
            Controller = ctrl;
            AbsolutePath = url.ToLower();
        }
        
        public async Task Get<TParam>(params TParam[] param)
        {
            var url = Context.Request.Url.AbsolutePath.ToLower();
            byte[] data = { };
            byte[] error = { };
            if (url == AbsolutePath)
            {
                string id = "";
                if (!IsNullOrEmptyParams(Context, param))
                {
                    foreach (var p in param)
                    {
                        if (p.ToString()?.ToLower() == "id")
                        {
                            id = Context.Request.Headers[p.ToString()];
                        }
                    }
                    var obj = Controller.Get(id);
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
                    data = await JsonSerialization(Controller.GetAll());
                    await SendResponse(data, "application/json", HttpStatusCode.OK);
                }
            }
            else
            {
                await SendResponse(error, "application/json", HttpStatusCode.NotFound);
            }
            
        }

        public async Task Post<TParam>(params TParam[] param)
        {
            var url = Context.Request.Url.AbsolutePath.ToLower();
            byte[] error = { };
            byte[] data = { };
            if (url == AbsolutePath)
            {
                if (!IsNullOrEmptyParams(Context,param))
                {
                    var array = new string[param.Length];
                    for (var i = 0; i < param.Length; i++)
                    {
                        if (param.ToString()?.ToLower() != "id")
                        {
                            array[i] = Context.Request.Headers[param.ToString()];
                        }
                    }
                    Controller.Save(array);
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

        public async Task Put<TParam>(params TParam[] param)
        {
            var url = Context.Request.Url.AbsolutePath.ToLower();
            byte[] error = { };
            byte[] data = { };
            if (url == AbsolutePath)
            {
                if (!IsNullOrEmptyParams(Context,param))
                {
                    var array = new string[param.Length];
                    var id = "";
                    for (var i = 0; i < param.Length; i++)
                    {
                        if (param.ToString()?.ToLower() != "id")
                        {
                            array[i] = Context.Request.Headers[param.ToString()];
                        }
                        else
                        {
                            id = Context.Request.Headers[param.ToString()];
                        }
                    }
                    Controller.Update(id, array);
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

        public async Task Delete<TParam>(params TParam[] param)
        {
            var url = Context.Request.Url.AbsolutePath.ToLower();
            byte[] error = { };
            byte[] data = { };
            if (url == AbsolutePath)
            {
                var id = "";
                if (!IsNullOrEmptyParams(Context, param))
                {
                    foreach (var p in param)
                    {
                        if (p.ToString()?.ToLower() == "id")
                        {
                            id = Context.Request.Headers[p.ToString()];
                        }
                    }
                    Controller.Delete(id);
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

        private static bool IsNullOrEmptyParams<TParams>(HttpListenerContext ctx, IEnumerable<TParams> param)
        {
            return param.Any(p => string.IsNullOrEmpty(ctx.Request.Headers[p.ToString()]));
        }

        private static async Task SendResponse(byte[] data, string contentType, HttpStatusCode status)
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