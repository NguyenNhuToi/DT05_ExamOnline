using ExamOnlineDB.Data;
using ExamOnlineDB.Models;
using ExamOnlineDB.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExamOnlineDB.Areas.Admin.Controllers;

[Area("Admin")]
public class SubjectsController : Controller
{
    private readonly ExamOnlineDBContext _db;
    public SubjectsController(ExamOnlineDBContext db) => _db = db;

    // Kiểm tra quyền Admin
    private bool IsAdmin() => HttpContext.Session.GetString("RoleName") == "Admin";

    // ================== MÔN HỌC ==================
    public async Task<IActionResult> Index()
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account", new { area = "" });

        var subjects = await _db.Subjects.OrderBy(s => s.SubjectCode).ToListAsync();
        return View(new SubjectListViewModel { Subjects = subjects });
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account", new { area = "" });
        return View(new SubjectCreateEditViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SubjectCreateEditViewModel model)
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account", new { area = "" });

        if (ModelState.IsValid)
        {
            _db.Subjects.Add(new Subject
            {
                SubjectCode = model.SubjectCode,
                SubjectName = model.SubjectName,
                Description = model.Description,
                IsActive = model.IsActive
            });
            await _db.SaveChangesAsync();
            TempData["Success"] = "Thêm môn học thành công!";
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account", new { area = "" });

        var subject = await _db.Subjects.FindAsync(id);
        if (subject == null) return NotFound();

        return View(new SubjectCreateEditViewModel
        {
            SubjectId = subject.SubjectId,
            SubjectCode = subject.SubjectCode,
            SubjectName = subject.SubjectName,
            Description = subject.Description,
            IsActive = subject.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SubjectCreateEditViewModel model)
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account", new { area = "" });

        var subject = await _db.Subjects.FindAsync(id);
        if (subject == null) return NotFound();

        if (ModelState.IsValid)
        {
            subject.SubjectCode = model.SubjectCode;
            subject.SubjectName = model.SubjectName;
            subject.Description = model.Description;
            subject.IsActive = model.IsActive;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Cập nhật môn học thành công!";
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    // Xóa mềm (đổi trạng thái)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account", new { area = "" });

        var subject = await _db.Subjects.FindAsync(id);
        if (subject != null)
        {
            subject.IsActive = false;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Đã khóa môn học!";
        }
        return RedirectToAction(nameof(Index));
    }

    // ================== CHỦ ĐỀ (Nằm trong Môn học) ==================
    public async Task<IActionResult> Topics(int subjectId)
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account", new { area = "" });

        var subject = await _db.Subjects.FindAsync(subjectId);
        if (subject == null) return NotFound();

        var topics = await _db.Topics.Where(t => t.SubjectId == subjectId).OrderBy(t => t.OrderIndex).ToListAsync();
        return View(new TopicListViewModel
        {
            SubjectId = subjectId,
            SubjectName = subject.SubjectName,
            Topics = topics
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTopic(int subjectId, string topicName, int orderIndex)
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account", new { area = "" });

        if (!string.IsNullOrWhiteSpace(topicName))
        {
            _db.Topics.Add(new Topic
            {
                SubjectId = subjectId,
                TopicName = topicName,
                OrderIndex = orderIndex,
                IsActive = true
            });
            await _db.SaveChangesAsync();
            TempData["Success"] = "Thêm chủ đề thành công!";
        }
        return RedirectToAction(nameof(Topics), new { subjectId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteTopic(int id, int subjectId)
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account", new { area = "" });

        var topic = await _db.Topics.FindAsync(id);
        if (topic != null)
        {
            topic.IsActive = false;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Đã khóa chủ đề!";
        }
        return RedirectToAction(nameof(Topics), new { subjectId });
    }
}