using ExamOnline.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExamOnline.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<MonHoc> MonHocs { get; set; }
        public DbSet<ChuDe> ChuDes { get; set; }
        public DbSet<CauHoi> CauHois { get; set; }
    }
}
