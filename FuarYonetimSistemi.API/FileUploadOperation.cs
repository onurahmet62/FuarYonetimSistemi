using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class FileUploadOperation : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var fileParams = context.MethodInfo
            .GetParameters()
            .Where(p => p.ParameterType.GetProperties().Any(prop => prop.PropertyType == typeof(IFormFile)));

        if (fileParams.Any())
        {
            var dtoType = fileParams.First().ParameterType;
            var properties = dtoType.GetProperties();
            
            var schemaProperties = new Dictionary<string, OpenApiSchema>();
            var requiredProperties = new HashSet<string>();

            foreach (var prop in properties)
            {
                var swaggerIgnore = prop.GetCustomAttribute<Swashbuckle.AspNetCore.Annotations.SwaggerIgnoreAttribute>();
                if (swaggerIgnore != null)
                    continue;

                var swaggerSchema = prop.GetCustomAttribute<Swashbuckle.AspNetCore.Annotations.SwaggerSchemaAttribute>();
                var required = prop.GetCustomAttribute<System.ComponentModel.DataAnnotations.RequiredAttribute>();

                if (prop.PropertyType == typeof(IFormFile))
                {
                    schemaProperties[prop.Name] = new OpenApiSchema
                    {
                        Type = "string",
                        Format = "binary",
                        Description = swaggerSchema?.Description ?? $"{prop.Name} dosyası"
                    };
                }
                else if (prop.PropertyType == typeof(string))
                {
                    schemaProperties[prop.Name] = new OpenApiSchema
                    {
                        Type = "string",
                        Description = swaggerSchema?.Description ?? prop.Name
                    };
                }
                else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
                {
                    schemaProperties[prop.Name] = new OpenApiSchema
                    {
                        Type = "integer",
                        Description = swaggerSchema?.Description ?? prop.Name
                    };
                }
                else if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?))
                {
                    schemaProperties[prop.Name] = new OpenApiSchema
                    {
                        Type = "boolean",
                        Description = swaggerSchema?.Description ?? prop.Name
                    };
                }
                else if (prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(decimal?))
                {
                    schemaProperties[prop.Name] = new OpenApiSchema
                    {
                        Type = "number",
                        Format = "decimal",
                        Description = swaggerSchema?.Description ?? prop.Name
                    };
                }
                else if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                {
                    schemaProperties[prop.Name] = new OpenApiSchema
                    {
                        Type = "string",
                        Format = "date-time",
                        Description = swaggerSchema?.Description ?? prop.Name
                    };
                }
                else
                {
                    schemaProperties[prop.Name] = new OpenApiSchema
                    {
                        Type = "string",
                        Description = swaggerSchema?.Description ?? prop.Name
                    };
                }

                if (required != null)
                {
                    requiredProperties.Add(prop.Name);
                }
            }

            operation.RequestBody = new OpenApiRequestBody
            {
                Content =
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = schemaProperties,
                            Required = requiredProperties
                        }
                    }
                }
            };
        }
    }
}
