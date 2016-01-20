using System.Configuration;
using Bandwidth.Net;
using Owin;
using System.Web.Configuration;
using Bandwidth.Net.Model;
using System.Collections.Generic;
using System.Linq;
using System;

namespace CallApp
{
    public static class Catapult
    {
        //Initialze Catapult specific data
        public static void Configure(IAppBuilder app)
        {
            var appSettings = ConfigurationManager.AppSettings;
            var catapult = Client.GetInstance(appSettings["userId"],
                appSettings["apiToken"],
                appSettings["apiSecret"]);
            var phoneNumber = appSettings["phoneNumberForCallbacks"];
            app.Use(async (context, next) =>
            {
                var builder = new UriBuilder(context.Request.Uri);
                builder.Path = "/";
                builder.Query = "";
                var baseUrl = builder.ToString();
                baseUrl = baseUrl.Substring(0, baseUrl.Length - 1); //remove last '/'

                if (string.IsNullOrEmpty(phoneNumber))
                {
                    //reserve a phone numbers for callbacks if need
                    var numbers = await AvailableNumber.SearchLocal(catapult, new Dictionary<string, object> { { "state", "NC" }, { "city", "Cary" }, { "quantity", 1 } });
                    phoneNumber = numbers.First().Number;
                    await PhoneNumber.Create(catapult, new Dictionary<string, object> { { "number", phoneNumber } });
                    var config = WebConfigurationManager.OpenWebConfiguration("~");
                    config.AppSettings.Settings.Add("phoneNumberForCallbacks",  phoneNumber);
                    config.Save(ConfigurationSaveMode.Minimal, false);
                    ConfigurationManager.RefreshSection("appSettings");
                }
                
                // make them alailvable from any request handlers
                context.Set("phoneNumberForCallbacks", phoneNumber);
                context.Set("catapultClient", catapult);
                context.Set("baseUrl", baseUrl);
                await next();
            });

        }

        public class CatapultState
        {
            public string PhoneNumberForCallbacks { get; set; }
        }
    }
}