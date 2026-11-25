using Code123.Data;
using Code123.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Code123.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<UserModel> _passwordHasher = new();
        private const string V = "~/Views/Admin/Users/";

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Fetch database entities and map them to UserModel for the view
            var users = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role) // Assuming navigation properties exist
                .AsNoTracking()
                .Select(u => new UserModel
                {
                    Id = u.Id,
                    Name = u.Name,
                    Username = u.Username,
                    Email = u.Email,
                    // Pulling the first role name or default to Student if none assigned
                    Role = u.UserRoles.Select(ur => ur.Role.RoleName).FirstOrDefault() ?? UserRoles.Student
                })
                .ToListAsync();

            return View(V + "Index.cshtml", users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please provide valid user information.";
                return RedirectToAction(nameof(Index));
            }

            // Map incoming UserModel view data to the database User entity
            var userEntity = new User
            {
                Name = model.Name,
                Username = string.IsNullOrWhiteSpace(model.Username) ? model.Email : model.Username,
                Email = model.Email,
                PasswordHash = _passwordHasher.HashPassword(model, model.Password ?? "DefaultPassword123!")
            };

            // Find the Role entity in the database matching model.Role ("Admin" or "Student")
            var roleEntity = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleName == model.Role);

            // Link the role to the user via the UserRoles join table
            if (roleEntity != null)
            {
                userEntity.UserRoles.Add(new UserRole
                {
                    User = userEntity,
                    Role = roleEntity
                });
            }

            _context.Users.Add(userEntity);
            await _context.SaveChangesAsync();

            TempData["Success"] = "User created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var model = new UserModel
            {
                Id = user.Id,
                Name = user.Name,
                Username = user.Username,
                Email = user.Email,
                Role = user.UserRoles.Select(ur => ur.Role.RoleName).FirstOrDefault() ?? UserRoles.Student
            };

            return View(V + "Edit.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please provide valid user information.";
                return RedirectToAction(nameof(Index));
            }

            var userEntity = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (userEntity == null)
            {
                return NotFound();
            }

            userEntity.Name = model.Name;
            userEntity.Username = string.IsNullOrWhiteSpace(model.Username) ? model.Email : model.Username;
            userEntity.Email = model.Email;

            // Update password only if a new one is provided
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                userEntity.PasswordHash = _passwordHasher.HashPassword(model, model.Password);
            }

            // Update Role
            userEntity.UserRoles.Clear();
            var roleEntity = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleName == model.Role);

            if (roleEntity != null)
            {
                userEntity.UserRoles.Add(new UserRole
                {
                    User = userEntity,
                    Role = roleEntity
                });
            }

            _context.Update(userEntity);
            await _context.SaveChangesAsync();

            TempData["Success"] = "User updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                TempData["Success"] = "User deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}