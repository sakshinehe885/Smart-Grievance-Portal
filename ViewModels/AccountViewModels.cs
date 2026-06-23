using System.ComponentModel.DataAnnotations;

namespace SmartGrievancePortal.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Department (Optional)")]
        public string? Department { get; set; }

        [Required]
        public string Gender { get; set; } = string.Empty;

        [Display(Name = "Passport Size Photo")]
        public Microsoft.AspNetCore.Http.IFormFile? PassportPhoto { get; set; }
    }

    public class StudentRegisterViewModel
    {
        [Required]
        [Display(Name = "Passport Size Photo")]
        public Microsoft.AspNetCore.Http.IFormFile? PassportPhoto { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string Gender { get; set; } = string.Empty;

        [Required]
        public string Course { get; set; } = string.Empty;

        [Required]
        public string Year { get; set; } = string.Empty;

        [Required]
        public string Division { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Roll No.")]
        public string RollNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Alternate Contact Number")]
        public string AlternateContactNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Hosteller")]
        public string IsHosteller { get; set; } = string.Empty; // using string "Yes" or "No" for simpler radio binding if needed, or bool. Actually, bool is cleaner.

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "You must confirm that all information is correct.")]
        [Display(Name = "I confirm that all the information provided is correct.")]
        public bool IsInformationCorrect { get; set; }
    }
}
