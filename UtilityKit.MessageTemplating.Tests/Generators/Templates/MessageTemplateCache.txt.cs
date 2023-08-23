using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace UtilityKit.MessageTemplating;

/// <summary>
/// Provides caching and retrieval functionality for message templates.
/// </summary>
public static class CacheClassName
{

    /// <summary>
    /// Gets the cache entries for the <see cref="CacheClassName"/>.
    /// </summary>
    /// <remarks>GeneratorTimeStamp</remarks>
    [GeneratedCode("GeneratorName", "GeneratorVersion")]
    private static readonly Dictionary<CacheKeyName, List<CacheItemName>> CacheFieldName = new();



    /// <summary>
    /// Creates a cache request with the specified file path, method name, and line number.
    /// </summary>
    /// <param name="filePath">The file path associated with the cache request (automatically provided by the caller).</param>
    /// <param name="methodName">The method name associated with the cache request (automatically provided by the caller).</param>
    /// <param name="lineNumber">The line number associated with the cache request (automatically provided by the caller).</param>
    /// <returns>A new <see cref="CacheRequest"/> instance.</returns>
    [DebuggerHidden, DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CacheRequest CacheRequestMethodName([CallerFilePath] in string filePath = "", [CallerMemberName] in string methodName = "", [CallerLineNumber] in int lineNumber = 0)
        => new(filePath, methodName, lineNumber);

    /// <summary>
    /// Creates a new cache request based on an existing request with the specified identifier.
    /// </summary>
    /// <param name="cacheRequest">The existing cache request.</param>
    /// <param name="identifier">The identifier associated with the cache request.</param>
    /// <returns>The updated <see cref="CacheRequest"/> instance.</returns>
    [DebuggerHidden, DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CacheRequest WithIdentifier(in this CacheRequest cacheRequest, in string identifier)
        => new(cacheRequest, identifier);


    /// <summary>
    /// Retrieves a <see cref="CacheItemName"/> from the cache based on the specified <see cref="CacheRequest"/>.
    /// </summary>
    /// <param name="request">The <see cref="CacheRequest"/> containing the search parameters.</param>
    /// <returns>The matching <see cref="CacheItemName"/> if successful; otherwise <see langword="null"/>.</returns>
    [DebuggerHidden, DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static CacheItemName? GetCacheEntry(CacheRequest request)
    {
        if (CacheFieldName is null || !CacheFieldName.TryGetValue(new(request.FilePath, request.MethodName), out List<CacheItemName>? CacheItemNames))
        {
            return null;
        }

        // Only one item for the given FilePath and MethodName
        if (CacheItemNames.Count == 1)
        {
            return CacheItemNames.FirstOrDefault();
        }

        // Multiple entries: Check for matching identifiers
        IEnumerable<CacheItemName> identifierMatches = CacheItemNames.Where(item => item.Identifier == request.Identifier);

        if (identifierMatches.Count() == 1)
        {
            return identifierMatches.FirstOrDefault();
        }

        // Multiple entries: select the closest match based on the LineNumber
        return identifierMatches.OrderBy(item => Math.Abs(item.LineNumber - request.LineNumber))
            .FirstOrDefault();
    }


    #region Data Structures

    /// <summary>
    /// Represents the location of an <see cref="Attributes.MessageTemplateAttribute"/> within a source file.
    /// </summary>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    private record struct CacheKeyName(string FilePath, string MethodName)
    {

        /// <summary>
        /// Implicitly converts a <see cref="CacheRequest"/> to an <see cref="CacheKeyName"/>.
        /// </summary>
        /// <param name="request">The cache request containing the file path and method name.</param>
        public static implicit operator CacheKeyName(in CacheRequest request) => new(request.FilePath, request.MethodName);
    }


    /// <summary>
    /// Represents a message template stored within a <see cref="CacheClassName"/>.
    /// </summary>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    private record struct CacheItemName(string MessageTemplate, int LineNumber, string Identifier);

    /// <summary>
    /// Represents a cache request for retrieving a <see cref="CacheItemName"/> from the <see cref="CacheClassName"/>.
    /// </summary>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public readonly struct CacheRequest
    {

        /// <summary>
        /// Gets the caller's file path.
        /// </summary>
        internal readonly string FilePath { get; }

        /// <summary>
        /// Gets the caller's method name.
        /// </summary>
        internal readonly string MethodName { get; }

        /// <summary>
        /// Gets the caller's line number.
        /// </summary>
        internal readonly int LineNumber { get; }

        /// <summary>
        /// Gets the identifier for the message template (optional).
        /// </summary>
        internal readonly string Identifier { get; }



        /// <summary>
        /// Initializes a new instance of the <see cref="CacheRequest"/> struct with the specified file path, method name, and line number.
        /// </summary>
        /// <param name="filePath">The caller's file path.</param>
        /// <param name="methodName">The caller's method name.</param>
        /// <param name="lineNumber">The caller's line number.</param>
        internal CacheRequest(in string filePath, in string methodName, in int lineNumber)
        {
            FilePath = filePath;
            MethodName = methodName;
            LineNumber = lineNumber;
            Identifier = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheRequest"/> struct with the specified cache request and identifier.
        /// </summary>
        /// <param name="request">The original <see cref="CacheRequest"/> instance.</param>
        /// <param name="identifier">The additional identifier to include.</param>
        internal CacheRequest(in CacheRequest request, in string identifier) : this(request.FilePath, request.MethodName, request.LineNumber)
        {
            Identifier = identifier;
        }



        /// <summary>
        /// Retrieves the message from the cached template with the specified arguments.
        /// </summary>
        /// <param name="args">An array of objects to format the message template.</param>
        /// <returns>
        /// The formatted message or <see cref="string.Empty"/> if the template is not found.
        /// </returns>
        /// <exception cref="FormatException">When the format of the template is invalid.</exception>
        /// <exception cref="ArgumentNullException">When <paramref name="args"/> is <see langword="null"/>.</exception>
        [DebuggerHidden, DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetMessage(params object[] args)
            => GetTemplate() is string template ? string.Format(CultureInfo.CurrentCulture, template, args) : "";

        /// <summary>
        /// Retrieves the message from the cached template with the specified arguments and format provider.
        /// </summary>
        /// <param name="formatProvider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information.</param>
        /// <param name="args">An array of objects to format the message template.</param>
        /// <returns>
        /// The formatted message or <see cref="string.Empty"/> if the template is not found.
        /// </returns>
        /// <exception cref="FormatException">When the format of the template is invalid.</exception>
        /// <exception cref="ArgumentNullException">When <paramref name="args"/> is <see langword="null"/>.</exception>
        [DebuggerHidden, DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetMessage(in IFormatProvider formatProvider, params object[] args)
            => GetTemplate() is string template ? string.Format(formatProvider, template, args) : "";


        /// <summary>
        /// Retrieves the cached message template.
        /// </summary>
        /// <returns>
        /// The cached message template or <see langword="null"/> if not found.
        /// </returns>
        [DebuggerHidden, DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string? GetTemplate()
            => CacheClassName.GetCacheEntry(this) is CacheItemName item ? item.MessageTemplate : null;
    }

    #endregion

}
