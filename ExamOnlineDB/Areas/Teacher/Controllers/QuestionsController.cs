using ExamOnlineDB.Data;
using ExamOnlineDB.Models;
using ExamOnlineDB.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.EF;

namespace ExamOnlineDB.Areas.Teacher.Controllers;

[Area("Teacher")]
public class QuestionsController : Controller
{
    private readonly ExamOnlineDBContext _db;
    public QuestionsController(ExamOnlineDBContext db) => _db = db;

    private bool IsTeacher() => HttpContext.Session.GetString("RoleName") == "Teacher";

    // GET: /Teacher/Questions?searchString=abc&page=1
    public async Task<IActionResult> Index(string searchString, int? page)
    {
        if (!IsTeacher())
            return RedirectToAction("Login", "Account", new { area = "" });

        // 1. Tạo truy vấn cơ bản (chỉ lấy câu hỏi chưa xóa)
        var query = _db.Questions
            .Include(q => q.Topic)
            .Include(q => q.Options)
            .Where(q => q.IsActive)
            .AsQueryable();

        // 2. Áp dụng Tìm kiếm nếu có
        if (!string.IsNullOrWhiteSpace(searchString))
        {
            query = query.Where(q => q.Content.Contains(searchString));
        }

        // 3. Tính toán các thống kê (dựa trên toàn bộ dữ liệu tìm kiếm được)
        var totalCount = await query.CountAsync();
        var easyCount = await query.CountAsync(q => q.DifficultyLevel == 1);
        var mediumCount = await query.CountAsync(q => q.DifficultyLevel == 2);
        var hardCount = await query.CountAsync(q => q.DifficultyLevel == 3);

        // 4. Phân trang (mặc định 10 câu/trang)
        int pageSize = 10;
        int pageNumber = page ?? 1;

        var pagedList = await query
            .OrderByDescending(q => q.QuestionId)
            .ToPagedListAsync(pageNumber, pageSize);

        // 5. Đổ dữ liệu vào ViewModel
        var vm = new QuestionListViewModel
        {
            Questions = pagedList.ToList(), // Chỉ hiển thị câu hỏi của trang hiện tại
            TotalCount = totalCount,
            EasyCount = easyCount,
            MediumCount = mediumCount,
            HardCount = hardCount
        };

        // Truyền dữ liệu phân trang và từ khóa qua ViewBag để View sử dụng
        ViewBag.SearchString = searchString;
        ViewBag.PagedList = pagedList;

        return View(vm);
    }

    // GET: /Teacher/Questions/Create
    public async Task<IActionResult> Create()
    {
        if (!IsTeacher())
            return RedirectToAction("Login", "Account", new { area = "" });
        ViewBag.Topics = await _db.Topics.ToListAsync();
        return View();
    }

