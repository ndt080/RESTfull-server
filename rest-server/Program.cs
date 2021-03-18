using System;
using System.Net;
using System.Threading.Tasks;
using rest_server.Models;
using rest_server.Services;

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
                Console.WriteLine("log> Request #: {0}", ++_requestCount);
                Console.WriteLine("     Time: {0}",DateTime.Now);
                Console.WriteLine("     {0}",req.Url.ToString());
                Console.WriteLine("     Method: {0}",req.HttpMethod);
                Console.WriteLine("     HostName: {0}",req.UserHostName);
                Console.WriteLine("     {0} \n",req.UserAgent);
                
                Api<Contacts> contacts = new Api<Contacts>(ctx, "/contacts");
                ContactsService service = new ContactsService();
                
                switch (ctx.Request.HttpMethod)
                {
                    case "GET":
                        await contacts.Get(service, "id");
                        break;
                    case "POST":
                        if (req.Url.AbsolutePath == "/shutdown")
                        {
                            Console.WriteLine("Shutdown requested");
                            _runServer = false;
                        }
                        await contacts.Post(service, "lastname", "firstname","numberphone");
                        break;
                    case "PUT":
                        await contacts.Put(service, "id","lastname", "firstname","numberphone");
                        break;
                    case "DELETE":
                        await contacts.Delete(service, "id");
                        break;
                }
            }
        }

        public static void Main(string[] args)
        {
            ContactsService.Init();
            int tasksCount = 32; 
            _listener = new HttpListener();
            _listener.Prefixes.Add(Url);
            _listener.Start();
            
            Console.WriteLine("Listening connections {0}", Url);
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