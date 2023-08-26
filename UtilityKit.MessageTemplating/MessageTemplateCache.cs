using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UtilityKit.MessageTemplating.Attributes;

namespace UtilityKit.MessageTemplating;

/// <summary>
/// Provides caching and retrieval of message templates based on attribute locations in an assembly.
/// </summary>
/// <summary xml:lang="de">
/// Stellt das Zwischenspeichern und Abrufen von Nachrichtenvorlagen basierend auf Attributpositionen in einer Assembly bereit.
/// </summary>
public static partial class MessageTemplateCache
{

    #region Fields & Properties

    private static readonly ConcurrentDictionary<AttributeLocation, List<MessageTemplateAttribute>> _templates = new();

    private static readonly object _initializeLock = new();

    private const BindingFlags _searchCriteria = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    private static int _initialized;

    private static int _templateCount;


    /// <summary>
    /// Gets a value indicating whether the <see cref="MessageTemplateCache"/> has been initialized.
    /// </summary>
    /// <summary xml:lang="de">
    /// Ruft einen Wert ab, der angibt, ob der <see cref="MessageTemplateCache"/> initialisiert wurde.
    /// </summary>
    public static bool IsInitialized
    {
        get
        {
            return _initialized == 1;
        }
    }

    /// <summary>
    /// Gets the number of message templates in the <see cref="MessageTemplateCache"/>.
    /// </summary>
    /// <summary xml:lang="de">
    /// Ruft die Anzahl der Nachrichtenvorlagen im <see cref="MessageTemplateCache"/> ab.
    /// </summary>
    public static int TemplateCount
    {
        get
        {
            return _templateCount;
        }
    }

    #endregion


    /// <summary>
    /// Asynchronously initializes the <see cref="MessageTemplateCache"/> with message templates found in the specified assembly.
    /// </summary>
    /// <param name="callingAssembly">The assembly to search for message templates.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <summary xml:lang="de">
    /// Initialisiert den <see cref="MessageTemplateCache"/> asynchron mit Nachrichtenvorlagen, die in der angegebenen Assembly gefunden wurden.
    /// </summary>
    /// <param xml:lang="de" name="callingAssembly">Die Assembly, in der nach Nachrichtenvorlagen gesucht werden soll.</param>
    /// <returns xml:lang="de">Ein <see cref="Task"/>, der die asynchrone Operation darstellt.</returns>
    [DebuggerHidden, DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task InitializeAsync(Assembly callingAssembly)
    {
        static Task ProcessMethod(MethodInfo methodInfo)
        {
            var attributes = methodInfo.GetCustomAttributes<MessageTemplateAttribute>(false).ToList();

            if (attributes is not null && attributes.Count > 0)
            {
                AttributeLocation key = new(attributes.First().FilePath, attributes.First().MethodName);

                _templates.AddOrUpdate(key, attributes, (_, templates) =>
                {
                    templates.AddRange(attributes);

                    return templates;
                });

                Interlocked.Add(ref _templateCount, attributes.Count);
            }

            return Task.CompletedTask;
        }


        if (Interlocked.Exchange(ref _initialized, 1) != 0) return;

        if (callingAssembly is null) throw new ArgumentNullException(nameof(callingAssembly), ""); // TODO: Exception message


        IEnumerable<Task> tasks = callingAssembly.GetTypes()
            .SelectMany(type => type.GetMethods(_searchCriteria))
            .Select(ProcessMethod);

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    /// <summary>
    /// Starts the fluent API for creating a new <see cref="CacheRequest"/> instance.
    /// </summary>
    /// <param name="filePath">The file path of the caller.</param>
    /// <param name="methodName">The method name of the caller.</param>
    /// <param name="lineNumber">The line number of the caller.</param>
    /// <returns>A new <see cref="CacheRequest"/> instance.</returns>
    /// <summary xml:lang="de">
    /// Startet die Fluent-API zum Erstellen einer neuen <see cref="CacheRequest"/>-Instanz.
    /// </summary>
    /// <param xml:lang="de" name="filePath">Der Dateipfad des Aufrufers.</param>
    /// <param xml:lang="de" name="methodName">Der Methodenname des Aufrufers.</param>
    /// <param xml:lang="de" name="lineNumber">Die Zeilennummer des Aufrufers.</param>
    /// <returns xml:lang="de">Eine neue <see cref="CacheRequest"/>-Instanz.</returns>
    [DebuggerHidden, DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CacheRequest Get([CallerFilePath] in string filePath = "", [CallerMemberName] in string methodName = "", [CallerLineNumber] in int lineNumber = 0)
        => new(filePath, methodName, lineNumber);


    /// <summary>
    /// Retrieves the cached message template associated with the specified <see cref="CacheRequest"/>.
    /// </summary>
    /// <param name="cacheRequest">The cache request specifying the template to retrieve.</param>
    /// <returns>The cached message template, or <see langword="null"/> if not found.</returns>
    /// <summary xml:lang="de">
    /// Ermittelt die zwischengespeicherte Nachrichtenvorlage, die der angegebenen <see cref="CacheRequest"/> zugeordnet ist.
    /// </summary>
    /// <param xml:lang="de" name="cacheRequest">Die Cacheanforderung, die die abzurufende Vorlage angibt.</param>
    /// <returns xml:lang="de">Die zwischengespeicherte Nachrichtenvorlage oder <see langword="null"/>, wenn sie nicht gefunden wurde.</returns>
    [DebuggerHidden, DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string? GetCacheItem(CacheRequest cacheRequest)
    {
        if (_templates.IsEmpty)
        {
            lock (_initializeLock)
            {
                Assembly? assembly = Assembly.GetEntryAssembly();

                if (assembly is null) return null;

                InitializeAsync(assembly)
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
            }
        }

        if (!_templates.TryGetValue(cacheRequest, out List<MessageTemplateAttribute> templates) || templates is null) return null;

        List<MessageTemplateAttribute> identifierMatches = templates
            .Where(template => template.Identifier == cacheRequest.Identifier)
            .ToList();


        if (identifierMatches.Count == 1) return identifierMatches.FirstOrDefault().Template;

        return identifierMatches
            .OrderBy(template => Math.Abs(template.LineNumber - cacheRequest.LineNumber))
            .FirstOrDefault().Template;
    }

}
