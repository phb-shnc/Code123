using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;

namespace AdminDashboard.Models;

public static class UserRoles
{
    public const string Admin = "Admin";
    public const string Student = "Student";
}

public class UserModel : IdentityModel
{
    [BindNever] public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required, AllowedValues(UserRoles.Admin, UserRoles.Student, ErrorMessage = "Role must be Admin or Student.")]
    public string Role { get; set; } = UserRoles.Student;

    
    [BindNever, ValidateNever, StringLength(100)]
    public string PasswordHash { get; set; } = string.Empty;


    [NotMapped, DataType(DataType.Password), StringLength(72, MinimumLength = 8)]
    public string? Password { get; set; }
}

