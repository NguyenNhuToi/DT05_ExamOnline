using ExamOnlineDB.Models;

namespace ExamOnlineDB.Services;

public interface IExamService
{
    Task<List<Exam>> GetExamsForStudentAsync(int studentId);
    Task<Exam?> GetExamAsync(int examId);
    Task<UserExam?> GetOrCreateUserExamAsync(int examId, int studentId);
    Task<bool> SaveAnswerAsync(int userExamQuestionId, int selectedOptionId);
    Task<bool> SubmitExamAsync(int userExamId);
}