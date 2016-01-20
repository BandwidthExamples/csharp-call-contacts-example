### csharp-call-contacts-example

Catapult Api demo app (creating calls and bridges)


#### Demonstrates
* Use [Catapult SDK](https://github.com/bandwidthcom/csharp-bandwidth)
* Making calls
* Creating  bridges


## Prerequisites
- Configured Machine with Ngrok/Port Forwarding -OR- Azure Account
  - [Ngrok](https://ngrok.com/)
  - [Azure](https://account.windowsazure.com/Home/Index)
- [Catapult Account](https://catapult.inetwork.com/pages/signup.jsf/?utm_medium=social&utm_source=github&utm_campaign=dtolb&utm_content=_)
- [Visual Studio 2015](https://www.visualstudio.com/en-us/downloads/download-visual-studio-vs.aspx)
- [Git](https://git-scm.com/)
- Common Azure Tools for Visual Studio (they are preinstalled with Visual Studio)


## Build and Deploy

### Azure One Click

#### Settings Required To Run
* ```Catapult User Id```
* ```Catapult Api Token```
* ```Catapult Api Secret```
* ```Contacts``` (your contact list in form `Ann:+1234567890;Joe:+1111111111`)
* ```Your Numbers``` (your numbers in form `Mobile:+1234567890;Work:+1111111111`)

[![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://azuredeploy.net/)

### Locally


#### Clone the repository

```console
git clone https://github.com/bandwidthcom/csharp-bandwidth-examples
```
Open this solution in Visual Studio. 

Fill sections <appSettings> of Web.config with valid values:
`userId`, `apiToken`, `apiSecret` are Catapult API auth data (you can get them on tab "Account" of https://catapult.inetwork.com/pages/catapult.jsf)

Open  App_Start/Data.cs and change test data there. All phone numbers must exists.

Build this solution. Missing modules will be downloaded on the build by nuget.

Click right button on thie project in solution explorer and select "Publish". Select "Microsoft Azure Websites" in opened dialog, then sign in on Azure and select exisitng website to deploy this project or create new (button "New"). If you create new site enter site name, select subscription and region. Database is not required. Change site profile options if need and press "Publish" to upload it on Azure.
Now you can open it in the browser.

## Demo

Open site in web browser and click to button "Click To Call" on contact to call it. Next select a your number for call (it will be bridged with contact number call).
