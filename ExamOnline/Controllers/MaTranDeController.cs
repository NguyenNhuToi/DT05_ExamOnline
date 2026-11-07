using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ExamOnline.Models;
using ExamOnline.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Collections.Generic;

namespace ExamOnline.Controllers
{
    [Authorize(Roles = "Admin,GiangVien")]
    public class MaTranDeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MaTranDeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Lấy danh sách môn học để hiển thị Dropdown
            ViewBag.MonHocId = new SelectList(_context.MonHocs, "Id", "TenMon");

            var chuDes = _context.ChuDes.ToList();
            return View(chuDes);
        }

        // HÀM API: Lấy danh sách Chủ đề theo Môn học
        [HttpGet]
        public IActionResult GetChuDeByMonHoc(int monHocId)
        {
            var chuDes = _context.ChuDes
                .Where(c => c.MonHocId == monHocId)
                .Select(c => new { id = c.Id, ten = c.TenChuDe })
                .ToList();

            return Json(chuDes);
        }

        // HÀM API MỚI: Xử lý thuật toán sinh đề thi
        [HttpPost]
        public IActionResult SinhDeThi([FromBody] List<MaTranRequest> maTran)
        {
            try
            {
                var danhSachCauHoiDaChon = new List<CauHoi>();
                var random = new Random();

                foreach (var item in maTran)
                {
                    // 1. Lấy tất cả câu hỏi thỏa mãn Chủ đề và Độ khó
                    var cauHoiKhaDung = _context.CauHois
                        .Where(c => c.ChuDeId == item.ChuDeId && c.DoKho == item.DoKho)
                        .ToList();

                    // 2. Kiểm tra xem có đủ câu hỏi trong ngân hàng không
                    if (cauHoiKhaDung.Count < item.SoLuong)
                    {
                        return BadRequest($"Chủ đề ID {item.ChuDeId} (Độ khó {item.DoKho}) chỉ có {cauHoiKhaDung.Count} câu, không đủ {item.SoLuong} câu yêu cầu!");
                    }

                    // 3. Chọn ngẫu nhiên số lượng câu hỏi cần thiết
                    var cauHoiDuocChon = cauHoiKhaDung.OrderBy(x => random.Next()).Take(item.SoLuong).ToList();
                    danhSachCauHoiDaChon.AddRange(cauHoiDuocChon);
                }

                // 4. Xáo trộn lại toàn bộ đề thi một lần nữa
                var deThiHoanChinh = danhSachCauHoiDaChon.OrderBy(x => random.Next()).ToList();

                // 5. Trả về kết quả (Tạm thời trả về JSON để test)
                return Ok(new { success = true, data = deThiHoanChinh });
            }
            catch (Exception ex)
            {
                return BadRequest("Có lỗi xảy ra: " + ex.Message);
            }
        }
    }
}