// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Azure.AI.Language.MCP.Server.Response
{
    public class ResponseContentConverter : JsonConverter<IResponseContent>
    {
        public override void WriteJson(JsonWriter writer, IResponseContent value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            serializer.Serialize(writer, value, value.GetType());
        }

        public override IResponseContent ReadJson(JsonReader reader, Type objectType, IResponseContent existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return default;

            var obj = JObject.Load(reader);
            // You may need to implement a type discriminator here.
            // For now, throw to indicate this must be implemented.
            throw new NotImplementedException("Deserialization for IResponseContent is not implemented. Add type discriminator logic here.");
        }
    }
}