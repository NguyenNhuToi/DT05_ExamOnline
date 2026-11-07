using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamOnline.Models
{
    [Table("ChuDe")]
    public class ChuDe
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Môn Học")]
        public int MonHocId { get; set; }
        [ForeignKey("MonHocId")]
        public virtual MonHoc? MonHoc { get; set; }

        [Required(ErrorMessage = "Tên chủ đề không được để trống")]
        [Display(Name = "Tên Chủ Đề")]
        public string TenChuDe { get; set; }
    }
}