using System;
using System.Collections.Generic;
using System.Linq;
using CallApp.Models;
using Owin;

namespace CallApp
{
    
    public class DataProvider: IDisposable
    {
        //replace phone numbers here with valid values (which can receive incoming call)
        //add or remove any records you need
        private static readonly Contact[] Contacts =  new[]
        {
            new Contact {Name = "Timon Cardenas", Message = "consequat, lectus sit", PhoneNumber = "+19196947781"},
            new Contact {Name = "Brent Lucas", Message = "vel arcu eu", PhoneNumber = "+12028883929"},
            new Contact {Name = "Rashad Wilcox", Message = "too too too", PhoneNumber = "+18662161172"},
            new Contact {Name = "Keegan Erickson", Message = "Cras lorem lorem,", PhoneNumber = "+13522569207"},
            new Contact {Name = "Dustin Figueroa", Message = "nec enim. Nunc", PhoneNumber = "+18768393387"}
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
            return Contacts;
        }

        public IList<Number> GetUserNumbers()
        {
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