
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExamOnline.Models;
using ExamOnline.Data;

public class ChuDeController : Controller
{
    private readonly ApplicationDbContext _context;

    public ChuDeController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: CHUDES
    public async Task<IActionResult> Index()    
    {
        return View(await _context.ChuDes.ToListAsync());
    }

    // GET: CHUDES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var chude = await _context.ChuDes
            .FirstOrDefaultAsync(m => m.Id == id);
        if (chude == null)
        {
            return NotFound();
        }

        return View(chude);
    }

    // GET: CHUDES/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: CHUDES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,MonHocId,MonHoc,TenChuDe")] ChuDe chude)
    {
        if (ModelState.IsValid)
        {
            _context.Add(chude);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(chude);
    }

    // GET: CHUDES/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var chude = await _context.ChuDes.FindAsync(id);
        if (chude == null)
        {
            return NotFound();
        }
        return View(chude);
    }

    // POST: CHUDES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,MonHocId,MonHoc,TenChuDe")] ChuDe chude)
    {
        if (id != chude.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(chude);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChuDeExists(chude.Id))
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
        return View(chude);
    }

    // GET: CHUDES/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var chude = await _context.ChuDes
            .FirstOrDefaultAsync(m => m.Id == id);
        if (chude == null)
        {
            return NotFound();
        }

        return View(chude);
    }

    // POST: CHUDES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var chude = await _context.ChuDes.FindAsync(id);
        if (chude != null)
        {
            _context.ChuDes.Remove(chude);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ChuDeExists(int? id)
    {
        return _context.ChuDes.Any(e => e.Id == id);
    }
}
