open Farmer
open Farmer.Builders
open System.Diagnostics

System.Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

/// UPDATE baseName TO SOMETHING UNIQUE TO YOU
let baseName = "farmerisaac"

let storageAccountName = $"{baseName}storage"
let webAppName = $"{baseName}-app"
let rg = $"{baseName}-rg"

// Create and configure a Farmer template of a web app and storage account
let myWeb = webApp {
    name webAppName
    system_identity
    operating_system OS.Windows
    setting "StorageAccountName" storageAccountName
    zip_deploy "../publish"
    run_from_package
}

let storage = storageAccount {
    name storageAccountName
    add_private_container "data"
    grant_access myWeb.SystemIdentity Roles.StorageBlobDataReader
}

let template = arm {
    location Location.WestEurope
    add_resources [ myWeb; storage ]
}

// Deploy the template to the "identity-demo" resource group
template |> Deploy.execute rg [] |> ignore

// Prime the storage account with some seed data
for file in [ "Program.fs"; "Infrastructure.fsproj" ] do
    printfn $"Uploading {file}..."
    Deploy.Az.az $"storage blob upload --account-name {storageAccountName} --container-name data --name {file} --file {file}" 
    |> Result.get
    |> ignore

// Finally, open a browser that navigates to the web app to show the seed data via the web app.
ProcessStartInfo ($"https://{webAppName}.azurewebsites.net/blobs/data", UseShellExecute = true)
|> Process.Start
|> ignore