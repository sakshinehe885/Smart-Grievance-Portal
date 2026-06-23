using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartGrievancePortal.Data;
using SmartGrievancePortal.Models;
using SmartGrievancePortal.ViewModels;

namespace SmartGrievancePortal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Dashboard(int? categoryId)
        {
            var query = _context.Complaints
                .Include(c => c.Category)
                .Include(c => c.Student)
                .Include(c => c.AssignedStaff)
                .AsQueryable();

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(c => c.CategoryId == categoryId.Value);
            }

            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", categoryId);

            // Statistics for Dashboard Cards
            ViewBag.TotalComplaints = await _context.Complaints.CountAsync();
            ViewBag.ResolvedComplaints = await _context.Complaints.CountAsync(c => c.Status == ComplaintStatus.Resolved);
            ViewBag.PendingComplaints = await _context.Complaints.CountAsync(c => c.Status == ComplaintStatus.Pending);
            
            var students = await _userManager.GetUsersInRoleAsync("Student");
            var staff = await _userManager.GetUsersInRoleAsync("Staff");
            ViewBag.TotalStudents = students.Count;
            ViewBag.TotalStaff = staff.Count;

            var complaints = await query.OrderByDescending(c => c.DateCreated).ToListAsync();
            return View(complaints);
        }

        public async Task<IActionResult> AssignStaff(int? id)
        {
            if (id == null) return NotFound();

            var complaint = await _context.Complaints.FindAsync(id);
            if (complaint == null) return NotFound();

            var staffUsers = await _userManager.GetUsersInRoleAsync("Staff");
            ViewBag.StaffList = new SelectList(staffUsers, "Id", "FullName");

            return View(complaint);
        }

        [HttpPost]
        public async Task<IActionResult> AssignStaff(int id, string assignedStaffId)
        {
            var complaint = await _context.Complaints.FindAsync(id);
            if (complaint == null) return NotFound();

            complaint.AssignedStaffId = assignedStaffId;
            if (complaint.Status == ComplaintStatus.Pending)
            {
                complaint.Status = ComplaintStatus.InProgress;
            }
            _context.Update(complaint);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Dashboard));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var complaint = await _context.Complaints
                .Include(c => c.Category)
                .Include(c => c.Student)
                .Include(c => c.AssignedStaff)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (complaint == null) return NotFound();

            return View(complaint);
        }

        [HttpPost]
        public async Task<IActionResult> AddResponse(int id, string adminResponse, ComplaintStatus status)
        {
            var complaint = await _context.Complaints.FindAsync(id);
            if (complaint == null) return NotFound();

            if (!string.IsNullOrEmpty(adminResponse))
            {
                complaint.AdminResponse = adminResponse;
            }
            complaint.Status = status;

            if (status == ComplaintStatus.Resolved && !complaint.DateResolved.HasValue)
            {
                complaint.DateResolved = DateTime.Now;
            }

            _context.Update(complaint);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = complaint.Id });
        }

        public async Task<IActionResult> Users(string role)
        {
            IList<ApplicationUser> users;
            if (!string.IsNullOrEmpty(role))
            {
                users = await _userManager.GetUsersInRoleAsync(role);
                ViewBag.SelectedRole = role;
            }
            else
            {
                users = await _userManager.Users.ToListAsync();
                ViewBag.SelectedRole = "All";
            }
            return View(users);
        }

        public IActionResult CreateStaff()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateStaff(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                string? photoPath = null;
                if (model.PassportPhoto != null && model.PassportPhoto.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "staff");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.PassportPhoto.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.PassportPhoto.CopyToAsync(fileStream);
                    }
                    photoPath = "/uploads/staff/" + uniqueFileName;
                }

                var user = new ApplicationUser
                {
                   UserName = model.Email,
                   Email = model.Email,
                   FullName = model.FullName,
                   Department = model.Department,
                   Gender = model.Gender,
                   PassportPhotoPath = photoPath,
                   EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Staff");
                    return RedirectToAction(nameof(Users));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStaff(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                // Unassign this staff from any complaints
                var complaints = await _context.Complaints.Where(c => c.AssignedStaffId == id).ToListAsync();
                foreach (var complaint in complaints)
                {
                    complaint.AssignedStaffId = null;
                }
                await _context.SaveChangesAsync();

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Users));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            // If we got this far, something failed, redisplay form
            var users = await _userManager.Users.ToListAsync();
            return View("Users", users);
        }
    }
}
