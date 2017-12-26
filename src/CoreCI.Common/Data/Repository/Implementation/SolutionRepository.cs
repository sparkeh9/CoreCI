namespace CoreCI.Common.Data.Repository.Implementation
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Extensions;
    using Infrastructure.Extensions;
    using Model;
    using MongoDb;
    using MongoDB.Bson;
    using MongoDB.Driver;

    public class SolutionRepository : ISolutionRepository
    {
        private readonly IMongoCollection<Solution> solutionCollection;

        public SolutionRepository( IMongoCollection<Solution> solutionCollection )
        {
            this.solutionCollection = solutionCollection;
        }

        public async Task<Solution> FindByIdAsync( ObjectId id, CancellationToken cancellationToken = new CancellationToken() )
        {
            return await solutionCollection.Find( x => x.Id == id )
                                           .SingleOrDefaultAsync( cancellationToken );
        }

        public async Task PersistAsync( Solution solution, CancellationToken cancellationToken = new CancellationToken() )
        {
            await solutionCollection.ReplaceOneAsync( x => x.Id == solution.Id, solution, cancellationToken : cancellationToken );
        }

        public async Task<Solution> CreateAsync( Solution solution, CancellationToken cancellationToken = new CancellationToken() )
        {
            await solutionCollection.InsertOneAsync( solution, cancellationToken : cancellationToken );
            return solution;
        }

        public async Task<IReadOnlyCollection<Solution>> ListByAsync( GetSolutionsRequest query, CancellationToken cancellationToken = new CancellationToken() )
        {
            return await GenerateListQueryExpression( query ).Skip( ( query.Page.EnsureAtLeast( 1 ) - 1 ) * 10 )
                                                             .Limit( 10 )
                                                             .ToListAsync( cancellationToken );
        }

        private IFindFluent<Solution, Solution> GenerateListQueryExpression( GetSolutionsRequest query )
        {
            var filter = BuildFilter( query );

            var sortOptions = query.Sort
                                   .Concat( new List<(string name, SortingDirection direction)>
                                                { ("_id", SortingDirection.Ascending) } )
                                   .Select( x => x.direction == SortingDirection.Ascending
                                                ? Builders<Solution>.Sort.Ascending( x.name )
                                                : Builders<Solution>.Sort.Descending( x.name ) );

            return solutionCollection.Find( filter )
                                     .Sort( Builders<Solution>.Sort.Combine( sortOptions ) );
        }

        private FilterDefinition<Solution> BuildFilter( GetSolutionsRequest query )
        {
            var expression = Builders<Solution>.Filter.Empty;

            if ( !query.Name.IsNullOrWhiteSpace() )
            {
                expression = expression & Builders<Solution>.Filter.Text( query.Name, new TextSearchOptions
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
