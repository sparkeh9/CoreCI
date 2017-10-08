namespace CoreCI.Web.Api.v1.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Common.Data.Repository;
    using Common.Data.Repository.Model;
    using Common.Extensions;
    using Common.Models;
    using Common.Models.Jobs;
    using Microsoft.AspNetCore.Mvc;
    using MongoDB.Bson;
    using Requests;

    /// <summary>
    ///     Provides access to the Jobs resource
    /// </summary>
    /// <inheritdoc />
    [ ApiVersion( "1.0" ) ]
    [ Route( "api/v{version:apiVersion}/[controller]" ) ]
    public class JobsController : Controller
    {
        private readonly IJobRepository jobRepository;

        public JobsController( IJobRepository jobRepository )
        {
            this.jobRepository = jobRepository;
        }

        /// <summary>
        ///     Lists jobs up for grabs
        /// </summary>
        /// <returns></returns>
        [ HttpGet ]
        public async Task<IActionResult> Index( [ FromQuery ] GetJobsRequest request )
        {
            var results = await jobRepository.ListByAsync( new JobQuery
            {
                BuildEnvironments = request.Environments,
                Page = request.Page,
                JobStatus = request.JobStatus
            } );

            return Json( new PagedResponse<JobDto>
            {
                Page = request.Page,
                Next = Url.Action( "Index", "Jobs", new { page = request.Page + 1 }, Request.Scheme ),
                Previous = request.Page <= 1 ? null : Url.Action( "Index", "Jobs", new { page = request.Page - 1 }, Request.Scheme ),
                Values = results.Select( x => new JobDto
                {
                    Environment = x.Environment,
                    JobId = x.Id,
                    Links = new Dictionary<string, Link>
                    {
                        { "details", new Link( Url.Action( "Details", "Jobs", new { jobId = x.Id }, Request.Scheme ) ) }
                    }
                } ).ToList()
            } );
        }

        /// <summary>
        ///     Lists detailed information regarding a job
        /// </summary>
        /// <returns></returns>
        [ HttpGet ]
        [ Route( "{jobId:ObjectId}" ) ]
        public async Task<IActionResult> Details( string jobId )
        {
            var objectId = ObjectId.Parse( jobId );

            var job = await jobRepository.FindByIdAsync( objectId );

            if ( job == null )
            {
                return NotFound();
            }

            return Json( new JobDto
            {
                Environment = job.Environment,
                JobId = job.Id,
                Data = job.Data,
                BuildAgentToken = job.BuildAgentToken
            } );
        }

        /// <summary>
        ///     Reserves a job for the calling build agent
        /// </summary>
        /// <returns></returns>
        [ HttpPost ]
        [ Route( "{jobId:ObjectId}/reserve" ) ]
        public async Task<IActionResult> Reserve( string jobId )
        {
            var objectId = ObjectId.Parse( jobId );
            var job = await jobRepository.FindByIdAsync( objectId );

            if ( !job.BuildAgentToken.IsNullOrWhiteSpace() )
            {
                return NotFound();
            }

            job.BuildAgentToken = Guid.NewGuid().ToString( "N" );
            job.JobStatus = JobStatus.Reserved;
            await jobRepository.PersistAsync( job );

            return Json( new JobReservedDto
            {
                JobId = job.Id,
                BuildAgentToken = job.BuildAgentToken
            } );
        }

        /// <summary>
        ///     Reserves a job for the calling build agent
        /// </summary>
        /// <returns></returns>
        [ HttpPost ]
        [ Route( "report" ) ]
        public async Task<IActionResult> Report( [ FromBody ] JobProgressDto jobProgress )
        {
            await jobRepository.AppendReportProgressAsync( jobProgress );
            return Ok();
        }
    }
}
