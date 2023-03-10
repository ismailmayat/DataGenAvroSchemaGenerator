# DataGenAvroSchemaGenerator
Console application that can be used to generate a DataGen Connector avro schema template from a json file.

The [DateGen](https://www.confluent.io/hub/confluentinc/kafka-connect-datagen) source connector can be used to generate mock data to Kafka.

You can generate an [avro schema](https://github.com/confluentinc/kafka-connect-datagen/blob/master/src/main/resources/product.avro) the format is based on [Avro Random Generatpr](https://github.com/confluentinc/avro-random-generator) and pass that into the connector.  

This tool can be used to generate a template schema for a json payload that you want to create mock data for. For example for json payload in a file at location /tmp/user.json:

```
{
  "firstName": "John",
  "lastName": "Smith",
  "age": 25
}
```
You can run the command,

```
./DataGenAvroSchemaGenerator /tmp/user.json User
```
Note, the second parameter for the type must start with capital letter as it is used to dynamically generate a class
This will give you 

```
{
  "type": "record",
  "name": "User",
  "fields": [

    {
      "name": "FirstName",
      "type": {
        "type": "string",
        "arg.properties": {
          "options": []
        }
      }
    },

    {
      "name": "LastName",
      "type": {
        "type": "string",
        "arg.properties": {
          "options": []
        }
      }
    },

    {
      "name": "Age",
      "type": {
        "type": "int",
        "arg.properties": {
          "options": []
        }
      }
    }
  ]
}
```

In the generated template json in the options field you can fill in the values you require for the properties.