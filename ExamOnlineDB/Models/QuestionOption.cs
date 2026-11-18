namespace ExamOnlineDB.Models;

public class QuestionOption
{
    public int OptionId { get; set; }
    public int QuestionId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }

    public Question? Question { get; set; }
}