using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace dionizos_backend_app.Authentication
{
    public class ApiKeyAuthFilter : IAuthorizationFilter
    {
        private readonly IConfiguration _configuration;
        private readonly IHelper _helper;

        public ApiKeyAuthFilter(IConfiguration configuration, IHelper helper)
        {
            _configuration = configuration;
            _helper = helper;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var header))
            {
                context.Result = new UnauthorizedObjectResult("API key missing");
                return;
            }

            // TODO: Validate properly

            var apiKey = _configuration.GetValue<string>(AuthConstants.ApiKeySectionName);
            if (!apiKey.Equals(header))
            {
                context.Result = new UnauthorizedObjectResult("Invalid API key");
                return;
            }
        }
    }
}
