using ExamOnlineDB.Models;
using System.ComponentModel.DataAnnotations;

namespace ExamOnlineDB.ViewModels;

public class ExamListViewModel
{
    public List<Exam> Exams { get; set; } = new();
}

public class CreateExamViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập tên kỳ thi")]
    public string ExamTitle { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn môn học")]
    public int SubjectId { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn ma trận")]
    public int MatrixId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập thời lượng")]
    [Range(1, 300, ErrorMessage = "Thời lượng phải từ 1 đến 300 phút")]
    public int DurationMinutes { get; set; } = 45;

    [Required(ErrorMessage = "Vui lòng chọn thời gian bắt đầu")]
    public DateTime StartTime { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "Vui lòng chọn thời gian kết thúc")]
    public DateTime EndTime { get; set; } = DateTime.Now.AddDays(1);

    public bool ShuffleQuestions { get; set; } = true;
    public bool ShuffleOptions { get; set; } = true;
}

public class ExamDetailViewModel
{
    public Exam Exam { get; set; } = null!;
    public List<ExamParticipant> Participants { get; set; } = new();
    public List<UserExam> Results { get; set; } = new();
}

public class ExamResultViewModel
{
    public string StudentCode { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal? TotalScore { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public string StatusText { get; set; } = string.Empty;
}

public class AssignStudentsViewModel
{
    public int ExamId { get; set; }
    public string ExamTitle { get; set; } = string.Empty;
    public List<StudentSelection> Students { get; set; } = new();
}

public class StudentSelection
{
    public int UserId { get; set; }
    public string UserCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsSelected { get; set; }
}