open Azure.Identity
open Azure.Storage.Blobs
open Giraffe
open Giraffe.GiraffeViewEngine
open Saturn
open System

// Get the storage account name from an environment variable - will be set by Farmer during ARM deploy.
let storageAccountName = Environment.GetEnvironmentVariable "storage-account-name"

// Connect to Azure Storage using the default azure credentials (which includes the system identity).
let blobClient =
    let accountUri = Uri $"https://{storageAccountName}.blob.core.windows.net"
    BlobServiceClient (accountUri, DefaultAzureCredential ())

/// Creates a HTML tree that displays a table of all files in a container. 
let listBlobs containerName =
    html [ ] [
        GiraffeViewEngine.head [] [
            link [ _rel "stylesheet"; _href "https://cdn.jsdelivr.net/npm/bulma@0.9.1/css/bulma.min.css" ]
        ]

        section [ _class "hero is-primary" ] [
            div [ _class "hero-body" ] [
                div [ _class "container" ] [
                    h1 [ _class "title" ] [ str "Farmer Azure Managed Identity Blob Viewer" ]
                    p [ _class "subtitle" ] [ str $"Connected to {storageAccountName}/{containerName}." ]
                ]
            ]
        ]

        section [ _class "section" ] [
            div [ _class "container" ] [
                table [ _class "table is-bordered is-striped is-fullwidth" ] [
                    thead [] [
                        tr [] [
                            th [] [ str "Name" ]
                            th [] [ str "Size" ]
                            th [] [ str "Created On" ]
                        ]
                    ]
                    tbody [] [
                        // Connect to Azure Storage and list all blobs
                        for blobPage in blobClient.GetBlobContainerClient(containerName).GetBlobs().AsPages() do
                        for blobItem in blobPage.Values do
                            tr [] [
                                td [] [ str blobItem.Name ]
                                td [] [ str (string blobItem.Properties.ContentLength) ]
                                td [] [ str (string blobItem.Properties.CreatedOn) ]
                            ]
                    ]
                ]
            ]
        ]
    ]

/// A parameterised route that shows all blobs in a container as an HTML response.
let displayBlobs = routef "/blobs/%s" (listBlobs >> htmlView)

/// An ASP .NET Core application.
let theApp = application {
    use_router displayBlobs
    disable_diagnostics
}

run theApp