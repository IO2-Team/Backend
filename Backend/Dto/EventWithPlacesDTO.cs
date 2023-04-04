/*
 * System rezerwacji miejsc na eventy
 *
 * Niniejsza dokumentacja stanowi opis REST API implemtowanego przez serwer centralny. Endpointy 
 *
 * OpenAPI spec version: 1.0.0
 * Contact: XXX@pw.edu.pl
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Org.OpenAPITools.Models;

namespace IO.Swagger.Models
{ 
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public partial class EventWithPlacesDTO : IEquatable<EventWithPlacesDTO>
    { 
        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [Required]

        [DataMember(Name="id")]
        public long? Id { get; set; }

        /// <summary>
        /// Gets or Sets Title
        /// </summary>
        [Required]

        [DataMember(Name="title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or Sets StartTime
        /// </summary>
        [Required]

        [DataMember(Name="startTime")]
        public long? StartTime { get; set; }

        /// <summary>
        /// Gets or Sets EndTime
        /// </summary>
        [Required]

        [DataMember(Name="endTime")]
        public long? EndTime { get; set; }

        /// <summary>
        /// Gets or Sets Latitude
        /// </summary>
        [Required]

        [DataMember(Name="latitude")]
        public string Latitude { get; set; }

        /// <summary>
        /// Gets or Sets Longitude
        /// </summary>
        [Required]

        [DataMember(Name="longitude")]
        public string Longitude { get; set; }

        /// <summary>
        /// Gets or Sets Name
        /// </summary>
        [Required]

        [DataMember(Name="name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets Status
        /// </summary>
        [Required]

        [DataMember(Name="status")]
        public EventStatus Status { get; set; }

        /// <summary>
        /// Gets or Sets Categories
        /// </summary>
        [Required]

        [DataMember(Name="categories")]
        public List<CategoryDTO> Categories { get; set; }

        /// <summary>
        /// Gets or Sets FreePlace
        /// </summary>
        [Required]

        [DataMember(Name="freePlace")]
        public long? FreePlace { get; set; }

        /// <summary>
        /// Gets or Sets MaxPlace
        /// </summary>
        [Required]

        [DataMember(Name="maxPlace")]
        public long? MaxPlace { get; set; }

        /// <summary>
        /// Gets or Sets Places
        /// </summary>
        [Required]

        [DataMember(Name="places")]
        public List<PlaceDTO> Places { get; set; }

        /// <summary>
        /// Gets or Sets PlaceSchema
        /// </summary>

        [DataMember(Name="placeSchema")]
        public string PlaceSchema { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class EventWithPlacesDTO {\n");
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
            sb.Append("  Places: ").Append(Places).Append("\n");
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
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((EventWithPlacesDTO)obj);
        }

        /// <summary>
        /// Returns true if EventWithPlacesDTO instances are equal
        /// </summary>
        /// <param name="other">Instance of EventWithPlacesDTO to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(EventWithPlacesDTO other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

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
                    Places == other.Places ||
                    Places != null &&
                    Places.SequenceEqual(other.Places)
                ) && 
                (
                    PlaceSchema == other.PlaceSchema ||
                    PlaceSchema != null &&
                    PlaceSchema.Equals(other.PlaceSchema)
                );
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
                    if (Status != null)
                    hashCode = hashCode * 59 + Status.GetHashCode();
                    if (Categories != null)
                    hashCode = hashCode * 59 + Categories.GetHashCode();
                    if (FreePlace != null)
                    hashCode = hashCode * 59 + FreePlace.GetHashCode();
                    if (MaxPlace != null)
                    hashCode = hashCode * 59 + MaxPlace.GetHashCode();
                    if (Places != null)
                    hashCode = hashCode * 59 + Places.GetHashCode();
                    if (PlaceSchema != null)
                    hashCode = hashCode * 59 + PlaceSchema.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
        #pragma warning disable 1591

        public static bool operator ==(EventWithPlacesDTO left, EventWithPlacesDTO right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EventWithPlacesDTO left, EventWithPlacesDTO right)
        {
            return !Equals(left, right);
        }

        #pragma warning restore 1591
        #endregion Operators
    }
}
