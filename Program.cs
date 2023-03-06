// See https://aka.ms/new-console-template for more information

using Newtonsoft.Json.Linq;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Text.RegularExpressions;
using Microsoft.Hadoop.Avro;
using Microsoft.Hadoop.Avro.Schema;
using Newtonsoft.Json;

string jsonFile = "";
string typeName ="";


if (Environment.GetEnvironmentVariable("DOCKER_CONTAINER") != null)
{
    jsonFile = Environment.GetEnvironmentVariable("JSONFILE");
    typeName = Environment.GetEnvironmentVariable("TYPENAME");
}
else
{
     jsonFile = args[0];
     typeName = args[1];
}

if (!File.Exists(jsonFile))
{
    throw new FileNotFoundException("File not found");
}

if (String.IsNullOrEmpty(typeName))
{
    throw new Exception("Enter type name");
}



string json = File.ReadAllText(jsonFile);

var type = BuildTypeDefinition(json, typeName);

var assembly = BuildTypeAssembly(type);

String schema = string.Empty;

if (assembly != null)
{
    schema = BuildSchemaStringFromType(assembly, typeName);
}

var parsedSchema = EnrichSchema(schema);

string outputJson = JsonConvert.SerializeObject(parsedSchema, Formatting.Indented).Replace("{},\\n",string.Empty);

Console.WriteLine(outputJson);

Object EnrichSchema(string schema)
{
    JObject jObject = JObject.Parse(schema);

    var fields = jObject.SelectTokens("$..fields").First().ToArray();

    for (int c = 0; c < fields.Length; c++)
    {
        var typeNode = fields[c];

        string type = typeNode["type"].ToString();

        string name = typeNode["name"].ToString();

        typeNode.Last.Remove();

        var newType = new JObject(
            new JProperty("type", type),
            new JProperty("arg.properties", new JObject(new JProperty("options", new JArray())))
        );

        var outerType = new JObject(
            new JProperty("name", name),
            new JProperty("type", newType));

        typeNode.AddAfterSelf(outerType);

        typeNode.First().Remove();
    }

    return jObject;
}

string BuildTypeDefinition(string s, string typeName)
{
    var jsonSchema = JsonSchema.FromSampleJson(s);

    var generatorSettings = new CSharpGeneratorSettings
    {
        Namespace = "Sample",
        GenerateDataAnnotations = false
    };

    var generator = new CSharpGenerator(jsonSchema, generatorSettings);

    var generatedType = generator.GenerateFile(jsonSchema, typeName);

    var reg = new Regex(@"\[[^\]]*\]"); //use to remove any newtonsoft json in the generated class

    var type1 = reg.Replace(generatedType, string.Empty);

    var addtionalPropsRegEx = new Regex("public System.Collections.*}", RegexOptions.IgnoreCase | RegexOptions.Singleline);

    type1 = addtionalPropsRegEx.Replace(type1,String.Empty);
    
    return type1.Replace("partial", String.Empty)
        .Replace("private System.Collections.Generic.IDictionary<string, object> _additionalProperties;", "}}");
}

Assembly? BuildTypeAssembly(string s1)
{
    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(s1);

    CSharpCompilation compilation = CSharpCompilation.Create("MyAssembly")
        .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
        .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
        .AddSyntaxTrees(syntaxTree);

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

string BuildSchemaStringFromType(Assembly assembly, string typeName1)
{
    string schema1;
    // Get the compiled type and create an instance of the class
    Type classType = assembly.GetType("Sample." + typeName1);

    AvroSerializerSettings settings = new AvroSerializerSettings
    {
        Resolver = new AvroPublicMemberContractResolver()
    };

    var method = typeof(AvroSerializer).GetMethod("Create", new Type[] { typeof(AvroSerializerSettings) })
        .MakeGenericMethod(new[] { classType });

    var createdTyped = method.Invoke(null, new object[] { settings });

    var writerSchema = createdTyped.GetType().GetProperty("WriterSchema");

    var actualValue = writerSchema.GetValue(createdTyped);

    var writerSchemaTyped = (TypeSchema)actualValue;

    schema1 = writerSchemaTyped.ToString();
    return schema1;
} 