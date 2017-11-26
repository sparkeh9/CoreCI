namespace CoreCI.Web.Api.v1.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Common.Data.MongoDb;
    using Common.Data.Repository;
    using Common.Data.Repository.Model;
    using Common.Models;
    using Common.Models.Solutions;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    ///     Provides access to the Solutions resource
    /// </summary>
    /// <inheritdoc />
    [ ApiVersion( "1.0" ) ]
    [ Route( "api/v{version:apiVersion}/[controller]" ) ]
    public class SolutionsController : Controller
    {
        private readonly ISolutionRepository projectRepository;

        public SolutionsController( ISolutionRepository projectRepository )
        {
            this.projectRepository = projectRepository;
        }

        /// <summary>
        ///     Lists solutions
        /// </summary>
        /// <returns></returns>
        [ HttpGet ]
        public async Task<IActionResult> Index( GetSolutionsRequest request )
        {
            var results = await projectRepository.ListByAsync( request );

            return Json( new PagedResponse<SolutionDto>
            {
                Page = request.Page,
                Next = Url.Action( "Index", "Solutions", new { page = request.Page + 1 }, Request.Scheme ),
                Previous = request.Page <= 1 ? null : Url.Action( "Index", "Solutions", new { page = request.Page - 1 }, Request.Scheme ),
                Values = results.Select( x => new SolutionDto
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
        public async Task<IActionResult> Details( string solutionId )
        {
            return Ok();
        }

        /// <summary>
        ///     Reserves a job for the calling build agent
        /// </summary>
        /// <returns></returns>
        [ HttpPost ]
        [ Route( "add" ) ]
        public async Task<IActionResult> Add( [ FromBody ] SolutionDto solutionDto )
        {
            await projectRepository.CreateAsync( new Solution
            {
                Name = solutionDto?.Name
            } );
            return Ok();
        }
    }
}
