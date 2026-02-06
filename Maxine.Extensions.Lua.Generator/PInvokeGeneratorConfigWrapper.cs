using ClangSharp;

namespace Maxine.Extensions.Lua.Generator;

/// <summary>
/// Extensions for <see cref="PInvokeGeneratorConfiguration"/>.
/// </summary>
public static class PInvokeGeneratorConfigurationExtensions
{
    /// <summary>
    /// Creates a new wrapper from a <see cref="PInvokeGeneratorConfiguration"/> instance.
    /// </summary>
    public static PInvokeGeneratorConfigWrapper ToWrapper(this PInvokeGeneratorConfiguration config) => PInvokeGeneratorConfigWrapper.FromConfiguration(config);
    
    /// <summary>
    /// Reconstructs the <see cref="PInvokeGeneratorConfigurationOptions"/> from the given
    /// <see cref="PInvokeGeneratorConfiguration"/> because this is a PITA to do manually.
    /// </summary>
    /// <param name="cfg">The ClangSharp config.</param>
    /// <returns>The options contained within.</returns>
    public static PInvokeGeneratorConfigurationOptions ReconstructOptions(
        this PInvokeGeneratorConfiguration cfg
    )
    {
        var options = PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.DontUseUsingStaticsForEnums
            ? PInvokeGeneratorConfigurationOptions.DontUseUsingStaticsForEnums
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.ExcludeAnonymousFieldHelpers
            ? PInvokeGeneratorConfigurationOptions.ExcludeAnonymousFieldHelpers
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.ExcludeComProxies
            ? PInvokeGeneratorConfigurationOptions.ExcludeComProxies
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.ExcludeEmptyRecords
            ? PInvokeGeneratorConfigurationOptions.ExcludeEmptyRecords
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.ExcludeEnumOperators
            ? PInvokeGeneratorConfigurationOptions.ExcludeEnumOperators
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.ExcludeFnptrCodegen
            ? PInvokeGeneratorConfigurationOptions.ExcludeFnptrCodegen
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.ExcludeFunctionsWithBody
            ? PInvokeGeneratorConfigurationOptions.ExcludeFunctionsWithBody
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.ExcludeNIntCodegen
            ? PInvokeGeneratorConfigurationOptions.ExcludeNIntCodegen
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.GenerateAggressiveInlining
            ? PInvokeGeneratorConfigurationOptions.GenerateAggressiveInlining
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.GenerateCompatibleCode
            ? PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.GenerateCppAttributes
            ? PInvokeGeneratorConfigurationOptions.GenerateCppAttributes
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.GenerateDocIncludes
            ? PInvokeGeneratorConfigurationOptions.GenerateDocIncludes
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.GenerateExplicitVtbls
            ? PInvokeGeneratorConfigurationOptions.GenerateExplicitVtbls
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.GenerateFileScopedNamespaces
            ? PInvokeGeneratorConfigurationOptions.GenerateFileScopedNamespaces
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.GenerateGuidMember
            ? PInvokeGeneratorConfigurationOptions.GenerateGuidMember
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.GenerateHelperTypes
            ? PInvokeGeneratorConfigurationOptions.GenerateHelperTypes
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.GenerateLatestCode
            ? PInvokeGeneratorConfigurationOptions.GenerateLatestCode
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.GenerateMacroBindings
            ? PInvokeGeneratorConfigurationOptions.GenerateMacroBindings
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.GenerateMarkerInterfaces
            ? PInvokeGeneratorConfigurationOptions.GenerateMarkerInterfaces
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.GenerateMultipleFiles
            ? PInvokeGeneratorConfigurationOptions.GenerateMultipleFiles
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.GenerateNativeBitfieldAttribute
            ? PInvokeGeneratorConfigurationOptions.GenerateNativeBitfieldAttribute
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.GenerateNativeInheritanceAttribute
            ? PInvokeGeneratorConfigurationOptions.GenerateNativeInheritanceAttribute
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.GeneratePreviewCode
            ? PInvokeGeneratorConfigurationOptions.GeneratePreviewCode
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.GenerateSetsLastSystemErrorAttribute
            ? PInvokeGeneratorConfigurationOptions.GenerateSetsLastSystemErrorAttribute
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.GenerateSourceLocationAttribute
            ? PInvokeGeneratorConfigurationOptions.GenerateSourceLocationAttribute
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.GenerateTemplateBindings
            ? PInvokeGeneratorConfigurationOptions.GenerateTemplateBindings
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.GenerateTestsNUnit
            ? PInvokeGeneratorConfigurationOptions.GenerateTestsNUnit
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.GenerateTestsXUnit
            ? PInvokeGeneratorConfigurationOptions.GenerateTestsXUnit
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.GenerateTrimmableVtbls
            ? PInvokeGeneratorConfigurationOptions.GenerateTrimmableVtbls
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.GenerateUnixTypes
            ? PInvokeGeneratorConfigurationOptions.GenerateUnixTypes
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.GenerateUnmanagedConstants
            ? PInvokeGeneratorConfigurationOptions.GenerateUnmanagedConstants
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.GenerateVtblIndexAttribute
            ? PInvokeGeneratorConfigurationOptions.GenerateVtblIndexAttribute
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.LogExclusions
            ? PInvokeGeneratorConfigurationOptions.LogExclusions
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.LogPotentialTypedefRemappings
            ? PInvokeGeneratorConfigurationOptions.LogPotentialTypedefRemappings
            : PInvokeGeneratorConfigurationOptions.None;
        options |= cfg.LogVisitedFiles
            ? PInvokeGeneratorConfigurationOptions.LogVisitedFiles
            : PInvokeGeneratorConfigurationOptions.None;
        if ((options & PInvokeGeneratorConfigurationOptions.GeneratePreviewCode) != 0)
        {
            options &= ~PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode;
            options &= ~PInvokeGeneratorConfigurationOptions.GenerateLatestCode;
        }
        if ((options & PInvokeGeneratorConfigurationOptions.GenerateLatestCode) != 0)
        {
            options &= ~PInvokeGeneratorConfigurationOptions.GeneratePreviewCode;
            options &= ~PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode;
        }
        if ((options & PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode) != 0)
        {
            options &= ~PInvokeGeneratorConfigurationOptions.GeneratePreviewCode;
            options &= ~PInvokeGeneratorConfigurationOptions.GenerateLatestCode;
        }
        return options;
    }
}

