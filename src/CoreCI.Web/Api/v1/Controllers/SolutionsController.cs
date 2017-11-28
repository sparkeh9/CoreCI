namespace CoreCI.Web.Api.v1.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Data.MongoDb;
    using Common.Data.Repository;
    using Common.Data.Repository.Model;
    using Common.Models;
    using Common.Models.Projects;
    using Common.Models.Solutions;
    using Microsoft.AspNetCore.Mvc;
    using MongoDB.Bson;

    /// <summary>
    ///     Provides access to the Solutions resource
    /// </summary>
    /// <inheritdoc />
    [ ApiVersion( "1.0" ) ]
    [ Route( "api/v{version:apiVersion}/[controller]" ) ]
    public class SolutionsController : Controller
    {
        private readonly ISolutionRepository solutionRepository;
        private readonly IProjectRepository projectRepository;

        public SolutionsController( ISolutionRepository solutionRepository, IProjectRepository projectRepository )
        {
            this.solutionRepository = solutionRepository;
            this.projectRepository = projectRepository;
        }

        /// <summary>
        ///     Lists solutions
        /// </summary>
        /// <returns></returns>
        [ HttpGet ]
        public async Task<IActionResult> Index( GetSolutionsRequest request, CancellationToken cancellationToken )
        {
            var results = await solutionRepository.ListByAsync( request, cancellationToken );

            return Json( new PagedResponse<SolutionMinimalDto>
            {
                Page = request.Page,
                Next = Url.Action( "Index", "Solutions", new { page = request.Page + 1 }, Request.Scheme ),
                Previous = request.Page <= 1 ? null : Url.Action( "Index", "Solutions", new { page = request.Page - 1 }, Request.Scheme ),
                Values = results.Select( x => new SolutionMinimalDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Links = new Dictionary<string, Link>
                    {
                        { "details", new Link( Url.Action( "Details", "Solutions", new { jobId = x.Id }, Request.Scheme ) ) }
                    }
                } ).ToList()
            } );
        }

        /// <summary>
        ///     Lists detailed information for a Solution
        /// </summary>
        /// <returns></returns>
        [ HttpGet ]
        [ Route( "{solutionId:ObjectId}" ) ]
        public async Task<IActionResult> Details( string solutionId, CancellationToken cancellationToken )
        {
            var objectId = ObjectId.Parse( solutionId );
            var solution = await solutionRepository.FindByIdAsync( objectId, cancellationToken );
            var projects = await projectRepository.FindBySolutionIdAsync( objectId, cancellationToken );

            return Json( new SolutionDetailDto
            {
                Id = solution.Id,
                Name = solution.Name,
                Projects = projects.Select( x => new ProjectDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Environment = x.Environment,
                    VcsType = x.VcsType
                } )
            } );
        }

        /// <summary>
        ///     Reserves a job for the calling build agent
        /// </summary>
        /// <returns></returns>
        [ HttpPost ]
        [ Route( "add" ) ]
        public async Task<IActionResult> Add( [ FromBody ] SolutionMinimalDto solutionMinimalDto, CancellationToken cancellationToken )
        {
            await solutionRepository.CreateAsync( new Solution
            {
                Name = solutionMinimalDto?.Name
            }, cancellationToken );
            return Ok();
        }
    }
}
