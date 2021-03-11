using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using rest_server.Controllers;
using rest_server.Models;

namespace rest_server.Controllers
{
    public static class APIController
    {
        public static ConcurrentDictionary<string, Contacts> Get()
        {
            return ContactsController.Get();
        }
        
        public static Contacts GetByID(string guid)
        {
            return ContactsController.GetByID(guid);
        }
                
        public static void Add(string lastName, string firstName, string numberPhone)
        {
            ContactsController.Add(lastName, firstName, numberPhone);
        }
        
        public static void Remove(string guid)
        {
            ContactsController.Remove(guid);
        }
        public static void Update(string guid, string lastName, string firstName, string numberPhone)
        {
            ContactsController.Update(guid, lastName, firstName, numberPhone);
        }
    }
}