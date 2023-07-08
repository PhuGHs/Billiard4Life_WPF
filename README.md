# Billiard4Life_WPF
IS201.N23 - Phân tích thiết kế hệ thống thông tin - Nhóm 4

# Giới thiệu ứng dụng

Ứng dụng quản lý quán Billiard được xây dựng bằng công nghệ WPF C# và cơ sở dữ liệu SQL Server nhằm cung cấp một giải pháp hiện đại và tiện ích cho việc quản lý và vận hành một quán Billiard.

Với giao diện đẹp mắt và dễ sử dụng, ứng dụng cho phép quản lý các hoạt động của quán billiard một cách thuận tiện và chuyên nghiệp. Người dùng có thể dễ dàng thực hiện các công việc như đặt bàn, quản lý khách hàng, tính giờ sử dụng bàn, lập hóa đơn, theo dõi doanh thu...

- Giao diện trang chủ với quyền admin:

![image](https://github.com/PhuGHs/Billiard4Life_WPF/assets/96371073/9c866ed6-caf4-49e2-b5b4-940a34f26364)

- Giao diện trang chủ với quyền nhân viên:

![image](https://github.com/PhuGHs/Billiard4Life_WPF/assets/96371073/565ba1e0-4a35-475c-a723-42d54ecadac5)

- Giao diện quản lý tình trạng bàn:

![image](https://github.com/PhuGHs/Billiard4Life_WPF/assets/96371073/685390b0-c78b-42ed-8554-d353672f0bd4)

=> Hãy cài đặt ứng dụng để trải nghiệm một cách đầy đủ nhất

# Hướng dẫn sử dụng

1. Các cài đặt cần thiết
- Visual Studio
- SQL Server và SSMS

2. Tải source code về máy
- Cách 1: Clone từ https://github.com/PhuGHs/Billiard4Life_WPF/tree/main/Billiard4Life
- Cách 2: Tải file zip
![image](https://github.com/PhuGHs/Billiard4Life_WPF/assets/96371073/9c3b8897-16da-4609-aecf-1273fdbf69f2)

3. Tạo database
- Trong thư mục về giải nén (clone về) tìm đến DatabaseBilliard4Life.sql, nhấn chuột phải và chọn mở bằng Open with SSMS
- Cửa sổ SSMS xuất hiện -> Connect Server
- Bôi đen toàn bộ script sau đó nhấn F5 (execute) để tạo database
- Lấy tên server:

![image](https://github.com/PhuGHs/Billiard4Life_WPF/assets/96371073/2ad3e445-4b05-4971-b578-72cf26fcde19)

- Copy tên server của máy

![image](https://github.com/PhuGHs/Billiard4Life_WPF/assets/96371073/12147f87-e082-4a1f-8f8c-6d6abc336703)

- Quay lại thư mục lúc đầu, mở file .sln để mở Project, tìm đến file App.config trong Solution Explorer, thay dòng màu đỏ bằng tên server vừa copy:

![image](https://github.com/PhuGHs/Billiard4Life_WPF/assets/96371073/29143665-7562-41fe-a95f-d7c49f2dcee9)

- Chạy chương trình bằng cách nhấn Ctrl + F5
- Tài khoản mặc định: admin - admin
- Sau đó vào tab 'Nhân viên', thêm mới nhân viên cùng tài khoản để có thể đăng nhập bằng quyền nhân viên
