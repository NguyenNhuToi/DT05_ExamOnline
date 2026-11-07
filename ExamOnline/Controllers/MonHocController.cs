using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExamOnline.Models;
using ExamOnline.Data;
using Microsoft.AspNetCore.Authorization; // Thêm thư viện phân quyền

namespace ExamOnline.Controllers
{
    // Chỉ những ai có Role "Admin" mới được phép truy cập các chức năng trong này
    [Authorize(Roles = "Admin")]
    public class MonHocController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MonHocController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MONHOCS
        public async Task<IActionResult> Index()
        {
            return View(await _context.MonHocs.ToListAsync());
        }

        // GET: MONHOCS/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monhoc = await _context.MonHocs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (monhoc == null)
            {
                return NotFound();
            }

            return View(monhoc);
        }

        // GET: MONHOCS/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MONHOCS/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TenMon")] MonHoc monhoc)
        {
            if (ModelState.IsValid)
            {
                _context.Add(monhoc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(monhoc);
        }

        // GET: MONHOCS/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monhoc = await _context.MonHocs.FindAsync(id);
            if (monhoc == null)
            {
                return NotFound();
            }
            return View(monhoc);
        }

        // POST: MONHOCS/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,TenMon")] MonHoc monhoc)
        {
            if (id != monhoc.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(monhoc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MonHocExists(monhoc.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(monhoc);
        }

        // GET: MONHOCS/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monhoc = await _context.MonHocs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (monhoc == null)
            {
                return NotFound();
            }

            return View(monhoc);
        }

        // POST: MONHOCS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var monhoc = await _context.MonHocs.FindAsync(id);
            if (monhoc != null)
            {
                _context.MonHocs.Remove(monhoc);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MonHocExists(int? id)
        {
            return _context.MonHocs.Any(e => e.Id == id);
        }
    }
}