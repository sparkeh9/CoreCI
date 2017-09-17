namespace CoreCI.Common.Data.Repository.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure.Extensions;
    using LinqKit;
    using Model;
    using Models;
    using MongoDb;
    using MongoDB.Bson;
    using MongoDB.Driver;

    public class JobRepository : IJobRepository
    {
        private readonly IMongoCollection<Job> jobCollection;

        public JobRepository( IMongoCollection<Job> jobCollection )
        {
            this.jobCollection = jobCollection;
        }

        public async Task<Job> FindByIdAsync( ObjectId id, CancellationToken cancellationToken = new CancellationToken() )
        {
            return await jobCollection.Find( x => x.Id == id )
                                      .SingleOrDefaultAsync( cancellationToken );
        }

        public async Task CreateAsync( Job job, CancellationToken cancellationToken = new CancellationToken() )
        {
            await jobCollection.InsertOneAsync( job, cancellationToken : cancellationToken );
        }

        public async Task<IReadOnlyCollection<Job>> ListByAsync( JobQuery query, CancellationToken cancellationToken = new CancellationToken() )
        {
            var meh = await GenerateExpression( query ).Skip( ( query.Page.EnsureAtLeast( 1 ) - 1 ) * 10 )
                                                       .Limit( 10 )
                                                       .ToListAsync( cancellationToken );

            return meh;
        }

        private IFindFluent<Job, Job> GenerateExpression( JobQuery query )
        {
            var filter = BuildFilter( query );

            var sortOptions = query.Sort
                                   .Concat( new List<(string name, SortingDirection direction)>
                                   {
                                       ("_id", SortingDirection.Ascending)
                                   } )
                                   .Select( x => x.direction == SortingDirection.Ascending
                                                ? Builders<Job>.Sort.Ascending( x.name )
                                                : Builders<Job>.Sort.Descending( x.name ) );

            return jobCollection.Find( filter )
                                .Sort( Builders<Job>.Sort.Combine( sortOptions ) );
        }

        private Expression<Func<Job, bool>> BuildFilter( JobQuery query )
        {
            if ( !query.HasFilter )
                return x => true;

            var expression = PredicateBuilder.New<Job>();

            if ( query.BuildEnvironment.HasValue )
            {
                expression = expression.And( PredicateBuilder.New<Job>()
                                                             .Or( x => x.Environment == query.BuildEnvironment.Value ) );
            }

            if ( query.JobStatus.HasValue )
            {
                expression = expression.And( PredicateBuilder.New<Job>()
                                                             .Or( x => x.JobStatus == query.JobStatus.Value ) );
            }
            return expression;
        }
    }
}
