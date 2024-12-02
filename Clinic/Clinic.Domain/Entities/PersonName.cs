using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace Clinic.Domain.Entities
{
    public class PersonName
    {
        public Guid Id { get; set; }

        [JsonIgnore]
        public Patient? Patient { get; set; }
        public string? Use { get; set; }
        [Required]
        public string Family { get; set; }
        public string[]? Given { get; set; }

        public void CopyTo(PersonName destination)
        {
            destination.Use = Use;
            destination.Family = Family;
            destination.Given = Given;
        }
    }
}
