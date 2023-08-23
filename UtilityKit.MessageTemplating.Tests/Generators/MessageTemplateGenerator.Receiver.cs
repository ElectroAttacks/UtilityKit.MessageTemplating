using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UtilityKit.MessageTemplating.Attributes;

namespace UtilityKit.MessageTemplating.Generators;

public partial class MessageTemplateGenerator
{

    /// <summary>
    /// A syntax receiver that visits syntax nodes and extracts message template attributes for caching.
    /// </summary>
    private sealed class MessageTemplateCacheReceiver : ISyntaxReceiver
    {

        /// <summary>
        /// Visits a syntax node to identify and process message template attributes for caching.
        /// </summary>
        /// <param name="syntaxNode">The syntax node to be visited.</param>
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            // Validate that the syntax node is an marker attribute with at least one argument
            if (syntaxNode is not AttributeSyntax { ArgumentList.Arguments.Count: > 0, Name: { } } attribute
                || attribute.Name.ToString() != nameof(MessageTemplateAttribute).Replace(nameof(Attribute), string.Empty)
                || GetParentMethod(attribute) is not MethodDeclarationSyntax methodDeclaration)
            {
                return;
            }


            // Get the location of the attribute
            FileLinePositionSpan positionSpan = attribute.GetLocation().GetLineSpan();

            // Create the key
            AttributeLocation attributeLocation = new(
                FilePath: positionSpan.Path,
                MethodName: methodDeclaration.Identifier.Text);

            // Create the value
            CacheItem messageTemplate = new(
                Template: attribute.ArgumentList.Arguments[0].ToString(),
                LineNumber: positionSpan.StartLinePosition.Line + 1,
                Identifier: attribute.ArgumentList.Arguments.Count > 1
                    ? attribute.ArgumentList.Arguments[1].ToString()
                    : string.Empty);


            // Add the key/value pair to the dictionary
            if (!_cacheEntries.TryGetValue(attributeLocation, out System.Collections.Generic.List<CacheItem>? items))
            {
                items = new();
                _cacheEntries.Add(attributeLocation, items);
            }

            items.Add(messageTemplate);
        }

        /// <summary>
        /// Gets the parent method syntax node of the given syntax node.
        /// </summary>
        /// <param name="syntaxNode">The syntax node for which to find the parent method.</param>
        /// <returns>The <see cref="MethodDeclarationSyntax"/> of the parent method, or <see langword="null"/> if not found.</returns>
        private static MethodDeclarationSyntax? GetParentMethod(SyntaxNode syntaxNode)
        {
            SyntaxNode? parent = syntaxNode.Parent;

            while (parent != null)
            {
                if (parent is MethodDeclarationSyntax methodDeclarationSyntax)
                {
                    return methodDeclarationSyntax;
                }

                parent = parent.Parent;
            }

            return null;
        }

    }

}
