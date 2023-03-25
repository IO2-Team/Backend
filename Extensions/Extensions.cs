using dionizos_backend_app.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Org.OpenAPITools.Models;
using System.Security.Cryptography;
using System.Text;
using OrganizerDTO = Org.OpenAPITools.Models.Organizer;
using Organizer = dionizos_backend_app.Models.Organizer;
using EventDTO = Org.OpenAPITools.Models.Event;
using Event = dionizos_backend_app.Models.Event;
using CategoryDTO = Org.OpenAPITools.Models.Category;
using Category = dionizos_backend_app.Models.Category;
using LoginOrganizer200ResponseDTO = Org.OpenAPITools.Models.LoginOrganizer200Response;

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
                Password = organizer.Password,
                Status = (OrganizerDTO.StatusEnum)organizer.Status,
                Events = context.Events
                                .Where(x => x.Owner == organizer.Id)
                                .AsEnumerable()
                                .Select(x => x.AsDto())
                                .ToList(),
            };
        }

        public static EventDTO AsDto(this Event ev)
        {
            DionizosDataContext context = new();
            return new EventDTO()
            {
                Id = ev.Id,
                FreePlace = ev.Placecapacity - context.Reservatons.Count(x => x.EventId == ev.Id),
                Title = ev.Title,
                StartTime = ((DateTimeOffset)ev.Starttime).ToUnixTimeSeconds(),
                EndTime = ((DateTimeOffset)ev.Endtime).ToUnixTimeSeconds(),
                Name = ev.Name ?? "unknown",
                PlaceSchema = ev.Placeschema ?? "",
                Status = (EventStatus)ev.Status,
                Categories = context.Eventincategories
                                    .Include(x => x.Categories)
                                    .Where(x => x.EventId == ev.Id)
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
    }
}
