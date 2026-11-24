using System.Security.Claims;
using Code123.Data;
using Code123.Models;
using Code123.RegistrationModels.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Code123.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ---------- LOGIN ----------

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectByRole(User.IsInRole("Admin"), User.IsInRole("Student"));

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var input = model.UsernameOrEmail.Trim();

            // Look up by username OR email, eager-loading roles
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == input || u.Email == input);

            // Same message for "user not found" and "wrong password" (avoids leaking which is wrong)
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Invalid User/Password");
                return View(model);
            }

            await SignInUserAsync(user);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            var roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList();
            return RedirectByRole(roles.Contains("Admin"), roles.Contains("Student"));
        }

        // ---------- SIGNUP ----------

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Signup()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectByRole(User.IsInRole("Admin"), User.IsInRole("Student"));

            return View(new SignupViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signup(SignupViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var email = model.Email.Trim().ToLowerInvariant();
            var username = model.Username.Trim();

            if (await _context.Users.AnyAsync(u => u.Email == email))
                ModelState.AddModelError(nameof(model.Email), "That email is already registered.");

            if (await _context.Users.AnyAsync(u => u.Username == username))
                ModelState.AddModelError(nameof(model.Username), "That username is already taken.");

            if (!ModelState.IsValid)
                return View(model);

            var studentRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Student");
            if (studentRole == null)
            {
                ModelState.AddModelError(string.Empty, "Default role is missing. Did you apply the migrations?");
                return View(model);
            }

            var user = new User
            {
                Name = model.Name.Trim(),
                Username = username,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                CreatedAt = DateTime.UtcNow
            };
            user.UserRoles.Add(new UserRole { Role = studentRole });

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Roles are already attached in memory, so no re-query is needed
            await SignInUserAsync(user);

            return RedirectToAction("Dashboard", "Student");
        }

        // ---------- LOGOUT ----------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // ---------- ACCESS DENIED ----------

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            Response.StatusCode = StatusCodes.Status403Forbidden;
            return View();
        }

        // ---------- HELPERS ----------

        private async Task SignInUserAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("FullName", user.Name)
            };

            foreach (var userRole in user.UserRoles)
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.RoleName));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = true });
        }

        private IActionResult RedirectByRole(bool isAdmin, bool isStudent)
        {
            if (isAdmin) return RedirectToAction("Dashboard", "Admin");
            if (isStudent) return RedirectToAction("Dashboard", "Student");
            return RedirectToAction("Index", "Home"); // e.g. Instructor (no dashboard yet)
        }
    }
}
