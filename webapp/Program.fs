open Azure.Identity
open Azure.Storage.Blobs
open Giraffe
open Saturn
open System

let accountUri = Environment.GetEnvironmentVariable "storage-account-name" |> sprintf "https://%s.blob.core.windows.net/" |> Uri
let client = BlobServiceClient (accountUri, DefaultAzureCredential ())

/// Lists all blob names in a container
let listBlobs container = json [
    let container = client.GetBlobContainerClient container
    for page in container.GetBlobs().AsPages() do
    for blob in page.Values do
        blob.Name
]

let routes = route "/blobs" >=> listBlobs "data"

let theApp = application {
    use_router routes
    disable_diagnostics
}

run theApp