/// <summary>
/// This record class exists to make it easier to override a few properties in <see cref="PInvokeGeneratorConfiguration"/>
/// </summary>
public record PInvokeGeneratorConfigWrapper
{
    // ----- Constructor parameters -----

    /// <inheritdoc cref="PInvokeGeneratorConfiguration.Language"/>
    public required string Language { get; init; }

    /// <inheritdoc cref="PInvokeGeneratorConfiguration.LanguageStandard"/>
    public required string LanguageStandard { get; init; }

    /// <inheritdoc cref="PInvokeGeneratorConfiguration.DefaultNamespace"/>
    public required string DefaultNamespace { get; init; }

    /// <inheritdoc cref="PInvokeGeneratorConfiguration.OutputLocation"/>
    public required string OutputLocation { get; init; }

    /// <inheritdoc cref="PInvokeGeneratorConfiguration.HeaderText"/>
    public required string HeaderText { get; init; }

    /// <inheritdoc cref="PInvokeGeneratorConfiguration.OutputMode"/>
    public required PInvokeGeneratorOutputMode OutputMode { get; init; }

    /// <summary>
    /// Options passed into <see cref="PInvokeGeneratorConfiguration"/>.
    /// Reconstructed using <see cref="ModUtils.ReconstructOptions"/>.
    /// </summary>
    public PInvokeGeneratorConfigurationOptions Options { get; init; }

    // ----- Properties -----

    /// <inheritdoc cref="PInvokeGeneratorConfiguration.DefaultClass"/>
    public required string DefaultClass { get; init; }

    /// <inheritdoc cref="PInvokeGeneratorConfiguration.ExcludeFnptrCodegen"/>
    public required bool ExcludeFnptrCodegen { get; init; }

    /// <inheritdoc cref="PInvokeGeneratorConfiguration.ExcludedNames"/>
    public required IReadOnlyCollection<string> ExcludedNames { get; init; }

    /// <inheritdoc cref="PInvokeGeneratorConfiguration.IncludedNames"/>
    public required IReadOnlyCollection<string> IncludedNames { get; init; }

    /// <inheritdoc cref="PInvokeGeneratorConfiguration.LibraryPath"/>
    public required string LibraryPath { get; init; }

