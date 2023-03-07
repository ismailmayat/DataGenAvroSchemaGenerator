using System.Reflection;
using Microsoft.Hadoop.Avro;
using Microsoft.Hadoop.Avro.Schema;

namespace DataGenAvroSchemaGenerator.Builder;

public class SchemaFromType
{
    public string BuildSchemaStringFromType(Assembly assembly, string typeName1)
    {
        string schema1;
        // Get the compiled type and create an instance of the class
        Type classType = assembly.GetType(typeName1);

        AvroSerializerSettings settings = new AvroSerializerSettings
        {
            Resolver = new AvroPublicMemberContractResolver()
        };

        var method = typeof(AvroSerializer).GetMethod("Create", new Type[] { typeof(AvroSerializerSettings) })
            .MakeGenericMethod(new[] { classType });

        var createdTyped = method.Invoke(null, new object[] { settings });

        var writerSchema = createdTyped.GetType().GetProperty("WriterSchema");

        var actualValue = writerSchema.GetValue(createdTyped);

        var writerSchemaTyped = (TypeSchema)actualValue;

        schema1 = writerSchemaTyped.ToString();
        return schema1;
    } 
}