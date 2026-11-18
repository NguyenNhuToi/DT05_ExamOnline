using ExamOnlineDB.Data;
using ExamOnlineDB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExamOnlineDB.Areas.Admin.Controllers;

[Area("Admin")]
public class UsersController : Controller
{
    private readonly ExamOnlineDBContext _context;

    public UsersController(ExamOnlineDBContext context) => _context = context;

    private bool IsAdmin() => HttpContext.Session.GetString("RoleName") == "Admin";

    public async Task<IActionResult> Index()
    {
        if (!IsAdmin())
            return RedirectToAction("Login", "Account", new { area = "" });

        var users = await _context.Users
            .Include(u => u.Role)
            .OrderBy(u => u.RoleId)
            .ThenBy(u => u.Username)
            .ToListAsync();

        return View(users);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        if (!IsAdmin())
            return RedirectToAction("Login", "Account", new { area = "" });

        var user = await _context.Users.FindAsync(id);
        if (user is null)
            return NotFound();

        if (user.UserId == HttpContext.Session.GetInt32("UserId"))
        {
            TempData["Error"] = "Không thể khóa chính mình.";
            return RedirectToAction(nameof(Index));
        }

        user.IsActive = !user.IsActive;
        await _context.SaveChangesAsync();
        TempData["Success"] = $"Đã {(user.IsActive ? "mở khóa" : "khóa")} {user.Username}.";
        return RedirectToAction(nameof(Index));
    }
}
