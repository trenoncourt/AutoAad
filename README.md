# AutoAad
A microservice REST API to query Azure Active Directory in .net core 2.0.

If you want to query Azure Active Directory easily with application right. Just drop this api on a server, configure and that's it!

## Installation
Download the [last release](https://github.com/trenoncourt/AutoMail/releases) and place it on your server.

You can download version **1.0.0** [here](https://github.com/trenoncourt/AutoAad/releases/download/1.0.0/AutoAad-1.0.0.zip)

AutoAad is designed to be as light as possible so it does not contains the *Microsoft.AspNetCore.Server.IISIntegration* package.
If you want to use it on **IIS**, download the **iis-1.0.0** version from [here](https://github.com/trenoncourt/AutoAad/releases/download/1.0.0/AutoAad-iis-1.0.0.zip)

## Configuration
All the configuration can be made in environment variable or appsettings.json file :

| Name                    | Description                                   | Type        | Default value |
| ----------------------- | --------------------------------------------- | ----------- |--------------:|
| **Cors**                | Cors settings                                 | Object      |               |
| ```Cors.Enabled```      | Define if cors are enabled                    | Boolean     | false         |
| ```Cors.Methods```      | Adds specific methods to the policy           | String      | *             |
| ```Cors.Origins```      | Adds specific origins to the policy           | String      | *             |
| ```Cors.Headers```      | Adds specific headers to the policy           | String      | *             |
| **AzureAD**             | Aad settings                                  | Object      |               |
| ```Instance*```         | Login instance url                            | String      |               |
| ```Domain*```           | Domain name (use common for multitenancy)     | String      |               |
| ```ClientId*```         | Client Id of your app                         | String      |               |
| ```ClientSecret*```     | Client secret of your app                     | String      |               |
| ```Resource*```         | The resource you want to use                  | String      |               |
| ```GraphVersion*```     | Version of ms graph api (v1.0 or beta)        | String      |               |

Exemple of appsettings.json
```json
{
  "Cors": {
    "Enabled": true
  },
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "gimeabeer.onmicrosoft.com",
    "ClientId": "25532b58-b686-4b76-87ad-49f73d2421b3",
    "ClientSecret": "kVMbJ8fogiR7BQg3eF34Snb",
    "Resource": "https://graph.microsoft.com/",
    "GraphVersion": "v1.0"
  }
}

```
App settings can be injected with environment variables too.

More settings & docker support will come.

## API Reference
AutoAad juste proxy the graph.micrososft.com api so if you drop AutoAad in a localhost:8080 server, you can use request like this:
[http://localhost:8080/users?$filter=startswith(displayName, 'thi')&$select=id,displayname](http://localhost:8080)

## Auth
Soon...


## Buy me a beer
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.me/trenoncourt/5)
