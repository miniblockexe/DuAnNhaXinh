using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NhaXinh.Models;
using NhaXinh.Services.Interfaces;
using NhaXinh.ViewModels.Account;

namespace NhaXinh.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IFileService _fileService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IFileService fileService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _fileService = fileService;
        }


        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user is { IsActive: false })
                {
                    await _signInManager.SignOutAsync();
                    ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khoá. Vui lòng liên hệ quản trị viên.");
                    return View(model);
                }

                var roles = await _userManager.GetRolesAsync(user!);

                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);

                if (roles.Contains("Staff"))
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

                return RedirectToAction("Index", "Home");
            }

            var error = result.IsLockedOut
                ? "Tài khoản bị khóa. Vui lòng thử lại sau ít phút."
                : "Email hoặc mật khẩu không chính xác.";

            ModelState.AddModelError(string.Empty, error);
            return View(model);
        }


        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                TempData["Success"] = "Đăng ký thành công! Chào mừng bạn đến với Nhà Xinh.";
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            return View(new ProfileViewModel
            {
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                AvatarUrl = user.AvatarUrl,
                Email = user.Email ?? string.Empty,
                CreatedAt = user.CreatedAt
            });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            model.AvatarUrl = user.AvatarUrl;
            model.Email = user.Email ?? string.Empty;
            model.CreatedAt = user.CreatedAt;

            if (!ModelState.IsValid) return View(model);

            if (model.AvatarFile is not null && model.AvatarFile.Length > 0)
            {
                var (isValid, errorMessage) = _fileService.IsValidImage(model.AvatarFile);
                if (!isValid)
                {
                    ModelState.AddModelError(nameof(model.AvatarFile), errorMessage);
                    return View(model);
                }

                _fileService.DeleteImage(user.AvatarUrl);

                (bool saved, string saveMsg, string? filePath) = await _fileService.SaveImageAsync(
                    model.AvatarFile, ImageFolder.Avatars);

                if (!saved)
                {
                    ModelState.AddModelError(nameof(model.AvatarFile), saveMsg);
                    return View(model);
                }

                user.AvatarUrl = filePath;
            }

            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = "Cập nhật thông tin thành công!";
                return RedirectToAction(nameof(Profile));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }
    }
}
