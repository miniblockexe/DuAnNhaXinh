# Nhà Xinh — Website Bán Hàng Nội Thất

> Môn **Lập trình Web 1** — Trường Đại học Yersin Đà Lạt, K21

**🌐 Demo trực tiếp:** [duannhaxinh.runasp.net](http://duannhaxinh.runasp.net/)

---

## Giới thiệu

Nhà Xinh là website thương mại điện tử cho cửa hàng nội thất, xây dựng bằng **ASP.NET Core MVC (.NET 8)**. Hệ thống phục vụ hai nhóm người dùng: khách hàng mua sắm trực tuyến và nhân viên/quản trị viên quản lý vận hành nội bộ.

Dự án được thực hiện từ đầu — phân tích yêu cầu, thiết kế CSDL, lập trình, kiểm thử đến triển khai — nhằm vận dụng thực tế mô hình MVC, Entity Framework Core và ASP.NET Core Identity.

---

## Tech Stack

| Thành phần | Công nghệ |
|---|---|
| Framework | ASP.NET Core MVC (.NET 8 LTS) |
| ORM | Entity Framework Core 8 (Code First, Migrations) |
| CSDL | Microsoft SQL Server |
| Xác thực / Phân quyền | ASP.NET Core Identity (cookie-based, role-based) |
| UI phía khách | Bootstrap 5, CSS tùy biến (tông kem–nâu gỗ–vàng đồng), JavaScript thuần |
| UI quản trị | SB Admin 2 (tùy biến cho `Areas/Admin`) |
| Gửi email | MailKit 4.16.0 (async SMTP) |
| Phân trang | X.PagedList.Mvc.Core 10.5.9 |
| Xử lý ảnh | SixLabors.ImageSharp 3.1.12 |
| IDE | Visual Studio / VS Code |
| Quản lý CSDL | SQL Server Management Studio (SSMS) |
| Version control | Git |

---

## Tính năng

### Khách hàng (Guest / Customer)

- Duyệt sản phẩm theo danh mục, xem chi tiết từng sản phẩm
- Tìm kiếm và lọc theo từ khóa, danh mục, khoảng giá
- Đọc tin tức và gợi ý nội thất
- Đăng ký tài khoản, đăng nhập
- Giỏ hàng: thêm sản phẩm, cập nhật số lượng, xóa
- Đặt hàng (Checkout) với hai phương thức thanh toán: **COD** hoặc **chuyển khoản ngân hàng**
- Xem lịch sử đơn hàng và chi tiết từng đơn
- Cập nhật hồ sơ cá nhân, đổi mật khẩu
- Nhận email xác nhận đơn hàng tự động (async, không chặn trang)

### Nhân viên (Staff)

- Xem danh sách và chi tiết đơn hàng
- Cập nhật trạng thái xử lý đơn hàng
- Xem Dashboard thống kê (tổng đơn hàng, doanh thu)

### Quản trị viên (Admin)

- Tất cả quyền của Staff
- CRUD: sản phẩm, danh mục, tin tức, banner
- Quản lý người dùng: khóa/mở khóa tài khoản, gán vai trò
- Nhận email thông báo mỗi khi có đơn hàng mới

---

## Kiến trúc

Hệ thống tổ chức theo kiến trúc 4 tầng:

```
Request → Controller → Service → Repository → EF Core → SQL Server
                ↓
             ViewModel → View → Response
```

Khu vực quản trị (`Areas/Admin/`) tách biệt hoàn toàn với phần khách hàng — có Controllers, Views và Layout riêng, được bảo vệ bằng `[Authorize(Roles = "Admin,Staff")]`.

---

## Cấu trúc thư mục

```
NhaXinh/
├── Areas/Admin/          # Controllers, Views quản trị
├── Controllers/          # Controllers phía người dùng
├── Data/                 # ApplicationDbContext, SeedData
├── DTOs/                 # Data Transfer Objects
├── Extensions/           # ServiceCollection extensions, RouteConfig
├── Helpers/              # SlugHelper, CurrencyHelper, ...
├── Migrations/           # Lịch sử Migrations EF Core
├── Models/               # Các lớp thực thể (entity)
├── Repositories/         # Tầng truy xuất dữ liệu + Interfaces/
├── Services/             # Tầng nghiệp vụ + Interfaces/
├── ViewModels/           # Dữ liệu truyền cho từng View
├── Views/                # Views phía người dùng
├── wwwroot/              # CSS, JS, ảnh tĩnh
├── appsettings.json      # Connection string, cấu hình app
└── Program.cs            # Khởi tạo, đăng ký DI, Middleware pipeline
```

---

## Cơ sở dữ liệu

Database `NhaXinhDB` gồm các bảng sau:

**Nghiệp vụ bán hàng:**
`Categories` · `Products` · `ProductImages` · `Orders` · `OrderDetails`

**Quản lý nội dung:**
`Banners` · `News`

**ASP.NET Core Identity (tự sinh):**
`AspNetUsers` · `AspNetRoles` · `AspNetUserRoles`

Quan hệ khóa ngoại và schema chi tiết xem trong thư mục `Migrations/` hoặc file ERD đính kèm báo cáo.

---

## Cài đặt và chạy

**Yêu cầu:**
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Microsoft SQL Server (hoặc SQL Server Express)

**Các bước:**

```bash
# 1. Clone repo
git clone https://github.com/miniblockexe/DuAnNhaXinh.git
cd nhaxinh

# 2. Cấu hình connection string
# Mở appsettings.json, cập nhật giá trị "DefaultConnection" cho đúng với SQL Server của bạn

# 3. Khôi phục gói NuGet
dotnet restore

# 4. Tạo CSDL (chọn một trong hai cách)
dotnet ef database update

# 5. Chạy ứng dụng
dotnet run
```

Sau khi khởi động, truy cập `https://localhost:5297`

**Tài khoản seed mẫu** (đã được khởi tạo sẵn để kiểm thử từng vai trò):

| Vai trò | Email | Mật khẩu |
|---|---|---|
| Admin | admin@nhaxinh.com | Admin@123 |
| Customer | customer@nhaxinh.com | User@123 |

---

## Gói NuGet chính

| Gói | Phiên bản | Mục đích |
|---|---|---|
| Microsoft.EntityFrameworkCore.SqlServer | 8.0.27 | Kết nối EF Core với SQL Server |
| Microsoft.EntityFrameworkCore.Tools | 8.0.27 | Migrations qua Package Manager Console |
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 8.0.27 | Lưu trữ dữ liệu Identity bằng EF Core |
| MailKit | 4.16.0 | Gửi email xác nhận đơn hàng qua SMTP |
| X.PagedList.Mvc.Core | 10.5.9 | Phân trang danh sách sản phẩm, đơn hàng |
| SixLabors.ImageSharp | 3.1.12 | Resize ảnh sản phẩm trước khi lưu |

---

## Thành viên thực hiện

| Họ tên | MSSV |
|---|---|
| Trần Tâm | 2401010567 |
| Lê Nguyễn Trọng Đạt | 2401010591 |

**Lớp:** Công nghệ thông tin – Công nghệ phần mềm K21  
**Giảng viên hướng dẫn:** Nguyễn Đức Tấn  
**Trường:** Đại học Yersin Đà Lạt  
**Môn học:** Lập trình Web 1 — Tháng 6/2026
