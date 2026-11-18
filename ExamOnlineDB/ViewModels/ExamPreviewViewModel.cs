using ExamOnlineDB.Models;

namespace ExamOnlineDB.ViewModels;

public class ExamPreviewViewModel
{
    public string ExamTitle { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public List<Question> Questions { get; set; } = new();
}