/*
 * System rezerwacji miejsc na eventy
 *
 * Niniejsza dokumentacja stanowi opis REST API implemtowanego przez serwer centralny. Endpointy 
 *
 * The version of the OpenAPI document: 1.0.0
 * Contact: XXX@pw.edu.pl
 * Generated by: https://openapi-generator.tech
 */

using System;
using System.ComponentModel.DataAnnotations;
using dionizos_backend_app;
using dionizos_backend_app.Extensions;
using dionizos_backend_app.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Org.OpenAPITools.Models;

namespace Org.OpenAPITools.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public class ReservationApiController : ControllerBase
    {
        DionizosDataContext _dionizosDataContext;
        IHelper _helper;

        public ReservationApiController(DionizosDataContext dionizosDataContext, IHelper helper)
        {
            _dionizosDataContext = dionizosDataContext;
            _helper = helper;
        }

        /// <summary>
        /// Create new reservation
        /// </summary>
        /// <param name="reservationToken">token of reservation</param>
        /// <response code="204">deleted</response>
        /// <response code="404">token not found</response>
        [HttpDelete]
        [Route("/reservation")]
        public virtual async Task<IActionResult> DeleteReservation([FromQuery (Name = "reservationToken")][Required()]string reservationToken)
        {
            Reservaton? res = await _dionizosDataContext.Reservatons.FirstOrDefaultAsync(x => x.Token == reservationToken);
            if(res is null) StatusCode(404);
            _dionizosDataContext.Reservatons.Remove(res!);
            await _dionizosDataContext.SaveChangesAsync();
            return StatusCode(204);
        }

        /// <summary>
        /// Create new reservation
        /// </summary>
        /// <param name="eventId">ID of event</param>
        /// <param name="placeID">ID of place</param>
        /// <response code="201">created</response>
        /// <response code="400">no free place</response>
        /// <response code="404">event not exist or done</response>
        [HttpPost]
        [Route("/reservation")]
        public virtual async Task<IActionResult> MakeReservation([FromQuery (Name = "eventId")][Required()]long eventId, [FromQuery (Name = "placeID")]long? placeID)
        {
            if (eventId < 1) return StatusCode(404);
            Event? e = await _dionizosDataContext.Events.Include(e => e.Reservatons).FirstOrDefaultAsync(x => x.Id == eventId);
            if(e is null || e.Status == (int)EventStatus.CancelledEnum || e.Status == (int)EventStatus.DoneEnum) StatusCode(404);
            if(e.Reservatons.Count() >= e.Placecapacity) StatusCode(400);
            if(placeID is null)
            {
                var busy = e.Reservatons.Select(x => x.PlaceId).ToList();
                for (long i = 0; i < e.Placecapacity; i++)
                    if(!busy.Contains(i))
                    {
                        placeID = i;
                        break;
                    }
            }
            else
            {
                if (e.Reservatons.Any(x => x.PlaceId == placeID)) return StatusCode(400);
            }
            var token = _helper.generateRandomToken(32);
            Reservaton reservaton = new Reservaton()
            {
                EventId = e.Id,
                PlaceId = placeID.Value,
                Token = token
            };
            await _dionizosDataContext.Reservatons.AddAsync(reservaton);
            await _dionizosDataContext.SaveChangesAsync();
            return StatusCode(201, reservaton.AsDto());
        }
    }
}
