using Code123.Data;
using Code123.Models;
using Microsoft.EntityFrameworkCore;
using BCryptNet = BCrypt.Net.BCrypt;

namespace Code123.Controllers.AdminControllers;

public class UsersController(ApplicationDbContext context)
    : CrudController<User>(context)
{
    protected override string DisplayName => "User";

    protected override IQueryable<User> GetQuery() =>
        DbSet.AsNoTracking()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .OrderBy(u => u.Name);

    protected override async Task ValidateAsync(User model, int? id)
    {
        model.Email = (model.Email ?? string.Empty).Trim().ToLowerInvariant();

        if (await Context.Users.AnyAsync(u =>
            u.Email == model.Email && u.Id != id))
        {
            ModelState.AddModelError(
                nameof(model.Email), // Fixed from User.Email
                "This email is already registered.");
        }

        if (id is null && string.IsNullOrWhiteSpace(model.Password))
        {
            ModelState.AddModelError(
                nameof(model.Password), // Fixed from User.Password
                "Password is required for new users.");
        }
    }

    protected override void OnCreating(User model)
    {
        if (!string.IsNullOrWhiteSpace(model.Password))
        {
            model.PasswordHash = BCryptNet.HashPassword(model.Password);
        }
    }

    protected override void Apply(User existing, User model)
    {
        existing.Name = model.Name;
        existing.Username = model.Username;
        existing.Email = model.Email;

        if (!string.IsNullOrWhiteSpace(model.Password))
        {
            existing.PasswordHash = BCryptNet.HashPassword(model.Password);
        }
    }

    protected override async Task<string?> CheckDeleteAsync(User entity)
    {
        // Check if user is an Admin via the new UserRoles relationship
        bool isAdmin = entity.UserRoles.Any(ur => ur.Role?.RoleName == "Admin");

        if (isAdmin)
        {
            int adminCount = await Context.Users
                .Where(u => u.UserRoles.Any(ur => ur.Role != null && ur.Role.RoleName == "Admin"))
                .CountAsync();

            if (adminCount <= 1)
            {
                return "You cannot delete the last Admin.";
            }
        }

        return null;
    }
}