    /// <inheritdoc cref="PInvokeGeneratorConfiguration.MethodPrefixToStrip"/>
    public required string MethodPrefixToStrip { get; init; }

    /// <inheritdoc cref="PInvokeGeneratorConfiguration.NativeTypeNamesToStrip"/>
    public required IReadOnlyCollection<string> NativeTypeNamesToStrip { get; init; }

    /// <inheritdoc cref="PInvokeGeneratorConfiguration.RemappedNames"/>
    public required IReadOnlyDictionary<string, string> RemappedNames { get; init; }

    /// <inheritdoc cref="PInvokeGeneratorConfiguration.TestOutputLocation"/>
    public required string TestOutputLocation { get; init; }

    /// <inheritdoc cref="PInvokeGeneratorConfiguration.TraversalNames"/>
    public required IReadOnlyCollection<string> TraversalNames { get; init; }

    /// <inheritdoc cref="PInvokeGeneratorConfiguration.WithAccessSpecifiers"/>
    public required IReadOnlyDictionary<string, AccessSpecifier> WithAccessSpecifiers { get; init; }

    /// <inheritdoc cref="PInvokeGeneratorConfiguration.WithAttributes"/>
    public required IReadOnlyDictionary<string, IReadOnlyList<string>> WithAttributes { get; init; }

    /// <inheritdoc cref="PInvokeGeneratorConfiguration.WithCallConvs"/>
    public required IReadOnlyDictionary<string, string> WithCallConvs { get; init; }

    /// <inheritdoc cref="PInvokeGeneratorConfiguration.WithClasses"/>
    public required IReadOnlyDictionary<string, string> WithClasses { get; init; }

    /// <inheritdoc cref="PInvokeGeneratorConfiguration.WithGuids"/>
    public required IReadOnlyDictionary<string, Guid> WithGuids { get; init; }

    /// <inheritdoc cref="PInvokeGeneratorConfiguration.WithLengths"/>
    public required IReadOnlyDictionary<string, string> WithLengths { get; init; }

    /// <inheritdoc cref="PInvokeGeneratorConfiguration.WithLibraryPaths"/>
    public required IReadOnlyDictionary<string, string> WithLibraryPaths { get; init; }

    /// <inheritdoc cref="PInvokeGeneratorConfiguration.WithManualImports"/>
    public required IReadOnlyCollection<string> WithManualImports { get; init; }

    /// <inheritdoc cref="PInvokeGeneratorConfiguration.WithNamespaces"/>
    public required IReadOnlyDictionary<string, string> WithNamespaces { get; init; }

    /// <inheritdoc cref="PInvokeGeneratorConfiguration.WithPackings"/>
    public required IReadOnlyDictionary<string, string> WithPackings { get; init; }

    /// <inheritdoc cref="PInvokeGeneratorConfiguration.WithReadonlys"/>
    public required IReadOnlyCollection<string> WithReadonlys { get; init; }

    /// <inheritdoc cref="PInvokeGeneratorConfiguration.WithSetLastErrors"/>
    public required IReadOnlyCollection<string> WithSetLastErrors { get; init; }

    /// <inheritdoc cref="PInvokeGeneratorConfiguration.WithSuppressGCTransitions"/>
    public required IReadOnlyCollection<string> WithSuppressGCTransitions { get; init; }

    /// <inheritdoc cref="PInvokeGeneratorConfiguration.WithTransparentStructs"/>
    public required IReadOnlyDictionary<string, (string Name, PInvokeGeneratorTransparentStructKind Kind)> WithTransparentStructs { get; init; }

    /// <inheritdoc cref="PInvokeGeneratorConfiguration.WithTypes"/>
    public required IReadOnlyDictionary<string, string> WithTypes { get; init; }

    /// <inheritdoc cref="PInvokeGeneratorConfiguration.WithUsings"/>
    public required IReadOnlyDictionary<string, IReadOnlyList<string>> WithUsings { get; init; }

    /// <summary>
    /// Creates a blank wrapper.
    /// </summary>
    /// <remarks>
    /// You probably want <see cref="PInvokeGeneratorConfigurationExtensions.ToWrapper"/>.
    /// </remarks>
    public PInvokeGeneratorConfigWrapper() { }

