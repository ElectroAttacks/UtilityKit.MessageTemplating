using Microsoft.CodeAnalysis;

namespace UtilityKit.MessageTemplating.Ressources;

/// <summary>
/// Contains static diagnostic rules for MessageTemplateCache analyzers.
/// </summary>
internal static class DiagnosticRules
{

    /// <summary>
    /// The URI of the help link for the MessageTemplateCache analyzers.
    /// </summary>
    public const string _helpLinkUri = "https://github.com/ElectroAttacks/UtilityKit.MessageTemplating/blob/master/README.md";


    /// <summary>
    /// Represents the diagnostic rule for invalid arguments.
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidArgumentRule = new(
        id: "MT0001",
        title: GetLocalizedString(nameof(DiagnosticMessages.InvalidArgument_Title)),
        messageFormat: GetLocalizedString(nameof(DiagnosticMessages.InvalidArgument_MessageFormat)),
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: GetLocalizedString(nameof(DiagnosticMessages.InvalidArgument_Description)),
        helpLinkUri: _helpLinkUri
        );

    /// <summary>
    /// Represents the diagnostic rule for missing identifiers.
    /// </summary>
    public static readonly DiagnosticDescriptor MissingIdentifierRule = new(
        id: "MT0002",
        title: GetLocalizedString(nameof(DiagnosticMessages.MissingIdentifier_Title)),
        messageFormat: GetLocalizedString(nameof(DiagnosticMessages.MissingIdentifier_MessageFormat)),
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: GetLocalizedString(nameof(DiagnosticMessages.MissingIdentifier_Description)),
        helpLinkUri: _helpLinkUri
        );


    /// <summary>
    /// Retrieves the localized string for the given resource key.
    /// </summary>
    /// <param name="resourceKey">The key of the resource string.</param>
    /// <returns>The localized string for the given resource key.</returns>
    public static LocalizableString GetLocalizedString(string resourceKey)
       => new LocalizableResourceString(resourceKey, DiagnosticMessages.ResourceManager, typeof(DiagnosticMessages));

}