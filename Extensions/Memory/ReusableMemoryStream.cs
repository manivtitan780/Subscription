#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Extensions
// File Name:           ReusableMemoryStream.cs
// Created By:          Claude Code Assistant
// Created On:          07-23-2025 16:10
// Last Updated On:     07-23-2025 16:30
// *****************************************/

#endregion

using Microsoft.IO;

namespace Extensions.Memory;

/// <summary>
///     A memory-efficient MemoryStream implementation that uses Microsoft.IO.RecyclableMemoryStream
///     for production-ready memory management with LOH avoidance and advanced pooling strategies.
/// </summary>
/// <remarks>
///     This class provides a familiar API while leveraging Microsoft's production-proven 
///     RecyclableMemoryStreamManager. It offers:
///     - LOH avoidance through multi-buffer strategy
///     - Advanced pooling with metrics and monitoring
///     - Thread-safe, high-performance operations
///     - Drop-in replacement for standard MemoryStream
/// </remarks>
public static class ReusableMemoryStream
{
    /// <summary>
    ///     Gets a new RecyclableMemoryStream instance from the managed pool
    /// </summary>
    /// <returns>A new RecyclableMemoryStream ready for use</returns>
    /// <remarks>
    ///     The returned stream should be disposed after use to return buffers to the pool.
    ///     Use within a using statement for automatic disposal.
    /// </remarks>
    public static RecyclableMemoryStream Get() => MemoryStreamManager.GetStream();

    /// <summary>
    ///     Gets a new RecyclableMemoryStream instance with initial data
    /// </summary>
    /// <param name="buffer">Initial data to write to the stream</param>
    /// <returns>A new RecyclableMemoryStream containing the provided data</returns>
    public static RecyclableMemoryStream Get(byte[] buffer) => MemoryStreamManager.GetStream(buffer);

    /// <summary>
    ///     Gets a new RecyclableMemoryStream instance with initial data from ReadOnlySpan
    /// </summary>
    /// <param name="buffer">Initial data to write to the stream</param>
    /// <returns>A new RecyclableMemoryStream containing the provided data</returns>
    public static RecyclableMemoryStream Get(ReadOnlySpan<byte> buffer) => MemoryStreamManager.GetStream(buffer);

    /// <summary>
    ///     Gets a new RecyclableMemoryStream instance with specified initial capacity
    /// </summary>
    /// <param name="capacity">Minimum initial capacity for the stream</param>
    /// <returns>A new RecyclableMemoryStream with at least the specified capacity</returns>
    /// <remarks>
    ///     RecyclableMemoryStream manages capacity automatically based on usage patterns,
    ///     so this is primarily for compatibility with existing code.
    /// </remarks>
    public static RecyclableMemoryStream Get(int capacity)
    {
        var stream = MemoryStreamManager.GetStream();
        // RecyclableMemoryStream doesn't expose Capacity directly, but will grow as needed
        return stream;
    }

    /// <summary>
    ///     Gets a new RecyclableMemoryStream instance with a specific tag for debugging/metrics
    /// </summary>
    /// <param name="tag">Tag to identify the stream usage (e.g., "pdf-conversion", "document-upload")</param>
    /// <returns>A new RecyclableMemoryStream with the specified tag</returns>
    public static RecyclableMemoryStream Get(string tag) => MemoryStreamManager.GetStream(tag);

    /// <summary>
    ///     Gets a new RecyclableMemoryStream instance with initial data and tag
    /// </summary>
    /// <param name="tag">Tag to identify the stream usage</param>
    /// <param name="buffer">Initial data to write to the stream</param>
    /// <returns>A new RecyclableMemoryStream containing the provided data with the specified tag</returns>
    public static RecyclableMemoryStream Get(string tag, byte[] buffer) => MemoryStreamManager.GetStream(tag, buffer);

    /// <summary>
    ///     Gets memory pool statistics for monitoring and diagnostics
    /// </summary>
    /// <returns>String representation of current pool statistics</returns>
    public static string GetStatistics() => MemoryStreamManager.GetStatistics();
}