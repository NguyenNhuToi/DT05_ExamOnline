using Microsoft.AspNetCore.Http;

namespace ExamOnlineDB.ViewModels;

public class ImportQuestionViewModel
{
    public IFormFile? FileUpload { get; set; }
    public int TopicId { get; set; } 
}