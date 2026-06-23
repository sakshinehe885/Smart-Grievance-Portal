using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartGrievancePortal.Data;
using SmartGrievancePortal.Models;

namespace SmartGrievancePortal.Controllers
{
    [Authorize(Roles = "Staff")]
    public class StaffController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StaffController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var complaints = await _context.Complaints
                .Include(c => c.Category)
                .Include(c => c.Student)
                .Where(c => c.AssignedStaffId == user.Id)
                .OrderByDescending(c => c.DateCreated)
                .ToListAsync();

            ViewBag.TotalCount = complaints.Count;
            ViewBag.PendingCount = complaints.Count(c => c.Status != ComplaintStatus.Resolved);
            ViewBag.ResolvedCount = complaints.Count(c => c.Status == ComplaintStatus.Resolved);

            return View(complaints);
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var complaint = await _context.Complaints
                 .Include(c => c.Student)
                 .Include(c => c.Category)
                 .FirstOrDefaultAsync(c => c.Id == id && c.AssignedStaffId == user.Id);

            if (complaint == null) return NotFound();

            return View(complaint);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, ComplaintStatus status, string? staffRemarks)
        {
            var complaint = await _context.Complaints.FindAsync(id);
            if (complaint == null) return NotFound();

            complaint.Status = status;
            complaint.StaffRemarks = staffRemarks;
            
            if (status == ComplaintStatus.Resolved)
            {
                complaint.DateResolved = DateTime.Now;
            }

            _context.Update(complaint);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Dashboard));
        }

        public async Task<IActionResult> StaffMembers(string? department)
        {
            var staffMembers = await _userManager.GetUsersInRoleAsync("Staff");
            
            if (!string.IsNullOrEmpty(department))
            {
                staffMembers = staffMembers.Where(s => s.Department == department).ToList();
            }

            ViewBag.SelectedDepartment = department ?? "All";
            return View(staffMembers.OrderBy(s => s.FullName).ToList());
        }

        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            return View(user);
        }
    }
}
