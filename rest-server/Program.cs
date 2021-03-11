using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
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

                
                API contacts = new API(ctx, "/contacts");
                switch (ctx.Request.HttpMethod)
                {
                    case "GET":
                        contacts.GET();
                        break;
                    case "POST":
                        if (req.Url.AbsolutePath == "/shutdown")
                        {
                            Console.WriteLine("Shutdown requested");
                            _runServer = false;
                        }
                        contacts.POST();
                        break;
                    case "PUT":
                        contacts.PUT();
                        break;
                    case "DELETE":
                        contacts.DELETE();
                        break;
                }
            }
        }
    

        public static void Main(string[] args)
        {
            ContactsController.InitTemplate();
            int tasksCount = 32; 
            _listener = new HttpListener();
            _listener.Prefixes.Add(Url);
            _listener.Start();
            
            Console.WriteLine("Прослушивание подключений {0}", Url);

            Task[] tasksPool = new Task[tasksCount];

            for (int i = 0; i < tasksCount; i++)
            {
                tasksPool[i] = HandleIncomingConnections();
                tasksPool[i].GetAwaiter().GetResult();
            }

            _listener.Close();
        }
    }
}