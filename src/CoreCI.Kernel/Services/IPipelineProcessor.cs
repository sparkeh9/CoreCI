namespace CoreCI.Kernel.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Models;

    public interface IPipelineProcessor
    {
        Task<PipelineProcessorResult> ProcessAsync( Pipeline pipeline, CancellationToken ctx = default( CancellationToken ) );
        Task CleanupWorkspaceAsync( Guid id );
    }
}