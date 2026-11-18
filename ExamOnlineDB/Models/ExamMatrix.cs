namespace ExamOnlineDB.Models;
using System.ComponentModel.DataAnnotations;

public class ExamMatrix
{
    public int MatrixId { get; set; }
    public string MatrixName { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public int TotalQuestions { get; set; }
    public int CreatedBy { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Subject? Subject { get; set; }
    public ICollection<ExamMatrixDetail> Details { get; set; } = new List<ExamMatrixDetail>();
}