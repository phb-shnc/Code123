using Code123.Controllers;
using Code123.Data;
using Code123.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCryptNet = BCrypt.Net.BCrypt;

namespace Code123.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class UsersController(ApplicationDbContext context)
    : CrudController<User>(context)
{
    private const int StudentRoleId = 1; // seeded in ApplicationDbContext

    protected override string DisplayName => "User";

    protected override IQueryable<User> GetQuery() =>
        DbSet.AsNoTracking()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .OrderBy(u => u.Name);

    protected override async Task ValidateAsync(User model, int? id)
    {
        model.Email = (model.Email ?? string.Empty).Trim().ToLowerInvariant();
        model.Username = (model.Username ?? string.Empty).Trim();

        if (await Context.Users.AnyAsync(u => u.Email == model.Email && u.Id != id))
        {
            ModelState.AddModelError(nameof(model.Email), "This email is already registered.");
        }

        if (await Context.Users.AnyAsync(u => u.Username == model.Username && u.Id != id))
        {
            ModelState.AddModelError(nameof(model.Username), "This username is already taken.");
        }

        if (id is null && string.IsNullOrWhiteSpace(model.Password))
        {
            ModelState.AddModelError(nameof(model.Password), "Password is required for new users.");
        }
    }

    protected override void OnCreating(User model)
    {
        if (!string.IsNullOrWhiteSpace(model.Password))
        {
            model.PasswordHash = BCryptNet.HashPassword(model.Password);
        }

        // New users start as Students so they can actually sign in
        if (!model.UserRoles.Any())
        {
            model.UserRoles.Add(new UserRole { RoleId = StudentRoleId });
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
        // Ask the database directly, so this doesn't depend on what was loaded
        bool isAdmin = await Context.UserRoles
            .AnyAsync(ur => ur.UserId == entity.Id && ur.Role.RoleName == "Admin");

        if (!isAdmin)
            return null;

        int adminCount = await Context.UserRoles
            .CountAsync(ur => ur.Role.RoleName == "Admin");

        return adminCount <= 1 ? "You cannot delete the last Admin." : null;
    }
}