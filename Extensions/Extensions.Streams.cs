#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Extensions
// File Name:           Extensions.Streams.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          07-29-2025 20:07
// Last Updated On:     07-29-2025 20:06
// *****************************************/

#endregion

#region Using

using System.Text;

using Microsoft.Extensions.ObjectPool;

using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;

#endregion

namespace Extensions;

public static partial class Extensions
{
    /// <summary>
    ///     Optimized StringBuilder ObjectPool for document processing operations.
    ///     Configured for large PDF text extraction (100-800KB documents).
    /// </summary>
    private static readonly ObjectPool<StringBuilder> StringBuilderPool =
        new DefaultObjectPoolProvider()
           .CreateStringBuilderPool(48 * 1024,    // 16KB initial capacity - better for larger documents
                                    15 *1024 * 1024   // 15MB max capacity - handles 800KB documents with headroom
                                   );

    /// <summary>
    ///     Gets a StringBuilder from the pool with automatic return capability.
    ///     Use within a using statement for automatic cleanup.
    /// </summary>
    /// <returns>A pooled StringBuilder wrapper that handles automatic return to pool</returns>
    private static PooledStringBuilder GetPooledStringBuilder() => new(StringBuilderPool);

    public static string ReadFromPdf(this Stream stream)
    {
        using PdfLoadedDocument _document = new(stream);
        // Memory optimization: Use pooled StringBuilder to eliminate allocations
        using PooledStringBuilder pooledBuilder = GetPooledStringBuilder();
        StringBuilder _resumeText = pooledBuilder.Builder;

        // Ensure capacity for expected content size
        int estimatedCapacity = _document.Pages.Count * 1024;
        if (_resumeText.Capacity < estimatedCapacity)
        {
            _resumeText.EnsureCapacity(estimatedCapacity);
        }

        foreach (PdfLoadedPage page in _document.Pages)
        {
            _resumeText.Append(page.ExtractText());
        }

        _document.Close();
        return _resumeText.ToString();
    }

    public static async Task<string> ReadFromText(this Stream stream)
    {
        using StreamReader reader = new(stream);
        string text = await reader.ReadToEndAsync();
        return text;
    }

    public static string ReadFromWord(this Stream stream)
    {
        using WordDocument _document = new(stream, FormatType.Automatic);
        string _resumeText = _document.GetText();
        _document.Close();
        return _resumeText;
    }

    /// <summary>
    ///     Wrapper for StringBuilder that automatically returns to pool on disposal
    /// </summary>
    private readonly struct PooledStringBuilder(ObjectPool<StringBuilder> pool) : IDisposable
    {
        public readonly StringBuilder Builder = pool.Get();

        public void Dispose() => pool.Return(Builder);
    }
}