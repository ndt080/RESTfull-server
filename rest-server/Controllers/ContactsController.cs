using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using rest_server.Models;

namespace rest_server.Controllers
{
    public static class ContactsController
    {
        private static readonly ConcurrentDictionary<String, Contacts> ContactsData =
            new ConcurrentDictionary<String, Contacts>();
        
        public static void InitTemplate()
        {
            ContactsData.TryAdd(
                "155e4b37-2cc1-40bd-ac27-a028dbe6d30f",
                new Contacts()
                {
                    LastName = "Petrov",
                    FirstName = "Andrei",
                    NumberPhone = "375257705629",
                }
            );
            ContactsData.TryAdd(
                "f786c43f-f8bb-4336-bad0-5a7b304365e1",
                new Contacts()
                {
                    LastName = "Pupkin",
                    FirstName = "Vasya",
                    NumberPhone = "375257705629",
                }
            );
            ContactsData.TryAdd(
                "c8c09b2b-0398-4c2e-800e-effe1dd9c83f",
                new Contacts()
                {
                    LastName = "Parhomenko",
                    FirstName = "Vladimir",
                    NumberPhone = "375257705629",
                }
            );
            ContactsData.TryAdd(
                "7d30f95d-71fd-40e3-8f83-6431e7c5c2c9",
                new Contacts()
                {
                    LastName = "Ivan",
                    FirstName = "Ivanov",
                    NumberPhone = "375257705629",
                }
            );
            
        }
        public static ConcurrentDictionary<String, Contacts> Get()
        {
            return ContactsData;
        }
        
        public static Contacts GetByID(string guid)
        {
            try
            {
                return ContactsData[guid];
            }
            catch (Exception)
            {
                return null;
            }
        }
                
        public static void Add(string lastName, string firstName, string numberPhone)
        {
            ContactsData.TryAdd(
                Guid.NewGuid().ToString(),
                new Contacts()
                {
                    LastName = lastName,
                    FirstName = firstName,
                    NumberPhone = numberPhone,
                }
            );
        }
        public static void Remove(string guid)
        {
            ContactsData.TryRemove(guid, out _);
        }

        public static void Update(string guid, string lastName, string firstName, string numberPhone)
        {
            try
            {
                ContactsData[guid].LastName = lastName;
                ContactsData[guid].FirstName = firstName;
                ContactsData[guid].NumberPhone = numberPhone;
            }
            catch (Exception)
            {
                ContactsData.TryAdd(
                    guid,
                    new Contacts()
                    {
                        LastName = lastName,
                        FirstName = firstName,
                        NumberPhone = numberPhone,
                    }
                );
            }
        } 

    }
}