using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NhaXinh.Models;
using NhaXinh.Repositories.Interfaces;

namespace NhaXinh.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private const int PageSize = 10;

        public UserController(
            IUserRepository userRepository,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, string? keyword = null)
        {
            var (items, totalCount) = await _userRepository.GetPagedAsync(page, PageSize, keyword);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
            ViewBag.TotalCount = totalCount;
            ViewBag.Keyword = keyword;
            return View(items);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user is null) return NotFound();

            ViewBag.CurrentRoles = await _userManager.GetRolesAsync(user);
            ViewBag.AllRoles = _roleManager.Roles.Select(r => r.Name!).ToList();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(string id, bool isActive)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user is null) return NotFound();

            await _userRepository.SetActiveStatusAsync(id, isActive);
            TempData["Success"] = isActive
                ? $"Đã mở khoá tài khoản «{user.FullName}»."
                : $"Đã khoá tài khoản «{user.FullName}».";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();

            if (await _userManager.IsInRoleAsync(user, role))
            {
                TempData["Error"] = $"Người dùng đã có quyền «{role}» rồi.";
                return RedirectToAction(nameof(Detail), new { id = userId });
            }

            await _userManager.AddToRoleAsync(user, role);
            TempData["Success"] = $"Đã gán quyền «{role}» cho «{user.FullName}».";
            return RedirectToAction(nameof(Detail), new { id = userId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();

            var me = await _userManager.GetUserAsync(User);
            if (me?.Id == userId && role == "Admin")
            {
                TempData["Error"] = "Không thể tự xóa quyền Admin của chính mình.";
                return RedirectToAction(nameof(Detail), new { id = userId });
            }

            await _userManager.RemoveFromRoleAsync(user, role);
            TempData["Success"] = $"Đã xóa quyền «{role}» khỏi «{user.FullName}».";
            return RedirectToAction(nameof(Detail), new { id = userId });
        }
    }
}
