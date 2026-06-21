using Microsoft.AspNetCore.Identity;

namespace NhaXinh.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? AvatarUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}