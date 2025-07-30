#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.API
// File Name:           General.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-23-2025 19:07
// Last Updated On:     07-29-2025 20:52
// *****************************************/

#endregion

namespace Subscription.API.Code;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public static class General
{
    internal static async Task CopyBlobs(string sourceCandidateID, string destinationCandidateID)
    {
        string _connectionString = Start.AzureBlob;
        string _containerName = Start.AzureBlobContainer;
        string _source = $"Candidate/{sourceCandidateID}/";
        string _destination = $"Candidate/{destinationCandidateID}/";
        BlobContainerClient containerClient = new(_connectionString, _containerName);

        List<Task> _copyTasks = [];
        await foreach (BlobItem _blobItem in containerClient.GetBlobsAsync(prefix: _source))
        {
            string _sourceBlobName = _blobItem.Name;
            string _targetBlobName = _destination + _sourceBlobName[_source.Length..];
            BlobClient _sourceBlob = containerClient.GetBlobClient(_sourceBlobName);
            BlobClient _targetBlob = containerClient.GetBlobClient(_targetBlobName);

            Uri _sourceUri = _sourceBlob.Uri;

            // Start server-side copy
            _copyTasks.Add(_targetBlob.StartCopyFromUriAsync(_sourceUri));
        }

        await Task.WhenAll(_copyTasks).ConfigureAwait(false);
    }

    /// <summary>
    ///     Deserializes a JSON string to an object of a specified type.
    /// </summary>
    /// <typeparam name="T">The type of object to deserialize to.</typeparam>
    /// <param name="array">The JSON string representing the object to be deserialized.</param>
    /// <returns>The deserialized object of type T.</returns>
    internal static string ExtractTextFromPdf(IFormFile file)
    {
        using PdfLoadedDocument _document = new(file.OpenReadStream());
        StringBuilder _resumeText = new();
        foreach (object page in _document.Pages)
        {
            _resumeText.Append(((PdfLoadedPage)page).ExtractText());
        }

        _document.Close();

        return _resumeText.ToString();
    }

    internal static string ExtractTextFromPdf(byte[] file)
    {
        // Memory optimization: Use RecyclableMemoryStream for PDF processing
        using RecyclableMemoryStream stream = ReusableMemoryStream.Get("pdf-text-extraction", file);
        using PdfLoadedDocument _document = new(stream);
        StringBuilder _resumeText = new();
        foreach (object page in _document.Pages)
        {
            _resumeText.Append(((PdfLoadedPage)page).ExtractText());
        }

        _document.Close();

        return _resumeText.ToString();
    }

    internal static string ExtractTextFromWord(IFormFile file)
    {
        using WordDocument _document = new(file.OpenReadStream(), FormatType.Automatic);
        string _resumeText = _document.GetText();
        _document.Close();

        return _resumeText;
    }

    internal static string ExtractTextFromWord(byte[] file)
    {
        // Memory optimization: Use RecyclableMemoryStream for Word processing
        using RecyclableMemoryStream stream = ReusableMemoryStream.Get("word-text-extraction", file);
        using WordDocument _document = new(stream, FormatType.Automatic);
        string _resumeText = _document.GetText();
        _document.Close();

        return _resumeText;
    }

    internal static async Task<byte[]> ReadFromBlob(string blobPath)
    {
        //Connect to the Azure Blob Storage
        IAzureBlobStorage _storage = StorageFactory.Blobs.AzureBlobStorageWithSharedKey(Start.AccountName, Start.AzureKey);

        //Read the file into a Bytes Array
        byte[] _memBytes = await _storage.ReadBytesAsync(blobPath);

        return _memBytes;
    }

    // ReSharper disable once UnusedMember.Local
    private static async Task<List<IntValues>> SetIntValues(SqlDataReader reader, byte keyType = 0) //0-Int32, 1=Int16, 2=Byte
    {
        return await reader.FillList<IntValues>(intValue => new()
                                                            {
                                                                KeyValue = keyType switch
                                                                           {
                                                                               0 => intValue.GetInt32(0),
                                                                               1 => intValue.GetInt16(0),
                                                                               2 => intValue.GetByte(0),
                                                                               _ => 0
                                                                           },
                                                                Text = intValue.GetString(1)
                                                            }).ToListAsync();
    }

    // ReSharper disable once UnusedMember.Local
    private static async Task<List<KeyValues>> SetKeyValues(SqlDataReader reader)
    {
        return await reader.FillList<KeyValues>(keyValue => new()
                                                            {
                                                                KeyValue = keyValue.GetString(0),
                                                                Text = keyValue.GetString(1)
                                                            }).ToListAsync();
    }

    /// <summary>
    ///     Computes the SHA-512 hash of the input text.
    /// </summary>
    /// <param name="inputText">The text to be hashed.</param>
    /// <returns>A byte array representing the SHA-512 hash of the input text.</returns>
    internal static byte[] SHA512PasswordHash(string inputText) => SHA512.HashData(Encoding.UTF8.GetBytes(inputText));

    /// <summary>
    ///     Computes the SHA-512 hash of the input text.
    /// </summary>
    /// <param name="inputText">The text to be hashed.</param>
    /// <returns>A byte array representing the SHA-512 hash of the input text.</returns>
    internal static byte[] SHA512PasswordHash(byte[] inputText) => SHA512.HashData(inputText);

    /// <summary>
    ///     Computes the SHA-512 hash of the input span.
    /// </summary>
    /// <param name="inputText">The span to be hashed.</param>
    /// <returns>A byte array representing the SHA-512 hash of the input span.</returns>
    internal static byte[] SHA512PasswordHash(ReadOnlySpan<byte> inputText) => SHA512.HashData(inputText);

    internal static async Task UploadToBlob(IFormFile file, string blobPath)
    {
        IAzureBlobStorage _storage = StorageFactory.Blobs.AzureBlobStorageWithSharedKey(Start.AccountName, Start.AzureKey);

        await using Stream stream = file.OpenReadStream();
        await _storage.WriteAsync(blobPath, stream);
    }

    internal static async Task UploadToBlob(byte[] file, string blobPath)
    {
        IAzureBlobStorage _storage = StorageFactory.Blobs.AzureBlobStorageWithSharedKey(Start.AccountName, Start.AzureKey);

        // Memory optimization: Use RecyclableMemoryStream for blob uploads
        await using RecyclableMemoryStream stream = ReusableMemoryStream.Get("blob-upload", file);
        await _storage.WriteAsync(blobPath, stream);
    }
}