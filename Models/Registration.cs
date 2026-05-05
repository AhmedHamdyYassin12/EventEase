// Models/Registration.cs

using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class Registration
    {
        // [Required] = this field must not be empty
        [Required(ErrorMessage = "Full name is required.")]
        // [StringLength] = limits how long the string can be
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be 2–100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        // [EmailAddress] = must look like a valid email
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        // [Phone] = must look like a phone number
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        public string Phone { get; set; } = string.Empty;

        // Which event this registration is for
        public int EventId { get; set; }

        // Computed from EventId — set when we process the registration
        public string EventName { get; set; } = string.Empty;
    }
}