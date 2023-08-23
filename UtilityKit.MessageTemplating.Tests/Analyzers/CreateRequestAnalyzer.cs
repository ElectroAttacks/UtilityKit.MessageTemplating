using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using UtilityKit.MessageTemplating.Ressources;

namespace UtilityKit.MessageTemplating.Analyzers;

/// <summary>
/// Analyzes method invocations of the 'CreateRequest' method from the 'MessageTemplateCache' class and checks the provided arguments for validity.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class CreateRequestAnalyzer : DiagnosticAnalyzer
{

    #region DiagnosticAnalyzer

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            return ImmutableArray.Create(DiagnosticRules.InvalidArgumentRule);
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
        context.RegisterSyntaxNodeAction(AnalyzeMethodInvocation, SyntaxKind.InvocationExpression);
    }

    #endregion


    /// <summary>
    /// Analyzes the given method invocation and checks the provided arguments for validity.
    /// </summary>
    /// <param name="context">The syntax node analysis context.</param>
    private void AnalyzeMethodInvocation(SyntaxNodeAnalysisContext context)
    {
        // Reports the diagnostic with the provided arguments at the specified location if the actual value does not equal the expected value.
        void ReportInvalidArgument<T>(Location location, string paramName, T expectedValue, T actualValue) where T : IEquatable<T>
        {
            if (!actualValue.Equals(expectedValue))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    descriptor: DiagnosticRules.InvalidArgumentRule,
                    location: location, paramName, expectedValue));
            }
        }

        // Gets the cleaned string representation of the given expression syntax by removing escape characters and trimming quotes.
        string GetExpressionValue(ExpressionSyntax expressionSyntax) => expressionSyntax.ToString()
            .Replace("\\\\", "\\")
            .Trim('"');


        // Ignore method invocations that are not CreateRequest() and not an the MessageTemplateCache class.
        if (context.Node is not InvocationExpressionSyntax { ArgumentList.Arguments.Count: > 0 } invocation
            || invocation.Expression is not MemberAccessExpressionSyntax { Name: { } } method
            || method.Name.Identifier.ValueText != Config.CacheRequestMethodName
            || method.Expression.ToString() != Config.CacheClassName)
        {
            return;
        }

        // Get the arguments of the method invocation.
        for (int index = 0; index < invocation.ArgumentList.Arguments.Count; index++)
        {
            Location location = invocation.ArgumentList.Arguments[index].GetLocation();

            string actualValue = GetExpressionValue(invocation.ArgumentList.Arguments[index].Expression);


            switch (index)
            {
                case 0:
                    {
                        ReportInvalidArgument(location, "filePath",
                            expectedValue: context.Node.SyntaxTree.FilePath, actualValue);

                        break;
                    }

                case 1:
                    {
                        ReportInvalidArgument(location, "memberName",
                            expectedValue: context.Node.FirstAncestorOrSelf<MethodDeclarationSyntax>()?.Identifier.ValueText ?? "", actualValue);

                        break;
                    }


                case 2:
                    {
                        ReportInvalidArgument(location, "lineNumber",
                            expectedValue: context.Node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                            actualValue: int.TryParse(actualValue, out int lineNumber) ? lineNumber : 0);

                        break;
                    }
            }
        }
    }

}
