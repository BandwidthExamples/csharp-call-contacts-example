### csharp-call-contacts-example

Catapult Api demo app (creating calls and bridges)

## Getting started
You need to have
    - Git
    - .Net Framework 4.5
    - Visual Studio 2012 (with nuget) or 2013
    - Bandwidth Application Platform account
    - Azure account
    - Common Azure Tools for Visual Studio


## Build and Deploy

#### Clone the repository

```console
git clone https://github.com/bandwidthcom/csharp-bandwidth-examples
```
Open this solution in Visual Studio 2013. 

Fill sections <appSettings> of Web.config with valid values:
`userId`, `apiToken`, `apiSecret` are Catapult API auth data (you can get them on tab "Account" of https://catapult.inetwork.com/pages/catapult.jsf)
`phoneNumberForCallbacks` is phone number of Catapult which will be used for callbacks
`baseUrl`  is base url of this site from internet (like http://<your-site>.azurewebsites.net).

Open  App_Start/Data.cs and change test data there. All phone numbers must exists.

Build this solution. Missing modules will be downloaded on the build by nuget.

Click right button on thie project in solution explorer and select "Publish". Select "Microsoft Azure Websites" in opened dialog, then sign in on Azure and select exisitng website to deploy this project or create new (button "New"). If you create new site enter site name, select subscription and region. Database is not required. Change site profile options if need and press "Publish" to upload it on Azure.
Now you can open it in the browser.
