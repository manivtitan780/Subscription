#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Extensions
// File Name:           Base64Helper.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-23-2025 19:07
// Last Updated On:     07-23-2025 19:15
// *****************************************/

#endregion

#region Using

using System.Buffers;

using Microsoft.IO;

#endregion

namespace Extensions.Memory;

/// <summary>
///     Memory-efficient Base64 conversion utilities that avoid Large Object Heap (LOH) allocations
///     for documents larger than 85KB by using ArrayPool for temporary char array allocations.
/// </summary>
public static class Base64Helper
{
    /// <summary>
    ///     Converts byte array to Base64 string using ArrayPool to avoid LOH allocation.
    ///     Optimized for large documents (100KB+) common in PDF processing.
    /// </summary>
    /// <param name="data">Byte array to convert to Base64</param>
    /// <returns>Base64 encoded string</returns>
    /// <remarks>
    ///     This method uses ArrayPool to rent a char array for Base64 conversion, avoiding
    ///     Large Object Heap allocations for large documents. The char array is automatically
    ///     returned to the pool after conversion, reducing GC pressure.
    /// </remarks>
    public static string ConvertToBase64Efficiently(byte[] data)
    {
        if (data.Length == 0)
        {
            return string.Empty;
        }

        // Calculate required Base64 length: (n + 2) / 3 * 4
        int base64Length = (data.Length + 2) / 3 * 4;

        // All PDFs are 100-200KB minimum, so always use ArrayPool to avoid LOH
        char[] rentedArray = ArrayPool<char>.Shared.Rent(base64Length);
        try
        {
            // Convert directly to rented char array
            return Convert.TryToBase64Chars(data, rentedArray, out int written) ? new(rentedArray, 0, written)
                       : Convert.ToBase64String(data); // Fallback to standard conversion if TryToBase64Chars fails
        }
        finally
        {
            // Always return the rented array to pool
            ArrayPool<char>.Shared.Return(rentedArray);
        }
    }

    /// <summary>
    ///     Converts MemoryStream content to Base64 string using ArrayPool to avoid LOH allocation.
    ///     This overload is optimized for stream-based operations.
    /// </summary>
    /// <param name="stream">MemoryStream containing data to convert</param>
    /// <returns>Base64 encoded string</returns>
    /// <remarks>
    ///     This method converts the stream's content to Base64 without creating additional
    ///     byte array copies when possible, using ToArray() only when necessary.
    /// </remarks>
    public static string ConvertToBase64Efficiently(MemoryStream stream) => stream.Length == 0 ? string.Empty :
                                                                                // Use ToArray() to get the stream's buffer - this is unavoidable for Base64 conversion
                                                                                ConvertToBase64Efficiently(stream.ToArray());

    /// <summary>
    ///     Converts RecyclableMemoryStream content to Base64 string using ArrayPool to avoid LOH allocation.
    ///     This overload is optimized for RecyclableMemoryStream operations with better memory efficiency.
    /// </summary>
    /// <param name="stream">RecyclableMemoryStream containing data to convert</param>
    /// <returns>Base64 encoded string</returns>
    /// <remarks>
    ///     This method leverages RecyclableMemoryStream's GetBuffer() method when possible
    ///     to avoid additional memory allocations during Base64 conversion.
    /// </remarks>
    public static string ConvertToBase64Efficiently(RecyclableMemoryStream stream)
    {
        if (stream.Length == 0)
        {
            return string.Empty;
        }

        // Try to use GetBuffer() for better performance, fallback to ToArray() if needed
        try
        {
            byte[] buffer = stream.GetBuffer();
            int length = (int)stream.Length;

            // Use span for more efficient conversion when possible
            return ConvertToBase64Efficiently(buffer.AsSpan(0, length));
        }
        catch
        {
            // Fallback to ToArray() if GetBuffer() fails
            return ConvertToBase64Efficiently(stream.ToArray());
        }
    }

    /// <summary>
    ///     Converts ReadOnlySpan&lt;byte&gt; to Base64 string using ArrayPool to avoid LOH allocation.
    ///     This overload provides the most efficient conversion for span-based operations.
    /// </summary>
    /// <param name="data">ReadOnlySpan of bytes to convert</param>
    /// <returns>Base64 encoded string</returns>
    public static string ConvertToBase64Efficiently(ReadOnlySpan<byte> data)
    {
        if (data.IsEmpty)
        {
            return string.Empty;
        }

        // Calculate required Base64 length
        int base64Length = (data.Length + 2) / 3 * 4;

        // Rent char array from pool
        char[] rentedArray = ArrayPool<char>.Shared.Rent(base64Length);
        try
        {
            // Convert span directly to rented char array
            return Convert.TryToBase64Chars(data, rentedArray, out int written) ? new(rentedArray, 0, written) :
                       // Fallback - should rarely be needed
                       Convert.ToBase64String(data);
        }
        finally
        {
            ArrayPool<char>.Shared.Return(rentedArray);
        }
    }

    /// <summary>
    ///     Creates a data URI string with Base64 encoded content, optimized to avoid LOH allocations.
    /// </summary>
    /// <param name="data">Byte array to encode</param>
    /// <param name="mimeType">MIME type for the data URI (e.g., "application/pdf")</param>
    /// <returns>Complete data URI string</returns>
    /// <remarks>
    ///     This method is specifically optimized for creating PDF data URIs for viewer components.
    ///     Example output: "data:application/pdf;base64,JVBERi0xLjQK..."
    /// </remarks>
    public static string CreateDataUri(byte[] data, string mimeType = "application/pdf")
    {
        if (data.Length == 0)
        {
            return $"data:{mimeType};base64,";
        }

        string base64Content = ConvertToBase64Efficiently(data);
        return $"data:{mimeType};base64,{base64Content}";
    }

    /// <summary>
    ///     Creates a PDF data URI string with Base64 encoded content, optimized for ViewPDFDocument component.
    /// </summary>
    /// <param name="data">PDF byte array to encode</param>
    /// <returns>PDF data URI string ready for SfPdfViewer2</returns>
    public static string CreatePdfDataUri(byte[] data) => CreateDataUri(data);

    /// <summary>
    ///     Creates a PDF data URI string from MemoryStream content.
    /// </summary>
    /// <param name="stream">MemoryStream containing PDF data</param>
    /// <returns>PDF data URI string ready for SfPdfViewer2</returns>
    public static string CreatePdfDataUri(MemoryStream stream) => CreatePdfDataUri(stream.ToArray());

    /// <summary>
    ///     Creates a PDF data URI string from RecyclableMemoryStream content with optimized memory usage.
    /// </summary>
    /// <param name="stream">RecyclableMemoryStream containing PDF data</param>
    /// <returns>PDF data URI string ready for SfPdfViewer2</returns>
    /// <remarks>
    ///     This overload provides the most efficient conversion for RecyclableMemoryStream,
    ///     leveraging its advanced buffer management for better performance.
    /// </remarks>
    public static string CreatePdfDataUri(RecyclableMemoryStream stream)
    {
        if (stream.Length == 0)
        {
            return "data:application/pdf;base64,";
        }

        string base64Content = ConvertToBase64Efficiently(stream);
        return $"data:application/pdf;base64,{base64Content}";
    }
}