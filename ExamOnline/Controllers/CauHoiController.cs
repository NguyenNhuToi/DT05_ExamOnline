using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Thêm dòng này để dùng SelectList
using Microsoft.EntityFrameworkCore;
using ExamOnline.Models;
using ExamOnline.Data;
using Microsoft.AspNetCore.Authorization; // Thêm để phân quyền

namespace ExamOnline.Controllers
{
    [Authorize(Roles = "Admin,GiangVien")] // Chỉ Admin và Giảng viên mới được vào
    public class CauHoiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CauHoiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CAUHOIS
        public async Task<IActionResult> Index()
        {
            // Dùng Include để lấy kèm tên Chủ đề hiển thị ra bảng danh sách
            var applicationDbContext = _context.CauHois.Include(c => c.ChuDe);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: CAUHOIS/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var cauhoi = await _context.CauHois
                .Include(c => c.ChuDe)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cauhoi == null) return NotFound();

            return View(cauhoi);
        }

        // GET: CAUHOIS/Create
        public IActionResult Create()
        {
            // LẤY DANH SÁCH CHỦ ĐỀ ĐỔ VÀO DROPDOWN
            ViewData["ChuDeId"] = new SelectList(_context.ChuDes, "Id", "TenChuDe");
            return View();
        }

        // POST: CAUHOIS/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ChuDeId,NoiDung,DoKho")] CauHoi cauhoi) // Đã bỏ ChuDe ra khỏi Bind
        {
            if (ModelState.IsValid)
            {
                _context.Add(cauhoi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Nếu có lỗi, phải nạp lại Dropdown
            ViewData["ChuDeId"] = new SelectList(_context.ChuDes, "Id", "TenChuDe", cauhoi.ChuDeId);
            return View(cauhoi);
        }

        // GET: CAUHOIS/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var cauhoi = await _context.CauHois.FindAsync(id);
            if (cauhoi == null) return NotFound();

            ViewData["ChuDeId"] = new SelectList(_context.ChuDes, "Id", "TenChuDe", cauhoi.ChuDeId);
            return View(cauhoi);
        }

        // POST: CAUHOIS/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,ChuDeId,NoiDung,DoKho")] CauHoi cauhoi) // Đã bỏ ChuDe ra khỏi Bind
        {
            if (id != cauhoi.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cauhoi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CauHoiExists(cauhoi.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ChuDeId"] = new SelectList(_context.ChuDes, "Id", "TenChuDe", cauhoi.ChuDeId);
            return View(cauhoi);
        }

        // GET: CAUHOIS/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var cauhoi = await _context.CauHois
                .Include(c => c.ChuDe)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cauhoi == null) return NotFound();

            return View(cauhoi);
        }

        // POST: CAUHOIS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var cauhoi = await _context.CauHois.FindAsync(id);
            if (cauhoi != null)
            {
                _context.CauHois.Remove(cauhoi);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CauHoiExists(int? id)
        {
            return _context.CauHois.Any(e => e.Id == id);
        }
    }
}