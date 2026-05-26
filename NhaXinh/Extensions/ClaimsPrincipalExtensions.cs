using System.Security.Claims;

namespace NhaXinh.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string? GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public static string? GetEmail(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Email);
        }

        public static string? GetFullName(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Name);
        }

        public static bool IsInRole(this ClaimsPrincipal user, string role)
        {
            return user.HasClaim(ClaimTypes.Role, role);
        }

        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return user.IsInRole("Admin");
        }

        public static bool IsLoggedIn(this ClaimsPrincipal user)
        {
            return user.Identity?.IsAuthenticated == true;
        }
    }
}
