using System.ComponentModel.DataAnnotations;

namespace ExamOnlineDB.Models.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Nhập tên đăng nhập")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nhập mật khẩu"), DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
