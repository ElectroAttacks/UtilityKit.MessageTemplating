using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace UtilityKit.MessageTemplating.Attributes;

/// <summary>
/// Marks a method with a message template, which is then stored in the <see cref="MessageTemplateCache"/>.
/// </summary>
/// <summary xml:lang="de">
/// Markiert eine Methode mit einer Nachrichtenvorlage, die anschließend im <see cref="MessageTemplateCache"/> gespeichert wird.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public sealed class MessageTemplateAttribute : Attribute
{

    #region Fields & Properties

    /// <summary>
    /// Gets the message template for the method.
    /// </summary>
    /// <summary xml:lang="de">
    /// Ruft die Nachrichtenvorlage für die Methode ab.
    /// </summary>
    public string Template { get; }

    /// <summary>
    /// Gets the unique identifier for the message template.
    /// </summary>
    /// <summary xml:lang="de">
    /// Ruft den eindeutigen Bezeichner für die Nachrichtenvorlage ab.
    /// </summary>
    public string Identifier { get; }

    /// <summary>
    /// Gets the file path associated with the attribute location.
    /// </summary>
    /// <summary xml:lang="de">
    /// Ruft den Dateipfad ab, der mit dem Attribut-Speicherort verknüpft ist.
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// Gets the method name associated with the attribute location.
    /// </summary>
    /// <summary xml:lang="de">
    /// Ruft den Methodennamen ab, der mit dem Attribut-Speicherort verknüpft ist.
    /// </summary>
    public string MethodName { get; }

    /// <summary>
    /// Gets the line number associated with the attribute location.
    /// </summary>
    /// <summary xml:lang="de">
    /// Ruft die Zeilennummer ab, die mit dem Attribut-Speicherort verknüpft ist.
    /// </summary>
    public int LineNumber { get; }

    #endregion


    /// <summary>
    /// Initializes a new instance of the <see cref="MessageTemplateAttribute"/> class with the specified template and identifier.
    /// </summary>
    /// <param name="template">The message template using Composite Format syntax.</param>
    /// <param name="identifier">A unique identifier for the message template.</param>
    /// <param name="filePath">The file path associated with the attribute location.</param>
    /// <param name="methodName">The method name associated with the attribute location.</param>
    /// <param name="lineNumber">The line number associated with the attribute location.</param>
    /// <summary xml:lang="de">
    /// Initialisiert eine neue Instanz der <see cref="MessageTemplateAttribute"/>-Klasse mit der angegebenen Vorlage und dem Bezeichner.
    /// </summary>
    /// <param xml:lang="de" name="template">Die Nachrichtenvorlage unter Verwendung der Composite-Format-Syntax.</param>
    /// <param xml:lang="de" name="identifier">Ein eindeutiger Bezeichner für die Nachrichtenvorlage.</param>
    /// <param xml:lang="de" name="filePath">Der Dateipfad, der mit dem Attribut-Speicherort verknüpft ist.</param>
    /// <param xml:lang="de" name="methodName">Der Methodenname, der mit dem Attribut-Speicherort verknüpft ist.</param>
    /// <param xml:lang="de" name="lineNumber">Die Zeilennummer, die mit dem Attribut-Speicherort verknüpft ist.</param>
    public MessageTemplateAttribute([StringSyntax("CompositeFormat")] string template, string identifier = "", [CallerFilePath] string filePath = "", [CallerMemberName] string methodName = "", [CallerLineNumber] int lineNumber = 0)
    {
        Template = template;
        Identifier = identifier;
        FilePath = filePath;
        MethodName = methodName;
        LineNumber = lineNumber;
    }

}
