namespace ExamOnlineDB.Models;

public class ExamParticipant
{
    public int ParticipantId { get; set; }
    public int ExamId { get; set; }
    public int StudentId { get; set; }
    public bool IsEligible { get; set; } = true;

    public Exam? Exam { get; set; }
    public User? Student { get; set; }
}