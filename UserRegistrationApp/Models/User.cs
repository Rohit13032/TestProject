using System;
using System.ComponentModel.DataAnnotations;

namespace UserRegistrationApp.Models
{
    public partial class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(25)]
        public string Name { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? Mobile { get; set; }

        public string? Phone { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a state.")]
        public int StateId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a city.")]
        public int CityId { get; set; }

        public string? Hobbies { get; set; }

        public string? PhotoPath { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "You must agree to the terms and conditions.")]
        public bool TermsAgreed { get; set; }

        public virtual State? State { get; set; }
        public virtual City? City { get; set; }
    }
} 