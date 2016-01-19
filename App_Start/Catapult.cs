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
            var reader = new AppSettingsReader();
            var catapult = Client.GetInstance(reader.GetValue("userId", typeof (string)) as string,
                reader.GetValue("apiToken", typeof (string)) as string,
                reader.GetValue("apiSecret", typeof (string)) as string);
            var phoneNumber = reader.GetValue("phoneNumberForCallbacks", typeof (string)) as string;
            app.Use(async (context, next) =>
            {
                var builder = new UriBuilder(context.Request.Uri);
                builder.Path = "";
                builder.Query = "";
                var baseUrl = builder.ToString();

                if (string.IsNullOrEmpty(phoneNumber))
                {
                    //reserve a phone numbers for callbacks if need
                    var config = WebConfigurationManager.OpenWebConfiguration("~");
                    var numbers = await AvailableNumber.SearchLocal(catapult, new Dictionary<string, object> { { "state", "NC" }, { "city", "Cary" }, { "quantity", 1 } });
                    phoneNumber = numbers.First().Number;
                    await PhoneNumber.Create(new Dictionary<string, object> { { "number", phoneNumber } });
                    config.AppSettings.Settings["phoneNumberForCallbacks"].Value = phoneNumber;
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