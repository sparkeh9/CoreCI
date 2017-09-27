namespace CoreCI.Common.Data.Repository
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Model;
    using Models.Jobs;
    using MongoDb;
    using MongoDB.Bson;

    public interface IJobRepository
    {
        Task<Job> FindByIdAsync( ObjectId id, CancellationToken cancellationToken = new CancellationToken() );
        Task CreateAsync( Job job, CancellationToken cancellationToken = new CancellationToken() );
        Task<IReadOnlyCollection<Job>> ListByAsync( JobQuery query, CancellationToken cancellationToken = new CancellationToken() );
        Task AppendReportProgressAsync( JobProgressDto jobProgress, CancellationToken cancellationToken = new CancellationToken() );
        Task PersistAsync( Job job, CancellationToken cancellationToken = new CancellationToken() );
    }
}
