namespace CoreCI.Common.Data.Repository.Implementation
{
    using System.Threading;
    using System.Threading.Tasks;
    using MongoDb.Project;
    using MongoDB.Bson;
    using MongoDB.Driver;

    public class ProjectRepository : IProjectRepository
    {
        private readonly IMongoCollection<ProjectBase> projectCollection;

        public ProjectRepository( IMongoCollection<ProjectBase> projectCollection )
        {
            this.projectCollection = projectCollection;
        }

        public async Task<ProjectBase> FindByIdAsync( ObjectId id, CancellationToken cancellationToken = new CancellationToken() )
        {
            return await projectCollection.Find( x => x.Id == id )
                                          .SingleOrDefaultAsync( cancellationToken );
        }

        public async Task CreateAsync( ProjectBase project, CancellationToken cancellationToken = new CancellationToken() )
        {
            await projectCollection.InsertOneAsync( project, cancellationToken : cancellationToken );
        }
    }
}
