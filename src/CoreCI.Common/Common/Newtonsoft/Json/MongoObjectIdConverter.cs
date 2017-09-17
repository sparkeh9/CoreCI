namespace CoreCI.Common.Common.Newtonsoft.Json
{
    using System;
    using global::Newtonsoft.Json;
    using global::Newtonsoft.Json.Linq;
    using MongoDB.Bson;

    public class MongoObjectIdConverter : JsonConverter
    {
        public override void WriteJson( JsonWriter writer, object value, JsonSerializer serializer )
        {
            serializer.Serialize( writer, value.ToString() );
        }

        public override object ReadJson( JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer )
        {
            var token = JToken.Load( reader );
            return new ObjectId( token.ToObject<string>() );
        }

        public override bool CanConvert( Type objectType )
        {
            return typeof( ObjectId ).IsAssignableFrom( objectType );
        }
    }
}
