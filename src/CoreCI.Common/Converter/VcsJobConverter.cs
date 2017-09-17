namespace CoreCI.Common.Converter
{
    using System;
    using Models;
    using Models.Jobs;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class VcsJobConverter : JsonConverter
    {
        public override bool CanWrite => false;
        public override bool CanRead => true;

        public override bool CanConvert( Type objectType )
        {
            return objectType == typeof( IVcsJob );
        }

        public override void WriteJson( JsonWriter writer, object value, JsonSerializer serializer )
        {
            throw new InvalidOperationException( "Use default serialization." );
        }

        public override object ReadJson( JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer )
        {
            var jsonObject = JObject.Load( reader );

            if ( !Enum.TryParse( jsonObject.GetValue( nameof( VcsType ), StringComparison.OrdinalIgnoreCase ).Value<string>(), out VcsType value ) )
            {
                throw new ArgumentOutOfRangeException( nameof( VcsType ) );
            }

            IVcsJob vcsType;

            switch ( value )
            {
                case VcsType.Git:
                    vcsType = new GitVcsJob();
                    break;
                case VcsType.BitBucketGit:
                    vcsType = new BitBucketVcsJob();
                    break;
                default:
                    throw new ArgumentOutOfRangeException( nameof( VcsType ) );
            }
            serializer.Populate( jsonObject.CreateReader(), vcsType );
            return vcsType;
        }
    }
}
