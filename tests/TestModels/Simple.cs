using Newtonsoft.Json;

namespace tests.TestModels;

internal class ArgProperties
{
    public List<object> options { get; set; }
}

internal class Field
{
    public string name { get; set; }
    public Type type { get; set; }
}

internal class Simple
{
    public string type { get; set; }
    public string name { get; set; }
    public List<Field> fields { get; set; }
}

internal class Type
{
    public string type { get; set; }

    [JsonProperty("arg.properties")]
    public ArgProperties argproperties { get; set; }
}
