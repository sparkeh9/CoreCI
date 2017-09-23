namespace CoreCI.Web.Infrastructure.Modules
{
    using Autofac;
    using Common.Data.MongoDb;
    using Common.Data.MongoDb.Project;
    using Common.Data.Repository;
    using Common.Data.Repository.Implementation;
    using Common.Models.Jobs;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Conventions;
    using MongoDB.Driver;
    using Options;

    public class MongoDbModule : Module
    {
        private readonly MongoDbOptions mongoDbOptions;

        public MongoDbModule( MongoDbOptions mongoDbOptions )
        {
            this.mongoDbOptions = mongoDbOptions;
        }

        protected override void Load( ContainerBuilder builder )
        {
            ConventionRegistry.Register( mongoDbOptions.DatabaseName, new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new EnumRepresentationConvention( BsonType.String ),
                new IgnoreExtraElementsConvention( true )
            }, t => true );

            builder.Register( cc => new MongoClient( mongoDbOptions.ConnectionString ) )
                   .AsSelf()
                   .InstancePerDependency();

            builder.Register( cc => cc.Resolve<MongoClient>()
                                      .GetDatabase( mongoDbOptions.DatabaseName )
                                      .GetCollection<ProjectBase>( new GitProject().CollectionName ) )
                   .As<IMongoCollection<ProjectBase>>()
                   .InstancePerDependency();

            builder.Register( cc => cc.Resolve<MongoClient>()
                                      .GetDatabase( mongoDbOptions.DatabaseName )
                                      .GetCollection<Job>( new Job().CollectionName ) )
                   .As<IMongoCollection<Job>>()
                   .InstancePerDependency();

            BsonClassMap.RegisterClassMap<ProjectBase>();
            BsonClassMap.RegisterClassMap<GitProject>();
            BsonClassMap.RegisterClassMap<BitBucketGitProject>();


            BsonClassMap.RegisterClassMap<VcsJob>();
            BsonClassMap.RegisterClassMap<BitBucketGitVcsJob>();
            BsonClassMap.RegisterClassMap<GitVcsJob>();


            builder.RegisterType<ProjectRepository>().As<IProjectRepository>();
            builder.RegisterType<JobRepository>().As<IJobRepository>();
        }
    }
}
