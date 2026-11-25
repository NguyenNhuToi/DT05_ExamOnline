using ExamOnlineDB.Data;
using ExamOnlineDB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExamOnlineDB.Areas.Student.Controllers;

[Area("Student")]
public class HistoryController : Controller
{
    private readonly ExamOnlineDBContext _db;
    public HistoryController(ExamOnlineDBContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        // 1. Kiểm tra Session thay vì dùng [Authorize]
        var role = HttpContext.Session.GetString("RoleName");
        var userId = HttpContext.Session.GetInt32("UserId");

        // Nếu chưa đăng nhập hoặc không phải Student -> Chuyển về trang Login
        if (role != "Student" || userId == null)
        {
            return RedirectToAction("Login", "Account", new { area = "" });
        }

        // 2. Lấy UserId từ Session
        int studentId = userId.Value;

        // 3. Truy vấn dữ liệu
        var userExams = await _db.UserExams
            .Include(u => u.Exam)
                .ThenInclude(e => e!.Subject)
            .Where(u => u.StudentId == studentId && u.Status == 2) // Status = 2 là đã nộp
            .OrderByDescending(u => u.SubmittedAt)
            .ToListAsync();

        return View(userExams);
    }
}