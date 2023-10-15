using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace EquatableGenerator;

[Generator]
internal class EquatableGenerator : ISourceGenerator
{
    private const string attributeText = """
        using System;
        namespace CodeGenerationSample;

        [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
        sealed class ImplementEquatableAttribute : Attribute
        {
            public ImplementEquatableAttribute() { }
        }
        """;

    public void Initialize(GeneratorInitializationContext context)
    {
        //#if DEBUG
        //            if (!Debugger.IsAttached)
        //            {
        //                Debugger.Launch();
        //            }
        //#endif
        Debug.WriteLine("Initialize Code Generator");
        // 注册一个语法接收器，该接收器将在每次生成时创建
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        Debug.WriteLine("Execute code generator");
        // add the attribute text
        context.AddSource("ImplementEquatableAttribute", SourceText.From(attributeText, Encoding.UTF8));
        //context.AddSource("ImplementEquatableAttribute.generated.cs", SourceText.From(attributeText, Encoding.UTF8));

        if (context.SyntaxReceiver is not SyntaxReceiver syntaxReceiver)
            return;

        CSharpParseOptions? options = (context.Compilation as CSharpCompilation)?.SyntaxTrees[0].Options as CSharpParseOptions;
        Compilation compilation = context.Compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(SourceText.From(attributeText, Encoding.UTF8), options));

        INamedTypeSymbol? attributeSymbol = compilation.GetTypeByMetadataName("CodeGenerationSample.ImplementEquatableAttribute");

        //接下来，检查 SyntaxReceiver 中存储的每个候选类是否应用了ImplementEquatableAttribute。
        //如果某个类应用了这个特性，就把类型符号添加到 typedSymbols 集合中。
        List<INamedTypeSymbol> typeSymbols = new();
        foreach (ClassDeclarationSyntax @class in syntaxReceiver.CandidateClasses)
        {
            SemanticModel model = compilation.GetSemanticModel(@class.SyntaxTree);

            INamedTypeSymbol? typeSymbol = model.GetDeclaredSymbol(@class);
            if (typeSymbol!.GetAttributes().Any(attr => attr.AttributeClass!.Equals(attributeSymbol, SymbolEqualityComparer.Default)))
            {
                typeSymbols.Add(typeSymbol);
            }
        }
        //在选代候选类后，选代剩下的候选类来添加它们的源代码。
        foreach (INamedTypeSymbol typeSymbol in typeSymbols)
        {
            string classSource = GetClassSource(typeSymbol);
            context.AddSource(typeSymbol.Name, SourceText.From(classSource, Encoding.UTF8));
            //context.AddSource($"{typeSymbol.Name}.generated.cs", SourceText.From(classSource, Encoding.UTF8));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="typeSymbol">在生成的代码中使用名称空间的名称和类的名称</param>
    /// <returns></returns>
    private string GetClassSource(ITypeSymbol typeSymbol)
    {
        string namespaceName = typeSymbol.ContainingNamespace.ToDisplayString();

        string source = $$"""
            using System;

            namespace {{namespaceName}};

            public partial class {{typeSymbol.Name}} : IEquatable<{{typeSymbol.Name}}>
            {
                private static partial bool IsTheSame({{typeSymbol.Name}}? left, {{typeSymbol.Name}}? right);

                public override bool Equals(object? obj) => this == obj as {{typeSymbol.Name}};

                public bool Equals({{typeSymbol.Name}}? other) => this == other;

                public static bool operator==({{typeSymbol.Name}}? left, {{typeSymbol.Name}}? right) => 
                    IsTheSame(left, right);

                public static bool operator!=({{typeSymbol.Name}}? left, {{typeSymbol.Name}}? right) =>
                    !(left == right);
            }
            """;
        return source.ToString();
    }
}

/// <summary>
/// Created on demand before each generation pass
/// </summary>
internal class SyntaxReceiver : ISyntaxReceiver
{
    public List<ClassDeclarationSyntax> CandidateClasses { get; } = new();

    /// <summary>
    /// 为编译中的每个语法节点调用，我们可以检查这些节点并保存任何对生成有用的信息
    /// </summary>
    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        // 任何具有至少一个attribute的类都是方法生成的候选对象
        if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax
            && classDeclarationSyntax.AttributeLists.Count > 0)
        {
            CandidateClasses.Add(classDeclarationSyntax);
        }
    }
}