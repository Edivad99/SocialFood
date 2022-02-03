using System;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MimeMapping;

namespace SocialFood.StorageProviders;

internal class AzureStorageProvider : IStorageProvider
{
    private readonly AzureStorageSettings settings;
    private readonly BlobServiceClient blobServiceClient;

    public AzureStorageProvider(AzureStorageSettings settings)
    {
        this.settings = settings;
        blobServiceClient = new BlobServiceClient(settings.ConnectionString);
    }

    public async Task DeleteAsync(string path)
    {
        var blobContainerClient = await GetBlobClientAsync(settings.ContainerName);
        await blobContainerClient.DeleteBlobIfExistsAsync(path);
    }

    public async Task<Stream>? ReadAsync(string path)
    {
        var blobContainerClient = await GetBlobClientAsync(settings.ContainerName);
        var blobClient = blobContainerClient.GetBlobClient(path);

        var stream = new MemoryStream();
        await blobClient.DownloadToAsync(stream);

        stream.Position = 0;
        return stream;
    }

    public async Task SaveAsync(string path, Stream stream)
    {
        var blobContainerClient = await GetBlobClientAsync(settings.ContainerName, true);
        var blobClient = blobContainerClient.GetBlobClient(path);

        stream.Position = 0;
        await blobClient.UploadAsync(stream, new BlobHttpHeaders
        {
            ContentType = MimeUtility.GetMimeMapping(path)
        });
    }

    private async Task<BlobContainerClient> GetBlobClientAsync(string containerName, bool createIfNotExists = false)
    {
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
        if(createIfNotExists)
        {
            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.None);
        }
        return blobContainerClient;
    }
}

