using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace UserRegistrationApp.Models
{
    public partial class User : IValidatableObject
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

        [StringLength(15)]
        public string? Mobile { get; set; }

        [StringLength(15)]
        public string? Phone { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "The State field is required.")]
        public int StateId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "The City field is required.")]
        public int CityId { get; set; }

        public string? Hobbies { get; set; }

        public string? PhotoPath { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "You must agree to the terms and conditions.")]
        public bool TermsAgreed { get; set; }

        public virtual State? State { get; set; }
        public virtual City? City { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Mobile) && string.IsNullOrWhiteSpace(Phone))
            {
                yield return new ValidationResult(
                    "Either Mobile Number or Phone Number is required.",
                    new[] { nameof(Mobile), nameof(Phone) });
            }
        }
    }
} 