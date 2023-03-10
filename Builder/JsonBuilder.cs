using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DataGenAvroSchemaGenerator.Builder;

public class JsonBuilder
{
    public string Enrich(string schema)
    {
        var parsedSchema = EnrichSchema(schema);

        string outputJson = JsonConvert.SerializeObject(parsedSchema, Formatting.Indented).Replace("{},",string.Empty);

        return outputJson;
    }
    
    private Object EnrichSchema(string schema)
    {
    
        JObject jObject = JObject.Parse(schema);

        Modify(jObject);

        return jObject;
    }

    private static void Modify(JObject jObject)
    {
        var fields = jObject.SelectTokens("$..fields").First().ToArray();

        for (int c = 0; c < fields.Length; c++)
        {
            var typeNode = fields[c];

            string type = typeNode["type"].ToString();

            if (type.Contains("record"))
            {
                var record = JObject.Parse(type);   
                Modify(record);
                typeNode["type"] = record;
            }
            else
            {
                ModifyToken(typeNode);     
            }
            
        }
    }

    private static void ModifyToken(JToken typeNode)
    {
        string type = typeNode["type"].ToString();
        
        string name = typeNode["name"].ToString();

        var newType = BuildTypeObject(type);

        var outerType = BuildOuterTypeObject(name, newType);
            
        typeNode.Last.Remove();
        typeNode.AddAfterSelf(outerType);
        typeNode.First().Remove();


    }

    private static JObject BuildOuterTypeObject(string name, JObject newType)
    {
        var outerType = new JObject(
            new JProperty("name", name),
            new JProperty("type", newType));
        return outerType;
    }

    private static JObject BuildTypeObject(string type)
    {
        var newType = new JObject(
            new JProperty("type", type),
            new JProperty("arg.properties", new JObject(new JProperty("options", new JArray())))
        );
        return newType;
    }
}