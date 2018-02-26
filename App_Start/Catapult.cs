using System;
using System.Configuration;
using System.Linq;
using System.Web.Configuration;
using Bandwidth.Net;
using Bandwidth.Net.Api;
using Owin;

namespace CallApp
{
  public static class Catapult
  {
    //Initialze Catapult specific data
    public static void Configure(IAppBuilder app)
    {
      var appSettings = ConfigurationManager.AppSettings;
      var client = new Client(appSettings["userId"],
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
          var numbers =
            await client.AvailableNumber.SearchLocalAsync(new LocalNumberQuery
            {
              State = "NC",
              City = "Cary",
              Quantity = 1
            });
          phoneNumber = numbers.First().Number;
          await client.PhoneNumber.CreateAsync(new CreatePhoneNumberData {Number = phoneNumber});
          var config = WebConfigurationManager.OpenWebConfiguration("~");
          config.AppSettings.Settings.Add("phoneNumberForCallbacks", phoneNumber);
          config.Save(ConfigurationSaveMode.Minimal, false);
          ConfigurationManager.RefreshSection("appSettings");
        }

        // make them alailvable from any request handlers
        context.Set("phoneNumberForCallbacks", phoneNumber);
        context.Set("catapultClient", client);
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