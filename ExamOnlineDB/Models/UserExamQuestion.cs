namespace ExamOnlineDB.Models;

public class UserExamQuestion
{
    public int UserExamQuestionId { get; set; }
    public int UserExamId { get; set; }
    public int QuestionId { get; set; }
    public int QuestionOrder { get; set; }
    public decimal Point { get; set; }
    public int? SelectedUserExamOptionId { get; set; }

    public UserExam? UserExam { get; set; }
    public Question? Question { get; set; }
    public ICollection<UserExamOption> Options { get; set; } = new List<UserExamOption>();
}