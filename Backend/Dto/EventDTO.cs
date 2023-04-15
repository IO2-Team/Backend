/*
 * System rezerwacji miejsc na EventDTOy
 *
 * Niniejsza dokumentacja stanowi opis REST API implemtowanego przez serwer centralny. Endpointy 
 *
 * The version of the OpenAPI document: 1.0.0
 * Contact: XXX@pw.edu.pl
 * Generated by: https://openapi-generator.tech
 */

using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using dionizos_backend_app.Models;

namespace Org.OpenAPITools.Models
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public partial class EventDTO : IEquatable<EventDTO>
    {
        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [Required]

        [DataMember(Name = "id")]
        public long? Id { get; set; }

        /// <summary>
        /// Gets or Sets Title
        /// </summary>
        [Required]

        [DataMember(Name = "title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or Sets StartTime
        /// </summary>
        [Required]

        [DataMember(Name = "startTime")]
        public long? StartTime { get; set; }

        /// <summary>
        /// Gets or Sets EndTime
        /// </summary>
        [Required]

        [DataMember(Name = "endTime")]
        public long? EndTime { get; set; }

        /// <summary>
        /// Gets or Sets Latitude
        /// </summary>
        [Required]

        [DataMember(Name = "latitude")]
        public string Latitude { get; set; }

        /// <summary>
        /// Gets or Sets Longitude
        /// </summary>
        [Required]

        [DataMember(Name = "longitude")]
        public string Longitude { get; set; }

        /// <summary>
        /// Gets or Sets Name
        /// </summary>
        [Required]

        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets Status
        /// </summary>
        [Required]

        [DataMember(Name = "status")]
        public EventStatus Status { get; set; }

        /// <summary>
        /// Gets or Sets Categories
        /// </summary>
        [Required]

        [DataMember(Name = "categories")]
        public List<CategoryDTO> Categories { get; set; }

        /// <summary>
        /// Gets or Sets FreePlace
        /// </summary>
        [Required]

        [DataMember(Name = "freePlace")]
        public long? FreePlace { get; set; }

        /// <summary>
        /// Gets or Sets MaxPlace
        /// </summary>
        [Required]

        [DataMember(Name = "maxPlace")]
        public long? MaxPlace { get; set; }

        /// <summary>
        /// Gets or Sets PlaceSchema
        /// </summary>

        [DataMember(Name = "placeSchema")]
        public string PlaceSchema { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ModelEvent {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  Title: ").Append(Title).Append("\n");
            sb.Append("  StartTime: ").Append(StartTime).Append("\n");
            sb.Append("  EndTime: ").Append(EndTime).Append("\n");
            sb.Append("  Latitude: ").Append(Latitude).Append("\n");
            sb.Append("  Longitude: ").Append(Longitude).Append("\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  Status: ").Append(Status).Append("\n");
            sb.Append("  Categories: ").Append(Categories).Append("\n");
            sb.Append("  FreePlace: ").Append(FreePlace).Append("\n");
            sb.Append("  MaxPlace: ").Append(MaxPlace).Append("\n");
            sb.Append("  PlaceSchema: ").Append(PlaceSchema).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
        public override bool Equals(object obj)
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((EventDTO)obj);
        }

        /// <summary>
        /// Returns true if ModelEvent instances are equal
        /// </summary>
        /// <param name="other">Instance of ModelEvent to be compared</param>
        /// <returns>Boolean</returns>
#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
        public bool Equals(EventDTO other)
#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
            return
                (
                    Id == other.Id ||
                    Id != null &&
                    Id.Equals(other.Id)
                ) &&
                (
                    Title == other.Title ||
                    Title != null &&
                    Title.Equals(other.Title)
                ) &&
                (
                    StartTime == other.StartTime ||
                    StartTime != null &&
                    StartTime.Equals(other.StartTime)
                ) &&
                (
                    EndTime == other.EndTime ||
                    EndTime != null &&
                    EndTime.Equals(other.EndTime)
                ) &&
                (
                    Latitude == other.Latitude ||
                    Latitude != null &&
                    Latitude.Equals(other.Latitude)
                ) &&
                (
                    Longitude == other.Longitude ||
                    Longitude != null &&
                    Longitude.Equals(other.Longitude)
                ) &&
                (
                    Name == other.Name ||
                    Name != null &&
                    Name.Equals(other.Name)
                ) &&
                (
                    Status == other.Status ||
                    Status != null &&
                    Status.Equals(other.Status)
                ) &&
                (
                    Categories == other.Categories ||
                    Categories != null &&
                    Categories.SequenceEqual(other.Categories)
                ) &&
                (
                    FreePlace == other.FreePlace ||
                    FreePlace != null &&
                    FreePlace.Equals(other.FreePlace)
                ) &&
                (
                    MaxPlace == other.MaxPlace ||
                    MaxPlace != null &&
                    MaxPlace.Equals(other.MaxPlace)
                ) &&
                (
                    PlaceSchema == other.PlaceSchema ||
                    PlaceSchema != null &&
                    PlaceSchema.Equals(other.PlaceSchema)
                );
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hashCode = 41;
                // Suitable nullity checks etc, of course :)
                if (Id != null)
                    hashCode = hashCode * 59 + Id.GetHashCode();
                if (Title != null)
                    hashCode = hashCode * 59 + Title.GetHashCode();
                if (StartTime != null)
                    hashCode = hashCode * 59 + StartTime.GetHashCode();
                if (EndTime != null)
                    hashCode = hashCode * 59 + EndTime.GetHashCode();
                if (Latitude != null)
                    hashCode = hashCode * 59 + Latitude.GetHashCode();
                if (Longitude != null)
                    hashCode = hashCode * 59 + Longitude.GetHashCode();
                if (Name != null)
                    hashCode = hashCode * 59 + Name.GetHashCode();
#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                if (Status != null)
                    hashCode = hashCode * 59 + Status.GetHashCode();
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                if (Categories != null)
                    hashCode = hashCode * 59 + Categories.GetHashCode();
                if (FreePlace != null)
                    hashCode = hashCode * 59 + FreePlace.GetHashCode();
                if (MaxPlace != null)
                    hashCode = hashCode * 59 + MaxPlace.GetHashCode();
                if (PlaceSchema != null)
                    hashCode = hashCode * 59 + PlaceSchema.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
#pragma warning disable 1591

        public static bool operator ==(EventDTO left, EventDTO right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EventDTO left, EventDTO right)
        {
            return !Equals(left, right);
        }

#pragma warning restore 1591
        #endregion Operators
    }
}