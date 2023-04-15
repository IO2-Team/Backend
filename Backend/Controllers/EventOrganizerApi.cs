/*
 * System rezerwacji miejsc na eventy
 *
 * Niniejsza dokumentacja stanowi opis REST API implemtowanego przez serwer centralny. Endpointy 
 *
 * The version of the OpenAPI document: 1.0.0
 * Contact: XXX@pw.edu.pl
 * Generated by: https://openapi-generator.tech
 */

using System.ComponentModel.DataAnnotations;
using dionizos_backend_app;
using dionizos_backend_app.Authentication;
using dionizos_backend_app.Extensions;
using dionizos_backend_app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Org.OpenAPITools.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Org.OpenAPITools.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public class EventOrganizerApiController : ControllerBase
    {
        private readonly DionizosDataContext _context;
        private readonly IHelper _helper;
        private readonly IMailing _mailing;
        private readonly ILogger _logger;
        private readonly Random _random = new();

        public EventOrganizerApiController(DionizosDataContext context, IHelper helper, IMailing mailing, ILogger logger)
        {
            _context = context;
            _helper = helper;
            _mailing = mailing;
            _logger = logger;
        }

        /// <summary>
        /// Confirm orginizer account
        /// </summary>
        /// <param name="id">id of OrganizerDTO</param>
        /// <param name="code">code from email</param>
        /// <response code="200">nothing to do, account already confirmed</response>
        /// <response code="202">account confirmed</response>
        /// <response code="400">code wrong</response>
        /// <response code="404">organizer id not found</response>
        [HttpPost]
        [Route("/organizer/{id}")]
        [SwaggerOperation("Confirm")]
        [SwaggerResponse(statusCode: 200, type: typeof(void), description: "Account already confirmed")]
        [SwaggerResponse(statusCode: 202, type: typeof(void), description: "Confirmed")]
        [SwaggerResponse(statusCode: 400, type: typeof(void), description: "Wrong code")]
        [SwaggerResponse(statusCode: 404, type: typeof(void), description: "Not Found")]
        public virtual async Task<IActionResult> Confirm([FromRoute][Required]string id, [FromHeader][Required()]string code)
        {
            long Id = long.Parse(id);
            DateTime currTime = DateTime.Now;
            Emailcode? entity = await _context.Emailcodes
                                            .Include(x => x.Organizer)
                                            .OrderByDescending(x => x.Time)
                                            .FirstOrDefaultAsync(x =>
                                                x.OrganizerId == Id
                                                && currTime < x.Time.AddDays(1.0));


            // verify id and code
            if (entity == null
                || entity.Code != code)
            {
                // wrong email/code, return bad request
                return StatusCode(400);
            }

            Organizer organizer = entity.Organizer;

            if(organizer.Status != (int)Organizer.StatusEnum.PendingEnum)
            {
                return StatusCode(400);
            }
            // update organizer status
            organizer.Status = (int)Organizer.StatusEnum.ConfirmedEnum;
            // save changes
            await _context.SaveChangesAsync();

            OrganizerDTO dto = organizer.AsDto();
            return StatusCode(201, dto);
        }

        /// <summary>
        /// Confirm orginizer account
        /// </summary>
        /// <param name="id">id of OrganizerDTO</param>
        /// <response code="204">deleted</response>
        /// <response code="403">invalid session</response>
        /// <response code="404">id not found</response>
        [HttpDelete]
        [Route("/organizer/{id}")]
        [ServiceFilter(typeof(ApiKeyAuthFilter))]
        [SwaggerOperation("DeleteOrganizer")]
        [SwaggerResponse(statusCode: 204, type: typeof(void), description: "Deleted")]
        [SwaggerResponse(statusCode: 403, type: typeof(void), description: "Invalid session")]
        [SwaggerResponse(statusCode: 404, type: typeof(void), description: "Not Found")]
        public virtual async Task<IActionResult> DeleteOrganizer([FromRoute][Required]string id)
        {
            HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var sessionToken);
            var organizer = _helper.Validate(sessionToken!);

            // Validate session
            if(organizer == null)
            {
                // invalid session
                return StatusCode(403);
            }

            long Id = long.Parse(id);

            // check if valid oraganizer
            if(organizer.Id != Id)
            {
                // invalid organizer id - trying to delete other organizer
                return StatusCode(404);
            }

            // check if any planned or pending events exist
            if(organizer.Events.Any(x => 
                x.Status == (int)EventStatus.InFutureEnum
                || x.Status == (int)EventStatus.PendingEnum))
            {
                return StatusCode(404);
            }

            // Hash all data of organizer
            organizer.Name = organizer.Name.GetHashCode().ToString();
            organizer.Email = organizer.Email.GetHashCode().ToString();

            organizer.Status = (int)Organizer.StatusEnum.DeletedEnum;

            // save in db
            await _context.SaveChangesAsync();

            return StatusCode(204);
        }

        /// <summary>
        /// Logs organizer into the system
        /// </summary>
        /// <param name="email">The organizer email for login</param>
        /// <param name="password">the password</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid email/password supplied</response>
        [HttpGet]
        [Route("/organizer/login")]
        [SwaggerOperation("LoginOrganizer")]
        [SwaggerResponse(statusCode: 200, type: typeof(SessionResponseDTO), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(void), description: "Invalid email/password")]
        public virtual async Task<IActionResult> LoginOrganizer([FromHeader][Required()]string email, [FromHeader][Required()]string password)
        {
            // Check if organizer exists
            Organizer? organizer = await _context.Organizers
                                                 .FirstOrDefaultAsync(x =>
                                                    string.Equals(x.Email, email)
                                                    && x.Status == (int)Organizer.StatusEnum.ConfirmedEnum);

            // check if organizer with email exists
            if (organizer == null
                // check if valid password
                || !string.Equals(organizer.Password, Extensions.EncryptPass(password)))
            {
                return StatusCode(400);
            }

            DateTime time = DateTime.Now;

            // Create new session with token
            Session session = new()
            {
                OrganizerId = organizer.Id,
                Time = time,
                Token = Session.GetToken(email, time),
            };

            // update db
            await _context.Sessions.AddAsync(session);
            await _context.SaveChangesAsync();

            SessionResponseDTO dto = session.AsDto();
            return StatusCode(200, dto);
        }

        /// <summary>
        /// Patch orginizer account
        /// </summary>
        /// <param name="id">id of OrganizerDTO</param>
        /// <param name="body">Update an existent user in the store</param>
        /// <response code="200">nothing to do, no field to patch</response>
        /// <response code="202">patched</response>
        /// <response code="400">invalid email or password</response>
        /// <response code="403">invalid session</response>
        /// <response code="404">id not found</response>
        [HttpPatch]
        [Route("/organizer/{id}")]
        [ServiceFilter(typeof(ApiKeyAuthFilter))]
        [Consumes("application/json")]
        [SwaggerOperation("PatchOrganizer")]
        [SwaggerResponse(statusCode: 200, type: typeof(void), description: "No field to patch")]
        [SwaggerResponse(statusCode: 202, type: typeof(void), description: "Patched")]
        [SwaggerResponse(statusCode: 400, type: typeof(void), description: "Invalid email or password")]
        [SwaggerResponse(statusCode: 403, type: typeof(void), description: "Invalid session")]
        [SwaggerResponse(statusCode: 404, type: typeof(void), description: "Not found")]
        public virtual async Task<IActionResult> PatchOrganizer([FromRoute][Required]string id, [FromBody]OrganizerPatchDTO body)
        {
            HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var sessionToken);
            var validatedOrganizer = _helper.Validate(sessionToken!);

            if(validatedOrganizer == null)
            {
                // not authenticated / wrong id
                return StatusCode(403);
            }

            long Id = long.Parse(id);
            // Check if organizer is trying to patch itself
            if(validatedOrganizer.Id != Id)
            {
                return StatusCode(404);
            }

            if(!string.IsNullOrEmpty(body.Name)) validatedOrganizer.Name = body.Name;
            if (!string.IsNullOrEmpty(body.Password)) validatedOrganizer.Password = Extensions.EncryptPass(body.Password);

            await _context.SaveChangesAsync();
            return StatusCode(202);
        }

        /// <summary>
        /// Get organizer account (my account)
        /// </summary>
        /// <response code="200">successful operation</response>
        /// <response code="400">invalid session</response>
        [HttpGet]
        [Route("/organizer")]
        [ServiceFilter(typeof(ApiKeyAuthFilter))]
        [SwaggerOperation("GetOrganizer")]
        [SwaggerResponse(statusCode: 200, type: typeof(OrganizerDTO), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(void), description: "Invalid session operation")]
        public virtual IActionResult GetOrganizer()
        {
            HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var sessionToken);
            var organizer = _helper.Validate(sessionToken!);

            if (organizer == null)
            {
                return StatusCode(403);
            }
            var dto = organizer.AsDto();
            return StatusCode(200, dto);
        }

        /// <summary>
        /// Create orginizer account
        /// </summary>
        /// <param name="body">Create organizer</param>
        /// <response code="201">successful operation</response>
        /// <response code="400">organizer already exist</response>
        [HttpPost]
        [Route("/organizer")]
        [SwaggerOperation("SignUp")]
        [SwaggerResponse(statusCode: 201, type: typeof(OrganizerDTO), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(void), description: "Organizer already exists")]
        public virtual async Task<IActionResult> SignUp([FromBody]OrganizerFormDTO body)
        {
            async Task GenerateEmailcode(DionizosDataContext _context, Organizer organizer)
            {
                // Create Email Code
                Emailcode emailcode = new()
                {
                    OrganizerId = organizer.Id,
                    Time = DateTime.Now,
                    Code = string.Join(
                        string.Empty,
                        Enumerable.Range(0, 6)
                                  .Select(_ => _random.Next(0, 9).ToString())
                    ),
                };

                // Add email code to db
                await _context.Emailcodes.AddAsync(emailcode);
                await _context.SaveChangesAsync();

                _mailing.SendEmailCode(organizer.Email, emailcode.Code);

                _logger.LogInformation($"Email sent to {organizer.Email} with code '{emailcode.Code}'");
            };
            /////////////////////////////////////////////////////////////////////////

            // Check if organizer with that name or email already exists
            Organizer? organizerInDb =
                await _context.Organizers
                              .FirstOrDefaultAsync(x => x.Name == body.Name || x.Email == body.Email);

            if (organizerInDb != null)
            {
                if(organizerInDb.Status == (int)Organizer.StatusEnum.PendingEnum
                    && organizerInDb.Name == body.Name
                    && organizerInDb.Email == body.Email)
                {
                    // resend code via mail
                    // gen email
                    await GenerateEmailcode(_context, organizerInDb);

                    return StatusCode(200, organizerInDb.AsDto());
                }

                // return bad request and no body
                return StatusCode(400);
            }

            // Create new organizer
            Organizer organizer = new()
            {
                Email = body.Email,
                Name = body.Name,
                Password = Extensions.EncryptPass(body.Password),
                Status = (int)Organizer.StatusEnum.PendingEnum,
            };
            // Add organizer to db
            await _context.Organizers.AddAsync(organizer);
            await _context.SaveChangesAsync();

            // generate email
            await GenerateEmailcode(_context, organizer);

            OrganizerDTO dto = organizer.AsDto();
            return StatusCode(201, dto);
        }
    }
}
