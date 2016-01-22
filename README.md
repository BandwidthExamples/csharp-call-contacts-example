### csharp-call-contacts-example

[Catapult](http://ap.bandwidth.com/?utm_medium=social&utm_source=github&utm_campaign=dtolb&utm_content=_) Api demo app (creating calls and bridges)


#### Demonstrates
* Use [Catapult SDK](https://github.com/bandwidthcom/csharp-bandwidth)
* [Searching for Phone Number](http://ap.bandwidth.com/docs/rest-api/available-numbers/#resourceGETv1availableNumberslocal/?utm_medium=social&utm_source=github&utm_campaign=dtolb&utm_content=_)
* [Ordering Phone Number](http://ap.bandwidth.com/docs/rest-api/phonenumbers/#resourcePOSTv1usersuserIdphoneNumbers/?utm_medium=social&utm_source=github&utm_campaign=dtolb&utm_content=_)
* [Making calls](http://ap.bandwidth.com/docs/rest-api/calls/#resourcePOSTv1usersuserIdcalls/?utm_medium=social&utm_source=github&utm_campaign=dtolb&utm_content=_)
* [Creating  bridges](http://ap.bandwidth.com/docs/rest-api/bridges/#resourcePOSTv1usersuserIdbridges/?utm_medium=social&utm_source=github&utm_campaign=dtolb&utm_content=_)


## Prerequisites
- Configured Machine with Ngrok/Port Forwarding -OR- Azure Account
  - [Ngrok](https://ngrok.com/)
  - [Azure](https://account.windowsazure.com/Home/Index)
- [Catapult Account](http://ap.bandwidth.com/?utm_medium=social&utm_source=github&utm_campaign=dtolb&utm_content=_)
- [Visual Studio 2015](https://www.visualstudio.com/en-us/downloads/download-visual-studio-vs.aspx)
- [Git](https://git-scm.com/)
- Common Azure Tools for Visual Studio (they are preinstalled with Visual Studio)


## Build and Deploy

### Azure One Click

#### Settings Required To Run
* ```Catapult User Id```
* ```Catapult Api Token```
* ```Catapult Api Secret```

[![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://azuredeploy.net/)

### Locally


#### Clone the repository

```console
git clone https://github.com/BandwidthExamples/csharp-call-contacts-example.git
```
Open this solution in Visual Studio. 

Fill sections <appSettings> of Web.config with valid values:
`userId`, `apiToken`, `apiSecret` are Catapult API auth data (you can get them on tab "Account" of https://catapult.inetwork.com/pages/catapult.jsf)

Build this solution. Missing modules will be downloaded on the build by nuget.

Click right button on thie project in solution explorer and select "Publish". Select "Microsoft Azure Websites" in opened dialog, then sign in on Azure and select exisitng website to deploy this project or create new (button "New"). If you create new site enter site name, select subscription and region. Database is not required. Change site profile options if need and press "Publish" to upload it on Azure.
Now you can open it in the browser.

## How it works
#### Basic Call Flow Chart
![Basic Flow Chart](/README_Images/Catapult_Flow.png?raw=true)

#### Create outbound call and set the ``` callbackURL ``` to the server's address
```C#
var c = await Call.Create(Client, new Dictionary<string, object>
{
    {"from", PhoneNumberForCallbacks},
    {"to", call.From},
    {"callbackUrl", BaseUrl + Url.Action("CatapultFromCallback")},
    {"tag", call.To}
});
```

#### Play Audio after answer event
```C#
private async Task ProcessCatapultFromEvent(AnswerEvent ev)
{
    // "from" number answered a call. speak him a message
    var call = new Call { Id = ev.CallId };
    call.SetClient(Client);
    var contact = DbContext.Contacts.FirstOrDefault(c => c.PhoneNumber == ev.Tag);
    if (contact == null)
    {
        throw new Exception("Missing contact for number " + ev.Tag);
    }
    await call.SpeakSentence(string.Format("We are connecting you to {0}", contact.Name), ev.Tag);
}
```

#### Wait until speak event is over and make 2nd call
```C#
private async Task ProcessCatapultFromEvent(SpeakEvent ev)
{
    if (ev.Status != "done") return;
    //a messages was spoken to "from" number. calling to "to" number
    var c = await Call.Create(Client, new Dictionary<string, object>
    {
        {"from", PhoneNumberForCallbacks},
        {"to", ev.Tag},
        {"callbackUrl", BaseUrl + Url.Action("CatapultToCallback")},
        {"tag", ev.CallId}
    });
    Debug.WriteLine("Call Id to {0} is {1}", ev.Tag, c.Id);
}
```

### Once 2nd call is answered, create bridge and place calls in bridge
```C#
private async Task ProcessCatapultToEvent(AnswerEvent ev)
{
    //"to" number answered a call. Making a bridge with "from" number's call
    var b = await Bridge.Create(Client, new[] {ev.CallId, ev.Tag}, true);
    Debug.WriteLine(string.Format("BridgeId is {0}", b.Id));
}
```

### Init
#### Search then order phone number
```C#
//reserve a phone numbers for callbacks if need
var numbers = await AvailableNumber.SearchLocal(catapult, new Dictionary<string, object> { { "state", "NC" }, { "city", "Cary" }, { "quantity", 1 } });
phoneNumber = numbers.First().Number;
await PhoneNumber.Create(catapult, new Dictionary<string, object> { { "number", phoneNumber } });
}
```


## Deploy and Demo

### Deploy
#### Click the 'Deploy to Azure' Button and enter Catapult Creds
![Fill in information](/README_Images/step1.png?raw=true)
---

![Click Deploy](/README_Images/step2.png?raw=true)
---

![Click Manage your resources](/README_Images/step3.png?raw=true)
---

![Find the newly Deployed App](/README_Images/step4.png?raw=true)
---

![Click the URL to open the web page](/README_Images/step5.png?raw=true)


### Demo
#### Add a contact (with real phone number)
![Click to add contact](/README_Images/step6.png?raw=true)
---

![Adding Contact](/README_Images/step7.png?raw=true)
---


#### Click to call contact
![Click to call contact](/README_Images/step8.png?raw=true)

#### Enter your 'From' phone number and click call
![Add your number](/README_Images/step9.png?raw=true)



