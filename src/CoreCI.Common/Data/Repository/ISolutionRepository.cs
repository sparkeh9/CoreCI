namespace CoreCI.Common.Data.Repository
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Model;
    using MongoDb;
    using MongoDB.Bson;

    public interface ISolutionRepository
    {
        Task<Solution> FindByIdAsync( ObjectId id, CancellationToken cancellationToken = new CancellationToken() );
        Task PersistAsync( Solution solution, CancellationToken cancellationToken = new CancellationToken() );
        Task<Solution> CreateAsync( Solution solution, CancellationToken cancellationToken = new CancellationToken() );
        Task<IReadOnlyCollection<Solution>> ListByAsync( GetSolutionsQuery query, CancellationToken cancellationToken = new CancellationToken() );
    }
}
