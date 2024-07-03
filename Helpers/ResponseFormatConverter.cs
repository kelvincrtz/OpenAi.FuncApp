using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenAi.FuncApp.Data.Response;

namespace OpenAi.FuncApp.Helpers
{
    public class ResponseFormatConverter : JsonConverter<ResponseFormat>
    {
        public override ResponseFormat ReadJson(JsonReader reader, Type objectType, ResponseFormat existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            if (token.Type == JTokenType.String)
            {
                return new ResponseFormat { Type = token.ToString() };
            }
            else if (token.Type == JTokenType.Object)
            {
                return token.ToObject<ResponseFormat>();
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, ResponseFormat value, JsonSerializer serializer)
        {
            if (value.Type != null)
            {
                writer.WriteValue(value.Type);
            }
            else
            {
                serializer.Serialize(writer, value);
            }
        }
    }
}