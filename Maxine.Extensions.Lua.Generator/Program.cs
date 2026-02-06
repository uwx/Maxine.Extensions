using System.Diagnostics;
using System.Text.RegularExpressions;
using ClangSharp;
using ClangSharp.Interop;
using Maxine.Extensions.Lua.Generator;
using Maxine.Extensions.Streams;

Console.WriteLine("Hello, World!");

for (var i = 0; i < (args.Length - 1); i += 3)
{
    var rootPath = args[i];
    var libPath = args[i + 1];
    var className = args[i + 2];
    var outputPath = Path.Combine(args[^1], className);
    
    Directory.CreateDirectory(outputPath);
    
    Console.WriteLine(rootPath);

    var headersDir = Directory.Exists(Path.Combine(rootPath, "include"))
        ? Path.Combine(rootPath, "include")
        : Path.Combine(rootPath, "src");

    var manualPath = Path.Combine(rootPath, "doc", "manual.html");

    var files = new Dictionary<string, Stream>();

    Stream OutputStreamFactory(string fileName)
    {
        // append .1 if file already exists to avoid collisions when multiple headers generate the same output file name
        var baseFileName = Path.GetFileNameWithoutExtension(fileName);
        var uniqueFileName = baseFileName;
        var counter = 1;
        while (files.ContainsKey(Path.ChangeExtension(uniqueFileName, ".gen.cs")))
        {
            uniqueFileName = $"{baseFileName}_{counter}";
            counter++;
        }
        
        var finalFileName = Path.ChangeExtension(uniqueFileName, ".gen.cs");
        return files[finalFileName] = new NoDisposeStream(new MemoryStream());
    }

    /*
    --config
    generate-aggressive-inlining
    generate-callconv-member-function
    generate-cpp-attributes
    generate-disable-runtime-marshalling
    generate-doc-includes
    generate-guid-member
    generate-macro-bindings
    generate-marker-interfaces
    generate-native-inheritance-attribute
    generate-setslastsystemerror-attribute
    generate-unmanaged-constants
    generate-vtbl-index-attribute
    log-potential-typedef-remappings
    single-file
    preview-codegen
    trimmable-vtbls
    unix-types
    strip-enum-member-type-name
    log-potential-typedef-remappings

    --with-suppressgctransition
    luaJIT_setmode
    luaopen_base
    luaopen_math
    luaopen_string
    luaopen_table
    luaopen_io
    luaopen_os
    luaopen_package
    luaopen_debug
    luaopen_bit
    luaopen_jit
    luaopen_ffi
    luaopen_string_buffer
    luaL_openlib
    luaL_register
    luaL_getmetafield
    luaL_typerror
    luaL_argerror
    luaL_checklstring
    luaL_optlstring
    luaL_checknumber
    luaL_optnumber
    luaL_checkinteger
    luaL_optinteger
    luaL_checkstack
    luaL_checktype
    luaL_checkany
    luaL_newmetatable
    luaL_checkudata
    luaL_where
    luaL_ref
    luaL_unref
    luaL_gsub
    luaL_findtable
    luaL_fileresult
    luaL_execresult
    luaL_setfuncs
    luaL_pushmodule
    luaL_testudata
    luaL_setmetatable
    luaL_buffinit
    luaL_prepbuffer
    luaL_addlstring
    luaL_addstring
    luaL_addvalue
    luaL_pushresult
    lua_newthread
    lua_atpanic
    lua_gettop
    lua_settop
    lua_pushvalue
    lua_remove
    lua_insert
    lua_replace
    lua_checkstack
    lua_xmove
    lua_isnumber
    lua_isstring
    lua_iscfunction
    lua_isuserdata
    lua_type
    lua_typename
    lua_equal
    lua_rawequal
    lua_lessthan
    lua_tonumber
    lua_tointeger
    lua_toboolean
    lua_objlen
    lua_tocfunction
    lua_touserdata
    lua_tothread
    lua_topointer
    lua_pushnil
    lua_pushnumber
    lua_pushinteger
    lua_pushlstring
    lua_pushstring
    lua_pushvfstring
    lua_pushfstring
    lua_pushcclosure
    lua_pushboolean
    lua_pushlightuserdata
    lua_pushthread
    lua_rawget
    lua_rawgeti
    lua_createtable
    lua_newuserdata
    lua_getmetatable
    lua_getfenv
    lua_rawset
    lua_rawseti
    lua_setmetatable
    lua_setfenv
    lua_yield
    lua_resume
    lua_status
    lua_error
    lua_concat
    lua_getallocf
    lua_setallocf
    lua_setlevel
    lua_getstack
    lua_getlocal
    lua_setlocal
    lua_getupvalue
    lua_setupvalue
    lua_sethook
    lua_gethook
    lua_gethookmask
    lua_gethookcount
    lua_upvalueid
    lua_upvaluejoin
    lua_version
    lua_copy
    lua_tonumberx
    lua_tointegerx
    lua_isyieldable

    --with-transparent-struct
    lua_Number=double
    lua_Integer=long

    --with-using
    lua_Alloc=delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, nuint, nuint, void*>

    --file
    lualib.h
    lauxlib.h
    lua.h
    luajit.h

    -l
    luajit
    -m
    LuaJit
    -n
    Maxine.Extensions.LuaJit
    -o
    LuaJit.Generated.cs

    -x
    c
     */
    using var pinvokeGenerator = new PInvokeGenerator(
        new PInvokeGeneratorConfigWrapper
            {
                Options = PInvokeGeneratorConfigurationOptions.GenerateAggressiveInlining |
                          PInvokeGeneratorConfigurationOptions.GenerateCallConvMemberFunction |
                          PInvokeGeneratorConfigurationOptions.GenerateCppAttributes |
                          PInvokeGeneratorConfigurationOptions.GenerateDisableRuntimeMarshalling |
                          PInvokeGeneratorConfigurationOptions.GenerateDocIncludes |
                          PInvokeGeneratorConfigurationOptions.GenerateGuidMember |
                          PInvokeGeneratorConfigurationOptions.GenerateMacroBindings |
                          PInvokeGeneratorConfigurationOptions.GenerateMarkerInterfaces |
                          PInvokeGeneratorConfigurationOptions.GenerateNativeInheritanceAttribute |
                          PInvokeGeneratorConfigurationOptions.GenerateSetsLastSystemErrorAttribute |
                          PInvokeGeneratorConfigurationOptions.GenerateUnmanagedConstants |
                          PInvokeGeneratorConfigurationOptions.GenerateVtblIndexAttribute |
                          PInvokeGeneratorConfigurationOptions.LogPotentialTypedefRemappings |
                          PInvokeGeneratorConfigurationOptions.GeneratePreviewCode |
                          PInvokeGeneratorConfigurationOptions.GenerateTrimmableVtbls |
                          PInvokeGeneratorConfigurationOptions.GenerateUnixTypes |
                          PInvokeGeneratorConfigurationOptions.StripEnumMemberTypeName |
                          PInvokeGeneratorConfigurationOptions.GenerateMultipleFiles,
                WithSuppressGCTransitions =
                [
                    "luaJIT_setmode",
                    "luaopen_base",
                    "luaopen_math",
                    "luaopen_string",
                    "luaopen_table",
                    "luaopen_io",
                    "luaopen_os",
                    "luaopen_package",
                    "luaopen_debug",
                    "luaopen_bit",
                    "luaopen_jit",
                    "luaopen_ffi",
                    "luaopen_string_buffer",
                    "luaL_openlib",
                    "luaL_register",
                    "luaL_getmetafield",
                    "luaL_typerror",
                    "luaL_argerror",
                    "luaL_checklstring",
                    "luaL_optlstring",
                    "luaL_checknumber",
                    "luaL_optnumber",
                    "luaL_checkinteger",
                    "luaL_optinteger",
                    "luaL_checkstack",
                    "luaL_checktype",
                    "luaL_checkany",
                    "luaL_newmetatable",
                    "luaL_checkudata",
                    "luaL_where",
                    "luaL_ref",
                    "luaL_unref",
                    "luaL_gsub",
                    "luaL_findtable",
                    "luaL_fileresult",
                    "luaL_execresult",
                    "luaL_setfuncs",
                    "luaL_pushmodule",
                    "luaL_testudata",
                    "luaL_setmetatable",
                    "luaL_buffinit",
                    "luaL_prepbuffer",
                    "luaL_addlstring",
                    "luaL_addstring",
                    "luaL_addvalue",
                    "luaL_pushresult",
                    "lua_newthread",
                    "lua_atpanic",
                    "lua_gettop",
                    "lua_settop",
                    "lua_pushvalue",
                    "lua_remove",
                    "lua_insert",
                    "lua_replace",
                    "lua_checkstack",
                    "lua_xmove",
                    "lua_isnumber",
                    "lua_isstring",
                    "lua_iscfunction",
                    "lua_isuserdata",
                    "lua_type",
                    "lua_typename",
                    "lua_equal",
                    "lua_rawequal",
                    "lua_lessthan",
                    "lua_tonumber",
                    "lua_tointeger",
                    "lua_toboolean",
                    "lua_objlen",
                    "lua_tocfunction",
                    "lua_touserdata",
                    "lua_tothread",
                    "lua_topointer",
                    "lua_pushnil",
                    "lua_pushnumber",
                    "lua_pushinteger",
                    "lua_pushlstring",
                    "lua_pushstring",
                    "lua_pushvfstring",
                    "lua_pushfstring",
                    "lua_pushcclosure",
                    "lua_pushboolean",
                    "lua_pushlightuserdata",
                    "lua_pushthread",
                    "lua_rawget",
                    "lua_rawgeti",
                    "lua_createtable",
                    "lua_newuserdata",
                    "lua_getmetatable",
                    "lua_getfenv",
                    "lua_rawset",
                    "lua_rawseti",
                    "lua_setmetatable",
                    "lua_setfenv",
                    "lua_yield",
                    "lua_resume",
                    "lua_status",
                    "lua_error",
                    "lua_concat",
                    "lua_getallocf",
                    "lua_setallocf",
                    "lua_setlevel",
                    "lua_getstack",
                    "lua_getlocal",
                    "lua_setlocal",
                    "lua_getupvalue",
                    "lua_setupvalue",
                    "lua_sethook",
                    "lua_gethook",
                    "lua_gethookmask",
                    "lua_gethookcount",
                    "lua_upvalueid",
                    "lua_upvaluejoin",
                    "lua_version",
                    "lua_copy",
                    "lua_tonumberx",
                    "lua_tointegerx",
                    "lua_isyieldable",
                ],
                RemappedNames = new Dictionary<string, string>
                {
                    ["lua_typename"] = "_lua_typename",
                    ["lua_getlocal"] = "_lua_getlocal",
                    ["lua_setlocal"] = "_lua_setlocal",
                    ["lua_getupvalue"] = "_lua_getupvalue",
                    ["lua_setupvalue"] = "_lua_setupvalue",
                    ["lua_version"] = "_lua_version",
                    ["char"] = "byte",
                    ["lua_Number"] = "lua_Number",
                    ["lua_Integer"] = "lua_Integer",
                    ["lua_CFunction"] = "lua_CFunction",
                    ["lua_Alloc"] = "lua_Alloc",
                    ["lua_Reader"] = "lua_Reader",
                    ["lua_Writer"] = "lua_Writer",
                    ["lua_Hook"] = "lua_Hook",
                    ["FILE"] = "void",
                },
                WithTransparentStructs = new Dictionary<string, (string Name, PInvokeGeneratorTransparentStructKind Kind)>
                {
                    ["lua_Number"] = ("double", PInvokeGeneratorTransparentStructKind.TypedefHex),
                    ["lua_Integer"] = ("long", PInvokeGeneratorTransparentStructKind.Typedef),
                },
                LibraryPath = libPath,
                DefaultClass = className,
                DefaultNamespace = $"Maxine.Extensions.{className}",
                OutputLocation = $"{className}.cs",
                Language = "c",
                LanguageStandard = "",
                HeaderText = "",
                OutputMode = PInvokeGeneratorOutputMode.CSharp,
                ExcludeFnptrCodegen = false,
                ExcludedNames = [],
                IncludedNames = [],
                MethodPrefixToStrip = "",
                NativeTypeNamesToStrip = [],
                TestOutputLocation = "",
                TraversalNames = [],
                WithAccessSpecifiers = new Dictionary<string, AccessSpecifier>(),
                WithAttributes = new Dictionary<string, IReadOnlyList<string>>(),
                WithCallConvs = new Dictionary<string, string>(),
                WithClasses = new Dictionary<string, string>(),
                WithGuids = new Dictionary<string, Guid>(),
                WithLengths = new Dictionary<string, string>(),
                WithLibraryPaths = new Dictionary<string, string>(),
                WithManualImports = [],
                WithNamespaces = new Dictionary<string, string>(),
                WithPackings = new Dictionary<string, string>(),
                WithReadonlys = [],
                WithSetLastErrors = [],
                WithTypes = new Dictionary<string, string>(),
                WithUsings = new Dictionary<string, IReadOnlyList<string>>()
            }
            .ToConfiguration(),
        OutputStreamFactory
    );

    foreach (var file in (string[])["lualib.h", "lauxlib.h", "lua.h", "luajit.h"])
    {
        var filePath = Path.Combine(headersDir, file);
        if (!File.Exists(filePath)) continue;

        var fileName = Path.GetFileName(file);
        
        var translationUnitError = CXTranslationUnit.TryParse(
            pinvokeGenerator.IndexHandle,
            filePath,
            [],
            Array.Empty<CXUnsavedFile>(),
            CXTranslationUnit_Flags.CXTranslationUnit_None,
            out var handle
        );
        
        if (translationUnitError != CXErrorCode.CXError_Success)
        {
            var msg = $"Parsing failed for '{fileName}' due to '{translationUnitError}'.";
            Console.WriteLine(msg);
            return;
        }

        var skipProcessing = false;
        if (handle.NumDiagnostics != 0)
        {
            Console.WriteLine($"Diagnostics for '{fileName}':");

            for (uint j = 0; j < handle.NumDiagnostics; ++j)
            {
                using var diagnostic = handle.GetDiagnostic(j);

                var severity = diagnostic.Severity switch
                {
                    CXDiagnosticSeverity.CXDiagnostic_Error => "Error",
                    CXDiagnosticSeverity.CXDiagnostic_Fatal => "Fatal",
                    CXDiagnosticSeverity.CXDiagnostic_Warning => "Warning",
                    CXDiagnosticSeverity.CXDiagnostic_Note => "Note",
                    _ => "Unknown"
                };
                
                Console.WriteLine($"  {severity}: {diagnostic.Spelling}");
                skipProcessing |= diagnostic.Severity == CXDiagnosticSeverity.CXDiagnostic_Error;
                skipProcessing |= diagnostic.Severity == CXDiagnosticSeverity.CXDiagnostic_Fatal;
            }
        }

        if (skipProcessing)
        {
            Console.WriteLine($"Skipping '{fileName}' due to one or more errors listed above.");
            return;
        }
        
        try
        {
            using var translationUnit = TranslationUnit.GetOrCreate(handle);
            Debug.Assert(translationUnit is not null);

            Console.WriteLine("Generating raw bindings for '{0}'", fileName);
            pinvokeGenerator.GenerateBindings(
                translationUnit,
                filePath,
                [],
                CXTranslationUnit_Flags.CXTranslationUnit_None
            );
            pinvokeGenerator.Close();
            Console.WriteLine(
                "Completed generation for {0}, file count: {1}",
                filePath,
                files.Count
            );
        }
        catch (Exception e)
        {
            Console.WriteLine("Unhandled exception when generating bindings for {0}: {1}", filePath, e);
        }
    }

    Dictionary<string, string> docs;
    if (File.Exists(manualPath))
    {
        var parser = new LuaDocumentationParser();
        var htmlContent = File.ReadAllText(manualPath);

        docs = parser.ParseManual(htmlContent);

        // Output results
        // foreach (var kvp in docs.OrderBy(x => x.Key))
        // {
        //     Console.WriteLine($"// {kvp.Key}");
        //     Console.WriteLine(kvp.Value);
        //     Console.WriteLine();
        // }
    }
    else
    {
        docs = new Dictionary<string, string>();
    }

    //         /// <include file='LuaJit.xml' path='doc/member[@name="LuaJit.luaL_findtable"]/*' />

    foreach (var (name, stream) in files)
    {
        stream.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(stream);
        var content = reader.ReadToEnd();
        
        content = Regex.Replace(
            
            $"""
            using unsafe lua_CFunction = delegate* unmanaged[Cdecl, SuppressGCTransition]<Maxine.Extensions.{className}.lua_State*, int>;
            using unsafe lua_Alloc = delegate* unmanaged[Cdecl, SuppressGCTransition]<void*, void*, nuint, nuint, void*>;
            using unsafe lua_Reader = delegate* unmanaged[Cdecl]<Maxine.Extensions.{className}.lua_State*, void*, nuint*, byte*>;
            using unsafe lua_Writer = delegate* unmanaged[Cdecl]<Maxine.Extensions.{className}.lua_State*, void*, nuint, void*, int>;
            using unsafe lua_Hook = delegate* unmanaged[Cdecl]<Maxine.Extensions.{className}.lua_State*, Maxine.Extensions.{className}.lua_Debug*, void>;
            
            // ReSharper disable All
            
            {content}
            """,
            """
            (?<whitespace> *)/// <include file='(?<file>[^']+)' path='doc/member\[@name="(?:\w+\.)?(?<member>[^"]+)"\]\/\*' \/>\r?\n
            """,
            match =>
            {
                var whitespace = match.Groups["whitespace"].ValueSpan;
                var member = match.Groups["member"].ValueSpan;
                if (member.StartsWith("_"))
                {
                    member = member[1..]; // Strip leading underscore added during remapping
                }
                if (docs.TryGetAlternateLookup<ReadOnlySpan<char>>(out var lookup) && lookup.TryGetValue(member, out var doc))
                {
                    return $"{whitespace}{string.Join($"\n{whitespace}", doc.Split("\r\n"))}".TrimEnd() + "\n";
                }

                Console.WriteLine($"Warning: No documentation found for key '{member}'");
                return "";
            });
        
        Console.WriteLine(Path.Combine(outputPath, Path.GetFileName(name)));
        File.WriteAllText(Path.Combine(outputPath, Path.GetFileName(name)), content);
    }
}
