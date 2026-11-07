using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamOnline.Models
{
    [Table("MonHoc")]
    public class MonHoc
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên môn học không được để trống")]
        [StringLength(100, ErrorMessage = "Tên môn học không quá 100 ký tự")]
        [Display(Name = "Tên Môn Học")]
        public string TenMon { get; set; }
    }
}