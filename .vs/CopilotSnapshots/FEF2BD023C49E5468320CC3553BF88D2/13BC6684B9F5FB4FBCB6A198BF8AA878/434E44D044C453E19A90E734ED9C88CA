using ExamOnlineDB.Models;

namespace ExamOnlineDB.Services;

public interface IExamService
{
    // Lấy danh sách kỳ thi của sinh viên
    Task<List<Exam>> GetExamsForStudentAsync(int studentId);

    // Lấy thông tin chi tiết kỳ thi
    Task<Exam?> GetExamAsync(int examId);

    // Lấy hoặc tạo mới bài làm
    Task<UserExam?> GetOrCreateUserExamAsync(int examId, int studentId);

    // Lưu đáp án và xác thực quyền sở hữu bài làm
    Task<bool> SaveAnswerAsync(int userExamQuestionId, int selectedOptionId, int studentId);

    // Nộp bài và xác thực quyền sở hữu bài làm
    Task<bool> SubmitExamAsync(int userExamId, int studentId);
}