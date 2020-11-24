## Azure .NET Identity Demo
This repository shows how to construct a storage account with an associated web application with
a trust granted to the web application using Azure's managed identity capabilities. It also
demonstrates how to use Compositional IT's [Farmer](https://compositionalit.github.io/farmer/) technology
to rapidly construct application topologies via Azure Resource Manager (ARM) templates.

## Prerequisites
* .NET 5 SDK
* Visual Studio 2019, Rider or Visual Studio Code with the Ionide F# extension
* Azure CLI

## Components
* Web Application: An ASP .NET Core web application running in F# using the Saturn and Giraffe libraries.
* Farmer Template: An F# script that creates all resources to Azure and deploys the application into it in a pre-configured state.

## Instructions
#### 1. Publish the web application to a local folder
```cmd
dotnet publish -c Release -o publish
```

#### 2. Configure the Farmer template
Open the `Infrastructure.fsx` file and update the `storageAccountName` and `webAppName` values to strings unique to you.

#### 3. Execute the script
* Highlight the contents of the script
* Execute the code (`ALT` + `ENTER` in VS or Code)

The script:

* Provisions Storage and Web Application resources
* Turns on the System Identity of the Web Application
* Grants Reader permissions of the identity to the storage account
* Deploys them to Azure
* Uploads a set of files into the created storage account
* Opens a browser to show the data served back by ASP .NET.

You will be prompted to log into Azure the first time you execute the script. After a short delay,
Farmer will generate and deploy both the ARM template and the deployed application into the provisioned
App Service.