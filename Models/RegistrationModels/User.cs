using System.ComponentModel.DataAnnotations.Schema;

namespace Code123.Models
{
    public class User : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        // Allows binding plaintext passwords from forms safely without DB migration
        [NotMapped]
        public string? Password { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        [NotMapped]
        public IEnumerable<string> RolesList => UserRoles?
            .Where(ur => ur.Role != null)
            .Select(ur => ur.Role.RoleName)
            ?? Enumerable.Empty<string>();
    }
}