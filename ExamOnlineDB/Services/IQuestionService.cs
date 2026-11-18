using ExamOnlineDB.Models;

namespace ExamOnlineDB.Services;

public interface IQuestionService
{
    Task<List<Question>> GetAllAsync();
    Task<Question?> GetByIdAsync(int id);
    Task<Question> CreateAsync(Question question, List<QuestionOption> options);
    Task<bool> UpdateAsync(Question question, List<QuestionOption> options);
    Task<bool> SoftDeleteAsync(int id);
}