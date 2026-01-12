# Web App User Management

Ứng dụng quản lý người dùng sử dụng ASP.NET Core MVC với SQL Server và Swagger API.

## Yêu cầu hệ thống

- .NET 8.0 SDK hoặc cao hơn
- SQL Server Express hoặc SQL Server (LocalDB cũng được)
- Visual Studio 2022 hoặc VS Code (khuyến nghị)

## Hướng dẫn cài đặt và chạy ứng dụng

### Bước 1: Clone code về máy

```bash
git clone https://github.com/NguyenBaCuong532/Web_UserMamagement.git
cd Web_UserMamagement/Web_App_UserManagement
```

### Bước 2: Cấu hình Connection String

Mở file `appsettings.json` và cập nhật connection string phù hợp với SQL Server của bạn:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TÊN_SERVER\\TÊN_INSTANCE;Database=UserManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**Ví dụ các trường hợp phổ biến:**

1. **SQL Server Express trên máy local:**
   ```json
   "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=UserManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
   ```

2. **SQL Server LocalDB:**
   ```json
   "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=UserManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
   ```

3. **SQL Server với tên instance cụ thể:**
   ```json
   "DefaultConnection": "Server=UNI-CUONGNB-PC1\\SQLEXPRESS01;Database=UserManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
   ```

**Lưu ý:** 
- Thay `TÊN_SERVER\\TÊN_INSTANCE` bằng tên server và instance SQL Server của bạn
- Nếu không có instance, chỉ cần `Server=TÊN_SERVER;`
- `TrustServerCertificate=True` để bỏ qua kiểm tra SSL certificate (dùng cho development)

### Bước 3: Restore NuGet Packages

```bash
dotnet restore
```

### Bước 4: Build ứng dụng

```bash
dotnet build
```

### Bước 5: Chạy ứng dụng

```bash
dotnet run
```

Hoặc chạy từ Visual Studio:
- Nhấn `F5` hoặc click `Run`

### Bước 6: Tạo Database

**Có 3 cách để tạo database:**

#### Cách 1: Tự động (Khuyến nghị cho intern/fresher) ⭐

**Ứng dụng sẽ tự động tạo database khi chạy:**
- Không cần làm gì thêm
- Chỉ cần chạy `dotnet run`
- Database `UserManagementDb` và bảng `Users` sẽ được tạo tự động
- Code đã được cấu hình sẵn với `EnsureCreated()`

**Ưu điểm:**
- ✅ Đơn giản nhất, không cần kiến thức về SQL
- ✅ Tự động, không cần thao tác thủ công
- ✅ Phù hợp cho người mới học

#### Cách 2: Tạo bằng tay trong SSMS

Nếu muốn tạo database bằng tay trước:

1. Mở **SQL Server Management Studio (SSMS)**
2. Kết nối đến SQL Server instance của bạn
3. Right-click vào **Databases** → **New Database**
4. Đặt tên: `UserManagementDb`
5. Click **OK**
6. Chạy ứng dụng: `dotnet run`
7. Ứng dụng sẽ tự động tạo bảng `Users` trong database đã có

**Khi nào dùng cách này:**
- Khi muốn kiểm soát quyền truy cập database
- Khi cần cấu hình database trước (backup, security, etc.)

#### Cách 3: Dùng Migrations (Nâng cao)

**Lưu ý:** Hiện tại project đã xóa migrations để đơn giản hóa cho intern.

Nếu muốn dùng migrations (cho production hoặc team lớn):

1. Tạo migration:
   ```bash
   dotnet ef migrations add InitialCreate
   ```

2. Cập nhật `Program.cs`:
   ```csharp
   // Thay EnsureCreated() bằng:
   context.Database.Migrate();
   ```

3. Chạy ứng dụng:
   ```bash
   dotnet run
   ```

**Khi nào dùng migrations:**
- Khi làm việc trong team (có version control cho database changes)
- Khi cần rollback database về phiên bản cũ
- Khi deploy lên production (cần kiểm soát chặt chẽ)

**Kiểm tra database đã được tạo:**

1. Mở **SQL Server Management Studio (SSMS)**
2. Kết nối đến SQL Server instance của bạn
3. Trong **Object Explorer**, kiểm tra:
   - Database `UserManagementDb` đã xuất hiện
   - Bảng `Users` trong database đó

## Cấu trúc Database

### Bảng Users

| Cột | Kiểu dữ liệu | Mô tả |
|-----|--------------|-------|
| Id | int (Primary Key, Identity) | ID tự động tăng |
| Code | nvarchar(50) (Unique) | Mã người dùng (duy nhất) |
| FullName | nvarchar(200) | Họ và tên |
| DateOfBirth | date | Ngày sinh |
| Email | nvarchar(200) (Unique) | Email (duy nhất) |
| PhoneNumber | nvarchar(20) | Số điện thoại |
| Address | nvarchar(500) | Địa chỉ |

## Các tính năng

### MVC (Web Interface)
- Xem danh sách người dùng: `/User/Index`
- Tạo người dùng mới: `/User/Create`
- Chỉnh sửa người dùng: `/User/Edit/{code}`
- Xóa người dùng: `/User/Delete/{code}`

### API Endpoints (Swagger)
- `GET /api/User` - Lấy danh sách tất cả người dùng
- `GET /api/User/{code}` - Lấy thông tin người dùng theo mã
- `POST /api/User` - Tạo người dùng mới
- `PUT /api/User/{code}` - Cập nhật thông tin người dùng
- `DELETE /api/User/{code}` - Xóa người dùng

## Xử lý lỗi thường gặp

### Lỗi: "Cannot open database"

**Nguyên nhân:** Connection string không đúng hoặc SQL Server chưa chạy

**Giải pháp:**
1. Kiểm tra SQL Server đã được cài đặt và đang chạy
2. Kiểm tra lại connection string trong `appsettings.json`
3. Đảm bảo có quyền truy cập SQL Server

### Lỗi: "Login failed for user"

**Nguyên nhân:** Không có quyền truy cập SQL Server

**Giải pháp:**
1. Sử dụng Windows Authentication (Trusted_Connection=True)
2. Hoặc thêm User ID và Password vào connection string:
   ```json
   "DefaultConnection": "Server=...;Database=...;User Id=sa;Password=your_password;TrustServerCertificate=True;"
   ```

### Lỗi: "A network-related or instance-specific error"

**Nguyên nhân:** Không kết nối được đến SQL Server

**Giải pháp:**
1. Kiểm tra SQL Server đang chạy (Services → SQL Server)
2. Kiểm tra tên server và instance có đúng không
3. Thử kết nối bằng SSMS trước

## Troubleshooting

### Xóa và tạo lại database

Nếu muốn xóa database và tạo lại từ đầu:

1. Xóa database trong SSMS:
   - Right-click database `UserManagementDb` → Delete
2. Chạy lại ứng dụng:
   ```bash
   dotnet run
   ```
3. Database sẽ được tạo lại tự động

### Kiểm tra logs

Logs được lưu trong thư mục `logs/` (nếu có cấu hình stdout logging)

## Tài liệu tham khảo

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [Swagger/OpenAPI](https://swagger.io/)

## Liên hệ

Nếu có vấn đề, vui lòng tạo issue trên GitHub repository.
