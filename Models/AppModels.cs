using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SmartGrievancePortal.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Department { get; set; }
        
        // New Registration Fields
        public string? PassportPhotoPath { get; set; }
        public string? Gender { get; set; }
        public string? Course { get; set; }
        public string? Year { get; set; }
        public string? Semester { get; set; }
        public string? Division { get; set; }
        public string? RollNumber { get; set; }
        public string? AlternateContactNumber { get; set; }
        public bool IsHosteller { get; set; }
        public string? Address { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
    }

    public enum ComplaintStatus
    {
        Pending,
        InProgress,
        Resolved,
        Rejected
    }

    public class Complaint
    {
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        // Student Info
        public string? StudentName { get; set; }
        public string? StudentCourse { get; set; }
        public string? StudentYear { get; set; }
        public string? StudentDivision { get; set; }
        public string? StudentRollNumber { get; set; }
        
        // Photo Evidence
        public string? PhotoPath { get; set; }
        
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        
        public string StudentId { get; set; } = string.Empty;
        public ApplicationUser? Student { get; set; }
        
        public string? AssignedStaffId { get; set; }
        public ApplicationUser? AssignedStaff { get; set; }
        
        public ComplaintStatus Status { get; set; } = ComplaintStatus.Pending;
        
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime? DateResolved { get; set; }
        
        public string? StaffRemarks { get; set; }
        public string? AdminResponse { get; set; }
        public string? StudentFeedback { get; set; }
        [Range(1, 5)]
        public int? Rating { get; set; }
    }
}
