using System.Collections.Generic;

namespace rest_server.Models
{
    public class Contacts
    {
        public string LastName { get; set; }       
        public string FirstName { get; set; }
        public string NumberPhone { get; set; }
        

        public static List<Contacts> List { get; }
    }
}