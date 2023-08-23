using System;
using System.Diagnostics.CodeAnalysis;

namespace UtilityKit.MessageTemplating.Attributes;

/// <summary>
/// Represents an attribute used to associate a message template with a method.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public sealed class MessageTemplateAttribute : Attribute
{

    /// <summary>
    /// Gets the template string of the message.
    /// </summary>
    public string Template { get; }

    /// <summary>
    /// Gets the identifier of the message template.
    /// </summary>
    public string Identifier { get; }



    /// <summary>
    /// Initializes a new instance of the <see cref="MessageTemplateAttribute"/> class with the specified template string and optional identifier.
    /// </summary>
    /// <param name="template">The template string associated with the message template attribute.</param>
    /// <param name="identifier">The identifier associated with the message template attribute.</param>
    public MessageTemplateAttribute([StringSyntax("CompositeFormat")] string template, string identifier = "")
    {
        Template = template;
        Identifier = identifier;
    }

}
