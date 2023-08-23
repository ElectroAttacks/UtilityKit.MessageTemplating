using System.Collections.Immutable;
using System.Composition;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UtilityKit.MessageTemplating.Ressources;

namespace UtilityKit.MessageTemplating.Analyzers;

/// <summary>
/// Provides code fixes for the 'InvalidArgumentRule' diagnostic.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CreateRequestCodeFixProvider)), Shared]
public sealed class CreateRequestCodeFixProvider : CodeFixProvider
{

    #region CodeFixProvider

    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get
        {
            return ImmutableArray.Create(DiagnosticRules.InvalidArgumentRule.Id);
        }
    }


    /// <inheritdoc/>
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc/>  
    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        foreach (Diagnostic diagnostic in context.Diagnostics)
        {
            string removeTitle = DiagnosticRules.GetLocalizedString(nameof(DiagnosticMessages.InvalidArgument_CodeFixRemove))
                .ToString(CultureInfo.DefaultThreadCurrentCulture);
            string replaceTitle = DiagnosticRules.GetLocalizedString(nameof(DiagnosticMessages.InvalidArgument_CodeFixReplace))
                .ToString(CultureInfo.DefaultThreadCurrentCulture);

            // Register the "Remove the parameters" code action.
            context.RegisterCodeFix(CodeAction.Create(
                    title: removeTitle,
                    createChangedDocument: c => RemoveParametersAsync(context.Document, diagnostic, c),
                    equivalenceKey: removeTitle),
                diagnostic);

            // Register the "Correct the parameter values" code action.
            context.RegisterCodeFix(CodeAction.Create(
                    title: replaceTitle,
                    createChangedDocument: c => CorrectParameterValuesAsync(context.Document, diagnostic, c),
                    equivalenceKey: replaceTitle
                    ),
                diagnostic);
        }

        return Task.CompletedTask;
    }

    #endregion


    /// <summary>
    /// Removes all parameters from the specified invocation.
    /// </summary>
    /// <param name="document">The document containing the invocation.</param>
    /// <param name="diagnostic">The diagnostic associated with the code fix.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The document with the updated syntax tree.</returns>
    private static async Task<Document> RemoveParametersAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
    {
        SyntaxNode? root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

        if (root?.FindNode(diagnostic.Location.SourceSpan) is not ArgumentSyntax argument
            || argument.Parent is not ArgumentListSyntax argumentList)
        {
            return document;
        }


        // Remove all arguments from the argument list.
        SyntaxNode newRoot = root!.ReplaceNode(argumentList, SyntaxFactory.ArgumentList());

        return document.WithSyntaxRoot(newRoot);
    }

    /// <summary>
    /// Corrects the parameter values in the specified invocation.
    /// </summary>
    /// <param name="document">The document containing the invocation.</param>
    /// <param name="diagnostic">The diagnostic associated with the code fix.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The document with the updated syntax tree.</returns>
    private static async Task<Document> CorrectParameterValuesAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
    {
        SyntaxNode? root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

        if (root?.FindNode(diagnostic.Location.SourceSpan) is not ArgumentSyntax argument
            || argument.Parent is not ArgumentListSyntax argumentList)
        {
            return document;
        }

        ArgumentSyntax updatedArgument = argument;

        // Replace the invalid argument with the updated argument.
        // FilePath, MethodName, LineNumber => 0, 1, 2
        switch (argumentList.Arguments.IndexOf(argument))
        {
            case 0:
                {
                    updatedArgument = SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(
                        kind: SyntaxKind.StringLiteralExpression,
                        token: SyntaxFactory.Literal(document.FilePath ?? "")));

                    break;
                }

            case 1:
                {
                    updatedArgument = SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(
                        kind: SyntaxKind.StringLiteralExpression,
                        token: SyntaxFactory.Literal(argumentList.FirstAncestorOrSelf<MethodDeclarationSyntax>()?.Identifier.ValueText ?? "")));

                    break;
                }

            case 2:
                {
                    updatedArgument = SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(
                        kind: SyntaxKind.NumericLiteralExpression,
                        token: SyntaxFactory.Literal(argumentList.GetLocation().GetLineSpan().StartLinePosition.Line + 1)));

                    break;
                }
        }

        // Replace the invalid argument with the updated argument.
        SyntaxNode newRoot = root!.ReplaceNode(argument, updatedArgument);

        return document.WithSyntaxRoot(newRoot);
    }

}
