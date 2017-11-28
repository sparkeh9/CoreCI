namespace CoreCI.Common.Models.Projects
{
    using Common.Newtonsoft.Json;
    using MongoDB.Bson;
    using Newtonsoft.Json;

    public class ProjectDto
    {
        [ JsonConverter( typeof( MongoObjectIdConverter ) ) ]
        public ObjectId Id { get; set; }

        public BuildEnvironmentOs Environment { get; set; }
        public VcsType VcsType { get; set; }        public string Name { get; set; }
        public string Description { get; set; }
    }
}
