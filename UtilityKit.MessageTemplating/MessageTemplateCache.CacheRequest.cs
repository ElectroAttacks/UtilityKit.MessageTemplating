using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace UtilityKit.MessageTemplating;

/// <summary>
/// Represents a request for retrieving a message template from the <see cref="MessageTemplateCache"/>.
/// </summary>
/// <summary xml:lang="de">
/// Repräsentiert eine Anforderung zum Abrufen einer Nachrichtenvorlage aus dem <see cref="MessageTemplateCache"/>.
/// </summary> 
public readonly struct CacheRequest : IEquatable<CacheRequest>
{

    #region Fields & Properties

    /// <summary>
    /// Gets the file path of the requested template.
    /// </summary>
    /// <summary xml:lang="de">
    /// Ruft den Dateipfad der angeforderten Vorlage ab.
    /// </summary>
    [Required, MinLength(1)]
    internal readonly string FilePath { get; }

    /// <summary>
    /// Gets the method name of the requested template.
    /// </summary>
    /// <summary xml:lang="de">
    /// Ruft den Methodennamen der angeforderten Vorlage ab.
    /// </summary>
    [Required, MinLength(1)]
    internal readonly string MethodName { get; }

    /// <summary>
    /// Gets the line number of the requested template.
    /// </summary>
    /// <summary xml:lang="de">
    /// Ruft die Zeilennummer der angeforderten Vorlage ab.
    /// </summary>
    [Required, Range(1, int.MaxValue)]
    internal readonly int LineNumber { get; }

    /// <summary>
    /// Gets the identifier of the requested template.
    /// </summary>
    /// <summary xml:lang="de">
    /// Ruft den Bezeichner der angeforderten Vorlage ab.
    /// </summary>
    internal readonly string Identifier { get; }

    #endregion


    #region Constructors & Operators

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheRequest"/> struct with the specified file path, method name and line number.
    /// </summary>
    /// <param name="filePath">The file path of the requested template.</param>
    /// <param name="methodName">The method name of the requested template.</param>
    /// <param name="lineNumber">The line number in the file of the requested template.</param>
    /// <summary xml:lang="de">
    /// Initialisiert eine neue Instanz der <see cref="CacheRequest"/>-Struktur mit dem angegebenen Dateipfad, Methodennamen und Zeilennummer.
    /// </summary>
    /// <param xml:lang="de" name="filePath">Der Dateipfad der angeforderten Vorlage.</param>
    /// <param xml:lang="de" name="methodName">Der Methodenname der angeforderten Vorlage.</param>
    /// <param xml:lang="de" name="lineNumber">Die Zeilennummer in der Datei der angeforderten Vorlage.</param>
    internal CacheRequest(in string filePath, in string methodName, in int lineNumber)
    {
        FilePath = filePath;
        MethodName = methodName;
        LineNumber = lineNumber;
        Identifier = "";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheRequest"/> struct based on the specified <paramref name="cacheRequest"/> and <paramref name="identifier"/>.
    /// </summary>
    /// <param name="cacheRequest">The existing <see cref="CacheRequest"/> to base the new instance on.</param>
    /// <param name="identifier">The identifier of the requested template.</param>
    /// <summary xml:lang="de">
    /// Initialisiert eine neue Instanz der <see cref="CacheRequest"/>-Struktur basierend auf der angegebenen <paramref name="cacheRequest"/> und <paramref name="identifier"/>.
    /// </summary>
    /// <param xml:lang="de" name="cacheRequest">Die vorhandene <see cref="CacheRequest"/>, auf dem die neue Instanz basieren soll.</param>
    /// <param xml:lang="de" name="identifier">Der Bezeichner der angeforderten Vorlage.</param>
    private CacheRequest(in CacheRequest cacheRequest, in string identifier) : this(cacheRequest.FilePath, cacheRequest.MethodName, cacheRequest.LineNumber)
    {
        Identifier = identifier;
    }



    /// <summary>
    /// Compares two <see cref="CacheRequest"/> instances for equality.
    /// </summary>
    /// <param name="left">The first <see cref="CacheRequest"/> instance to compare.</param>
    /// <param name="right">The second <see cref="CacheRequest"/> instance to compare.</param>
    /// <returns><see langword="true"/> if the instances are equal; otherwise, <see langword="false"/>.</returns>
    /// <summary xml:lang="de">
    /// Vergleicht zwei <see cref="CacheRequest"/>-Instanzen auf Gleichheit.
    /// </summary>
    /// <param xml:lang="de" name="left">Die erste zu vergleichende <see cref="CacheRequest"/>-Instanz.</param>
    /// <param xml:lang="de" name="right">Die zweite zu vergleichende <see cref="CacheRequest"/>-Instanz.</param>
    /// <returns xml:lang="de"><see langword="true"/> wenn die Instanzen gleich sind; andernfalls <see langword="false"/>.</returns>
    public static bool operator ==(CacheRequest left, CacheRequest right)
        => left.Equals(right);

    /// <summary>
    /// Compares two <see cref="CacheRequest"/> instances for inequality.
    /// </summary>
    /// <param name="left">The first <see cref="CacheRequest"/> instance to compare.</param>
    /// <param name="right">The second <see cref="CacheRequest"/> instance to compare.</param>
    /// <returns><see langword="true"/> if the instances are not equal; otherwise, <see langword="false"/>.</returns>
    /// <summary xml:lang="de">
    /// Vergleicht zwei <see cref="CacheRequest"/>-Instanzen auf Ungleichheit.
    /// </summary>
    /// <param xml:lang="de" name="left">Die erste zu vergleichende <see cref="CacheRequest"/>-Instanz.</param>
    /// <param xml:lang="de" name="right">Die zweite zu vergleichende <see cref="CacheRequest"/>-Instanz.</param>
    /// <returns xml:lang="de"><see langword="true"/> wenn die Instanzen nicht gleich sind; andernfalls <see langword="false"/>.</returns>
    public static bool operator !=(CacheRequest left, CacheRequest right)
        => !left.Equals(right);

    #endregion


    #region IEquatable & Object Overrides

    /// <summary>
    /// Determines whether the current <see cref="CacheRequest"/> instance is equal to another <see cref="CacheRequest"/> instance.
    /// </summary>
    /// <param name="other">The <see cref="CacheRequest"/> instance to compare with the current instance.</param>
    /// <returns><see langword="true"/> if the instances are equal; otherwise, <see langword="false"/>.</returns>
    /// <summary xml:lang="de">
    /// Bestimmt, ob die aktuelle <see cref="CacheRequest"/>-Instanz einer anderen <see cref="CacheRequest"/>-Instanz entspricht.
    /// </summary>
    /// <param xml:lang="de" name="other">Die <see cref="CacheRequest"/>-Instanz, die mit der aktuellen Instanz verglichen werden soll.</param>
    /// <returns xml:lang="de"><see langword="true"/> wenn die Instanzen gleich sind; andernfalls <see langword="false"/>.</returns>
    public bool Equals(CacheRequest other)
        => FilePath == other.FilePath && MethodName == other.MethodName && LineNumber == other.LineNumber && Identifier == other.Identifier;



    /// <summary>
    /// Determines whether the current <see cref="CacheRequest"/> instance is equal to another <see cref="object"/>.
    /// </summary>
    /// <param name="obj">The <see cref="object"/> to compare with the current instance.</param>
    /// <returns><see langword="true"/> if the objects are equal; otherwise, <see langword="false"/>.</returns>
    /// <summary xml:lang="de">
    /// Bestimmt, ob die aktuelle <see cref="CacheRequest"/>-Instanz einer anderen <see cref="object"/> entspricht.
    /// </summary>
    /// <param xml:lang="de" name="obj">Das <see cref="object"/> zum Vergleich mit der aktuellen Instanz.</param>
    /// <returns xml:lang="de"><see langword="true"/> wenn die Objekte gleich sind; andernfalls <see langword="false"/>.</returns>
    public override bool Equals(object obj)
        => obj is CacheRequest cacheRequest && Equals(cacheRequest);


    /// <summary>
    /// Generates a hash code for the current <see cref="CacheRequest"/> instance.
    /// </summary>
    /// <returns>A hash code value.</returns>
    /// <summary xml:lang="de">
    /// Erzeugt einen Hash-Code für die aktuelle <see cref="CacheRequest"/>-Instanz.
    /// </summary>
    /// <returns xml:lang="de">Ein Hash-Code-Wert.</returns>
    public override int GetHashCode()
        => HashCode.Combine(FilePath, MethodName, LineNumber, Identifier);


    /// <summary>
    /// Returns a string that represents the current <see cref="CacheRequest"/> instance.
    /// </summary>
    /// <returns>A string representation of the instance.</returns>
    /// <summary xml:lang="de">
    /// Gibt eine Zeichenfolge zurück, die die aktuelle <see cref="CacheRequest"/>-Instanz darstellt.
    /// </summary>
    /// <returns xml:lang="de">Eine String-Darstellung der Instanz.</returns>
    public override string ToString()
        => $"Method: {FilePath}::{MethodName} | Line: {LineNumber} | Id: {Identifier}";

    #endregion


    /// <summary>
    /// Creates a new <see cref="CacheRequest"/> instance with the specified identifier.
    /// </summary>
    /// <param name="identifier">The identifier for the new cache request.</param>
    /// <returns>A new <see cref="CacheRequest"/> instance with the specified identifier.</returns>
    /// <summary xml:lang="de">
    /// Erstellt eine neue <see cref="CacheRequest"/>-Instanz mit der angegebenen Kennung.
    /// </summary>
    /// <param xml:lang="de" name="identifier">Die Kennung für die neue Cache-Anfrage.</param>
    /// <returns xml:lang="de">Eine neue <see cref="CacheRequest"/>-Instanz mit dem angegebenen Bezeichner.</returns>
    public CacheRequest WithIdentifier(in string identifier)
        => new(this, identifier);


    /// <summary>
    /// Retrieves the template associated with the cache request from the <see cref="MessageTemplateCache"/>.
    /// </summary>
    /// <returns>The template associated with the cache request, or an empty string if not found.</returns>
    /// <summary xml:lang="de">
    /// Ruft die mit der Cache-Anfrage verknüpfte Vorlage aus <see cref="MessageTemplateCache"/> ab.
    /// </summary>
    /// <returns xml:lang="de">Die mit der Cache-Anfrage verknüpfte Vorlage oder eine leere Zeichenfolge, wenn sie nicht gefunden wird.</returns>
    [DebuggerHidden, DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string Template()
        => MessageTemplateCache.GetCacheItem(this) ?? string.Empty;


    /// <summary>
    /// Generates a message using the template and provided arguments.
    /// </summary>
    /// <param name="arguments">The arguments to be used in the message.</param>
    /// <returns>The generated message string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="arguments"/> is <see langword="null"/>.</exception>
    /// <exception cref="FormatException">Thrown when the template is invalid or the format of the template is incorrect.</exception>
    /// <summary xml:lang="de">
    /// Erzeugt eine Nachricht mithilfe der Vorlage und der bereitgestellten Argumente.
    /// </summary>
    /// <param xml:lang="de" name="arguments">Die in der Nachricht zu verwendenden Argumente.</param>
    /// <returns xml:lang="de">Der generierte Nachrichtenstring.</returns>
    /// <exception xml:lang="de" cref="ArgumentNullException">Wird ausgelöst, wenn <paramref name="arguments"/> <see langword="null"/> ist.</exception>
    /// <exception xml:lang="de" cref="FormatException">Wird ausgelöst, wenn die Vorlage ungültig ist oder das Format der Vorlage falsch ist.</exception>
    [DebuggerHidden, DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string Message(params object[] arguments)
        => Message(CultureInfo.CurrentCulture, arguments);

    /// <summary>
    /// Generates a message using the template, provided format provider, and arguments.
    /// </summary>
    /// <param name="formatProvider">The format provider to be used for formatting.</param>
    /// <param name="arguments">The arguments to be used in the message.</param>
    /// <returns>The generated message string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="formatProvider"/> or <paramref name="arguments"/> is <see langword="null"/>.</exception>
    /// <exception cref="FormatException">Thrown when the template is invalid or the format of the template is incorrect.</exception>
    /// <summary xml:lang="de">
    /// Erzeugt eine Nachricht mithilfe der Vorlage, des bereitgestellten Formatanbieters und der Argumente.
    /// </summary>
    /// <param xml:lang="de" name="formatProvider">Der Formatanbieter, der für die Formatierung verwendet werden soll.</param>
    /// <param xml:lang="de" name="arguments">Die in der Nachricht zu verwendenden Argumente.</param>
    /// <returns xml:lang="de">Der generierte Nachrichtenstring.</returns>
    /// <exception xml:lang="de" cref="ArgumentNullException">Wird ausgelöst, wenn <paramref name="formatProvider"/> oder <paramref name="arguments"/> <see langword="null"/> ist. </exception>
    /// <exception xml:lang="de" cref="FormatException">Wird ausgelöst, wenn die Vorlage ungültig ist oder das Format der Vorlage falsch ist.</exception>
    [DebuggerHidden, DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string Message(in IFormatProvider formatProvider, params object[] arguments)
    {
        if (formatProvider is null) throw new ArgumentNullException(nameof(formatProvider), "The format provider cannot be null.");
        if (arguments is null) throw new ArgumentNullException(nameof(arguments), "The arguments cannot be null.");
        // TODO: Localize exception message

        if (MessageTemplateCache.GetCacheItem(this) is not string template)
            return string.Empty;

        return string.Format(formatProvider, template, arguments);
    }

}
