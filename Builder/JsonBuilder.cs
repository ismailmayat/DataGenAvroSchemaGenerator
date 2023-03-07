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
}