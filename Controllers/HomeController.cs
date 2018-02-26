using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Bandwidth.Net;
using Bandwidth.Net.Api;
using CallApp.Models;
using Microsoft.AspNet.Identity.Owin;

namespace CallApp.Controllers
{
  public class HomeController : Controller
  {
    private string _baseUrl;

    private Client _client;

    private CallAppDbContext _dbContext;

    private string _phoneNumberForCallbacks;

    public CallAppDbContext DbContext
    {
      get => _dbContext ?? Request.GetOwinContext().Get<CallAppDbContext>();
      set => _dbContext = value;
    }

    public Client Client
    {
      get => _client ?? Request.GetOwinContext().Get<Client>("catapultClient");
      set => _client = value;
    }

    public string PhoneNumberForCallbacks
    {
      get => _phoneNumberForCallbacks ?? Request.GetOwinContext().Get<string>("phoneNumberForCallbacks");
      set => _phoneNumberForCallbacks = value;
    }

    public string BaseUrl
    {
      get => _baseUrl ?? Request.GetOwinContext().Get<string>("baseUrl");
      set => _baseUrl = value;
    }

    // GET / (and /Home)
    public ActionResult Index()
    {
      ViewBag.UserNumbers = DbContext.Numbers.ToArray();
      return View(DbContext.Contacts.ToArray());
    }

    //POST /Home/Call
    [HttpPost]
    [ActionName("Call")]
    public async Task<ActionResult> PostCall(MakeCall call)
    {
      try
      {
        if (string.IsNullOrEmpty(call.From)) throw new ArgumentException("Field From is missing");
        if (string.IsNullOrEmpty(call.To)) throw new ArgumentException("Field To is missing");

        //save 'From' in db if need
        if (!DbContext.Numbers.Any(n => n.PhoneNumber == call.From))
        {
          DbContext.Numbers.Add(new Number {PhoneNumber = call.From});
          await DbContext.SaveChangesAsync();
        }

        //make a call
        var cId = await Client.Call.CreateAsync(new CreateCallData
        {
          From = PhoneNumberForCallbacks,
          To = call.From,
          CallbackUrl = BaseUrl + Url.Action("CatapultFromCallback"),
          Tag = call.To
        });
        Debug.WriteLine("Call Id to {0} is {1}", call.From, cId);
        return Json(new object());
      }
      catch (Exception ex)
      {
        return Json(new {error = ex.Message});
      }
    }

    //POST /Home/Contact
    [HttpPost]
    [ActionName("Contact")]
    public async Task<ActionResult> PostContact(Contact contact)
    {
      try
      {
        var item = DbContext.Contacts.Add(contact);
        await DbContext.SaveChangesAsync();
        return Json(item);
      }
      catch (DbEntityValidationException e)
      {
        var builder = new StringBuilder();
        foreach (var eve in e.EntityValidationErrors)
        foreach (var ve in eve.ValidationErrors)
        {
          builder.AppendFormat("Property: \"{0}\", Error: \"{1}\"",
            ve.PropertyName, ve.ErrorMessage);
          builder.AppendLine();
        }
        return Json(new {error = builder.ToString()});
      }
      catch (Exception ex)
      {
        while (ex.InnerException != null)
          ex = ex.InnerException; //last exception contains usefull info
        return Json(new {error = ex.Message});
      }
    }


    //POST /Home/CatapultFromCallback (it's used for Catapult events for call to "from" number)
    [HttpPost]
    [ActionName("CatapultFromCallback")]
    public async Task<ActionResult> PostCatapultFromCallback()
    {
      try
      {
        using (var reader = new StreamReader(Request.InputStream, Encoding.UTF8))
        {
          var json = await reader.ReadToEndAsync();
          Debug.WriteLine("CatapultFromCallback:" + json);
          var ev = CallbackEvent.CreateFromJson(json);
          switch (ev.EventType)
          {
            case CallbackEventType.Answer:
            {
              var contact = DbContext.Contacts.FirstOrDefault(c => c.PhoneNumber == ev.Tag);
              if (contact == null)
                throw new Exception("Missing contact for number " + ev.Tag);
              await Client.Call.SpeakSentenceAsync(ev.CallId,
                string.Format("We are connecting you to {0}", contact.Name), Gender.Female, "susan", "en_US", ev.Tag);
              break;
            }
            case CallbackEventType.Speak:
            {
              if (ev.Status != CallbackEventStatus.Done) break;
              //a messages was spoken to "from" number. calling to "to" number
              var cId = await Client.Call.CreateAsync(new CreateCallData
              {
                From = PhoneNumberForCallbacks,
                To = ev.Tag,
                CallbackUrl = BaseUrl + Url.Action("CatapultToCallback"),
                Tag = ev.CallId
              });
              Debug.WriteLine("Call Id to {0} is {1}", ev.Tag, cId);
              break;
            }
            case CallbackEventType.Hangup:
            {
              await HandleHangup(ev);
              break;
            }
          }
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine(string.Format("CatapultFromCallback error: {0}", ex.Message));
      }
      return Json(new object());
    }

    private async Task HandleHangup(CallbackEvent ev)
    {
      var activeCalls = HttpContext.Application["ActiveCalls"] as Dictionary<string, string> ??
                        new Dictionary<string, string>();
      //hang up another leg
      string anotherCallId;
      if (activeCalls.TryGetValue(ev.CallId, out anotherCallId))
      {
        activeCalls.Remove(ev.CallId);
        activeCalls.Remove(anotherCallId);
        Debug.WriteLine("Hang up another call of bridge (id {0})", anotherCallId);
        try
        {
          await Client.Call.HangupAsync(anotherCallId);
        }
        catch (Exception ex)
        {
          Debug.WriteLine("Error on hang up another call (id {0}) of the bridge: {1}", anotherCallId,
            ex.Message);
        }
      }
    }


    //POST /Home/CatapultToCallback (it's used for Catapult events for call to "to" number)
    [HttpPost]
    [ActionName("CatapultToCallback")]
    public async Task<ActionResult> PostCatapultToCallback()
    {
      try
      {
        using (var reader = new StreamReader(Request.InputStream, Encoding.UTF8))
        {
          var json = await reader.ReadToEndAsync();
          Debug.WriteLine("PostCatapultToCallback:" + json);
          var ev = CallbackEvent.CreateFromJson(json);
          switch (ev.EventType)
          {
            case CallbackEventType.Answer:
            {
              //"to" number answered a call. Making a bridge with "from" number's call
              var bId = await Client.Bridge.CreateAsync(new CreateBridgeData
              {
                BridgeAudio = true,
                CallIds = new[] {ev.CallId, ev.Tag}
              });
              var activeCalls = HttpContext.Application["ActiveCalls"] as Dictionary<string, string> ??
                                new Dictionary<string, string>();
              activeCalls.Add(ev.CallId, ev.Tag);
              activeCalls.Add(ev.Tag, ev.CallId);
              HttpContext.Application["ActiveCalls"] = activeCalls;
              Debug.WriteLine(string.Format("BridgeId is {0}", bId));
              break;
            }
            case CallbackEventType.Hangup:
            {
              await HandleHangup(ev);
              break;
            }
          }
          return Json(new object());
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine(string.Format("CatapultToCallback error: {0}", ex.Message));
      }
      return Json(new object());
    }
  }
}