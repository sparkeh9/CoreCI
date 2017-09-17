namespace CoreCI.Common.Data.MongoDb.Project
{
    using Models;
    using Models.Vcs;
    using MongoDB.Bson.Serialization.Attributes;

    [ BsonDiscriminator( nameof( VcsType.BitBucketGit ) ) ]
    public class BitBucketGitProject : ProjectBase
    {
        public override VcsType VcsType => VcsType.BitBucketGit;
        public string Url { get; set; }
        public BasicAuthenticationCredentials BasicAuthenticationCredentials { get; set; }
        public OAuth2Credentials OAuth2Credentials { get; set; }
    }
}
