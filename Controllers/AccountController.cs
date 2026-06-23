using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartGrievancePortal.Models;
using SmartGrievancePortal.ViewModels;

namespace SmartGrievancePortal.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _webHostEnvironment;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, Microsoft.AspNetCore.Hosting.IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(StudentRegisterViewModel model)
        {
            if (!model.IsInformationCorrect)
            {
                ModelState.AddModelError("IsInformationCorrect", "You must confirm the information is correct.");
            }

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser 
                { 
                    UserName = model.Email, 
                    Email = model.Email, 
                    FullName = model.FullName,
                    Gender = model.Gender,
                    Course = model.Course,
                    Year = model.Year,
                    Division = model.Division,
                    RollNumber = model.RollNumber,
                    PhoneNumber = model.ContactNumber,
                    AlternateContactNumber = model.AlternateContactNumber,
                    IsHosteller = model.IsHosteller == "True",
                    Address = model.Address
                };

                if (model.PassportPhoto != null && model.PassportPhoto.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "passports");
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
                    user.PassportPhotoPath = "/uploads/passports/" + uniqueFileName;
                }

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Student");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (await _userManager.IsInRoleAsync(user!, "Admin"))
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else if (await _userManager.IsInRoleAsync(user!, "Staff"))
                    {
                        return RedirectToAction("Dashboard", "Staff");
                    }
                    else
                    {
                        return RedirectToAction("Dashboard", "Student");
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
