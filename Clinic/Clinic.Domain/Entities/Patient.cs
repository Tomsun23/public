using Clinic.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Clinic.Domain.Entities
{
    public class Patient
    {
        public Guid Id { get; set; }
        public PersonName Name { get; set; }

        public string Gender { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }
        public string Active { get; set; }

        public void CopyTo(Patient destination) 
        {
            destination.Id = Id;
            destination.BirthDate = BirthDate;
            destination.Gender = Gender;
            destination.Active = Active;
            Name.CopyTo(destination.Name);
        }
    }
}