    /// <inheritdoc cref="ToConfiguration" />
    /// <remarks>
    /// This cast is to improve the syntax when overriding properties of
    /// <see cref="PInvokeGeneratorConfiguration"/> with the 'with' keyword
    /// and needing to immediately cast back to <see cref="PInvokeGeneratorConfiguration"/> afterwards.
    /// </remarks>
    public static implicit operator PInvokeGeneratorConfiguration(PInvokeGeneratorConfigWrapper wrapper) => wrapper.ToConfiguration();

    /// <summary>
    /// Creates a new wrapper from a <see cref="PInvokeGeneratorConfiguration"/> instance.
    /// </summary>
    public static PInvokeGeneratorConfigWrapper FromConfiguration(PInvokeGeneratorConfiguration config) =>
        new() {
            // Constructor parameters
            Language = config.Language,
            LanguageStandard = config.LanguageStandard,
            DefaultNamespace = config.DefaultNamespace,
            OutputLocation = config.OutputLocation,
            HeaderText = config.HeaderText,
            OutputMode = config.OutputMode,
            Options = config.ReconstructOptions(),

            // Properties
            DefaultClass = config.DefaultClass,
            ExcludeFnptrCodegen = config.ExcludeFnptrCodegen,
            ExcludedNames = config.ExcludedNames,
            IncludedNames = config.IncludedNames,
            LibraryPath = config.LibraryPath,
            MethodPrefixToStrip = config.MethodPrefixToStrip,
            NativeTypeNamesToStrip = config.NativeTypeNamesToStrip,
            RemappedNames = config.RemappedNames,
            TestOutputLocation = config.TestOutputLocation,
            TraversalNames = config.TraversalNames,
            WithAccessSpecifiers = config.WithAccessSpecifiers,
            WithAttributes = config.WithAttributes,
            WithCallConvs = config.WithCallConvs,
            WithClasses = config.WithClasses,
            WithGuids = config.WithGuids,
            WithLengths = config.WithLengths,
            WithLibraryPaths = config.WithLibraryPaths,
            WithManualImports = config.WithManualImports,
            WithNamespaces = config.WithNamespaces,
            WithPackings = config.WithPackings,
            WithReadonlys = config.WithReadonlys,
            WithSetLastErrors = config.WithSetLastErrors,
            WithSuppressGCTransitions = config.WithSuppressGCTransitions,
            WithTransparentStructs = config.WithTransparentStructs,
            WithTypes = config.WithTypes,
            WithUsings = config.WithUsings,
        };

    /// <summary>
    /// Converts the wrapper back into a new <see cref="PInvokeGeneratorConfiguration"/> instance.
    /// </summary>
    public PInvokeGeneratorConfiguration ToConfiguration()
    {
        var tempHeaderFilePath = Path.GetTempFileName();
        File.WriteAllText(tempHeaderFilePath, HeaderText);

        var config = new PInvokeGeneratorConfiguration(
            Language,
            LanguageStandard,
            DefaultNamespace,
            OutputLocation,
            tempHeaderFilePath,
            OutputMode,
            Options
        ) {
            DefaultClass = DefaultClass,
            ExcludeFnptrCodegen = ExcludeFnptrCodegen,
            ExcludedNames = ExcludedNames,
            IncludedNames = IncludedNames,
            LibraryPath = LibraryPath,
            MethodPrefixToStrip = MethodPrefixToStrip,
            NativeTypeNamesToStrip = NativeTypeNamesToStrip,
            RemappedNames = RemappedNames,
            TestOutputLocation = TestOutputLocation,
            TraversalNames = TraversalNames,
            WithAccessSpecifiers = WithAccessSpecifiers,
            WithAttributes = WithAttributes,
            WithCallConvs = WithCallConvs,
            WithClasses = WithClasses,
            WithGuids = WithGuids,
            WithLengths = WithLengths,
            WithLibraryPaths = WithLibraryPaths,
            WithManualImports = WithManualImports,
            WithNamespaces = WithNamespaces,
            WithPackings = WithPackings,
            WithReadonlys = WithReadonlys,
            WithSetLastErrors = WithSetLastErrors,
            WithSuppressGCTransitions = WithSuppressGCTransitions,
            WithTransparentStructs = WithTransparentStructs,
            WithTypes = WithTypes,
            WithUsings = WithUsings,
        };

        // Header is immediately read by PInvokeGeneratorConfiguration so we can delete it afterwards
        File.Delete(tempHeaderFilePath);

        return config;
    }
}