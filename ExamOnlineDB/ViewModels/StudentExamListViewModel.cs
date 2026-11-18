using ExamOnlineDB.Models;

namespace ExamOnlineDB.ViewModels;

public class StudentExamListViewModel
{
    public List<Exam> Exams { get; set; } = new();
    public List<int> CompletedExamIds { get; set; } = new();
}