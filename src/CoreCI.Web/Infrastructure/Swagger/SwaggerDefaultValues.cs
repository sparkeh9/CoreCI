﻿namespace CoreCI.Web.Infrastructure.Swagger
{
    using System.Linq;
    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerGen;

    public class SwaggerDefaultValues : IOperationFilter
    {
        /// <summary>
        /// Applies the filter to the specified operation using the given context.
        /// </summary>
        /// <param name="operation">The operation to apply the filter to.</param>
        /// <param name="context">The current operation filter context.</param>
        public void Apply( Operation operation, OperationFilterContext context )
        {
            // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/412
            // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/413
            foreach ( var parameter in operation.Parameters.OfType<NonBodyParameter>() )
            {
                var description = context.ApiDescription.ParameterDescriptions.First( p => p.Name == parameter.Name );

                if ( description.ModelMetadata != null )
                {
                    if ( parameter.Description == null )
                    {
                        parameter.Description = description?.ModelMetadata?.Description;
                    }
                }


                if ( description.RouteInfo != null )
                {
                    if ( parameter.Default == null )
                    {
                        parameter.Default = description?.RouteInfo?.DefaultValue;
                    }

                    parameter.Required |= !description.RouteInfo.IsOptional;
                }
            }
        }
    }
}
