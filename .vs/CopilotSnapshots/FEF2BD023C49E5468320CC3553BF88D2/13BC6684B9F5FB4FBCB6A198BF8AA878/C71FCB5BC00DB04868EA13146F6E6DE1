using ExamOnlineDB.Data;
using ExamOnlineDB.Models;
using ExamOnlineDB.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExamOnlineDB.Areas.Teacher.Controllers;

[Area("Teacher")]
public class QuestionsController : Controller
{
    private readonly ExamOnlineDBContext _db;
    public QuestionsController(ExamOnlineDBContext db) => _db = db;

    // GET: /Teacher/Questions
    public async Task<IActionResult> Index()
    {
        // Lấy danh sách câu hỏi kèm theo Chủ đề và Đáp án
        var list = await _db.Questions
            .Include(q => q.Topic)
            .Include(q => q.Options)
            .Where(q => q.IsActive) // Chỉ lấy câu hỏi chưa bị xóa mềm
            .ToListAsync();

        var vm = new QuestionListViewModel
        {
            Questions = list,
            TotalCount = list.Count,
            EasyCount = list.Count(q => q.DifficultyLevel == 1),
            MediumCount = list.Count(q => q.DifficultyLevel == 2),
            HardCount = list.Count(q => q.DifficultyLevel == 3)
        };
        return View(vm);
    }

    // GET: /Teacher/Questions/Create
    public async Task<IActionResult> Create()
    {
        ViewBag.Topics = await _db.Topics.ToListAsync();
        return View();
    }

    // POST: /Teacher/Questions/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Content,DifficultyLevel,TopicId")] Question question, List<string> optionContent, int correctIndex)
    {
        // Lọc bỏ các ô đáp án người dùng để trống
        if (optionContent != null)
        {
            optionContent = optionContent.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
        }

        // Kiểm tra dữ liệu đầu vào
        if (string.IsNullOrWhiteSpace(question.Content) || optionContent == null || optionContent.Count < 2)
        {
            ModelState.AddModelError("", "Cần nhập nội dung câu hỏi và ít nhất 2 đáp án.");
            ViewBag.Topics = await _db.Topics.ToListAsync();
            return View(question);
        }

        // Gán thông tin mặc định
        question.CreatedBy = 2; // Tạm thời gán cứng ID Giảng viên (UserId = 2)
        question.CreatedAt = DateTime.Now;
        question.IsActive = true;

        _db.Questions.Add(question);
        await _db.SaveChangesAsync(); // Lưu để lấy được QuestionId

        // Lưu các đáp án
        for (int i = 0; i < optionContent.Count; i++)
        {
            _db.QuestionOptions.Add(new QuestionOption
            {
                QuestionId = question.QuestionId,
                Content = optionContent[i],
                IsCorrect = (i == correctIndex)
            });
        }
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: /Teacher/Questions/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var q = await _db.Questions
            .Include(x => x.Options)
            .FirstOrDefaultAsync(x => x.QuestionId == id);

        if (q == null || !q.IsActive) return NotFound();

        ViewBag.Topics = await _db.Topics.ToListAsync();
        return View(q);
    }

    // POST: /Teacher/Questions/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Content,DifficultyLevel,TopicId")] Question question, List<string> optionContent, int correctIndex)
    {
        var q = await _db.Questions
            .Include(x => x.Options)
            .FirstOrDefaultAsync(x => x.QuestionId == id);

        if (q == null || !q.IsActive) return NotFound();

        // Cập nhật thông tin câu hỏi
        q.Content = question.Content;
        q.DifficultyLevel = question.DifficultyLevel;
        q.TopicId = question.TopicId;
        q.UpdatedAt = DateTime.Now;

        // Xóa các đáp án cũ
        _db.QuestionOptions.RemoveRange(q.Options);

        // Thêm các đáp án mới
        if (optionContent != null)
        {
            optionContent = optionContent.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            for (int i = 0; i < optionContent.Count; i++)
            {
                _db.QuestionOptions.Add(new QuestionOption
                {
                    QuestionId = q.QuestionId,
                    Content = optionContent[i],
                    IsCorrect = (i == correctIndex)
                });
            }
        }

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // POST: /Teacher/Questions/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var q = await _db.Questions.FindAsync(id);
        if (q != null)
        {
            q.IsActive = false; // Xóa mềm (Soft Delete)
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}