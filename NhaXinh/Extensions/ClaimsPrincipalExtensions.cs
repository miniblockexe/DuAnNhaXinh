using System.Security.Claims;

namespace NhaXinh.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string? GetUserId(this ClaimsPrincipal user)
             => user.FindFirstValue(ClaimTypes.NameIdentifier);

        public static string? GetEmail(this ClaimsPrincipal user)
            => user.FindFirstValue(ClaimTypes.Email);

        public static string? GetFullName(this ClaimsPrincipal user)
            => user.FindFirstValue(ClaimTypes.Name);
        public static bool IsAdmin(this ClaimsPrincipal user)
            => user.IsInRole("Admin");

        public static bool IsLoggedIn(this ClaimsPrincipal user)
            => user.Identity?.IsAuthenticated == true;
    }
}
