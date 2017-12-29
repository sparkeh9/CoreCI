namespace CoreCI.Common.Data.Repository.Implementation
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Extensions;
    using Infrastructure.Extensions;
    using Model;
    using Model.Projects;
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

        public async Task<IReadOnlyCollection<ProjectBase>> FindBySolutionIdAsync( ObjectId solutionId, CancellationToken cancellationToken = new CancellationToken() )
        {
            return await projectCollection.Find( x => x.Solution == solutionId )
                                          .ToListAsync( cancellationToken );
        }

        public async Task<IReadOnlyCollection<ProjectBase>> ListByAsync( GetProjectsQuery query, CancellationToken cancellationToken = new CancellationToken() )
        {
            return await GenerateListQueryExpression( query ).Skip( ( query.Page.EnsureAtLeast( 1 ) - 1 ) * 10 )
                                                             .Limit( 10 )
                                                             .ToListAsync( cancellationToken );
        }

        public async Task CreateAsync( ProjectBase project, CancellationToken cancellationToken = new CancellationToken() )
        {
            await projectCollection.InsertOneAsync( project, cancellationToken : cancellationToken );
        }


        private IFindFluent<ProjectBase, ProjectBase> GenerateListQueryExpression( GetProjectsQuery query )
        {
            var filter = BuildFilter( query );

            var sortOptions = query.Sort
                                   .Concat( new List<(string name, SortingDirection direction)>
                                                { ("_id", SortingDirection.Ascending) } )
                                   .Select( x => x.direction == SortingDirection.Ascending
                                                ? Builders<ProjectBase>.Sort.Ascending( x.name )
                                                : Builders<ProjectBase>.Sort.Descending( x.name ) );

            return projectCollection.Find( filter )
                                    .Sort( Builders<ProjectBase>.Sort.Combine( sortOptions ) );
        }

        private FilterDefinition<ProjectBase> BuildFilter( GetProjectsQuery query )
        {
            var expression = Builders<ProjectBase>.Filter.Empty;

            if ( !query.Solution.IsNullOrWhiteSpace() && ObjectId.TryParse( query.Solution, out var solutionObjectId ))
            {
                expression = expression & Builders<ProjectBase>.Filter.Where( x => x.Solution == solutionObjectId );
            }

            if ( !query.Name.IsNullOrWhiteSpace() )
            {
                expression = expression & Builders<ProjectBase>.Filter.Text( query.Name, new TextSearchOptions
                {
                    CaseSensitive = false,
                    DiacriticSensitive = false,
                    Language = "en"
                } );
            }

            return expression;
        }
    }
}
