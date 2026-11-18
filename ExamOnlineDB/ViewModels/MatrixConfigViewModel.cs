namespace ExamOnlineDB.ViewModels;

public class MatrixConfigViewModel
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public List<MatrixRow> Rows { get; set; } = new();
    public int TotalQuestions { get; set; }
    public bool IsEnough { get; set; }
}

public class MatrixRow
{
    public int TopicId { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public byte DifficultyLevel { get; set; }
    public int RequiredCount { get; set; }
    public int AvailableCount { get; set; }
}