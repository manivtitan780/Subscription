using System;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

class Program
{
    static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        string connectionString = config["ConnectionStrings:DBConnect"];
        string blobConnectionString = config["ConnectionStrings:AzureBlob"];
        string containerName = "sub";

        var blobServiceClient = new BlobServiceClient(blobConnectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        Console.WriteLine("Clearing existing blobs in sub/Candidate/...");
        await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: "sub/Candidate/"))
        {
            await containerClient.DeleteBlobIfExistsAsync(blobItem.Name);
            Console.WriteLine($"Deleted {blobItem.Name}");
        }

        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        string query = @"
            SELECT C.CANDIDATE_ID, F.FILE_ID, F.FILE_DATA
            FROM CANDIDATE C
            JOIN PROFSVC_FILE F ON F.FILE_ID = C.ORIGINAL_RESUME_FILE_ID OR F.FILE_ID = C.PRONTO_RESUME_FILE_ID
        ";

        using var command = new SqlCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var candidateId = reader.GetInt32(0);
            var fileId = reader.GetGuid(1);
            var fileData = reader.GetSqlBytes(2).Value;

            var blobName = $"sub/Candidate/{candidateId}/{fileId}";
            var blobClient = containerClient.GetBlobClient(blobName);

            using var stream = new MemoryStream(fileData);
            await blobClient.UploadAsync(stream, overwrite: true);

            Console.WriteLine($"Uploaded {blobName}");
        }

        Console.WriteLine("Done!");
    }
}
