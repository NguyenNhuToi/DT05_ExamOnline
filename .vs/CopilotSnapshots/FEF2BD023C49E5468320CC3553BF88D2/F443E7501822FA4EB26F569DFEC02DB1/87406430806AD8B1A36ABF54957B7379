using ExamOnlineDB.Data;
using ExamOnlineDB.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExamOnlineDB.Areas.Student.Controllers;

[Area("Student")]
public class ExamsController : Controller
{
    private readonly ExamOnlineDBContext _db;
    public ExamsController(ExamOnlineDBContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var studentId = HttpContext.Session.GetInt32("UserId");
        if (studentId is null)
            return RedirectToAction("Login", "Account", new { area = "" });

        var vm = new StudentExamListViewModel
        {
            Exams = await _db.Exams
                .Where(e => e.Participants.Any(p => p.StudentId == studentId.Value && p.IsEligible))
                .Include(e => e.Subject)
                .ToListAsync(),
            CompletedExamIds = await _db.UserExams
                .Where(u => u.StudentId == studentId.Value && u.Status == 2)
                .Select(u => u.ExamId).ToListAsync()
        };
        ViewBag.StudentId = studentId;
        return View(vm);
    }
}