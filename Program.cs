// See https://aka.ms/new-console-template for more information

using DataGenAvroSchemaGenerator;
using DataGenAvroSchemaGenerator.Builder;

string jsonFile = "";
string typeName ="";


if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != null)
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

var jsonToAvro = new JsonToAvro(new JsonBuilder(), new ClassFromJson(), new SchemaFromType(), new TypeAssembly());

var outputJson = jsonToAvro.ConvertJsonToDataGenAvro(json, typeName);

Console.WriteLine(outputJson);




