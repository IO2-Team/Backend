using dionizos_backend_app.Models;
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
            if (!context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var token))
            {
                context.Result = new ObjectResult("API key missing")
                {
                    StatusCode = 403,
                };
                return;
            }

            var apiKey = _configuration.GetValue<string>(AuthConstants.ApiKeySectionName);
            if (!apiKey.Equals(token))
            {
                context.Result = new ObjectResult("Invalid API key")
                {
                    StatusCode = 403,
                };
                return;
            }

            //Organizer? organizer = _helper.Validate(token!);

            //if(organizer == null)
            //{
            //    context.Result = new UnauthorizedObjectResult("Invalid API key");
            //    return;
            //}
        }
    }
}
