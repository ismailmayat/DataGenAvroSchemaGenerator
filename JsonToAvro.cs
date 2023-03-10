using System.Text.RegularExpressions;
using DataGenAvroSchemaGenerator.Builder;

namespace DataGenAvroSchemaGenerator;

public class JsonToAvro
{
    private readonly JsonBuilder _jsonBuilder;
    private readonly ClassFromJson _classFromJson;
    private readonly SchemaFromType _schemaFromType;
    private readonly TypeAssembly _typeAssembly;

    public JsonToAvro(JsonBuilder jsonBuilder,ClassFromJson classFromJson,SchemaFromType schemaFromType, TypeAssembly typeAssembly)
    {
        _jsonBuilder = jsonBuilder;
        _classFromJson = classFromJson;
        _schemaFromType = schemaFromType;
        _typeAssembly = typeAssembly;
    }

    public string ConvertJsonToDataGenAvro(string json, string typeName)
    {
        var type = _classFromJson.BuildClassDefinition(json, typeName);

        var types =  BuildTypes(type);

        var assembly = _typeAssembly.BuildTypeAssembly(types);

        String schema = string.Empty;

        if (assembly != null)
        {
            schema = _schemaFromType.BuildSchemaStringFromType(assembly, typeName);
        }
        else
        {
            throw new Exception($"failed to create type and assembly for {type}");
        }
        
        var outputJson = _jsonBuilder.Enrich(schema);
        
        return outputJson;
    }

    private static List<string> BuildTypes(string type)
    {
        var types = new List<string>();
        //do we have more than one type generated
        if (Regex.Matches(type, "class").Count > 0)
        {
            string pattern =
                @"(public|private|protected|internal|static|\s)*\s*(class)\s+(\w+)\s*{[^{}]*({[^{}]*}[^{}]*)*}";
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

        return types;
    }
}