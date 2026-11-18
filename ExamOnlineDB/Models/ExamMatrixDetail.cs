namespace ExamOnlineDB.Models;

public class ExamMatrixDetail
{
    public int DetailId { get; set; }
    public int MatrixId { get; set; }
    public int TopicId { get; set; }
    public byte DifficultyLevel { get; set; }
    public int QuestionCount { get; set; }
    public decimal ScorePerQuestion { get; set; }

    public ExamMatrix? Matrix { get; set; }
    public Topic? Topic { get; set; }
}