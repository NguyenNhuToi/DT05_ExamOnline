# Dự án DT05 - Hệ thống thi trắc nghiệm trực tuyến (ExamOnline)

Chào mừng thầy/cô và các thành viên nhóm đến với kho chứa code của dự án. 
Dưới đây là hướng dẫn chi tiết để cài đặt và chạy dự án trên máy cá nhân.

## 1. Yêu cầu môi trường (Bắt buộc phải cài)
- **Visual Studio 2026** (hoặc VS 2022 bản mới nhất) - Nhớ tích chọn workload `ASP.NET and web development`.
- **.NET 10.0 SDK**.
- **SQL Server LocalDB** (Thường đã được cài kèm khi cài Visual Studio).

## 2. Hướng dẫn cài đặt và chạy dự án (Làm theo đúng thứ tự)
1. Clone code từ GitHub về máy: `git clone <link_repo_cua_ban>`
2. Mở file `ExamOnline.sln` bằng Visual Studio.
3. Mở file `appsettings.json`, kiểm tra chuỗi kết nối (ConnectionStrings). Mặc định đang dùng LocalDB.
4. Mở **Tools** -> **NuGet Package Manager** -> **Package Manager Console**.
5. Gõ lệnh sau để tự động tạo Database (Tạo các bảng hệ thống, tài khoản, phân quyền):
   `Update-Database`
6. **Nạp dữ liệu mẫu (Bắt buộc):**
   - Mở SQL Server Management Studio (SSMS), kết nối vào `(localdb)\MSSQLLocalDB`.
   - Chọn đúng Database `ExamOnlineDB`.
   - Mở file `DatabaseScript.sql` (có trong thư mục dự án) và bấm **Execute (F5)** để tạo dữ liệu mẫu (Môn học, Chủ đề, Câu hỏi).
7. Bấm **F5** trong Visual Studio để chạy dự án.

## 3. Tài khoản mặc định (Dành cho Giảng viên/Admin)
Hệ thống đã cấu hình tự động tạo sẵn tài khoản Quản trị viên cao nhất khi khởi động lần đầu:
- **Email:** `admin@examonline.com`
- **Mật khẩu:** `Admin@123`

## 4. Chức năng đã hoàn thành (Cập nhật Tuần 3)
- **Phân quyền:** Hệ thống phân quyền 3 cấp (Admin, Giảng viên, Sinh viên).
- **Quản lý Danh mục:** Thêm/Sửa/Xóa Môn học, Chủ đề.
- **Quản lý Câu hỏi:** Thêm/Sửa/Xóa câu hỏi, phân loại theo Môn học, Chủ đề, Độ khó.
- **Thiết lập Ma trận đề:** Giao diện thiết lập số lượng câu hỏi, tự động load Chủ đề theo Môn học.
- **Giao diện:** Sử dụng Bootstrap 5, responsive, hiển thị tốt trên các thiết bị.

---
*Ghi chú: Dự án đang trong quá trình phát triển. Các chức năng Sinh đề và Thi trực tuyến sẽ được cập nhật trong các tuần tiếp theo.*