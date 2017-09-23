﻿namespace CoreCI.Common.Data.Repository
{
    using System.Threading;
    using System.Threading.Tasks;
    using MongoDb.Project;
    using MongoDB.Bson;

    public interface IProjectRepository
    {
        Task<ProjectBase> FindByIdAsync( ObjectId id, CancellationToken cancellationToken = new CancellationToken() );
        Task CreateAsync( ProjectBase project, CancellationToken cancellationToken = new CancellationToken() );
    }
}