using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using WorldXaml.Generator.Common;

namespace Maxine.Extensions.UnionGen;

[Generator(LanguageNames.CSharp)]
public class UnionIncrementalGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var unions = context.SyntaxProvider.ForAttributeWithMetadataName(
            "Maxine.Extensions.UnionGen.UnmanagedUnionAttribute",
            static (node, ct) => node is StructDeclarationSyntax,
            static (ctx, ct) =>
            {
                var node = (StructDeclarationSyntax)ctx.TargetNode;
                var semanticModel = ctx.SemanticModel;
                var typeSymbol = semanticModel.GetDeclaredSymbol(node)!;
                var attr = ctx.Attributes.First();
                
                return (
                    DeclaringNamespace: typeSymbol.ContainingNamespace
                        .ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                        .Remove(0, "global::".Length),
                    Hierarchy: GetTypeHierarchy(typeSymbol).ToArray(),
                    Types: attr.ConstructorArguments.FirstOrDefault().Values
                        .Select(v => v.ToCSharpString()["typeof(".Length..^")".Length])
                        .Distinct()
                        .ToArray(),
                    Nullable: attr.NamedArguments.FirstOrDefault(kv => kv.Key == "Nullable").Value.Value is true
                );
                
                IEnumerable<(string Type, bool TypeIsRecord, bool TypeIsStruct)> GetTypeHierarchy(ITypeSymbol type)
                {
                    while (true)
                    {
                        yield return (type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat), type.IsRecord, type.IsValueType);
                        if (type.ContainingType is { } containingType)
                        {
                            type = containingType;
                            continue;
                        }

                        break;
                    }
                }
            });
        
        context.RegisterSourceOutput(unions, static (context, union) =>
        {
            var declaringTypeNamespace = union.DeclaringNamespace;
                
            var sb = new IndentedStringBuilder();

            sb.AppendLine("#nullable enable");
            sb.AppendLine();
            
            sb.AppendLine("using System.Runtime.CompilerServices;");
            sb.AppendLine("using System.Runtime.InteropServices;");
            sb.AppendLine();
                
            sb.AppendLine($"namespace {declaringTypeNamespace};");
            sb.AppendLine();

            var iter = union.Hierarchy.Reverse();
            foreach (var type in iter.SkipLast(1))
            {
                sb.AppendLine($"partial {(type.TypeIsRecord ? "record" : type.TypeIsStruct ? "struct" : "class")} {type.Type}");
                sb.AppendLine("{");
                sb.IncrementIndent();
            }

            var last = iter.Last();
            sb.AppendLine("[StructLayout(LayoutKind.Explicit)]");
            sb.AppendLine($"partial {(last.TypeIsRecord ? "record" : last.TypeIsStruct ? "struct" : "class")} {last.Type} : IUnion");
            sb.AppendLine("{");
            using (sb.Indent())
            {
                sb.AppendLine("[FieldOffset(0)] private readonly sbyte __union_Kind;");

                if (union.Nullable)
                {
                    sb.AppendLine($"public {last.Type}()");
                    sb.AppendLine("{");
                    using (sb.Indent())
                    {
                        sb.AppendLine("__union_Kind = 0;");
                    }
                    sb.AppendLine("}");
                    sb.AppendLine();
                }

                foreach (var (index, type) in union.Types.Index())
                {
                    var simpleTypeName = type.LastIndexOf('.') is >= 0 and var v ? type[(v + 1)..] : type;
                    if (simpleTypeName.IndexOf('<') is >= 0 and var v2)
                    {
                        simpleTypeName = simpleTypeName[..v2];
                    }
                    sb.AppendLine($"[FieldOffset(8)] private readonly {type} __union_{index + 1}_{simpleTypeName};");
                    sb.AppendLine();
                    
                    sb.AppendLine($"public {last.Type}({type} value)");
                    sb.AppendLine("{");
                    using (sb.Indent())
                    {
                        sb.AppendLine($"__union_{index + 1}_{simpleTypeName} = value;");
                        sb.AppendLine($"__union_Kind = {index + 1};");
                    }
                    sb.AppendLine("}");
                    sb.AppendLine();
                    
                    sb.AppendLine($"public bool TryGetValue(out {type} value)");
                    sb.AppendLine("{");
                    using (sb.Indent())
                    {
                        sb.AppendLine($"if (__union_Kind == {index + 1})");
                        sb.AppendLine("{");
                        using (sb.Indent())
                        {
                            sb.AppendLine($"value = __union_{index + 1}_{simpleTypeName};");
                            sb.AppendLine("return true;");
                        }
                        sb.AppendLine("}");
                        sb.AppendLine();
                        sb.AppendLine("value = default!;");
                        sb.AppendLine("return false;");
                    }
                    sb.AppendLine("}");
                    sb.AppendLine();
                }
    
                sb.AppendLine("public object? Value => __union_Kind switch");
                sb.AppendLine("{");
                using (sb.Indent())
                {
                    foreach (var (index, type) in union.Types.Index())
                    {
                        var simpleTypeName = type.LastIndexOf('.') is >= 0 and var v ? type[(v + 1)..] : type;
                        if (simpleTypeName.IndexOf('<') is >= 0 and var v2)
                        {
                            simpleTypeName = simpleTypeName[..v2];
                        }
                        sb.AppendLine($"{index + 1} => __union_{index + 1}_{simpleTypeName},");
                    }
                    sb.AppendLine("_ => null");
                }
                sb.AppendLine("};");
                sb.AppendLine();
                sb.AppendLine("public bool HasValue => __union_Kind != 0;");
            }
            
            sb.AppendLine("}");;

            foreach (var type in iter.SkipLast(1))
            {
                sb.DecrementIndent();
                sb.AppendLine("}");
            }
            
            context.AddSource($"{declaringTypeNamespace.Replace('.', '_')}_{string.Join("_", iter.Select(t => t.Type))}.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        });
    }
}