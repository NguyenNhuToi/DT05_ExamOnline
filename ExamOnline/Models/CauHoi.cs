using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamOnline.Models
{
    [Table("CauHoi")]
    public class CauHoi
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Chủ Đề")]
        public int ChuDeId { get; set; }
        [ForeignKey("ChuDeId")]
        public virtual ChuDe? ChuDe { get; set; }

        [Required(ErrorMessage = "Nội dung không được để trống")]
        [Display(Name = "Nội dung câu hỏi")]
        public string NoiDung { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn độ khó")]
        [Display(Name = "Độ khó")]
        public int DoKho { get; set; }
    }
}