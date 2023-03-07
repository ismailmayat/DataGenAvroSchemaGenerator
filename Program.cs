// See https://aka.ms/new-console-template for more information

using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using DataGenAvroSchemaGenerator.Builder;
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

var classBuilder = new ClassFromJson();

var type = classBuilder.BuildClassDefinition(json, typeName);

var types = new List<string>();

//do we have more than one type generated
if (Regex.Matches(type, "class").Count > 0)
{
    string pattern = @"(public|private|protected|internal|static|\s)*\s*(class)\s+(\w+)\s*{[^{}]*({[^{}]*}[^{}]*)*}";
    Regex regex = new Regex(pattern);
    MatchCollection matches = regex.Matches(type);

    foreach (Match match in matches)
    {
        string classDefinition = match.Groups[0].Value;
        types.Add(classDefinition);
    }
}
else
{

    types.Add(type);
}

var typeAssemblyBuilder = new TypeAssembly();

var assembly = typeAssemblyBuilder.BuildTypeAssembly(types);

String schema = string.Empty;

if (assembly != null)
{
    var schemaBuilder = new SchemaFromType();
    schema = schemaBuilder.BuildSchemaStringFromType(assembly, typeName);
}
else
{
    throw new Exception($"failed to create type and assembly for {type}");
}

var jsonBuilder = new JsonBuilder();

var outputJson = jsonBuilder.Enrich(schema);


Console.WriteLine(outputJson);




