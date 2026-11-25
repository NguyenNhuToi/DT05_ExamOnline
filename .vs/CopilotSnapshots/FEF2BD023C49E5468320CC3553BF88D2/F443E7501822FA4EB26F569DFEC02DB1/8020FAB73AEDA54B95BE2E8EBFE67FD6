using ExamOnlineDB.Services;
using ExamOnlineDB.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ExamOnlineDB.Areas.Student.Controllers;

[Area("Student")]
public class TakeExamController : Controller
{
    private readonly IExamService _examService;
    public TakeExamController(IExamService examService) => _examService = examService;

    public async Task<IActionResult> Index(int examId = 1, int studentId = 3)
    {
        var ue = await _examService.GetOrCreateUserExamAsync(examId, studentId);
        if (ue == null) return NotFound();

        var exam = await _examService.GetExamAsync(examId);
        if (exam == null) return NotFound();

        var vm = new TakeExamViewModel
        {
            UserExam = ue,
            Exam = exam,
            EndTime = ue.StartedAt.AddMinutes(exam.DurationMinutes),
            Questions = ue.Questions.OrderBy(q => q.QuestionOrder).ToList()
        };
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> SaveAnswer(int userExamQuestionId, int selectedOptionId)
    {
        var ok = await _examService.SaveAnswerAsync(userExamQuestionId, selectedOptionId);
        return Json(new { success = ok });
    }

    [HttpPost]
    public async Task<IActionResult> Submit(int userExamId)
    {
        await _examService.SubmitExamAsync(userExamId);
        return Json(new
        {
            success = true,
            redirectUrl = Url.Action("Index", "Exams", new { area = "Student" })
        });
    }
}