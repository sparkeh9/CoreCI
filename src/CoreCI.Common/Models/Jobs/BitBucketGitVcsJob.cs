namespace CoreCI.Common.Models.Jobs
{
    using MongoDB.Bson.Serialization.Attributes;
    using Vcs;

    [ BsonDiscriminator( nameof( BitBucketGitVcsJob ) ) ]
    public class BitBucketGitVcsJob : GitVcsJob
    {
        public override VcsType VcsType => VcsType.BitBucketGit;
        public OAuth2Credentials OAuth2Credentials { get; set; }
    }
}
