using ExamOnline.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExamOnline
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // 1. Thêm RoleManager và cấu hình Identity
            builder.Services.AddDefaultIdentity<IdentityUser>(options => {
                options.SignIn.RequireConfirmedAccount = false; // Đổi thành false để dễ test đăng nhập
            })
            .AddRoles<IdentityRole>() // Thêm dòng này để dùng được Phân quyền (Roles)
            .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // ==========================================================
            // 2. ĐOẠN CODE TỰ ĐỘNG TẠO ROLE VÀ TÀI KHOẢN ADMIN KHI KHỞI ĐỘNG
            // ==========================================================
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    // Tạo các Role nếu chưa có
                    if (!roleManager.RoleExistsAsync("Admin").Result)
                    {
                        roleManager.CreateAsync(new IdentityRole("Admin")).Wait();
                        roleManager.CreateAsync(new IdentityRole("GiangVien")).Wait();
                        roleManager.CreateAsync(new IdentityRole("SinhVien")).Wait();
                    }

                    // Tạo tài khoản Admin mặc định
                    var adminEmail = "admin@examonline.com";
                    var adminUser = userManager.FindByEmailAsync(adminEmail).Result;
                    if (adminUser == null)
                    {
                        var newAdmin = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                        var createAdminResult = userManager.CreateAsync(newAdmin, "Admin@123").Result;
                        if (createAdminResult.Succeeded)
                        {
                            userManager.AddToRoleAsync(newAdmin, "Admin").Wait();
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Ghi log lỗi nếu có (tùy chọn)
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Lỗi khi khởi tạo dữ liệu Role/Admin.");
                }
            }
            // ==========================================================

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            // 3. Bắt buộc phải có Authentication trước Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }
    }
}