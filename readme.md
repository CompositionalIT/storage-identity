## Azure .NET Identity Demo
This repository shows how to construct a storage account with an associated web application with
a trust granted to the web application using Azure's managed identity capabilities. It also
demonstrates how to use Compositional IT's [Farmer](https://compositionalit.github.io/farmer/) technology
to rapidly construct application topologies via Azure Resource Manager (ARM) templates.

## Prerequisites
* .NET 3.1 SDK or higher
* Visual Studio 2019, Rider or Visual Studio Code with the Ionide F# extension
* Azure CLI

## Components
* Web Application: An ASP .NET Core web application running in F# using the Saturn and Giraffe libraries.
* Farmer Template: An F# script that creates all resources to Azure and deploys the application into it in a pre-configured state.

## Instructions
#### 1. Publish the web application to a local folder
```cmd
dotnet publish webapp -c Release -o publish
```

#### 2. Configure the Farmer template
Open the `infrastructure/Program.fs` file and update the `storageAccountName` and `webAppName` values to strings unique to you.

#### 3. Run the Farmer application
```cmd
dotnet run -p infrastructure
```

The Infrastructure application uses Farmer to do the following:

* Provisions Storage and Web Application resources
* Turns on the System Identity of the Web Application
* Grants Reader permissions of the identity to the storage account
* Deploys them to Azure
* Uploads a set of files into the created storage account

You will be prompted to log into Azure the first time you execute the script. After a short delay,
Farmer will generate and deploy both the ARM template and the deployed web application into the
provisioned App Service.

Finally, the application opens a browser to show the data served back by ASP .NET.