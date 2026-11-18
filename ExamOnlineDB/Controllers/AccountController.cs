using ExamOnlineDB.Data;
using ExamOnlineDB.Models;
using ExamOnlineDB.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExamOnlineDB.Controllers;

public class AccountController : Controller
{
    private readonly ExamOnlineDBContext _context;

    public AccountController(ExamOnlineDBContext context) => _context = context;

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var username = model.Username.Trim();
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user is null || user.PasswordHash != model.Password)
        {
            ModelState.AddModelError(string.Empty, "Sai tên đăng nhập hoặc mật khẩu");
            return View(model);
        }

        if (!user.IsActive)
        {
            ModelState.AddModelError(string.Empty, "Tài khoản đã bị khóa");
            return View(model);
        }

        HttpContext.Session.SetInt32("UserId", user.UserId);
        HttpContext.Session.SetString("Username", user.Username);
        HttpContext.Session.SetString("FullName", user.FullName);
        HttpContext.Session.SetString("RoleName", user.Role?.RoleName ?? "Student");

        return user.Role?.RoleName switch
        {
            "Admin" => RedirectToAction("Index", "Dashboard", new { area = "Admin" }),
            "Teacher" => RedirectToAction("Index", "Questions", new { area = "Teacher" }),
            _ => RedirectToAction("Index", "Exams", new { area = "Student" })
        };
    }

    [HttpGet]
    public async Task<IActionResult> Register()
    {
        var vm = new RegisterViewModel
        {
            IsFirstAdmin = !await HasAdminAsync()
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        model.Username = model.Username.Trim();
        model.FullName = model.FullName.Trim();
        model.Email = model.Email.Trim();
        model.UserCode = string.IsNullOrWhiteSpace(model.UserCode) ? null : model.UserCode.Trim();
        model.IsFirstAdmin = !await HasAdminAsync();

        if (!ModelState.IsValid)
            return View(model);

        if (model.RoleName is not ("Student" or "Teacher" or "Admin"))
        {
            ModelState.AddModelError(nameof(model.RoleName), "Vai trò không hợp lệ.");
            return View(model);
        }

        if (model.RoleName == "Admin" && await HasAdminAsync())
        {
            ModelState.AddModelError(string.Empty, "Hệ thống đã có Admin. Không thể đăng ký thêm Admin.");
            return View(model);
        }

        if (await _context.Users.AnyAsync(u => u.Username == model.Username))
        {
            ModelState.AddModelError(nameof(model.Username), "Tên đăng nhập đã có người dùng.");
            return View(model);
        }

        if (await _context.Users.AnyAsync(u => u.Email == model.Email))
        {
            ModelState.AddModelError(nameof(model.Email), "Email đã được dùng.");
            return View(model);
        }

        var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == model.RoleName);
        if (role is null)
        {
            ModelState.AddModelError(string.Empty, "Lỗi: chưa có role trong DB. Chạy file sql/init-database.sql trước.");
            return View(model);
        }

        _context.Users.Add(new User
        {
            Username = model.Username,
            PasswordHash = model.Password,
            FullName = model.FullName,
            Email = model.Email,
            UserCode = model.UserCode,
            RoleId = role.RoleId,
            IsActive = true,
            CreatedAt = DateTime.Now
        });

        await _context.SaveChangesAsync();
        TempData["Success"] = $"Đăng ký {model.RoleName} thành công! Mời đăng nhập.";
        return RedirectToAction(nameof(Login));
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Login));
    }

    private async Task<bool> HasAdminAsync()
    {
        var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Admin");
        return adminRole is not null && await _context.Users.AnyAsync(u => u.RoleId == adminRole.RoleId);
    }
}
