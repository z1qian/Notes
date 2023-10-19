using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Diagnostics;
using System.Text;

namespace HelloWorldGenerator;

//https://learn.microsoft.com/zh-cn/dotnet/csharp/roslyn-sdk/source-generators-overview
[Generator]
public class HelloWorldGenerator : ISourceGenerator
{
    /// <summary>
    /// Execute 完成生成器的主要工作
    /// </summary>
    /// <param name="context">用于在编译过程中访问语法树，这里可以访问使用了源代码生成器的代码，以便进行修改或者扩展</param>
    public void Execute(GeneratorExecutionContext context)
    {
        // Create the source code to inject into the users compilation
        StringBuilder sourceBuilder = new(
            """
            using System;
            namespace CodeGenerationSample;

            public static class HelloWorld
            {
                public static void Hello() 
                {
                    Console.WriteLine("Hello from generated code!");
                    Console.WriteLine("编译时存在以下源文件：");
            """);

        // using the context, get a list of files from the syntax trees in the users compilation
        IEnumerable<SyntaxTree> syntaxTrees = context.Compilation.SyntaxTrees;

        // add the filepath of each tree to the class we're building
        foreach (SyntaxTree tree in syntaxTrees)
        {
            //将编译的源文件的文件路径写入控制台
            sourceBuilder.AppendLine(
            $$"""
              
                    Console.WriteLine(@"source file: {{tree.FilePath}}");
            """);
        }

        // closing brackets to inject
        sourceBuilder.Append(
            """
                }
            }
            """);
        // inject the created source into the users compilation
        context.AddSource("helloWorld.generated.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
    }

    /// <summary>
    /// 在启动代码生成之前，先调用 Initialize 方法
    /// </summary>
    public void Initialize(GeneratorInitializationContext context)
    {
        //// 附加调试器进程
        //if (!Debugger.IsAttached)
        //{
        //    Debugger.Launch();
        //}
    }
}