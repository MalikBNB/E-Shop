using System.Text.Json.Serialization;

namespace Core.Entities.Identity
{
    public class Address : BaseEntity
    {
        public required string Line1 { get; set; }
        public string? Line2 { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string ZipCode { get; set; }
        public required string Country { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

    }
}