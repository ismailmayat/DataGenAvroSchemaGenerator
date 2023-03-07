using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DataGenAvroSchemaGenerator.Builder;

public class TypeAssembly
{
    public Assembly? BuildTypeAssembly(List<string> classDefinitions)
    {


        SyntaxTree[] sourceTrees = new SyntaxTree[classDefinitions.Count];

        int i=0;
        foreach (var item in classDefinitions)
        {
            sourceTrees[i] = CSharpSyntaxTree.ParseText(item);
            i++;
        }
    
        CSharpCompilation compilation = CSharpCompilation.Create("MyAssembly")
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddSyntaxTrees(sourceTrees);

        Assembly assembly1 = null;

        using (var ms = new System.IO.MemoryStream())
        {
            var result = compilation.Emit(ms);
            if (!result.Success)
            {
                // Handle compilation errors
                foreach (Diagnostic diagnostic in result.Diagnostics)
                {
                    Console.WriteLine(diagnostic.ToString());
                }
            }
            else
            {
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                assembly1 = Assembly.Load(ms.ToArray());
            }
        }

        return assembly1;
    }
}