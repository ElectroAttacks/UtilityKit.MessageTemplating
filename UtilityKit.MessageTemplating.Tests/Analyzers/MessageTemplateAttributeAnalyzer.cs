using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using UtilityKit.MessageTemplating.Attributes;
using UtilityKit.MessageTemplating.Ressources;

namespace UtilityKit.MessageTemplating.Analyzers;

/// <summary>
/// Analyzes methods with multiple MessageTemplateAttributes and checks if their identifiers are unique.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MessageTemplateAttributeAnalyzer : DiagnosticAnalyzer
{

    private static readonly string _markerAttributeName = nameof(MessageTemplateAttribute).Replace(nameof(Attribute), string.Empty);


    #region DiagnosticAnalyzer

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            return ImmutableArray.Create(DiagnosticRules.MissingIdentifierRule);
        }
    }


    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        if (context is null)
        {
            return;
        }

        // Configure analysis.
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

        // Register analysis.
        context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
    }

    #endregion


    /// <summary>
    /// Analyzes the given method for MessageTemplateAttributes and checks their identifiers' uniqueness.
    /// </summary>
    /// <param name="context">The syntax node analysis context.</param>
    private void AnalyzeMethod(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not MethodDeclarationSyntax methodDeclaration)
        {
            return;
        }

        // Contains all MessageTemplateAttributes with an ArgumentList
        IEnumerable<AttributeSyntax> messageTemplateAttributes = methodDeclaration.AttributeLists
            .SelectMany(attributeList => attributeList.Attributes)
            .Where(attribute => attribute.Name is IdentifierNameSyntax identifierName &&
                                identifierName.Identifier.Text == _markerAttributeName &&
                                attribute.ArgumentList is not null);

        // Check if every MessageTemplateAttribute has a unique identifier
        HashSet<string> identifiers = new();
        foreach (AttributeSyntax attribute in messageTemplateAttributes)
        {
            if (attribute.ArgumentList!.Arguments.Count == 1)
            {
                if (!identifiers.Add(""))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        descriptor: DiagnosticRules.MissingIdentifierRule,
                        location: attribute.GetLocation()));
                }
            }

            if (attribute.ArgumentList!.Arguments.Count == 2)
            {
                string identifier = attribute.ArgumentList.Arguments[1].Expression.ToString().Trim('"');

                if (!identifiers.Add(identifier))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        descriptor: DiagnosticRules.MissingIdentifierRule,
                        location: attribute.GetLocation()));
                }
            }
        }
    }

}
