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
using dionizos_backend_app.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.OpenAPITools.Models;
using Category = Org.OpenAPITools.Models.Category;
using Event = Org.OpenAPITools.Models.Event;
using EventStatus = Org.OpenAPITools.Models.EventStatus;
using Organizer = dionizos_backend_app.Models.Organizer;

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
        public EventApiController(DionizosDataContext dionizosDataContext, IHelper helper)
        {
            _dionizosDataContext = dionizosDataContext;
            _helper = helper;
        }
        /// <summary>
        /// Add new event
        /// </summary>
        /// <param name="sessionToken">session Token</param>
        /// <param name="title">title of Event</param>
        /// <param name="name">title of Event</param>
        /// <param name="freePlace">No of free places</param>
        /// <param name="startTime">Unix time stamp of begin of event</param>
        /// <param name="endTime">Unix time stamp of end of event</param>
        /// <param name="latitude">Latitude of event</param>
        /// <param name="longitude">Longitude of event</param>
        /// <param name="categories">Unix time stamp of end of event</param>
        /// <param name="placeSchema">seralized place schema</param>
        /// <response code="201">event created</response>
        /// <response code="400">event can not be created</response>
        [HttpPost]
        [Route("/events")]
        public virtual IActionResult AddEvent([FromHeader][Required()] string sessionToken, [FromQuery][Required()] string title, [FromQuery][Required()] string name, [FromQuery][Required()] int? freePlace, [FromQuery][Required()] int? startTime, [FromQuery][Required()] int? endTime, [FromQuery][Required()] string latitude, [FromQuery][Required()] string longitude, [FromQuery][Required()] List<int?> categories, [FromQuery] string placeSchema)
        {
            //TODO: Uncomment the next line to return response 201 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(201, default(Event));
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            var organizer = _helper.Validate(sessionToken);
            if (organizer is null) return StatusCode(400);
            if (freePlace is null || startTime is null || endTime is null)
            {
                return StatusCode(400);
            }
            if (title.Length == 0 || title.Length > 250 || latitude.Length == 0 || latitude.Length > 20 ||
                longitude.Length == 0 || longitude.Length < 20) return StatusCode(400);
            if (categories is not null)
            {
                int exisitng_categories_cnt =  _dionizosDataContext.Categories.Where(c => categories.Contains(c.Id)).Count();
                if (exisitng_categories_cnt == categories.Count)
                {
                    return StatusCode(400);
                }
            }

            if (freePlace is not null  && freePlace <= 0 )return StatusCode(400);

            if (startTime is not null)
            {
                if (startTime < DateTimeOffset.UtcNow.ToUnixTimeSeconds()) return StatusCode(400);
                if (endTime is not null && endTime >= startTime) return StatusCode(400);
            }

            if (endTime is not null && endTime < DateTimeOffset.UtcNow.ToUnixTimeSeconds())return StatusCode(400);

            dionizos_backend_app.Models.Event newEvent = new dionizos_backend_app.Models.Event();
            newEvent.Title = title;
            newEvent.Latitude = latitude;
            newEvent.Longitude = longitude;
            newEvent.Owner = organizer.Id;
            newEvent.Name = name;
            newEvent.Starttime = DateTimeOffset.FromUnixTimeSeconds((long)startTime).DateTime;
            newEvent.Endtime = DateTimeOffset.FromUnixTimeSeconds((long)endTime).DateTime;
            newEvent.Placecapacity = (int)freePlace;
            newEvent.Status = (int)EventStatus.InFutureEnum;
            //newEvent.OwnerNavigation = organizer; //czy to trzeba uzupelnic?
            newEvent.Placeschema = placeSchema;

            _dionizosDataContext.Events.Add(newEvent);
            _dionizosDataContext.SaveChanges(); //aby uzyskac id eventu

            //newEvent.Eventincategories;
            //add categories table
            foreach (dionizos_backend_app.Models.Category category in _dionizosDataContext.Categories.Where(x => categories.Contains(x.Id)).ToArray())
            {
                Eventincategory eventincategory = new Eventincategory();
                eventincategory.CategoriesId = category.Id;
                eventincategory.EventId = newEvent.Id;
                _dionizosDataContext.Eventincategories.Add(eventincategory);
            }

            _dionizosDataContext.SaveChanges();


            //newEvent.Eventincategories; //czy uzupelniane automatucznie
            return StatusCode(200, newEvent);
            return StatusCode(400);
        }

        /// <summary>
        /// Cancel event
        /// </summary>
        /// <param name="sessionToken">session Token</param>
        /// <param name="id">id of Event</param>
        /// <response code="204">deleted</response>
        /// <response code="404">id not found</response>
        [HttpDelete]
        [Route("/events/{id}")]
        public virtual IActionResult CancelEvent([FromHeader][Required()]string sessionToken, [FromRoute (Name = "id")][Required]string id)
        {

            //TODO: Uncomment the next line to return response 204 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(204);
            //TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(404);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Return list of all events in category
        /// </summary>
        /// <param name="categoryId">ID of category</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid category ID supplied</response>
        [HttpGet]
        [Route("/events/getByCategory")]
        public virtual IActionResult GetByCategory([FromQuery (Name = "categoryId")][Required()]long categoryId)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(List<Event>));
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            string exampleJson = null;
            exampleJson = "[ {\n  \"latitude\" : \"40.4775315\",\n  \"name\" : \"Long description of Event\",\n  \"freePlace\" : 2,\n  \"startTime\" : 1673034164,\n  \"id\" : 10,\n  \"endTime\" : 1683034164,\n  \"categories\" : [ {\n    \"name\" : \"Sport\",\n    \"id\" : 1\n  }, {\n    \"name\" : \"Sport\",\n    \"id\" : 1\n  } ],\n  \"title\" : \"Short description of Event\",\n  \"longitude\" : \"-3.7051359\",\n  \"placeSchema\" : \"Seralized place schema\",\n  \"status\" : \"done\"\n}, {\n  \"latitude\" : \"40.4775315\",\n  \"name\" : \"Long description of Event\",\n  \"freePlace\" : 2,\n  \"startTime\" : 1673034164,\n  \"id\" : 10,\n  \"endTime\" : 1683034164,\n  \"categories\" : [ {\n    \"name\" : \"Sport\",\n    \"id\" : 1\n  }, {\n    \"name\" : \"Sport\",\n    \"id\" : 1\n  } ],\n  \"title\" : \"Short description of Event\",\n  \"longitude\" : \"-3.7051359\",\n  \"placeSchema\" : \"Seralized place schema\",\n  \"status\" : \"done\"\n} ]";

            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<List<Event>>(exampleJson)
            : default(List<Event>);
            //TODO: Change the data returned
            return new ObjectResult(example);
        }

        /// <summary>
        /// Find event by ID
        /// </summary>
        /// <remarks>Returns a single event</remarks>
        /// <param name="id">ID of event to return</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid ID supplied</response>
        /// <response code="404">Event not found</response>
        [HttpGet]
        [Route("/events/{id}")]
        public virtual IActionResult GetEventById([FromRoute (Name = "id")][Required]long id)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(Event));
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            //TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(404);
            string exampleJson = null;
            exampleJson = "{\n  \"latitude\" : \"40.4775315\",\n  \"name\" : \"Long description of Event\",\n  \"freePlace\" : 2,\n  \"startTime\" : 1673034164,\n  \"id\" : 10,\n  \"endTime\" : 1683034164,\n  \"categories\" : [ {\n    \"name\" : \"Sport\",\n    \"id\" : 1\n  }, {\n    \"name\" : \"Sport\",\n    \"id\" : 1\n  } ],\n  \"title\" : \"Short description of Event\",\n  \"longitude\" : \"-3.7051359\",\n  \"placeSchema\" : \"Seralized place schema\",\n  \"status\" : \"done\"\n}";

            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<Event>(exampleJson)
            : default(Event);
            //TODO: Change the data returned
            return new ObjectResult(example);
        }

        /// <summary>
        /// Return list of all events
        /// </summary>
        /// <response code="200">successful operation</response>
        [HttpGet]
        [Route("/events")]
        public virtual IActionResult GetEvents()
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(List<Event>));
            string exampleJson = null;
            exampleJson = "[ {\n  \"latitude\" : \"40.4775315\",\n  \"name\" : \"Long description of Event\",\n  \"freePlace\" : 2,\n  \"startTime\" : 1673034164,\n  \"id\" : 10,\n  \"endTime\" : 1683034164,\n  \"categories\" : [ {\n    \"name\" : \"Sport\",\n    \"id\" : 1\n  }, {\n    \"name\" : \"Sport\",\n    \"id\" : 1\n  } ],\n  \"title\" : \"Short description of Event\",\n  \"longitude\" : \"-3.7051359\",\n  \"placeSchema\" : \"Seralized place schema\",\n  \"status\" : \"done\"\n}, {\n  \"latitude\" : \"40.4775315\",\n  \"name\" : \"Long description of Event\",\n  \"freePlace\" : 2,\n  \"startTime\" : 1673034164,\n  \"id\" : 10,\n  \"endTime\" : 1683034164,\n  \"categories\" : [ {\n    \"name\" : \"Sport\",\n    \"id\" : 1\n  }, {\n    \"name\" : \"Sport\",\n    \"id\" : 1\n  } ],\n  \"title\" : \"Short description of Event\",\n  \"longitude\" : \"-3.7051359\",\n  \"placeSchema\" : \"Seralized place schema\",\n  \"status\" : \"done\"\n} ]";

            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<List<Event>>(exampleJson)
            : default(List<Event>);
            //TODO: Change the data returned
            return new ObjectResult(example);
        }

        /// <summary>
        /// Return list of events made by organizer, according to session
        /// </summary>
        /// <param name="sessionToken">session Token</param>
        /// <response code="200">successful operation</response>
        [HttpGet]
        [Route("/events/my")]
        public virtual IActionResult GetMyEvents([FromHeader][Required()]string sessionToken)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(List<Event>));
            string exampleJson = null;
            exampleJson = "[ {\n  \"latitude\" : \"40.4775315\",\n  \"name\" : \"Long description of Event\",\n  \"freePlace\" : 2,\n  \"startTime\" : 1673034164,\n  \"id\" : 10,\n  \"endTime\" : 1683034164,\n  \"categories\" : [ {\n    \"name\" : \"Sport\",\n    \"id\" : 1\n  }, {\n    \"name\" : \"Sport\",\n    \"id\" : 1\n  } ],\n  \"title\" : \"Short description of Event\",\n  \"longitude\" : \"-3.7051359\",\n  \"placeSchema\" : \"Seralized place schema\",\n  \"status\" : \"done\"\n}, {\n  \"latitude\" : \"40.4775315\",\n  \"name\" : \"Long description of Event\",\n  \"freePlace\" : 2,\n  \"startTime\" : 1673034164,\n  \"id\" : 10,\n  \"endTime\" : 1683034164,\n  \"categories\" : [ {\n    \"name\" : \"Sport\",\n    \"id\" : 1\n  }, {\n    \"name\" : \"Sport\",\n    \"id\" : 1\n  } ],\n  \"title\" : \"Short description of Event\",\n  \"longitude\" : \"-3.7051359\",\n  \"placeSchema\" : \"Seralized place schema\",\n  \"status\" : \"done\"\n} ]";

            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<List<Event>>(exampleJson)
            : default(List<Event>);
            //TODO: Change the data returned
            return new ObjectResult(example);
        }

        /// <summary>
        /// patch existing event
        /// </summary>
        /// <param name="sessionToken">session Token</param>
        /// <param name="id">id of Event</param>
        /// <param name="_event">Update an existent user in the store</param>
        /// <response code="202">patched</response>
        /// <response code="404">id not found</response>
        [HttpPatch]
        [Route("/events/{id}")]
        [Consumes("application/json")]
        public virtual IActionResult PatchEvent([FromHeader][Required()]string sessionToken, [FromRoute (Name = "id")][Required]string id, [FromBody]Event _event)
        {

            //TODO: Uncomment the next line to return response 202 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(202);
            //TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(404);

            throw new NotImplementedException();
        }
    }
}
