using ExamOnlineDB.Data;
using ExamOnlineDB.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExamOnlineDB.Areas.Teacher.Controllers;

[Area("Teacher")]
public class MatrixController : Controller
{
    private readonly ExamOnlineDBContext _db;
    public MatrixController(ExamOnlineDBContext db) => _db = db;

    public async Task<IActionResult> Index(int subjectId = 1)
    {
        var subject = await _db.Subjects.FindAsync(subjectId);
        if (subject == null) return NotFound();

        var vm = new MatrixConfigViewModel { SubjectId = subjectId, SubjectName = subject.SubjectName };
        var topics = await _db.Topics.Where(t => t.SubjectId == subjectId && t.IsActive).ToListAsync();

        foreach (var t in topics)
        {
            for (byte d = 1; d <= 3; d++)
            {
                var available = await _db.Questions
                    .CountAsync(q => q.TopicId == t.TopicId && q.DifficultyLevel == d && q.IsActive);
                vm.Rows.Add(new MatrixRow
                {
                    TopicId = t.TopicId,
                    TopicName = t.TopicName,
                    DifficultyLevel = d,
                    RequiredCount = 1,
                    AvailableCount = available
                });
            }
        }
        vm.TotalQuestions = vm.Rows.Sum(r => r.RequiredCount);
        vm.IsEnough = vm.Rows.All(r => r.RequiredCount <= r.AvailableCount);
        return View(vm);
    }
}