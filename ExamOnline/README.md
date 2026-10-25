# Dự án DT05 - Hệ thống thi trắc nghiệm trực tuyến (ExamOnline)

Chào mừng thầy/cô và các thành viên nhóm đến với kho chứa code của dự án. 
Dưới đây là hướng dẫn chi tiết để chạy dự án trên máy cá nhân.

## 1. Yêu cầu môi trường (Bắt buộc phải cài)
- **Visual Studio 2026** (hoặc VS 2022 bản mới nhất) - Nhớ tích chọn workload `ASP.NET and web development`.
- **.NET 10.0 SDK**.
- **SQL Server LocalDB** (Thường đã được cài kèm khi cài Visual Studio).

## 2. Hướng dẫn cài đặt và chạy dự án (Làm theo đúng thứ tự)
1. Clone code từ GitHub về máy: `git clone https://github.com/TenTaiKhoanCuaBan/TenRepo.git`
2. Mở file `ExamOnline.sln` bằng Visual Studio.
3. Mở file `appsettings.json`, kiểm tra chuỗi kết nối (ConnectionStrings). Mặc định đang dùng LocalDB.
4. Mở **Tools** -> **NuGet Package Manager** -> **Package Manager Console**.
5. Gõ lệnh sau để tự động tạo Database (Lệnh này sẽ tạo các bảng hệ thống như tài khoản, phân quyền):
   `Update-Database`
6. **Nạp dữ liệu mẫu (Bắt buộc):**
   - Mở SQL Server Management Studio (SSMS), kết nối vào `(localdb)\MSSQLLocalDB`.
   - Chọn đúng Database `ExamOnlineDB`.
   - Mở file `DatabaseScript.sql` (có trong thư mục dự án) và bấm **Execute (F5)** để tạo dữ liệu mẫu (Môn học, Câu hỏi).
7. Bấm **F5** trong Visual Studio để chạy dự án trên trình duyệt.

## 3. Quy tắc làm việc nhóm (Git)
- Nhánh `main`: Chứa code ổn định nhất, chỉ merge khi đã test kỹ.
- Nhánh `develop`: Nhánh tích hợp code của cả nhóm.
- Nhánh `feature/...`: Mỗi thành viên tạo nhánh riêng để code chức năng của mình, sau đó tạo Pull Request để gộp vào `develop`.

*Chúc thầy và cả nhóm làm việc hiệu quả!*