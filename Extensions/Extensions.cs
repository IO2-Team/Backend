﻿using dionizos_backend_app.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Org.OpenAPITools.Models;

namespace dionizos_backend_app.Extensions
{
    internal static class Extensions
    {
        // FIXME: (kutakw) make these extensions async
        public static OrganizerDTO AsDto(this Organizer organizer, DionizosDataContext context)
        {
            return new OrganizerDTO()
            {
                Id = organizer.Id,
                Email = organizer.Email,
                // FIXME: (kutakw) currently in db we got int as organizers name
                Name = "FIXME",
                Password = organizer.Password,
                Status = (OrganizerDTO.StatusEnum)organizer.Status,
                Events = context.Events
                                .Where(x => x.Owner == organizer.Id)
                                .AsEnumerable()
                                .Select(x => x.AsDto(context))
                                .ToList(),
            };
        }

        public static EventDTO AsDto(this Event ev, DionizosDataContext context)
        {
            return new EventDTO()
            {
                Id = ev.Id,

                // FIXME: (kutakw) where do we keep currently available places
                FreePlace = ev.Placecapacity - 0,

                Title = ev.Title,
                StartTime = ((DateTimeOffset)ev.Starttime).ToUnixTimeSeconds(),
                EndTime = ((DateTimeOffset)ev.Endtime).ToUnixTimeSeconds(),

                // FIXME: (kutakw) currently in db we got int as events name
                Name = "FIXME",

                // FIXME: (kutakw) placeSchema not functioning yet
                PlaceSchema = ev.Placeschema.ToString() ?? "",
                Status = (EventStatus)ev.Status,
                Categories = context.Eventincategories
                                    .Include(x => x.Categories)
                                    .Where(x => x.CategoriesId == ev.Categories)
                                    .Select(x => x.Categories.AsDto())
                                    .ToList(),
            };
        }

        public static CategoryDTO AsDto(this Category category)
        {
            return new CategoryDTO()
            {
                Id = category.Id,
                Name = category.Name,
            };
        }

        public static LoginOrganizer200ResponseDTO AsDto(this Session session)
        {
            return new LoginOrganizer200ResponseDTO()
            {
                SessionToken = session.Token,
            };
        }

        /// <summary>
        /// Encodes a base64 string
        /// </summary>
        /// <param name="text">Text to be encoded</param>
        /// <returns>Base64 encoded string</returns>
        public static string Base64Encode(this string text)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Decodes a base64 string
        /// </summary>
        /// <param name="text">Encoded base64 text string</param>
        /// <returns>Decoded string</returns>
        public static string Base64Decode(this string text)
        {
            byte[] encodedBytes = Convert.FromBase64String(text);
            return System.Text.Encoding.UTF8.GetString(encodedBytes);
        }
    }
}
