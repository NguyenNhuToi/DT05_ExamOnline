using System.ComponentModel.DataAnnotations;

namespace ExamOnlineDB.Models.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Nhập tên đăng nhập")]
    [StringLength(50, MinimumLength = 3)]
    [Display(Name = "Tên đăng nhập")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nhập họ tên")]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nhập email")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Mã SV / Mã GV")]
    public string? UserCode { get; set; }

    [Required(ErrorMessage = "Nhập mật khẩu")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu ≥ 6 ký tự")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Xác nhận mật khẩu")]
    [Compare(nameof(Password), ErrorMessage = "Mật khẩu không khớp")]
    [DataType(DataType.Password)]
    [Display(Name = "Xác nhận mật khẩu")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Bạn là")]
    public string RoleName { get; set; } = "Student";

    public bool IsFirstAdmin { get; set; }
}
