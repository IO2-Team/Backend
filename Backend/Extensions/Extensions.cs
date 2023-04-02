using dionizos_backend_app.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Org.OpenAPITools.Models;
using System.Security.Cryptography;
using System.Text;

namespace dionizos_backend_app.Extensions
{
    public static class Extensions
    {

        public static OrganizerDTO AsDto(this Organizer organizer)
        {
            DionizosDataContext context = new();
            return new OrganizerDTO()
            {
                Id = organizer.Id,
                Email = organizer.Email,
                Name = organizer.Name,
                Password = "",
                Status = (OrganizerDTO.StatusEnum)organizer.Status,
                Events = context.Events
                                .Where(x => x.Owner == organizer.Id)
                                .AsEnumerable()
                                .Select(x => x.AsDto(false))
                                .ToList(),
            };
        }

        public static EventDTO AsDto(this Event ev, bool withCatAndPlace)
        {
            DionizosDataContext context = new();
            var busyPlaces = ev.Reservatons.Select(x => x.PlaceId);
            return new EventDTO()
            {
                Id = ev.Id,
                MaxPlace = ev.Placecapacity,
                Title = ev.Title,
                StartTime = ((DateTimeOffset)ev.Starttime).ToUnixTimeSeconds(),
                EndTime = ((DateTimeOffset)ev.Endtime).ToUnixTimeSeconds(),
                Name = ev.Name ?? "unknown",
                PlaceSchema = ev.Placeschema ?? "",
                Status = (EventStatus)ev.Status,
                Categories = withCatAndPlace ? context.Eventincategories
                                    .Include(x => x.Categories)
                                    .Where(x => x.EventId == ev.Id)
                                    .Select(x => x.Categories.AsDto())
                                    .ToList()
                                    : new List<CategoryDTO>(),
                Latitude = ev.Latitude,
                Longitude = ev.Longitude,

                FreePlace = ev.Placecapacity - busyPlaces.Count(),
                Places = withCatAndPlace ? Enumerable.Range(0, ev.Placecapacity).Select(i => new PlaceDTO() { Id = i, Free = !busyPlaces.Contains(i) }).ToList() : new List<PlaceDTO>()
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

        public static SessionResponse AsDto(this Session session)
        {
            return new SessionResponse()
            {
                SessionToken = session.Token,
            };
        }

        public static ReservationDTO AsDto(this Reservaton reservaton)
        {
            return new ReservationDTO()
            {
                EventId = reservaton.EventId,
                PlaceId = reservaton.PlaceId,
                ReservationToken = reservaton.Token
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

        public static string EncryptPass(string password)
        {
            byte[] salt = Encoding.ASCII.GetBytes("Dionizos to bog wina");
            int iterations = 5000;
            int hashLength = 255;
            // Create a new instance of the Rfc2898DeriveBytes class using the password, salt, and number of iterations
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);

            // Generate the hash value for the password
            byte[] hash = pbkdf2.GetBytes(hashLength);

            // Combine the salt and hash values into a single byte array
            byte[] hashBytes = new byte[salt.Length + hash.Length];
            Array.Copy(salt, 0, hashBytes, 0, salt.Length);
            Array.Copy(hash, 0, hashBytes, salt.Length, hash.Length);

            // Convert the byte array to a base64-encoded string
            string hashedPassword = Convert.ToBase64String(hashBytes);

            return hashedPassword;
        }


        public static string toStringEnum(this EventStatus es)
        {
            switch (es)
            {
                case EventStatus.InFutureEnum:
                    return "in future";
                case EventStatus.PendingEnum:
                    return "pending";
                case EventStatus.DoneEnum:
                    return "done";
                case EventStatus.CancelledEnum:
                    return "cancelled";
                default:
                    return "Unknown";
            }
        }
    }
}
