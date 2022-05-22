using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

namespace Nexus.OAuth.Api
{
    internal static class Swagger
    {
        public static void AddSwagger(SwaggerGenOptions? options)
        {
            string markdownDescription = string.Empty;

            try
            {
                markdownDescription = Encoding.ASCII.GetString(Properties.Resources.SwaggerDescription);
            }
            catch (Exception)
            {
            }

            OpenApiInfo info = new()
            {
                Version = "v1",
                Title = "Nexus OAuth API",
                Description = markdownDescription,
                Contact = new OpenApiContact
                {
                    Name = "Juan Douglas (Developer/CEO)",
                    Email = "juandouglas2004@gmail.com"
                },
                License = new OpenApiLicense
                {
                    Name = "Apache license 2.0",
                    Url = new Uri("https://www.apache.org/licenses/LICENSE-2.0.txt")
                }
            };

            options.SwaggerDoc("v1", info);
        }
    }
}
