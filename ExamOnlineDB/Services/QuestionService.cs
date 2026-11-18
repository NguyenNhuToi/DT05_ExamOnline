using ExamOnlineDB.Data;
using ExamOnlineDB.Models;
using Microsoft.EntityFrameworkCore;

namespace ExamOnlineDB.Services;

public class QuestionService : IQuestionService
{
    private readonly ExamOnlineDBContext _db;
    public QuestionService(ExamOnlineDBContext db) => _db = db;

    public Task<List<Question>> GetAllAsync()
        => _db.Questions.Include(q => q.Topic).Include(q => q.Options)
                        .Where(q => q.IsActive).ToListAsync();

    public Task<Question?> GetByIdAsync(int id)
        => _db.Questions.Include(q => q.Options)
                        .FirstOrDefaultAsync(q => q.QuestionId == id);

    public async Task<Question> CreateAsync(Question question, List<QuestionOption> options)
    {
        question.CreatedAt = DateTime.Now;
        _db.Questions.Add(question);
        await _db.SaveChangesAsync();
        foreach (var o in options) o.QuestionId = question.QuestionId;
        _db.QuestionOptions.AddRange(options);
        await _db.SaveChangesAsync();
        return question;
    }

    public async Task<bool> UpdateAsync(Question question, List<QuestionOption> options)
    {
        var existing = await _db.Questions.Include(q => q.Options)
                                           .FirstOrDefaultAsync(q => q.QuestionId == question.QuestionId);
        if (existing == null) return false;
        existing.Content = question.Content;
        existing.DifficultyLevel = question.DifficultyLevel;
        existing.TopicId = question.TopicId;
        existing.UpdatedAt = DateTime.Now;
        _db.QuestionOptions.RemoveRange(existing.Options);
        foreach (var o in options) o.QuestionId = question.QuestionId;
        _db.QuestionOptions.AddRange(options);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var q = await _db.Questions.FindAsync(id);
        if (q == null) return false;
        q.IsActive = false;
        await _db.SaveChangesAsync();
        return true;
    }
}