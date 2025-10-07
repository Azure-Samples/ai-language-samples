// Copyright (c) Microsoft Corporation.
//  Licensed under the MIT License.

using Azure.AI.Language.Text;
using Newtonsoft.Json;

namespace Azure.AI.Language.MCP.Server.Utils.Converters
{
    /// <summary>
    /// Converts <see cref="ScriptCode"/> objects to and from JSON.
    /// Serializes the <see cref="ScriptCode"/> as a JSON object with a single property "scriptCode" in lowercase.
    /// </summary>
    public class ScriptCodeConverter : JsonConverter
    {
        /// <summary>
        /// Determines whether this converter can convert the specified object type.
        /// </summary>
        /// <param name="objectType">The type of the object to check.</param>
        /// <returns>True if the object type is <see cref="ScriptCode"/> or nullable <see cref="ScriptCode"/>; otherwise, false.</returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ScriptCode) || objectType == typeof(ScriptCode?);
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The deserialized object value.</returns>
        /// <exception cref="NotImplementedException">Always thrown as deserialization is not implemented.</exception>
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("scriptCode");
            writer.WriteValue(value?.ToString()?.ToLowerInvariant());
            writer.WriteEndObject();
        }
    }
}
