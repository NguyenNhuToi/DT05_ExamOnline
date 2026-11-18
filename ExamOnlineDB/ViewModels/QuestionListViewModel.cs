using ExamOnlineDB.Models;

namespace ExamOnlineDB.ViewModels;

public class QuestionListViewModel
{
    public List<Question> Questions { get; set; } = new();
    public int TotalCount { get; set; }
    public int EasyCount { get; set; }
    public int MediumCount { get; set; }
    public int HardCount { get; set; }
}