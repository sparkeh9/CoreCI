namespace CoreCI.Web.Infrastructure.Routing
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using MongoDB.Bson;

    public class ObjectIdRouteConstraint : IRouteConstraint
    {
        public bool Match( HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection )
        {
            if ( !values.TryGetValue( routeKey, out object value ) || value == null )
            {
                return false;
            }

            return ObjectId.TryParse( value.ToString(), out ObjectId _ );
        }
    }
}
