namespace CoreCI.Web.Api.v1.Controllers
{
    using System;
    using System.Collections.Generic;
    using Api.Models;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Requests;

    /// <summary>
    /// Provides access to the Jobs resource
    /// </summary>
    /// <inheritdoc />
    [ ApiVersion( "1.0" ) ]
    [ Route( "api/v{version:apiVersion}/[controller]" ) ]
    public class JobsController : Controller
    {
        /// <summary>
        /// Lists jobs up for grabs
        /// </summary>
        /// <returns></returns>
        [ HttpGet ]
        public IActionResult Index( GetJobsRequest request )
        {
            var newGuid = Guid.NewGuid();
            return Json( new PagedResponse<JobsDto>
            {
                Page = request.Page,
                PageLength = 1,
                Size = 1,
                Values = new List<JobsDto>
                {
                    new JobsDto
                    {
                        Environment = BuildEnvironment.Any,
                        JobId = newGuid,
                        Links = new Dictionary<string, Link>
                        {
                            { "details", new Link( Url.Action( "Details", "Jobs", new { jobId = newGuid }, Request.Scheme ) ) }
                        }
                    }
                },
                Next = Url.Action( "Index", "Jobs", new { page = request.Page + 1 }, Request.Scheme )
            } );
        }

        /// <summary>
        /// Lists detailed information regarding a job
        /// </summary>
        /// <returns></returns>
        [ HttpGet ]
        [ Route( "{jobId:Guid}" ) ]
        public IActionResult Details( Guid jobId )
        {
            return Json( new JobsDto() );
        }
    }
}
