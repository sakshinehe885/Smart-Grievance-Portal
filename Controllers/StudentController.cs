using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartGrievancePortal.Data;
using SmartGrievancePortal.Models;

namespace SmartGrievancePortal.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public StudentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            return View(user);
        }

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var complaints = await _context.Complaints
                .Include(c => c.Category)
                .Where(c => c.StudentId == user.Id)
                .OrderByDescending(c => c.DateCreated)
                .ToListAsync();

            ViewBag.TotalComplaints = complaints.Count;
            ViewBag.ResolvedComplaints = complaints.Count(c => c.Status == ComplaintStatus.Resolved);
            ViewBag.PendingComplaints = complaints.Count(c => c.Status == ComplaintStatus.Pending);
            ViewBag.RejectedComplaints = complaints.Count(c => c.Status == ComplaintStatus.Rejected);

            return View(complaints);
        }

        public IActionResult Support()
        {
            return RedirectToAction("Support", "Home");
        }

        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var complaint = new Complaint
            {
                StudentName = user.FullName,
                StudentCourse = user.Course,
                StudentYear = user.Year,
                StudentDivision = user.Division,
                StudentRollNumber = user.RollNumber
            };

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            return View(complaint);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Complaint complaint, IFormFile? photoFile)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Manually re-validate model state after setting user
            // Or remove validation for Student/StudentId if needed, but since they are required in model...
            // Actually, we should use a ViewModel or ignore validation for properties we set in controller.
            // Let's modify the ModelState here.
            
            ModelState.Remove("Student");
            ModelState.Remove("StudentId");
            ModelState.Remove("Category"); // EF Core fix

            if (ModelState.IsValid)
            {
                if (photoFile != null && photoFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + photoFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await photoFile.CopyToAsync(fileStream);
                    }
                    complaint.PhotoPath = "/uploads/" + uniqueFileName;
                }

                complaint.StudentId = user.Id;
                complaint.DateCreated = DateTime.Now;
                complaint.Status = ComplaintStatus.Pending;

                // Ensure student details are exactly from the user record
                complaint.StudentName = user.FullName;
                complaint.StudentCourse = user.Course;
                complaint.StudentYear = user.Year;
                complaint.StudentDivision = user.Division;
                complaint.StudentRollNumber = user.RollNumber;

                _context.Complaints.Add(complaint);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Dashboard));
            }
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            return View(complaint);
        }
        
        public async Task<IActionResult> Track(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null) return NotFound();

            Complaint? complaint = null;

            if (id.HasValue)
            {
                complaint = await _context.Complaints
                    .Include(c => c.Category)
                    .Include(c => c.AssignedStaff)
                    .FirstOrDefaultAsync(m => m.Id == id && m.StudentId == user.Id);
            }
            else
            {
                // If no ID is provided, automatically load the most recent complaint
                complaint = await _context.Complaints
                    .Include(c => c.Category)
                    .Include(c => c.AssignedStaff)
                    .Where(m => m.StudentId == user.Id)
                    .OrderByDescending(c => c.DateCreated)
                    .FirstOrDefaultAsync();
            }

            if (complaint == null) 
            {
                return RedirectToAction(nameof(Dashboard));
            }

            return View(complaint);
        }

        public async Task<IActionResult> DownloadCopy(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null) return NotFound();

            Complaint? complaint = null;

            if (id.HasValue)
            {
                complaint = await _context.Complaints
                    .Include(c => c.Category)
                    .Include(c => c.AssignedStaff)
                    .FirstOrDefaultAsync(m => m.Id == id && m.StudentId == user.Id);
            }
            else
            {
                complaint = await _context.Complaints
                    .Include(c => c.Category)
                    .Include(c => c.AssignedStaff)
                    .Where(m => m.StudentId == user.Id)
                    .OrderByDescending(c => c.DateCreated)
                    .FirstOrDefaultAsync();
            }

            if (complaint == null) 
            {
                return RedirectToAction(nameof(Dashboard));
            }

            return View(complaint);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitFeedback(int id, string studentFeedback, int? rating)
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null) return NotFound();

            var complaint = await _context.Complaints
                .FirstOrDefaultAsync(m => m.Id == id && m.StudentId == user.Id);

            if (complaint == null) return NotFound();

            if (complaint.Status == ComplaintStatus.Resolved)
            {
                complaint.StudentFeedback = studentFeedback;
                complaint.Rating = rating;
                _context.Update(complaint);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Track), new { id = complaint.Id });
        }
    }
}
