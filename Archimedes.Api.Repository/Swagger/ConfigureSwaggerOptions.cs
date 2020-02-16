using System;
using Archimedes.Library.Domain;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Archimedes.Api.Repository.Swagger
{
public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        readonly IApiVersionDescriptionProvider _provider;
        private readonly Config _config;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider,IOptions<Config> config)
        {
            _provider = provider;
            _config = config.Value;
        }


        public void Configure(SwaggerGenOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));


            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
            }
        }

        OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo()
            {
                Title = $"{_config.ApplicationName} {description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
                Description = _config?.DatabaseServer
                //Contact = (apiSettings != null && apiSettings.Contact != null) ? new OpenApiContact { Name = apiSettings.Contact.Name, Email = apiSettings.Contact.Email, Url = new Uri(apiSettings.Contact.Url) } : null,
                //License = (apiSettings != null && apiSettings.License != null) ? new OpenApiLicense { Name = apiSettings.License.Name, Url = new Uri(apiSettings.License.Url) } : null,
                //TermsOfService = !string.IsNullOrEmpty(apiSettings?.TermsOfServiceUrl) ? new Uri(apiSettings.TermsOfServiceUrl) : null
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }
    }
}