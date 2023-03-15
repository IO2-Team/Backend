/*
 * System rezerwacji miejsc na EventDTOy
 *
 * Niniejsza dokumentacja stanowi opis REST API implemtowanego przez serwer centralny. Endpointy 
 *
 * The version of the OpenAPI document: 1.0.0
 * Contact: XXX@pw.edu.pl
 * Generated by: https://openapi-generator.tech
 */

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Org.OpenAPITools.Converters;

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
        [DataMember(Name="id", EmitDefaultValue=true)]
        public long Id { get; set; }

        /// <summary>
        /// Gets or Sets FreePlace
        /// </summary>
        [DataMember(Name="freePlace", EmitDefaultValue=true)]
        public long FreePlace { get; set; }

        /// <summary>
        /// Gets or Sets Title
        /// </summary>
        [DataMember(Name="title", EmitDefaultValue=false)]
        public string Title { get; set; }

        /// <summary>
        /// Gets or Sets StartTime
        /// </summary>
        [DataMember(Name="startTime", EmitDefaultValue=true)]
        public long StartTime { get; set; }

        /// <summary>
        /// Gets or Sets EndTime
        /// </summary>
        [DataMember(Name="endTime", EmitDefaultValue=true)]
        public long EndTime { get; set; }

        /// <summary>
        /// Gets or Sets Name
        /// </summary>
        [DataMember(Name="name", EmitDefaultValue=false)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets PlaceSchema
        /// </summary>
        [DataMember(Name="placeSchema", EmitDefaultValue=false)]
        public string PlaceSchema { get; set; }

        /// <summary>
        /// Gets or Sets Status
        /// </summary>
        [DataMember(Name="status", EmitDefaultValue=true)]
        public EventStatus Status { get; set; }

        /// <summary>
        /// Gets or Sets Categories
        /// </summary>
        [DataMember(Name="categories", EmitDefaultValue=false)]
        public List<CategoryDTO> Categories { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class EventDTO {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  FreePlace: ").Append(FreePlace).Append("\n");
            sb.Append("  Title: ").Append(Title).Append("\n");
            sb.Append("  StartTime: ").Append(StartTime).Append("\n");
            sb.Append("  EndTime: ").Append(EndTime).Append("\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  PlaceSchema: ").Append(PlaceSchema).Append("\n");
            sb.Append("  Status: ").Append(Status).Append("\n");
            sb.Append("  Categories: ").Append(Categories).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((EventDTO)obj);
        }

        /// <summary>
        /// Returns true if EventDTO instances are equal
        /// </summary>
        /// <param name="other">Instance of EventDTO to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(EventDTO other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    Id == other.Id ||
                    
                    Id.Equals(other.Id)
                ) && 
                (
                    FreePlace == other.FreePlace ||
                    
                    FreePlace.Equals(other.FreePlace)
                ) && 
                (
                    Title == other.Title ||
                    Title != null &&
                    Title.Equals(other.Title)
                ) && 
                (
                    StartTime == other.StartTime ||
                    
                    StartTime.Equals(other.StartTime)
                ) && 
                (
                    EndTime == other.EndTime ||
                    
                    EndTime.Equals(other.EndTime)
                ) && 
                (
                    Name == other.Name ||
                    Name != null &&
                    Name.Equals(other.Name)
                ) && 
                (
                    PlaceSchema == other.PlaceSchema ||
                    PlaceSchema != null &&
                    PlaceSchema.Equals(other.PlaceSchema)
                ) && 
                (
                    Status == other.Status ||
                    
                    Status.Equals(other.Status)
                ) && 
                (
                    Categories == other.Categories ||
                    Categories != null &&
                    other.Categories != null &&
                    Categories.SequenceEqual(other.Categories)
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
                    
                    hashCode = hashCode * 59 + Id.GetHashCode();
                    
                    hashCode = hashCode * 59 + FreePlace.GetHashCode();
                    if (Title != null)
                    hashCode = hashCode * 59 + Title.GetHashCode();
                    
                    hashCode = hashCode * 59 + StartTime.GetHashCode();
                    
                    hashCode = hashCode * 59 + EndTime.GetHashCode();
                    if (Name != null)
                    hashCode = hashCode * 59 + Name.GetHashCode();
                    if (PlaceSchema != null)
                    hashCode = hashCode * 59 + PlaceSchema.GetHashCode();
                    
                    hashCode = hashCode * 59 + Status.GetHashCode();
                    if (Categories != null)
                    hashCode = hashCode * 59 + Categories.GetHashCode();
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
