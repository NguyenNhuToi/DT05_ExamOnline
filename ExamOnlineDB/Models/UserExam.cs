namespace ExamOnlineDB.Models;

public class UserExam
{
    public int UserExamId { get; set; }
    public int ExamId { get; set; }
    public int StudentId { get; set; }
    public string GeneratedExamCode { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; } = DateTime.Now;
    public DateTime? SubmittedAt { get; set; }
    public decimal? TotalScore { get; set; }
    public byte Status { get; set; } = 1;

    public Exam? Exam { get; set; }
    public User? Student { get; set; }
    public ICollection<UserExamQuestion> Questions { get; set; } = new List<UserExamQuestion>();
}