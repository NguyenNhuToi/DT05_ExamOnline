namespace ExamOnlineDB.Models;

public class Question
{
    public int QuestionId { get; set; }
    public int TopicId { get; set; }
    public string Content { get; set; } = string.Empty;
    public byte DifficultyLevel { get; set; }
    public int CreatedBy { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }

    public Topic? Topic { get; set; }
    public User? Creator { get; set; }
    public ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
}