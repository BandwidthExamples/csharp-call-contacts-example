using System.Configuration;
using Bandwidth.Net;
using Owin;

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
            var baseUrl = reader.GetValue("baseUrl", typeof(string)) as string;
            app.Use((context, next) =>
            {
                // make them alailvable from any request handlers
                context.Set("phoneNumberForCallbacks", phoneNumber);
                context.Set("catapultClient", catapult);
                context.Set("baseUrl", baseUrl);
                return next();
            });
        }
    }
}