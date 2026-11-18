using ExamOnlineDB.Data;
using ExamOnlineDB.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExamOnlineDB.Areas.Teacher.Controllers;

[Area("Teacher")]
public class PreviewController : Controller
{
    private readonly ExamOnlineDBContext _db;
    public PreviewController(ExamOnlineDBContext db) => _db = db;

    public async Task<IActionResult> Index(int examId = 1)
    {
        var exam = await _db.Exams.FindAsync(examId);
        if (exam == null) return NotFound();

        var questions = await _db.Questions.Include(q => q.Options)
                                           .Where(q => q.IsActive)
                                           .Take(5).ToListAsync();
        var vm = new ExamPreviewViewModel
        {
            ExamTitle = exam.ExamTitle,
            DurationMinutes = exam.DurationMinutes,
            Questions = questions
        };
        return View(vm);
    }
}