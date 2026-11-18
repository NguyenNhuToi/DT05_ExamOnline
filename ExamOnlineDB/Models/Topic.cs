namespace ExamOnlineDB.Models;

public class Topic
{
    public int TopicId { get; set; }
    public int SubjectId { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public int OrderIndex { get; set; } = 1;
    public bool IsActive { get; set; } = true;

    public Subject? Subject { get; set; }
    public ICollection<Question> Questions { get; set; } = new List<Question>();
}