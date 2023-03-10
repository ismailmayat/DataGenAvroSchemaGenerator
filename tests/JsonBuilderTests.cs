using DataGenAvroSchemaGenerator.Builder;
using FluentAssertions;

namespace tests;

[TestFixture]
public class JsonBuilderTests
{
    [Test]
    public void Can_Build_With_Nested_Types()
    {
        var jsonBuilder = new JsonBuilder();
        
        var nestedSchema = "{\"type\":\"record\"," +
                           "          \"name\":\"Nested\"," +
                           "\"fields\":[{\"name\":\"StringProperty\"," +
                           "\"type\":\"string\"}," +
                           "{\"name\":\"IntegerProperty\"," +
                           "\"type\":\"int\"}," +
                           "{\"name\":\"NestedProperty\"," +
                           "\"type\":{\"type\":\"record\",\"name\":\"NestedProperty\",\"fields\":[{\"name\":\"InnerString\",\"type\":\"string\"},{\"name\":\"InnerInt\",\"type\":\"int\"}]}}]}"
            ;
        
        var result = jsonBuilder.Enrich(nestedSchema);
        
        var expected = Expected.NestedExpected;

        result.Should().Be(expected);
    }
    
}