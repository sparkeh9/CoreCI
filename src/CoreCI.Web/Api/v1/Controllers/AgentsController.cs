namespace CoreCI.Web.Api.v1.Controllers
{
    using Common.Models.Agents;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Provides access to the Jobs resource
    /// </summary>
    /// <inheritdoc />
    [ ApiVersion( "1.0" ) ]
    [ Route( "api/v{version:apiVersion}/[controller]" ) ]
    public class AgentsController : Controller
    {
        /// <summary>
        /// Lets the build co-ordinator know that an agent is ready to take on builds
        /// </summary>
        /// <returns></returns>
        [ HttpPost ]
        [ Route( "register" ) ]
        public IActionResult Register( RegisterAgentDto dto )
        {
            return new OkResult();
        }

        /// <summary>
        /// Lets the build-co-ordinator know that the agent will not be available
        /// </summary>
        /// <returns></returns>
        [ HttpPost ]
        [ Route( "deregister" ) ]
        public IActionResult Deregister()
        {
            return new OkResult();
        }
    }
}
