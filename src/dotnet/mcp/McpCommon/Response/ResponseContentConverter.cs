// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Azure.AI.Language.MCP.Common.Response
{
    /// <summary>
    /// Converter for serializing and deserializing IResponseContent types.
    /// </summary>
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
            throw new NotImplementedException("Deserialization for IResponseContent is not implemented.");
        }
    }
}