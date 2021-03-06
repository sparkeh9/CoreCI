﻿namespace CoreCI.Common.Models.Jobs
{
    using MongoDB.Bson.Serialization.Attributes;
    using Vcs;

    [ BsonDiscriminator( nameof( GitVcsJob ) ) ]
    public class GitVcsJob : VcsJob
    {
        public override VcsType VcsType => VcsType.Git;
        public string Url { get; set; }
        public BasicAuthenticationCredentials BasicAuthenticationCredentials { get; set; }
    }
}
