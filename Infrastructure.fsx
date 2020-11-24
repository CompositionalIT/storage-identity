#r @"nuget:Farmer"

open Farmer
open Farmer.Builders
open System
open System.Diagnostics

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__ 

let storageAccountName = "demoidentitystorage"
let webAppName = "demoidentityweb"

let myWeb = webApp {
    name webAppName
    system_identity
    setting "storage-account-name" storageAccountName
    zip_deploy "deploy"
    run_from_package
}

let storage = storageAccount {
    name storageAccountName
    add_blob_container "data"
    grant_access myWeb.SystemIdentity Roles.StorageBlobDataReader
}

let template = arm {
    location Location.WestEurope
    add_resources [ myWeb; storage ]
}

// Deploy the template to the "identity-demo" resource group
template |> Deploy.execute "identity-demo" []

// Prime the storage account with some seed data
let deployFile (fileName:string) =
    printfn $"Uploading {fileName}..."
    Deploy.Az.az $"storage blob upload --account-name {storageAccountName} --container-name data --name {fileName} --file {fileName}"
    |> Result.get
    |> ignore

for file in [ "readme.md"; "Program.fs"; "Infrastructure.fsx" ] do
    deployFile file

// Finally, open a browser that navigates to the web app to show the seed data via the web app.
Process.Start $"https://{webAppName}.azurewebsites.net/blobs"