// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Microsoft.AspNetCore.OData.Query;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebAPI.Common.Filters;

public class SwaggerOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        context.ApiDescription.ParameterDescriptions
            .Where(desc => desc?.ParameterDescriptor?.ParameterType?.BaseType == typeof(ODataQueryOptions)).ToList()
            .ForEach(param =>
            {
                var toRemove = operation.Parameters.SingleOrDefault(p => p.Name == param.Name);
                if (null != toRemove)
                {
                    _ = operation.Parameters.Remove(toRemove);
                }

                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "$filter",
                    In = ParameterLocation.Query,
                    Required = false,
                    Description = "OData filter query",
                    Schema = new OpenApiSchema
                    {
                        Type = "string"
                    }
                });

                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "$top",
                    In = ParameterLocation.Query,
                    Required = false,
                    Description = "OData top query",
                    Schema = new OpenApiSchema
                    {
                        Type = "integer"
                    }
                });

                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "$skip",
                    In = ParameterLocation.Query,
                    Required = false,
                    Description = "OData skip query",
                    Schema = new OpenApiSchema
                    {
                        Type = "integer"
                    }
                });

                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "$orderby",
                    In = ParameterLocation.Query,
                    Required = false,
                    Description = "OData orderby query",
                    Schema = new OpenApiSchema
                    {
                        Type = "integer"
                    }
                });

            });
    }
}
