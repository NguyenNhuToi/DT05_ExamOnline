using ExamOnlineDB.Data;
using ExamOnlineDB.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExamOnlineDB.Controllers;

public class ProfileController : Controller
{
    private readonly ExamOnlineDBContext _db;
    public ProfileController(ExamOnlineDBContext db) => _db = db;

    private int? GetUserId() => HttpContext.Session.GetInt32("UserId");

    // GET: /Profile
    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });

        var user = await _db.Users.FindAsync(userId.Value);
        if (user == null) return NotFound();

        return View(new ProfileUpdateViewModel
        {
            UserId = user.UserId,
            FullName = user.FullName,
            Email = user.Email,
            UserCode = user.UserCode
        });
    }

    // POST: /Profile/UpdateInfo
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateInfo(ProfileUpdateViewModel model)
    {
        var userId = GetUserId();
        if (userId == null || userId.Value != model.UserId) return RedirectToAction("Login", "Account", new { area = "" });

        if (!ModelState.IsValid) return View("Index", model);

        var user = await _db.Users.FindAsync(userId.Value);
        if (user == null) return NotFound();

        // Kiểm tra trùng email
        if (await _db.Users.AnyAsync(u => u.Email == model.Email && u.UserId != userId.Value))
        {
            ModelState.AddModelError("Email", "Email này đã có người sử dụng.");
            return View("Index", model);
        }

        user.FullName = model.FullName;
        user.Email = model.Email;
        user.UserCode = model.UserCode;

        // Cập nhật lại Session nếu cần
        HttpContext.Session.SetString("FullName", user.FullName);

        await _db.SaveChangesAsync();
        TempData["Success"] = "Cập nhật thông tin thành công!";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Profile/ChangePassword
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        var userId = GetUserId();
        if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });

        if (!ModelState.IsValid)
        {
            // Nếu lỗi mật khẩu, phải load lại thông tin Profile để hiển thị
            var userInfo = await _db.Users.FindAsync(userId.Value);
            var profileModel = new ProfileUpdateViewModel
            {
                UserId = userInfo.UserId,
                FullName = userInfo.FullName,
                Email = userInfo.Email,
                UserCode = userInfo.UserCode
            };
            // Trả về view Index với lỗi
            return View("Index", profileModel);
        }

        var user = await _db.Users.FindAsync(userId.Value);
        if (user == null) return NotFound();

        // Kiểm tra mật khẩu cũ (Lưu ý: đang so sánh plain-text, nên dùng Hash trong thực tế)
        if (user.PasswordHash != model.OldPassword)
        {
            ModelState.AddModelError("OldPassword", "Mật khẩu cũ không đúng.");
            var profileModel = new ProfileUpdateViewModel
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                UserCode = user.UserCode
            };
            return View("Index", profileModel);
        }

        // Cập nhật mật khẩu mới
        user.PasswordHash = model.NewPassword; // Lưu plain-text cho đồ án
        await _db.SaveChangesAsync();

        TempData["Success"] = "Đổi mật khẩu thành công!";
        return RedirectToAction(nameof(Index));
    }
}