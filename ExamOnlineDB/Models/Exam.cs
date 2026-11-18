namespace ExamOnlineDB.Models;

public class Exam
{
    public int ExamId { get; set; }
    public string ExamCode { get; set; } = string.Empty;
    public string ExamTitle { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public int MatrixId { get; set; }
    public int DurationMinutes { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? ExamPassword { get; set; }
    public bool ShuffleQuestions { get; set; } = true;
    public bool ShuffleOptions { get; set; } = true;
    public bool ShowResultImmediately { get; set; } = true;
    public byte Status { get; set; } = 0;
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Subject? Subject { get; set; }
    public ExamMatrix? Matrix { get; set; }
    public ICollection<ExamParticipant> Participants { get; set; } = new List<ExamParticipant>();
}