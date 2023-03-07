using System.Text.RegularExpressions;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;

namespace DataGenAvroSchemaGenerator.Builder;

public class ClassFromJson
{
    //takes json payload string and creates json schema and from that creates a c# class definition
    public string BuildClassDefinition(string json, string typeName)
    {
        var jsonSchema = JsonSchema.FromSampleJson(json);

        var generatorSettings = new CSharpGeneratorSettings
        {
            Namespace = "Sample",
            GenerateDataAnnotations = false
        };

        var generator = new CSharpGenerator(jsonSchema, generatorSettings);

        var generatedType = generator.GenerateFile(jsonSchema, typeName);

        var reg = new Regex(@"\[[^\]]*\]"); //use to remove any newtonsoft json in the generated class

        var type = reg.Replace(generatedType, string.Empty);
    
        string privateVarToMatch = @"private System.Collections.Generic.IDictionary<string, object> _additionalProperties;";

        string publicVarToMatch = @"public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }";

        type = type.Replace("ICollection", "List");
        
        return type.Replace("partial", String.Empty)
            .Replace(privateVarToMatch, string.Empty).Replace(publicVarToMatch,string.Empty);
    }
}