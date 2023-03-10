using Newtonsoft.Json;

namespace tests.TestModels.Complex;

public class ArgProperties
{
    public List<object> options { get; set; }
}

public class Field
{
    public string name { get; set; }
    public System.Type type { get; set; }
}

public class Nested
{
    public string type { get; set; }
    public string name { get; set; }
    public List<Field> fields { get; set; }
}

public class Type
{
    public object type { get; set; }

    [JsonProperty("arg.properties")]
    public ArgProperties argproperties { get; set; }
}

