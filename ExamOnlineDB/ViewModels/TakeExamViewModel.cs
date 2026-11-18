using ExamOnlineDB.Models;

namespace ExamOnlineDB.ViewModels;

public class TakeExamViewModel
{
    public UserExam UserExam { get; set; } = null!;
    public Exam Exam { get; set; } = null!;
    public DateTime EndTime { get; set; }
    public List<UserExamQuestion> Questions { get; set; } = new();
}