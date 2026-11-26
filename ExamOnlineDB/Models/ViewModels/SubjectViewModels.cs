using System.ComponentModel.DataAnnotations;
using ExamOnlineDB.Models;

namespace ExamOnlineDB.ViewModels;

// --- MÔN HỌC ---
public class SubjectListViewModel
{
    public List<Subject> Subjects { get; set; } = new();
}

public class SubjectCreateEditViewModel
{
    public int SubjectId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mã môn học")]
    public string SubjectCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập tên môn học")]
    public string SubjectName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}

// --- CHỦ ĐỀ ---
public class TopicListViewModel
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public List<Topic> Topics { get; set; } = new();
}

public class TopicCreateEditViewModel
{
    public int TopicId { get; set; }
    public int SubjectId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên chủ đề")]
    public string TopicName { get; set; } = string.Empty;

    public int OrderIndex { get; set; } = 1;
    public bool IsActive { get; set; } = true;
}