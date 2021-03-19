using System;
using System.Net;
using System.Threading;
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
                
                ContactsService service = new ContactsService();
                Api<Contacts> contacts = new Api<Contacts>(ctx, service, "/contacts");

                switch (ctx.Request.HttpMethod)
                {
                    case "GET":
                        await contacts.Get("id");
                        break;
                    case "POST":
                        if (req.Url.AbsolutePath == "/shutdown")
                        {
                            Console.WriteLine("Shutdown requested");
                            _runServer = false;
                        }
                        await contacts.Post("lastname", "firstname","numberphone");
                        break;
                    case "PUT":
                        await contacts.Put( "id","lastname", "firstname","numberphone");
                        break;
                    case "DELETE":
                        await contacts.Delete("id");
                        break;
                }
            }
        }

        private static WaitCallback StartListenTask(int tasksCount)
        {
            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();
            return null;
        }

        public static void Main()
        {
            ContactsService.Init();
            const int maxWorkerThreads = 30; 
            const int maxCompletionThreads = 20; 
            _listener = new HttpListener();
            _listener.Prefixes.Add(Url);
            _listener.Start();
            
            if (ThreadPool.SetMaxThreads(maxWorkerThreads, maxCompletionThreads))
            {
                Console.WriteLine("log> \tSet max number threads complete!");
            }
            ThreadPool.GetMaxThreads(out var nWorkerThreads, out var nCompletionThreads);
            Console.WriteLine("log> \tMax number threads: {0} \n" +
                              "\tMax number asynchronous I/O threads: {1}", nWorkerThreads, nCompletionThreads);
            Console.WriteLine("log> \tListening connections {0}", Url);
            
            ThreadPool.QueueUserWorkItem(StartListenTask(maxCompletionThreads));

            _listener.Close();
        }
    }
}