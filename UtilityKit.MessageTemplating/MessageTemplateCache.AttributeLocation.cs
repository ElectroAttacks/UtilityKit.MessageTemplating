namespace UtilityKit.MessageTemplating;

partial class MessageTemplateCache
{

    /// <summary>
    /// Represents information about the location of attributes used to cache message templates in the <see cref="MessageTemplateCache"/>.
    /// </summary>
    /// <summary xml:lang="de">
    /// Repräsentiert Informationen zum Speicherort von Attributen, die zum Zwischenspeichern von Nachrichtenvorlagen im <see cref="MessageTemplateCache"/> verwendet werden.
    /// </summary>
    private readonly record struct AttributeLocation
    {

        #region Fields & Properties

        /// <summary>
        /// Gets the file path associated with the attribute location.
        /// </summary>
        /// <summary xml:lang="de">
        /// Ruft den Dateipfad ab, der mit dem Attribut-Speicherort verknüpft ist.
        /// </summary>
        public readonly string FilePath { get; }

        /// <summary>
        /// Gets the method name associated with the attribute location.
        /// </summary>
        /// <summary xml:lang="de">
        /// Ruft den Methodennamen ab, der mit dem Attribut-Speicherort verknüpft ist.
        /// </summary>
        public readonly string MethodName { get; }

        #endregion


        #region Constructors & Operators

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeLocation"/> struct.
        /// </summary>
        /// <param name="filePath">The file path associated with the attribute location.</param>
        /// <param name="methodName">The method name associated with the attribute location.</param>
        /// <summary xml:lang="de">
        /// Initialisiert eine neue Instanz der <see cref="AttributeLocation"/>-Struktur.
        /// </summary>
        /// <param xml:lang="de" name="filePath">Der Dateipfad, der mit dem Attribut-Speicherort verknüpft ist.</param>
        /// <param xml:lang="de" name="methodName">Der Methodenname, der mit dem Attribut-Speicherort verknüpft ist.</param>
        public AttributeLocation(string filePath, string methodName)
        {
            FilePath = filePath;
            MethodName = methodName;
        }



        /// <summary>
        /// Implicitly converts a <see cref="CacheRequest"/> instance to an <see cref="AttributeLocation"/> instance.
        /// </summary>
        /// <param name="cacheRequest">The <see cref="CacheRequest"/> instance to convert.</param>
        /// <returns>The converted <see cref="AttributeLocation"/> instance.</returns>
        /// <summary xml:lang="de">
        /// Konvertiert implizit eine <see cref="CacheRequest"/>-Instanz in eine <see cref="AttributeLocation"/>-Instanz.
        /// </summary>
        /// <param xml:lang="de" name="cacheRequest">Die zu konvertierende <see cref="CacheRequest"/>-Instanz.</param>
        /// <returns xml:lang="de">Die konvertierte <see cref="AttributeLocation"/>-Instanz.</returns>
        public static implicit operator AttributeLocation(in CacheRequest cacheRequest)
            => new(cacheRequest.FilePath, cacheRequest.MethodName);

        #endregion

    }

}
