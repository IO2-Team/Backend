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
    public class EventApiController : ControllerBase
    {
        DionizosDataContext _dionizosDataContext;
        IHelper _helper;
        ILogger _logger;
        public EventApiController(DionizosDataContext dionizosDataContext, IHelper helper, ILogger<EventApiController> logger)
        {
            _dionizosDataContext = dionizosDataContext;
            _helper = helper;
            _logger = logger;
        }

        /// <summary>
        /// Add new event
        /// </summary>
        /// <param name="body">Add event</param>
        /// <response code="201">event created</response>
        /// <response code="400">event can not be created, field invalid</response>
        /// <response code="403">invalid session</response>
        [HttpPost]
        [Route("/events")]
        [ServiceFilter(typeof(ApiKeyAuthFilter))]
        [SwaggerOperation("AddEvent")]
        [SwaggerResponse(statusCode: 201, type: typeof(EventDTO), description: "event created")]
        [SwaggerResponse(statusCode: 400, type: typeof(void), description: "Invalid field")]
        [SwaggerResponse(statusCode: 403, type: typeof(void), description: "Unauthoraized")]
        public virtual async Task<IActionResult> AddEvent([FromBody] EventFormDTO body)
        {
            HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var sessionToken);
            var organizer = _helper.Validate(sessionToken!);

            if (organizer is null) return StatusCode(403);
            if (body.MaxPlace is null || body.StartTime is null || body.EndTime is null)
            {
                return StatusCode(400);
            }
            if (body.Title.Length == 0 || body.Title.Length > 250 || body.Latitude.Length == 0 || body.Latitude.Length > 20 ||
                body.Longitude.Length == 0 || body.Longitude.Length > 20) return StatusCode(400);
            int exisitng_categories_cnt =  _dionizosDataContext.Categories.Where(c => body.CategoriesIds.Contains((int)c.Id)).Count();
            if (exisitng_categories_cnt != body.CategoriesIds.Count) return StatusCode(400);
            if (body.MaxPlace < 0) return StatusCode(400);
            if (body.StartTime < DateTimeOffset.UtcNow.ToUnixTimeSeconds()) return StatusCode(400);
            if (body.EndTime < body.StartTime) return StatusCode(400);
            if (body.EndTime < DateTimeOffset.UtcNow.ToUnixTimeSeconds())return StatusCode(400);

            Event newEvent = new Event();
            newEvent.Title = body.Title;
            newEvent.Latitude = body.Latitude;
            newEvent.Longitude = body.Longitude;
            newEvent.Owner = organizer.Id;
            newEvent.Name = body.Name;
            newEvent.Starttime = DateTime.SpecifyKind(DateTimeOffset.FromUnixTimeSeconds(body.StartTime.Value).DateTime, DateTimeKind.Unspecified);
            newEvent.Endtime = DateTime.SpecifyKind(DateTimeOffset.FromUnixTimeSeconds(body.EndTime.Value).DateTime, DateTimeKind.Unspecified);
            newEvent.Placecapacity = (int) body.MaxPlace;
            newEvent.Status = (int)EventStatus.InFutureEnum;
            newEvent.Placeschema = body.PlaceSchema ?? "";

            await _dionizosDataContext.Events.AddAsync(newEvent);
            await _dionizosDataContext.SaveChangesAsync(); //aby uzyskac id eventu

            //add categories table
            foreach (var category in body.CategoriesIds)
            {
                Eventincategory eventincategory = new Eventincategory();
                eventincategory.CategoriesId = (long) category!;
                eventincategory.EventId = newEvent.Id;
                await _dionizosDataContext.Eventincategories.AddAsync(eventincategory);
            }

            await _dionizosDataContext.SaveChangesAsync();
            EventDTO dto = newEvent.AsDto();
            return StatusCode(201, dto);
        }

        /// <summary>
        /// Cancel event
        /// </summary>
        /// <param name="id">id of Event</param>
        /// <response code="204">deleted</response>
        /// <response code="403">invalid session</response>
        /// <response code="404">id not found</response>
        [HttpDelete]
        [Route("/events/{id}")]
        [ServiceFilter(typeof(ApiKeyAuthFilter))]
        [SwaggerOperation("CancelEvent")]
        [SwaggerResponse(statusCode: 204, type: typeof(void), description: "deleted")]
        [SwaggerResponse(statusCode: 403, type: typeof(void), description: "Unauthorized")]
        [SwaggerResponse(statusCode: 404, type: typeof(void), description: "Not found")]
        public virtual async Task<IActionResult> CancelEvent([FromRoute][Required] string id)
        {
            HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var sessionToken);
            Organizer organizer = _helper.Validate(sessionToken!)!;

            long Id = long.Parse(id);
            Event? @event = organizer.Events.FirstOrDefault(x => x.Id == Id);
            if(@event == null
                || @event.Status != (int)EventStatus.InFutureEnum)
            {
                return StatusCode(404);
            }

            @event.Status = (int)EventStatus.CancelledEnum;
            await _dionizosDataContext.SaveChangesAsync();
            return StatusCode(204);
        }

        /// <summary>
        /// Return list of all events in category
        /// </summary>
        /// <param name="categoryId">ID of category</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid category ID supplied</response>
        [HttpGet]
        [Route("/events/getByCategory")]
        [SwaggerOperation("GetByCategory")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<EventDTO>), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(void), description: "Invalid category ID")]
        public virtual async Task<IActionResult> GetByCategory([FromHeader][Required()] long? categoryId)
        {
            if (categoryId < 1) return StatusCode(400);
            List<Eventincategory> eInC = await _dionizosDataContext.Eventincategories.Include(x => x.Event)
                                                                                     .Where(x => x.CategoriesId == categoryId)
                                                                                     .ToListAsync();
            return new ObjectResult(eInC.Select(x => x.Event.AsDto()).ToList());
        }

        /// <summary>
        /// Find event by ID
        /// </summary>
        /// <remarks>Returns a single event</remarks>
        /// <param name="id">ID of event to return</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">EventDTO not found</response>
        [HttpGet]
        [Route("/events/{id}")]
        [SwaggerOperation("GetEventById")]
        [SwaggerResponse(statusCode: 200, type: typeof(EventWithPlacesDTO), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(void), description: "Invalid ID supplied")]
        [SwaggerResponse(statusCode: 404, type: typeof(void), description: "Not found")]
        public virtual async Task<IActionResult> GetEventById([FromRoute][Required] long? id)
        {
            if (id < 1) return StatusCode(400);


            Event? e = await _dionizosDataContext.Events.FirstOrDefaultAsync(x => x.Id == id);
            if(e is null) return StatusCode(404);
            return new ObjectResult(e.AsDtoWithPlace());
        }

        /// <summary>
        /// Return list of all events
        /// </summary>
        /// <response code="200">successful operation</response>
        [HttpGet]
        [Route("/events")]
        [SwaggerOperation("GetEvents")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<EventDTO>), description: "successful operation")]
        public virtual async Task<IActionResult> GetEvents()
        {
            List<EventDTO> events = await _dionizosDataContext.Events.Select(x => x.AsDto()).ToListAsync();

            return new ObjectResult(events);
        }

        /// <summary>
        /// Return list of events made by organizer, according to session
        /// </summary>
        /// <response code="200">successful operation</response>
        /// <response code="403">invalid session</response>
        [HttpGet]
        [Route("/events/my")]
        [ServiceFilter(typeof(ApiKeyAuthFilter))]
        [SwaggerOperation("GetMyEvents")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<EventDTO>), description: "successful operation")]
        [SwaggerResponse(statusCode: 403, type: typeof(void), description: "Invalid session")]
        public virtual async Task<IActionResult> GetMyEvents()
        {
            HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var sessionToken);
            var organizer = _helper.Validate(sessionToken!);

            if (organizer is null) return StatusCode(403);

            List<EventDTO> events = await _dionizosDataContext.Events.Where(x => x.Owner == organizer.Id)
                                                                     .Select(x => x.AsDto()).ToListAsync();
            return StatusCode(200, events);
        }

        /// <summary>
        /// patch existing event
        /// </summary>
        /// <param name="id">id of Event</param>
        /// <param name="body">Update an existent user in the store</param>
        /// <response code="200">nothing to do, no field to patch</response>
        /// <response code="202">patched</response>
        /// <response code="400">invalid id or fields in body</response>
        /// <response code="403">invalid session</response>
        /// <response code="404">id not found</response>
        [HttpPatch]
        [Route("/events/{id}")]
        [ServiceFilter(typeof(ApiKeyAuthFilter))]
        [Consumes("application/json")]
        [SwaggerResponse(statusCode: 200, type: typeof(void), description: "No patch needed")]
        [SwaggerResponse(statusCode: 202, type: typeof(void), description: "Pathed")]
        [SwaggerResponse(statusCode: 400, type: typeof(void), description: "Inalid Id of fields")]
        [SwaggerResponse(statusCode: 403, type: typeof(void), description: "Invalid session")]
        [SwaggerResponse(statusCode: 404, type: typeof(void), description: "Not found")]
        public virtual async Task<IActionResult> PatchEvent([FromRoute][Required] string id, [FromBody] EventPatchDTO body)
        {
            HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var sessionToken);
            var organizer = _helper.Validate(sessionToken!);

            // check if validated session
            if(organizer == null)
            {
                return StatusCode(403);
            }

            long Id = long.Parse(id);
            Event? @event = await _dionizosDataContext.Events
                                                      .Where(x => x.Owner == organizer.Id
                                                        && x.Id == Id)
                                                      .FirstOrDefaultAsync();
            // Check if event with provided ID exists
            if(@event == null)
            {
                return StatusCode(404);
            }

            // Update time
            if (!string.IsNullOrEmpty(body.Title)) @event.Title = body.Title;
            if (!string.IsNullOrEmpty(body.Name)) @event.Name = body.Name;

            if (body.StartTime != null)
            {
                DateTime newStartTime = DateTime.SpecifyKind(DateTimeOffset.FromUnixTimeSeconds(body.StartTime.Value).DateTime, DateTimeKind.Unspecified);
                if(newStartTime < DateTime.Now)
                {
                    return StatusCode(400);
                }

                @event.Starttime = newStartTime;
            }
            if (body.EndTime != null)
            {
                DateTime newEndTime = DateTime.SpecifyKind(DateTimeOffset.FromUnixTimeSeconds(body.EndTime.Value).DateTime, DateTimeKind.Unspecified);
                if(newEndTime < @event.Starttime)
                {
                    return StatusCode(400);
                }

                @event.Endtime = newEndTime;
            }

            if(!string.IsNullOrEmpty(body.Latitude)) @event.Latitude = body.Latitude;
            if(!string.IsNullOrEmpty(body.Longitude)) @event.Longitude = body.Longitude;
            if(!string.IsNullOrEmpty(body.PlaceSchema)) @event.Placeschema = body.PlaceSchema;
            if (body.MaxPlace != null)
            {
                if(body.MaxPlace >= @event.Placecapacity)
                {
                    return StatusCode(400);
                }
                @event.Placecapacity = (int)body.MaxPlace.Value;
            }

            if (_dionizosDataContext.Entry(@event).State == EntityState.Unchanged)
            {
                return StatusCode(200);
            }

            await _dionizosDataContext.SaveChangesAsync();

            return StatusCode(202);
        }
    }
}
