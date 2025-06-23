using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Koperasi.API.Filters.Operation
{
    public class LanguageIdHeaderOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            // Add LanguageID header parameter if not already present
            if (!operation.Parameters.Any(p => p.Name == "LanguageID"))
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "LanguageID",
                    In = ParameterLocation.Header,
                    Required = true,
                    Schema = new OpenApiSchema
                    {
                        Type = "integer",
                        Enum = new List<IOpenApiAny>
                    {
                        new Microsoft.OpenApi.Any.OpenApiInteger(1),
                        new Microsoft.OpenApi.Any.OpenApiInteger(2)
                    }
                    },
                    Description = "Required LanguageID header. Allowed values: 1 or 2."
                });
            }
        }
    }

}
