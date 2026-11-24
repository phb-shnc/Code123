using Code123.Data;
using Code123.Models;
using Microsoft.EntityFrameworkCore;
using BCryptNet = BCrypt.Net.BCrypt;

namespace Code123.Controllers;

public class UsersController(ApplicationDbContext context) : CrudController<UserModel>(context)
{
    protected override string DisplayName => "User";

    protected override async Task ValidateAsync(UserModel model, int? id)
    {
        model.Email = (model.Email ?? string.Empty).Trim().ToLowerInvariant();

        if (await Context.Users.AnyAsync(u => u.Email == model.Email && u.Id != id))
            ModelState.AddModelError(nameof(UserModel.Email), "This email is already registered.");

        if (id is null && string.IsNullOrWhiteSpace(model.Password))
            ModelState.AddModelError(nameof(UserModel.Password), "Password is required for new users.");
    }

    protected override void OnCreating(UserModel model) =>
        model.PasswordHash = BCryptNet.HashPassword(model.Password!);

    protected override void Apply(UserModel existing, UserModel model)
    {
        existing.Name = model.Name;
        existing.Email = model.Email;
        existing.Role = model.Role;
        if (!string.IsNullOrWhiteSpace(model.Password))             
            existing.PasswordHash = BCryptNet.HashPassword(model.Password);
    }

    protected override async Task<string?> CheckDeleteAsync(UserModel entity) =>
        entity.Role == UserRoles.Admin && await Context.Users.CountAsync(u => u.Role == UserRoles.Admin) <= 1
            ? "You cannot delete the last Admin."
            : null;
}
