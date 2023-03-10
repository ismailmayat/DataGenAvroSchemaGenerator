using DataGenAvroSchemaGenerator;
using DataGenAvroSchemaGenerator.Builder;
using FluentAssertions;
using Newtonsoft.Json;
using tests.TestModels;

namespace tests;

public class Tests
{
    private JsonToAvro _jsonToAvro;
    
    [SetUp]
    public void Setup()
    {
        _jsonToAvro = new JsonToAvro(new JsonBuilder(), new ClassFromJson(), new SchemaFromType(), new TypeAssembly());
    }

    [Test]
    public void Can_Convert_Single_Level_Json()
    {
        string singleLevelJson = @"{""stringProperty"":""test"",""integerProperty"":1}";
       
        string type = "Simple";

        var json = _jsonToAvro.ConvertJsonToDataGenAvro(singleLevelJson, type);
  
        var result = JsonConvert.DeserializeObject<Simple>(json);

        result.name.Should().Be(type);
        result.type.Should().Be("record");
        result.fields[0].name.Should().Be("StringProperty");
        result.fields[0].type.type.Should().Be("string");
        result.fields[1].name.Should().Be("IntegerProperty");
        result.fields[1].type.type.Should().Be("int");
    }

    [Test]
    public void Can_Convert_Nested_Json()
    {
        
    }

}