# Swebi Coffee

**Dự án:** Trang quản lý cửa hàng cà phê - Đồ án môn Lập Trình Web

**Chủ sở hữu:** Phan Nguyễn Hoàng Nhân — 2280602184

**Năm:** 2025 — Trường Đại học Công nghệ Thành phố Hồ Chí Minh (HUTECH)

---

## Mô tả

Trang web quản lý cửa hàng (Swebi Coffee) được xây dựng để làm bài tập lớn và phục vụ đánh giá môn Lập Trình Web. Ứng dụng được xây dựng bằng **ASP.NET Core (.NET 8)** theo mô hình **MVC**, sử dụng **SQL Server** làm hệ quản trị cơ sở dữ liệu.

Hai phần chính của hệ thống:

1. **Trang khách hàng** — giao diện cho khách hàng xem sản phẩm, thêm vào giỏ hàng, thanh toán.
2. **Trang quản trị (Admin)** — quản lý sản phẩm, nhóm sản phẩm, hóa đơn, người dùng, xuất báo cáo.

---

## Công nghệ

* ASP.NET Core (.NET 8)
* ASP.NET Core MVC (Razor Views)
* Entity Framework Core (Code First)
* SQL Server
* HTML / CSS / JavaScript (Bootstrap)

---

## Chức năng chính

* Trang chủ, danh sách sản phẩm, trang chi tiết sản phẩm
* Giỏ hàng, Thanh toán
* Quản trị: CRUD sản phẩm, nhóm sản phẩm, quản lý hóa đơn, xuất Excel
* Phân quyền: Administrator / Staff / Customer (Identity hoặc custom)
* Tùy chọn: hiển thị size sản phẩm, tính giá theo size, chỉnh sửa chi tiết hóa đơn (Admin)

---

## Hình ảnh minh họa

* Trang chủ
  ![Trang chủ](https://i.postimg.cc/gjgd5KD4/Screenshot-2025-09-03-200638.png)

* Trang sản phẩm
  ![Trang sản phẩm](https://github.com/ngquy02/ASP.NET_CuaHangCafe/assets/85392867/a5386a74-56d2-484c-ad6b-95799873e89f)

* Trang chi tiết sản phẩm
  ![Chi tiết sản phẩm](https://github.com/ngquy02/ASP.NET_CuaHangCafe/assets/85392867/8acbc68e-40e8-4d31-bef3-0c22b469b7eb)

* Trang giỏ hàng
  ![Giỏ hàng](https://github.com/ngquy02/ASP.NET_CuaHangCafe/assets/85392867/af99f488-15df-4b48-95c2-cf1fcd26cd80)

* Trang thanh toán
  ![Thanh toán](https://github.com/ngquy02/ASP.NET_CuaHangCafe/assets/85392867/888390d5-4da7-4df3-a70e-57346b40dd26)

* Trang quản trị
  ![Admin](https://github.com/ngquy02/ASP.NET_CuaHangCafe/assets/85392867/b959ecf8-a95c-49f8-a113-733d889cec0d)

---

## Tài liệu tham khảo

* W3Schools: [https://www.w3schools.com](https://www.w3schools.com)
* Microsoft Docs (ASP.NET Core, EF Core, Identity): [https://learn.microsoft.com/vi-vn/docs](https://learn.microsoft.com/vi-vn/docs)

---

## Hướng dẫn nhanh đổi tên dự án thành "Swebi Coffee"

1. **Thay Title/Brand trong Views:**

   * Mở `Views/Shared/_Layout.cshtml` và đổi `<title>`, logo và tên thương hiệu thành `Swebi Coffee`.
2. **Thay text in README / About / Footer:**

   * Tìm và thay tất cả chuỗi `The Space Coffee` thành `Swebi Coffee` (IDE: tìm & thay toàn cục).
3. **Cập nhật tập tin cấu hình (nếu cần):**

   * `appsettings.json` (tên database nếu muốn đổi), `appsettings.Development.json`.
4. **Namespace / Project name (tùy chọn):**

   * Nếu muốn đổi tên project/namespace: đổi tên project file (`.csproj`) và cập nhật namespace trong mã nguồn (cẩn thận với EF Migrations).
5. **Cập nhật trang giới thiệu & thông tin tác giả:**

   * Chèn dòng: `Thiết kế và phát triển bởi: Phan Nguyễn Hoàng Nhân (2280602184)` vào About/README và footer.
6. **Logo & hình ảnh:**

   * Thay file logo trong `wwwroot/images` bằng logo Swebi Coffee (kích thước tối ưu 256×256 hoặc responsive SVG).
7. **Kiểm tra & chạy thử:**

   * Chạy `dotnet build` và `dotnet ef database update` (nếu có) và kiểm thử các chức năng chính.

---

## Ghi chú kỹ thuật

* Nếu đang sử dụng Identity: kiểm tra lớp `ApplicationUser` và `ApplicationDbContext`.
* Nếu đổi namespace/project name: chạy lại migration hoặc tạo migration mới để tránh lỗi EF.
* Backup database trước khi đổi tên hoặc chạy migration lớn.

---

## Liên hệ

Phan Nguyễn Hoàng Nhân — Sinh viên, Trường Đại học Công nghệ Thành phố Hồ Chí Minh (HUTECH)

---

