using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using rest_server.Models;

namespace rest_server.Controllers
{
    public class ContactsController: IBaseController<string, Contacts>
    {
        private static readonly ConcurrentDictionary<string, Contacts> ContactsData =
            new ConcurrentDictionary<string, Contacts>();

        public static void Init()
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

        Contacts IBaseController<string, Contacts>.Get(string guid)
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

        ConcurrentDictionary<string, Contacts> IBaseController<string,Contacts>.GetAll()
        {
           return ContactsData;
        }

        void IBaseController<string,Contacts>.Save(string[] param)
        {
            ContactsData.TryAdd(
                Guid.NewGuid().ToString(),
                new Contacts()
                {
                    LastName = param[0],
                    FirstName = param[1],
                    NumberPhone = param[2],
                }
            );
        }

        void IBaseController<string,Contacts>.Update(string guid, string[] param)
        {
            try
            {
                ContactsData[guid].LastName = param[0];
                ContactsData[guid].FirstName = param[1];
                ContactsData[guid].NumberPhone = param[2];
            }
            catch (Exception)
            {
                ContactsData.TryAdd(
                    guid,
                    new Contacts()
                    {
                        LastName = param[0],
                        FirstName = param[1],
                        NumberPhone = param[2],
                    }
                );
            }
        }
        
        void IBaseController<string,Contacts>.Delete(string guid)
        {
            ContactsData.TryRemove(guid, out _);
        }
    }
}