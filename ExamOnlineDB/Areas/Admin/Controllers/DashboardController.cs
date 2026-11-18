using ExamOnlineDB.Data;
using ExamOnlineDB.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExamOnlineDB.Areas.Admin.Controllers;

[Area("Admin")]
public class DashboardController : Controller
{
    private readonly ExamOnlineDBContext _db;
    public DashboardController(ExamOnlineDBContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var vm = new DashboardViewModel
        {
            TotalUsers = await _db.Users.CountAsync(),
            TotalQuestions = await _db.Questions.CountAsync(q => q.IsActive),
            TotalExams = await _db.Exams.CountAsync(),
            TotalSubjects = await _db.Subjects.CountAsync()
        };
        return View(vm);
    }
}