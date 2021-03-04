using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using rest_server.Controllers;
using rest_server.Models;

namespace rest_server {
    static class HttpServer {
        private static HttpListener _listener;
        private const string Url = "http://localhost:5050/";
        private static int _requestCount = 0;
        private static bool _runServer = true;
        private static string pageData = 
            "<!DOCTYPE>" +
            "<html>" +
            "  <head>" +
            "    <meta charset=\"utf-8\">" +
            "    <title>HttpServer</title>" +
            "  </head>" +
            "  <body>" +
            "    <form method=\"post\" action=\"shutdown\">" +
            "      <input type=\"submit\" value=\"Завершить работу сервера\" {0}>" +
            "    </form>" +
            "  </body>" +
            "</html>";
        private static async void READ(HttpListenerContext context)
        {
            if (context.Request.Url.AbsolutePath == "/contacts")
            {
                var json = "";
                if (context.Request.Headers["id"] != null)
                {
                    var obj = ContactsController.GetByID(context.Request.Headers["id"]);
                    json = JsonConvert.SerializeObject(
                        obj != null ? (object) obj : "[{ error: 'incorrect id'}]");
                    
                }
                else
                {
                    json = JsonConvert.SerializeObject(ContactsController.Get());
                }
                byte[] data = Encoding.UTF8.GetBytes(json);
                OutputData(context, data, "application/json");
            } else {
                string disableSubmit = !_runServer ? "disabled" : "";
                byte[] data = Encoding.UTF8.GetBytes(String.Format(pageData, disableSubmit));
                OutputData(context, data, "text/html");
            }
            
        }
        private static void CREATE(HttpListenerContext context)
        {

        }
        private static void UPDATE(HttpListenerContext context)
        {
            
        }
        private static void DELETE(HttpListenerContext context)
        {
            if (context.Request.Url.AbsolutePath == "/contacts")
            {
                if (context.Request.Headers["id"] != null)
                {
                   ContactsController.Remove(context.Request.Headers["id"]);
                   var json = JsonConvert.SerializeObject("[{ action: 'success'}]");
                   byte[] data = Encoding.UTF8.GetBytes(json);
                   OutputData(context, data, "application/json");
                }
            } else {
                string disableSubmit = !_runServer ? "disabled" : "";
                byte[] data = Encoding.UTF8.GetBytes(String.Format(pageData, disableSubmit));
                OutputData(context, data, "text/html");
            }
        }

        private static async void OutputData( HttpListenerContext ctx, byte[] data, string contentType)
        {
            HttpListenerRequest req = ctx.Request;         
            HttpListenerResponse resp = ctx.Response;
            
            //Создаем ответ
            Stream inputStream = req.InputStream;
            Encoding encoding = req.ContentEncoding;
            StreamReader reader = new StreamReader(inputStream, encoding);
            var requestBody = await reader.ReadToEndAsync();


            Console.WriteLine("log> {0} запрос был пойман: {1}",
                ctx.Request.HttpMethod, ctx.Request.Url);
            Console.WriteLine("     Статус: {0}", resp.StatusCode);
            
            //Возвращаем ответ
            resp.StatusCode = (int)HttpStatusCode.OK;   
            resp.ContentType = contentType;
            resp.ContentEncoding = Encoding.UTF8;
            resp.ContentLength64 = data.LongLength;
            await resp.OutputStream.WriteAsync(data, 0, data.Length);
        }
        private static async Task HandleIncomingConnections()
        {
            while (_runServer)
            {
                HttpListenerContext ctx = await _listener.GetContextAsync();

                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;
                resp.AppendHeader("Access-Control-Allow-Origin", "*");
                // Выводим сведения о запросе
                Console.WriteLine("log> Запрос #: {0}", ++_requestCount);
                Console.WriteLine("     Время: {0}",DateTime.Now);
                Console.WriteLine("     {0}",req.Url.ToString());
                Console.WriteLine("     Метод: {0}",req.HttpMethod);
                Console.WriteLine("     HostName: {0}",req.UserHostName);
                Console.WriteLine("     {0} \n",req.UserAgent);

                
                switch (ctx.Request.HttpMethod)
                {
                    case "GET":
                        READ(ctx);
                        break;
                    case "POST":
                        if (req.Url.AbsolutePath == "/shutdown")
                        {
                            Console.WriteLine("Shutdown requested");
                            _runServer = false;
                        }
                        CREATE(ctx);
                        break;
                    case "PUT":
                        UPDATE(ctx);
                        break;
                    case "DELETE":
                        DELETE(ctx);
                        break;
                }
            }
        }
    

        public static void Main(string[] args)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(Url);
            _listener.Start();
            
            Console.WriteLine("Прослушивание подключений {0}", Url);

            // Handle requests
            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();
            
            _listener.Close();
        }
    }
}