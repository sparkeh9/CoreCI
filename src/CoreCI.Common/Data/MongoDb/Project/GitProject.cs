namespace CoreCI.Common.Data.MongoDb.Project
{
    using Models;
    using Models.Vcs;
    using MongoDB.Bson.Serialization.Attributes;

    [ BsonDiscriminator( nameof( VcsType.Git ) ) ]
    public class GitProject : ProjectBase
    {
        public override VcsType VcsType => VcsType.Git;
        public string Url { get; set; }
        public BasicAuthenticationCredentials BasicAuthenticationCredentials { get; set; }
    }
}
