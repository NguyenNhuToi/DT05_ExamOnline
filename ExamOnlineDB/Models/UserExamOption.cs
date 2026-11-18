namespace ExamOnlineDB.Models;

public class UserExamOption
{
    public int UserExamOptionId { get; set; }
    public int UserExamQuestionId { get; set; }
    public int OriginalOptionId { get; set; }
    public char DisplayLabel { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsCorrectSnapshot { get; set; }

    public UserExamQuestion? UserExamQuestion { get; set; }
    public QuestionOption? OriginalOption { get; set; }
}