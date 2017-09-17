namespace CoreCI.Common.Common.Newtonsoft.Json
{
    using System;
    using global::Newtonsoft.Json;
    using global::Newtonsoft.Json.Linq;
    using Models;
    using Models.Jobs;

    public class VcsJobConverter : JsonConverter
    {
        public override bool CanWrite => false;
        public override bool CanRead => true;

        public override bool CanConvert( Type objectType )
        {
            return objectType == typeof( VcsJob );
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

            VcsJob vcsType;

            switch ( value )
            {
                case VcsType.Git:
                    vcsType = new GitVcsJob();
                    break;
                case VcsType.BitBucketGit:
                    vcsType = new BitBucketGitVcsJob();
                    break;
                default:
                    throw new ArgumentOutOfRangeException( nameof( VcsType ) );
            }
            serializer.Populate( jsonObject.CreateReader(), vcsType );
            return vcsType;
        }
    }
}
