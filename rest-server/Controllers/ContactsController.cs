using System;
using System.Collections.Generic;
using System.Linq;
using rest_server.Models;

namespace rest_server.Controllers
{
    public class ContactsController
    {
        private static readonly List<Contacts> ContactsData = new List<Contacts>
        {
            new Contacts()
            {
                ID = "155e4b37-2cc1-40bd-ac27-a028dbe6d30f",
                LastName = "Petrov",
                FirstName = "Andrei",
                NumberPhone = 375257705629,
            }, 
            new Contacts()
            {
                ID = "f786c43f-f8bb-4336-bad0-5a7b304365e1",
                LastName = "Ivanov",
                FirstName = "Ivan",
                NumberPhone = 375257705629,
            }, 
            new Contacts()
            {
                ID = "c8c09b2b-0398-4c2e-800e-effe1dd9c83f",
                LastName = "Parhomenko",
                FirstName = "Vladimir",
                NumberPhone = 375257705629,
            }, 
            new Contacts()
            {
                ID = "7d30f95d-71fd-40e3-8f83-6431e7c5c2c9",
                LastName = "Pupkin",
                FirstName = "Vasya",
                NumberPhone = 375257705629,
            }, 
            
        };
        
        public static List<Contacts> Get()
        {
            return ContactsData;
        }
        
        public static Contacts GetByID(string guid)
        {
            try
            {
                var ind = ContactsData.FindIndex(c => c.ID == guid);
                return ContactsData[ind];
            }
            catch (Exception e)
            {
                return null;
            }

        }
                
        public static void Add(string lastName, string firstName, long numberPhone)
        {
            ContactsData.Add(
                new Contacts()
                {
                    ID = Guid.NewGuid().ToString(),
                    LastName = lastName,
                    FirstName = firstName,
                    NumberPhone = numberPhone,
                }
            );
        }
        public static void Remove(string guid)
        {
            ContactsData.RemoveAll((x) => x.ID == guid);
        }
        
        
        
        
        
        
       
        private static int GetValue(string value)
        {
            for (int i = 0; i < ContactsData.Count; i++)
            {
                if (ContactsData[i].ID == value)
                    return i;
            }
            return -1;
        }
    }
}