    // POST: /Teacher/Questions/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Content,DifficultyLevel,TopicId")] Question question, List<string> optionContent, int correctIndex)
    {
        var filteredOptions = (optionContent ?? new List<string>())
            .Select((content, originalIndex) => new { content, originalIndex })
            .Where(x => !string.IsNullOrWhiteSpace(x.content))
            .ToList();

        if (!filteredOptions.Any(x => x.originalIndex == correctIndex))
            ModelState.AddModelError("", "Vui lòng chọn một đáp án hợp lệ làm đáp án đúng.");

        optionContent = filteredOptions.Select(x => x.content.Trim()).ToList();

        if (string.IsNullOrWhiteSpace(question.Content) || optionContent.Count < 2)
        {
            ModelState.AddModelError("", "Cần nhập nội dung câu hỏi và ít nhất 2 đáp án.");
            ViewBag.Topics = await _db.Topics.ToListAsync();
            return View(question);
        }

        var creatorId = HttpContext.Session.GetInt32("UserId");
        if (!IsTeacher() || creatorId is null)
            return RedirectToAction("Login", "Account", new { area = "" });

        question.CreatedBy = creatorId.Value;
        question.CreatedAt = DateTime.Now;
        question.IsActive = true;

        _db.Questions.Add(question);
        await _db.SaveChangesAsync();

        var filteredCorrectIndex = filteredOptions.FindIndex(x => x.originalIndex == correctIndex);
        for (int i = 0; i < optionContent.Count; i++)
        {
            _db.QuestionOptions.Add(new QuestionOption
            {
                QuestionId = question.QuestionId,
                Content = optionContent[i],
                IsCorrect = (i == filteredCorrectIndex)
            });
        }
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: /Teacher/Questions/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        if (!IsTeacher())
            return RedirectToAction("Login", "Account", new { area = "" });
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
        if (!IsTeacher())
            return RedirectToAction("Login", "Account", new { area = "" });
        var q = await _db.Questions
            .Include(x => x.Options)
            .FirstOrDefaultAsync(x => x.QuestionId == id);

        if (q == null || !q.IsActive) return NotFound();

        var filteredOptions = (optionContent ?? new List<string>())
            .Select((content, originalIndex) => new { content, originalIndex })
            .Where(x => !string.IsNullOrWhiteSpace(x.content))
            .Select(x => x.content.Trim())
            .ToList();

        if (string.IsNullOrWhiteSpace(question.Content)
            || filteredOptions.Count < 2
            || correctIndex < 0
            || correctIndex >= (optionContent?.Count ?? 0)
            || string.IsNullOrWhiteSpace(optionContent?[correctIndex]))
        {
            ModelState.AddModelError("", "Cần nội dung câu hỏi, ít nhất 2 đáp án và một đáp án đúng hợp lệ.");
            ViewBag.Topics = await _db.Topics.ToListAsync();
            return View(q);
        }

        q.Content = question.Content;
        q.DifficultyLevel = question.DifficultyLevel;
        q.TopicId = question.TopicId;
        q.UpdatedAt = DateTime.Now;

        _db.QuestionOptions.RemoveRange(q.Options);

        var filteredCorrectIndex = 0;
        if (optionContent != null)
        {
            var indexedOptions = optionContent
                .Select((content, originalIndex) => new { content, originalIndex })
                .Where(x => !string.IsNullOrWhiteSpace(x.content))
                .ToList();
            filteredCorrectIndex = indexedOptions.FindIndex(x => x.originalIndex == correctIndex);
            for (int i = 0; i < indexedOptions.Count; i++)
            {
                _db.QuestionOptions.Add(new QuestionOption
                {
                    QuestionId = q.QuestionId,
                    Content = indexedOptions[i].content.Trim(),
                    IsCorrect = (i == filteredCorrectIndex)
                });
            }
        }

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: /Teacher/Questions/Import
    public async Task<IActionResult> Import()
    {
        if (!IsTeacher()) return RedirectToAction("Login", "Account", new { area = "" });
        ViewBag.Topics = await _db.Topics.ToListAsync();
        return View(new ImportQuestionViewModel());
    }

    // POST: /Teacher/Questions/Import
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Import(ImportQuestionViewModel model)
    {
        if (!IsTeacher()) return RedirectToAction("Login", "Account", new { area = "" });
        ViewBag.Topics = await _db.Topics.ToListAsync();

        if (model.FileUpload == null || model.FileUpload.Length == 0)
        {
            ModelState.AddModelError("FileUpload", "Vui lòng chọn file CSV.");
            return View(model);
        }

        if (model.TopicId <= 0)
        {
            ModelState.AddModelError("TopicId", "Vui lòng chọn chủ đề.");
            return View(model);
        }

        var creatorId = HttpContext.Session.GetInt32("UserId");
        if (creatorId == null) return RedirectToAction("Login", "Account", new { area = "" });

        try
        {
            using var reader = new StreamReader(model.FileUpload.OpenReadStream(), System.Text.Encoding.UTF8);
            string? line;
            bool isFirstLine = true;
            int successCount = 0;

            // Đọc từng dòng trong file CSV
            while ((line = await reader.ReadLineAsync()) != null)
            {
                // Bỏ qua dòng tiêu đề (Header) đầu tiên
                if (isFirstLine)
                {
                    isFirstLine = false;
                    continue;
                }

                if (string.IsNullOrWhiteSpace(line)) continue;

                // Tách các cột bằng dấu phẩy (Giả định file CSV không chứa dấu phẩy trong nội dung)
                var parts = line.Split(',');

                // Cấu trúc file CSV yêu cầu: Nội dung câu hỏi, Mức độ (1,2,3), Đáp án A, Đáp án B, Đáp án C, Đáp án D, Đáp án đúng (A/B/C/D)
                if (parts.Length < 7) continue;

                var question = new Question
                {
                    TopicId = model.TopicId,
                    Content = parts[0].Trim(),
                    DifficultyLevel = byte.TryParse(parts[1].Trim(), out var diff) ? diff : (byte)1,
                    CreatedBy = creatorId.Value,
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                _db.Questions.Add(question);
                await _db.SaveChangesAsync();

                // Thêm đáp án
                var options = new List<QuestionOption>();
                string correctAnswerChar = parts[6].Trim().ToUpper(); // A, B, C hoặc D

                for (int i = 2; i <= 5; i++) // Cột 2,3,4,5 là đáp án A,B,C,D
                {
                    if (string.IsNullOrWhiteSpace(parts[i])) continue;

                    // Xác định đáp án đúng dựa trên chữ cái
                    char charIndex = (char)('A' + (i - 2));
                    bool isCorrect = (correctAnswerChar == charIndex.ToString());

                    _db.QuestionOptions.Add(new QuestionOption
                    {
                        QuestionId = question.QuestionId,
                        Content = parts[i].Trim(),
                        IsCorrect = isCorrect
                    });
                }
                await _db.SaveChangesAsync();
                successCount++;
            }

            TempData["Success"] = $"Đã nhập thành công {successCount} câu hỏi từ file CSV!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Lỗi khi đọc file: " + ex.Message);
            return View(model);
        }
    }

    // POST: /Teacher/Questions/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (!IsTeacher())
            return RedirectToAction("Login", "Account", new { area = "" });
        var q = await _db.Questions.FindAsync(id);
        if (q != null)
        {
            q.IsActive = false;
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}