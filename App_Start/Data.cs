using System;
using System.Collections.Generic;
using System.Linq;
using CallApp.Models;
using Owin;
using System.Web.Configuration;
using System.Configuration;

namespace CallApp
{
    
    public class DataProvider: IDisposable
    {
        //replace phone numbers here with valid values (which can receive incoming call)
        //add or remove any records you need
        private static readonly Contact[] Contacts =  new[]
        {
            new Contact {Name = "Timon Cardenas",  PhoneNumber = "+19196947781"},
            new Contact {Name = "Brent Lucas",  PhoneNumber = "+12028883929"},
            new Contact {Name = "Rashad Wilcox", PhoneNumber = "+18662161172"},
            new Contact {Name = "Keegan Erickson",  PhoneNumber = "+13522569207"},
            new Contact {Name = "Dustin Figueroa",  PhoneNumber = "+18768393387"}
        };

        //replace phone numbers here with own numbers (which can receive incoming call too)
        private static readonly Number[] UserNumbers = new[]
        {
            new Number{PhoneNumber = "+12522569207", Type = "Primary Number"},
            new Number{PhoneNumber = "+19196943243", Type = "Cell Number"},
            new Number{PhoneNumber = "+12522569209", Type = "Home Number"}
        };
       
        public IList<Contact> GetContacts()
        {
            var contacts = ConfigurationManager.AppSettings["contacts"];
            if(string.IsNullOrWhiteSpace(contacts))
            {
                return (from c in contacts.Split(';')
                        let values = c.Split(':')
                        select new Contact
                        {
                            Name = values[0].Trim(),
                            PhoneNumber = values[1].Trim()
                        }).ToList();
            }
            return Contacts;
        }

        public IList<Number> GetUserNumbers()
        {
            var numbers = ConfigurationManager.AppSettings["userNumbers"];
            if (numbers != null && !string.IsNullOrWhiteSpace(numbers))
            {
                return (from c in numbers.Split(';')
                        let values = c.Split(':')
                        select new Number
                        {
                            Type = values[0].Trim(),
                            PhoneNumber = values[1].Trim()
                        }).ToList();
            }
            return UserNumbers;
        }

        public Contact FindContactByPhoneNumber(string phoneNumber)
        {
            return Contacts.FirstOrDefault(c => c.PhoneNumber == phoneNumber);
        }

        public void Dispose()
        {
            //free resources. do nothing
        }
    }

    public static class Data
    {
        public static void Load(IAppBuilder app)
        {
            app.CreatePerOwinContext(() => new DataProvider());
        }

    }
}