open Azure.Identity
open Azure.Storage.Blobs
open Giraffe
open Giraffe.GiraffeViewEngine
open Saturn
open System

// Connect to Azure Storage using the default azure credentials (which includes system identity).
let storageAccountName = Environment.GetEnvironmentVariable "storage-account-name"
let client =
    let accountUri = storageAccountName |> sprintf "https://%s.blob.core.windows.net" |> Uri
    BlobServiceClient (accountUri, DefaultAzureCredential ())

/// Lists all blob names in a container
let listBlobs container =
    let view = html [ ] [
        GiraffeViewEngine.head [] [
            link [ _rel "stylesheet"; _href "https://cdn.jsdelivr.net/npm/bulma@0.9.1/css/bulma.min.css" ]
        ]

        section [ _class "hero is-primary" ] [
            div [ _class "hero-body" ] [
                div [ _class "container" ] [
                    h1 [ _class "title" ] [ str "Farmer Azure Managed Identity Blob Viewer" ]
                    p [ _class "subtitle" ] [ str (sprintf "Connected to %s/%s." storageAccountName container) ]
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
                        let container = client.GetBlobContainerClient container
                        for page in container.GetBlobs().AsPages() do
                        for blob in page.Values do
                            tr [] [
                                td [] [ str blob.Name ]
                                td [] [ str (string blob.Properties.ContentLength) ]
                                td [] [ str (string blob.Properties.CreatedOn) ]
                            ]
                    ]
                ]
            ]
        ]
    ]

    htmlView view

let routes = routef "/blobs/%s" listBlobs

let theApp = application {
    use_router routes
    disable_diagnostics
}

run theApp