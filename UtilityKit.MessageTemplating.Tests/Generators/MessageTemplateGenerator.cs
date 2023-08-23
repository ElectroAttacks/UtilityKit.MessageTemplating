using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UtilityKit.MessageTemplating.Ressources;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace UtilityKit.MessageTemplating.Generators;

/// <summary>
/// A source generator that generates message template cache based on syntax analysis.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed partial class MessageTemplateGenerator : ISourceGenerator
{

    private record struct AttributeLocation(string FilePath, string MethodName);

    private record struct CacheItem(string Template, int LineNumber, string Identifier);

    private static readonly Dictionary<AttributeLocation, List<CacheItem>> _cacheEntries = new();

    private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();



    /// <summary>
    /// Initializes the source generator.
    /// </summary>
    /// <param name="context">The generator initialization context.</param>
    public void Initialize(GeneratorInitializationContext context)
    {
        if (!Debugger.IsAttached)
        {
            // Uncomment the following line to debug the source generator.
            //Debugger.Launch();
        }

        context.RegisterForSyntaxNotifications(() => new MessageTemplateCacheReceiver());
    }

    /// <summary>
    /// Executes the source generator to generate the message template cache.
    /// </summary>
    /// <param name="context">The generator execution context.</param>
    public async void Execute(GeneratorExecutionContext context)
    {
        // Get the generated source and fix the indentation.
        StringBuilder generatedSourceBuilder = new StringBuilder(GetCacheInitializer()
                .NormalizeWhitespace()
                .ToFullString())
            .Replace("\n", "\n\t");


        // Get the source template and replace the placeholders.
        StringBuilder sourceBuilder = new StringBuilder(await GetCodeTemplateAsync("MessageTemplateCache.txt")
                .ConfigureAwait(false))
            .Replace("GeneratorTimeStamp", $"Generated at: {DateTime.Now}")
            .Replace("GeneratorName", nameof(MessageTemplateGenerator))
            .Replace("GeneratorVersion", _assembly.GetName().Version.ToString());


        // Replace the dynamic placeholders.
        foreach (DictionaryEntry dictionaryEntry in Config.ResourceManager.GetResourceSet(CultureInfo.InvariantCulture, false, false))
        {
            if (dictionaryEntry.Key.ToString().Equals(nameof(Config.CacheFieldName), StringComparison.OrdinalIgnoreCase))
            {
                sourceBuilder.Replace($"{dictionaryEntry.Key} = new()", generatedSourceBuilder.ToString());
            }

            sourceBuilder.Replace(dictionaryEntry.Key.ToString(), dictionaryEntry.Value.ToString());
        }


        // Add the source to the context and clear the cache.
        context.AddSource($"{Config.CacheClassName}.cs", sourceBuilder.ToString());

        _cacheEntries.Clear();
    }


    #region Code Generation

    /// <summary>
    /// Generates an initializer expression for the message template cache.
    /// </summary>
    /// <returns>The initializer expression representing the populated message template cache.</returns>
    private static AssignmentExpressionSyntax GetCacheInitializer()
    {
        List<ExpressionSyntax> expressions = new();

        foreach (KeyValuePair<AttributeLocation, List<CacheItem>> cacheEntry in _cacheEntries)
        {
            // new AttributeLocation("...", "...")
            ObjectCreationExpressionSyntax keyExpression = GetKeySyntax(cacheEntry.Key);

            // new List<MessageTemplate>() { ... }
            ObjectCreationExpressionSyntax valueExpression = ObjectCreationExpression(GenericName("List")
                .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName(Config.CacheItemName)))))
                .WithInitializer(InitializerExpression(
                    kind: SyntaxKind.CollectionInitializerExpression,
                    expressions: SeparatedList<ExpressionSyntax>(cacheEntry.Value.Select(GetCollectionValueSyntax))));

            // { new AttributeLocation("...", "..."), new List<MessageTemplate>() { ... } }
            expressions.Add(InitializerExpression(
                kind: SyntaxKind.ComplexElementInitializerExpression,
                expressions: SeparatedList<ExpressionSyntax>(new[] { keyExpression, valueExpression }))
                );

        }

        return AssignmentExpression(
            kind: SyntaxKind.SimpleAssignmentExpression,
            left: IdentifierName(Config.CacheFieldName),
            right: ImplicitObjectCreationExpression().WithInitializer(
                initializer: InitializerExpression(
                    kind: SyntaxKind.CollectionInitializerExpression,
                    expressions: SeparatedList(expressions))));
    }

    /// <summary>
    /// Creates an attribute location object for the cache entry.
    /// </summary>
    /// <param name="attributeLocation">The attribute location.</param>
    /// <returns>An <see cref="ObjectCreationExpressionSyntax"/> representing the attribute location.</returns>
    private static ObjectCreationExpressionSyntax GetKeySyntax(AttributeLocation attributeLocation)
        => ObjectCreationExpression(IdentifierName(Config.CacheKeyName))
                .WithArgumentList(ArgumentList(
                    SeparatedList<ArgumentSyntax>(
                        new SyntaxNodeOrToken[]
                        {
                            Argument(LiteralExpression(
                                kind: SyntaxKind.StringLiteralExpression,
                                token: Literal(GetCleanArg(attributeLocation.FilePath)))),

                            Token(SyntaxKind.CommaToken),

                            Argument(LiteralExpression(
                                kind: SyntaxKind.StringLiteralExpression,
                                token: Literal(GetCleanArg(attributeLocation.MethodName)))),
                        })
                    )
                );

    /// <summary>
    /// Creates a message template object for the cache entry.
    /// </summary>
    /// <param name="messageTemplate">The message template.</param>
    /// <returns>An <see cref="ObjectCreationExpressionSyntax"/> representing the message template.</returns>
    private static ObjectCreationExpressionSyntax GetCollectionValueSyntax(CacheItem messageTemplate)
        => ObjectCreationExpression(IdentifierName(Config.CacheItemName))
                .WithArgumentList(ArgumentList(
                    SeparatedList<ArgumentSyntax>(
                        new SyntaxNodeOrToken[]
                        {
                            Argument(LiteralExpression(
                                kind: SyntaxKind.StringLiteralExpression,
                                token: Literal(GetCleanArg(messageTemplate.Template)))),

                            Token(SyntaxKind.CommaToken),

                            Argument(LiteralExpression(
                                kind: SyntaxKind.NumericLiteralExpression,
                                token: Literal(messageTemplate.LineNumber))),

                            Token(SyntaxKind.CommaToken),

                            Argument(LiteralExpression(
                                kind: SyntaxKind.StringLiteralExpression,
                                token: Literal(GetCleanArg(messageTemplate.Identifier)))),
                        })
                    )
                );

    #endregion


    /// <summary>
    /// Removes surrounding double quotes from an argument string.
    /// </summary>
    /// <param name="argument">The argument string.</param>
    /// <returns>The cleaned argument string.</returns>
    private static string GetCleanArg(string argument) => argument.Trim('"');

    /// <summary>
    /// Gets the code template from an embedded resource.
    /// </summary>
    /// <param name="resourceName">The name of the embedded resource to retrieve.</param>
    /// <returns>The code template as a string.</returns>
    private static async Task<string> GetCodeTemplateAsync(string resourceName)
    {
        using Stream? stream = _assembly.GetManifestResourceStream($"{typeof(MessageTemplateGenerator).Namespace}.Templates.{resourceName}");

        // Resource not found
        if (stream is null)
        {
            return string.Empty;
        }


        using StreamReader reader = new(stream);

        return await reader.ReadToEndAsync().ConfigureAwait(false);
    }

}
