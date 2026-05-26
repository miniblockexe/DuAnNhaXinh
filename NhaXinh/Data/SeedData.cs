using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using NhaXinh.Models;

namespace NhaXinh.Data
{
    public class SeedData
    {
        public static async Task InitializeAsync(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await context.Database.MigrateAsync();

            await SeedRolesAsync(roleManager);
            await SeedUsersAsync(userManager);
            await SeedCategoriesAsync(context);
            await SeedProductsAsync(context);
            await SeedBannersAsync(context);
            await SeedNewsAsync(context);
            await SeedOrdersAsync(context, userManager);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Customer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
        {
            if (await userManager.FindByEmailAsync("admin@nhaxinh.vn") == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin@nhaxinh.vn",
                    Email = "admin@nhaxinh.vn",
                    FullName = "Quản trị viên",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };
                var result = await userManager.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }

            if (await userManager.FindByEmailAsync("user1@nhaxinh.vn") == null)
            {
                var user1 = new ApplicationUser
                {
                    UserName = "user1@nhaxinh.vn",
                    Email = "user1@nhaxinh.vn",
                    FullName = "Nguyễn Văn An",
                    PhoneNumber = "0901234567",
                    Address = "123 Nguyễn Huệ, Quận 1, TP.HCM",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };
                var result = await userManager.CreateAsync(user1, "User@123");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user1, "Customer");
            }

            if (await userManager.FindByEmailAsync("user2@nhaxinh.vn") == null)
            {
                var user2 = new ApplicationUser
                {
                    UserName = "user2@nhaxinh.vn",
                    Email = "user2@nhaxinh.vn",
                    FullName = "Trần Thị Bình",
                    PhoneNumber = "0912345678",
                    Address = "456 Lê Lợi, Quận 3, TP.HCM",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };
                var result = await userManager.CreateAsync(user2, "User@123");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user2, "Customer");
            }
        }

        private static async Task SeedCategoriesAsync(ApplicationDbContext context)
        {
            if (await context.Categories.AnyAsync()) return;

            var phongKhach = new Category { Name = "Phòng khách", Slug = "phong-khach", DisplayOrder = 1, IsActive = true };
            var phongNgu = new Category { Name = "Phòng ngủ", Slug = "phong-ngu", DisplayOrder = 2, IsActive = true };
            var phongAn = new Category { Name = "Phòng ăn", Slug = "phong-an", DisplayOrder = 3, IsActive = true };
            var phongLamViec = new Category { Name = "Phòng làm việc", Slug = "phong-lam-viec", DisplayOrder = 4, IsActive = true };

            await context.Categories.AddRangeAsync(phongKhach, phongNgu, phongAn, phongLamViec);
            await context.SaveChangesAsync();

            var subCategories = new List<Category>
            {
                new() { Name = "Sofa",          Slug = "sofa",           ParentId = phongKhach.Id,   DisplayOrder = 1, IsActive = true },
                new() { Name = "Kệ tivi",       Slug = "ke-tivi",        ParentId = phongKhach.Id,   DisplayOrder = 2, IsActive = true },
                new() { Name = "Bàn trà",       Slug = "ban-tra",        ParentId = phongKhach.Id,   DisplayOrder = 3, IsActive = true },
                new() { Name = "Giường ngủ",    Slug = "giuong-ngu",     ParentId = phongNgu.Id,     DisplayOrder = 1, IsActive = true },
                new() { Name = "Tủ quần áo",    Slug = "tu-quan-ao",     ParentId = phongNgu.Id,     DisplayOrder = 2, IsActive = true },
                new() { Name = "Bàn ăn",        Slug = "ban-an",         ParentId = phongAn.Id,      DisplayOrder = 1, IsActive = true },
                new() { Name = "Ghế ăn",        Slug = "ghe-an",         ParentId = phongAn.Id,      DisplayOrder = 2, IsActive = true },
                new() { Name = "Bàn làm việc",  Slug = "ban-lam-viec",   ParentId = phongLamViec.Id, DisplayOrder = 1, IsActive = true },
                new() { Name = "Ghế văn phòng", Slug = "ghe-van-phong",  ParentId = phongLamViec.Id, DisplayOrder = 2, IsActive = true },
            };

            await context.Categories.AddRangeAsync(subCategories);
            await context.SaveChangesAsync();
        }

        private static async Task SeedProductsAsync(ApplicationDbContext context)
        {
            if (await context.Products.AnyAsync()) return;

            var catIds = await context.Categories
                .Where(c => c.ParentId != null)
                .ToDictionaryAsync(c => c.Slug, c => c.Id);

            var products = new List<Product>
            {
                new() { Name = "Sofa góc chữ L Premium",   Slug = "sofa-goc-chu-l-premium",  ShortDescription = "Sofa góc bọc nỉ cao cấp, khung gỗ sồi", Price = 18_500_000, DiscountPrice = 16_200_000, StockQuantity = 5,  CategoryId = catIds["sofa"],          MainImageUrl = "/images/products/sofa-l.jpg",      IsFeatured = true,  IsActive = true },
                new() { Name = "Sofa đơn Bắc Âu",          Slug = "sofa-don-bac-au",          ShortDescription = "Ghế sofa đơn phong cách tối giản",        Price = 4_200_000,                             StockQuantity = 12, CategoryId = catIds["sofa"],          MainImageUrl = "/images/products/sofa-don.jpg",    IsFeatured = false, IsActive = true },
                new() { Name = "Sofa 3 chỗ Classic",       Slug = "sofa-3-cho-classic",       ShortDescription = "Sofa 3 chỗ ngồi, đệm dày 15cm",          Price = 9_800_000, DiscountPrice = 8_500_000,  StockQuantity = 8,  CategoryId = catIds["sofa"],          MainImageUrl = "/images/products/sofa-3cho.jpg",   IsFeatured = true,  IsActive = true },

                new() { Name = "Kệ tivi gỗ óc chó 1m8",   Slug = "ke-tivi-go-oc-cho-1m8",   ShortDescription = "Kệ tivi mặt gỗ óc chó tự nhiên",         Price = 7_500_000,                             StockQuantity = 6,  CategoryId = catIds["ke-tivi"],       MainImageUrl = "/images/products/ke-tivi-oc-cho.jpg", IsFeatured = true,  IsActive = true },
                new() { Name = "Kệ tivi treo tường",       Slug = "ke-tivi-treo-tuong",       ShortDescription = "Kệ treo tường tiết kiệm không gian",      Price = 3_200_000,                             StockQuantity = 0,  CategoryId = catIds["ke-tivi"],       MainImageUrl = "/images/products/ke-tivi-treo.jpg",   IsFeatured = false, IsActive = true },

                new() { Name = "Bàn trà mặt kính khung sắt", Slug = "ban-tra-mat-kinh",       ShortDescription = "Bàn trà hiện đại, chân sắt sơn tĩnh điện", Price = 2_800_000,                           StockQuantity = 15, CategoryId = catIds["ban-tra"],       MainImageUrl = "/images/products/ban-tra-kinh.jpg",   IsFeatured = false, IsActive = true },

                new() { Name = "Giường ngủ gỗ tự nhiên 1m6", Slug = "giuong-ngu-go-1m6",     ShortDescription = "Giường gỗ cao su tự nhiên, đầu giường bọc da", Price = 12_500_000, DiscountPrice = 11_000_000, StockQuantity = 4, CategoryId = catIds["giuong-ngu"], MainImageUrl = "/images/products/giuong-1m6.jpg", IsFeatured = true, IsActive = true },
                new() { Name = "Giường ngủ gỗ MDF 1m8",     Slug = "giuong-ngu-mdf-1m8",     ShortDescription = "Giường gỗ MDF phủ Melamine chống ẩm",     Price = 6_800_000,                             StockQuantity = 7,  CategoryId = catIds["giuong-ngu"],    MainImageUrl = "/images/products/giuong-1m8.jpg",     IsFeatured = false, IsActive = true },

                new() { Name = "Tủ quần áo 4 cánh gỗ sồi", Slug = "tu-quan-ao-4-canh",       ShortDescription = "Tủ 4 cánh có ngăn kéo và ô treo",        Price = 14_200_000, DiscountPrice = 12_800_000, StockQuantity = 3, CategoryId = catIds["tu-quan-ao"],   MainImageUrl = "/images/products/tu-4-canh.jpg",      IsFeatured = true,  IsActive = true },

                new() { Name = "Bộ bàn ăn 6 ghế gỗ sồi",  Slug = "bo-ban-an-6-ghe",         ShortDescription = "Bàn ăn mặt gỗ sồi + 6 ghế bọc nỉ",      Price = 22_000_000, DiscountPrice = 19_500_000, StockQuantity = 2, CategoryId = catIds["ban-an"],       MainImageUrl = "/images/products/ban-an-6-ghe.jpg",   IsFeatured = true,  IsActive = true },
                new() { Name = "Bàn ăn mặt đá 4 ghế",     Slug = "ban-an-mat-da-4-ghe",      ShortDescription = "Bàn mặt đá marble trắng, chân inox",     Price = 15_500_000,                            StockQuantity = 4,  CategoryId = catIds["ban-an"],        MainImageUrl = "/images/products/ban-an-da.jpg",      IsFeatured = false, IsActive = true },

                new() { Name = "Ghế ăn bọc nỉ xanh",      Slug = "ghe-an-boc-ni-xanh",       ShortDescription = "Ghế ăn chân gỗ, đệm bọc nỉ xanh navy",  Price = 1_200_000,                             StockQuantity = 20, CategoryId = catIds["ghe-an"],        MainImageUrl = "/images/products/ghe-an-ni.jpg",      IsFeatured = false, IsActive = true },

                new() { Name = "Bàn làm việc gỗ óc chó",  Slug = "ban-lam-viec-go-oc-cho",   ShortDescription = "Bàn làm việc mặt gỗ óc chó 1m4 × 70cm", Price = 8_500_000,                             StockQuantity = 6,  CategoryId = catIds["ban-lam-viec"],  MainImageUrl = "/images/products/ban-lam-viec.jpg",   IsFeatured = false, IsActive = true },
                new() { Name = "Bàn làm việc đứng gấp",   Slug = "ban-lam-viec-dung-gap",     ShortDescription = "Bàn đứng điều chỉnh độ cao, có ngăn kéo", Price = 5_200_000, DiscountPrice = 4_600_000, StockQuantity = 0, CategoryId = catIds["ban-lam-viec"],  MainImageUrl = "/images/products/ban-dung.jpg",       IsFeatured = false, IsActive = true },

                new() { Name = "Ghế văn phòng lưới ergonomic", Slug = "ghe-van-phong-luoi",   ShortDescription = "Ghế lưới thoáng khí, tựa đầu điều chỉnh", Price = 3_800_000, DiscountPrice = 3_200_000, StockQuantity = 10, CategoryId = catIds["ghe-van-phong"], MainImageUrl = "/images/products/ghe-luoi.jpg",      IsFeatured = true, IsActive = true },
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();

            var firstProducts = await context.Products.Take(4).ToListAsync();
            var images = new List<ProductImage>();
            foreach (var p in firstProducts)
            {
                images.Add(new ProductImage { ProductId = p.Id, ImageUrl = p.MainImageUrl?.Replace(".jpg", "-2.jpg") ?? "", DisplayOrder = 1 });
                images.Add(new ProductImage { ProductId = p.Id, ImageUrl = p.MainImageUrl?.Replace(".jpg", "-3.jpg") ?? "", DisplayOrder = 2 });
            }
            await context.ProductImages.AddRangeAsync(images);
            await context.SaveChangesAsync();
        }

        private static async Task SeedBannersAsync(ApplicationDbContext context)
        {
            if (await context.Banners.AnyAsync()) return;

            var banners = new List<Banner>
            {
                new() { Title = "Nội thất cao cấp — Phong cách sống tinh tế",   SubTitle = "Giảm đến 15% toàn bộ sản phẩm tháng này", ImageUrl = "/images/banners/banner-1.jpg", LinkUrl = "/san-pham", DisplayOrder = 1, IsActive = true },
                new() { Title = "Bộ sưu tập phòng ngủ mới nhất 2024",           SubTitle = "Khám phá không gian nghỉ ngơi hoàn hảo",   ImageUrl = "/images/banners/banner-2.jpg", LinkUrl = "/san-pham?category=phong-ngu", DisplayOrder = 2, IsActive = true },
                new() { Title = "Sofa góc — Điểm nhấn cho phòng khách của bạn", SubTitle = "Đa dạng mẫu mã, giao hàng toàn quốc",      ImageUrl = "/images/banners/banner-3.jpg", LinkUrl = "/san-pham?category=sofa",      DisplayOrder = 3, IsActive = true },
            };

            await context.Banners.AddRangeAsync(banners);
            await context.SaveChangesAsync();
        }

        private static async Task SeedNewsAsync(ApplicationDbContext context)
        {
            if (await context.News.AnyAsync()) return;

            var news = new List<News>
            {
                new() { Title = "5 gợi ý trang trí phòng khách hiện đại năm 2024",     Slug = "goi-y-trang-tri-phong-khach-2024",   Summary = "Khám phá xu hướng trang trí phòng khách mới nhất với những gam màu trung tính và vật liệu tự nhiên.", ThumbnailUrl = "/images/news/news-1.jpg", Author = "Nhà Xinh", IsFeatured = true,  IsPublished = true, PublishedAt = DateTime.Now.AddDays(-10), CreatedAt = DateTime.Now.AddDays(-10), Content = "<p>Nội dung bài viết...</p>" },
                new() { Title = "Xu hướng nội thất gỗ tự nhiên được ưa chuộng nhất",   Slug = "xu-huong-noi-that-go-tu-nhien",      Summary = "Gỗ tự nhiên đang trở lại mạnh mẽ trong thiết kế nội thất hiện đại với vẻ đẹp bền vững.",              ThumbnailUrl = "/images/news/news-2.jpg", Author = "Nhà Xinh", IsFeatured = true,  IsPublished = true, PublishedAt = DateTime.Now.AddDays(-7),  CreatedAt = DateTime.Now.AddDays(-7),  Content = "<p>Nội dung bài viết...</p>" },
                new() { Title = "Cách chọn sofa phù hợp với diện tích căn phòng",      Slug = "cach-chon-sofa-phu-hop-dien-tich",   Summary = "Hướng dẫn chi tiết cách đo đạc và chọn sofa vừa đẹp vừa phù hợp với không gian phòng khách.",        ThumbnailUrl = "/images/news/news-3.jpg", Author = "Nhà Xinh", IsFeatured = false, IsPublished = true, PublishedAt = DateTime.Now.AddDays(-5),  CreatedAt = DateTime.Now.AddDays(-5),  Content = "<p>Nội dung bài viết...</p>" },
                new() { Title = "Phòng ngủ tối giản — Bí quyết cho không gian nhỏ",   Slug = "phong-ngu-toi-gian-khong-gian-nho",  Summary = "Thiết kế phòng ngủ tối giản không chỉ tiết kiệm diện tích mà còn mang lại cảm giác thư thái.",        ThumbnailUrl = "/images/news/news-4.jpg", Author = "Nhà Xinh", IsFeatured = false, IsPublished = true, PublishedAt = DateTime.Now.AddDays(-3),  CreatedAt = DateTime.Now.AddDays(-3),  Content = "<p>Nội dung bài viết...</p>" },
                new() { Title = "Góc làm việc tại nhà: Đầu tư đúng để tăng hiệu suất", Slug = "goc-lam-viec-tai-nha-dau-tu-dung",  Summary = "Một góc làm việc được thiết kế tốt có thể tăng năng suất lên đến 30%. Cùng Nhà Xinh khám phá bí quyết.", ThumbnailUrl = "/images/news/news-5.jpg", Author = "Nhà Xinh", IsFeatured = false, IsPublished = true, PublishedAt = DateTime.Now.AddDays(-1),  CreatedAt = DateTime.Now.AddDays(-1),  Content = "<p>Nội dung bài viết...</p>" },
            };

            await context.News.AddRangeAsync(news);
            await context.SaveChangesAsync();
        }

        private static async Task SeedOrdersAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            if (await context.Orders.AnyAsync()) return;

            var user1 = await userManager.FindByEmailAsync("user1@nhaxinh.vn");
            var user2 = await userManager.FindByEmailAsync("user2@nhaxinh.vn");
            if (user1 == null || user2 == null) return;

            var products = await context.Products.Take(5).ToListAsync();
            if (products.Count < 3) return;

            var order1 = new Order
            {
                OrderCode = "NX20240001",
                UserId = user1.Id,
                ReceiverName = "Nguyễn Văn An",
                ReceiverPhone = "0901234567",
                ShippingAddress = "123 Nguyễn Huệ, Quận 1, TP.HCM",
                Note = "Giao giờ hành chính",
                Status = OrderStatus.Delivered,
                PaymentMethod = PaymentMethod.COD,
                CreatedAt = DateTime.Now.AddDays(-15),
                UpdatedAt = DateTime.Now.AddDays(-10),
                OrderDetails = new List<OrderDetail>
                {
                    new() { ProductId = products[0].Id, ProductName = products[0].Name, ProductImage = products[0].MainImageUrl, UnitPrice = products[0].DiscountPrice ?? products[0].Price, Quantity = 1, SubTotal = products[0].DiscountPrice ?? products[0].Price },
                    new() { ProductId = products[2].Id, ProductName = products[2].Name, ProductImage = products[2].MainImageUrl, UnitPrice = products[2].DiscountPrice ?? products[2].Price, Quantity = 2, SubTotal = (products[2].DiscountPrice ?? products[2].Price) * 2 },
                }
            };
            order1.TotalAmount = order1.OrderDetails.Sum(od => od.SubTotal);

            var order2 = new Order
            {
                OrderCode = "NX20240002",
                UserId = user1.Id,
                ReceiverName = "Nguyễn Văn An",
                ReceiverPhone = "0901234567",
                ShippingAddress = "123 Nguyễn Huệ, Quận 1, TP.HCM",
                Status = OrderStatus.Pending,
                PaymentMethod = PaymentMethod.BankTransfer,
                CreatedAt = DateTime.Now.AddDays(-2),
                UpdatedAt = DateTime.Now.AddDays(-2),
                OrderDetails = new List<OrderDetail>
                {
                    new() { ProductId = products[1].Id, ProductName = products[1].Name, ProductImage = products[1].MainImageUrl, UnitPrice = products[1].DiscountPrice ?? products[1].Price, Quantity = 1, SubTotal = products[1].DiscountPrice ?? products[1].Price },
                }
            };
            order2.TotalAmount = order2.OrderDetails.Sum(od => od.SubTotal);

            var order3 = new Order
            {
                OrderCode = "NX20240003",
                UserId = user2.Id,
                ReceiverName = "Trần Thị Bình",
                ReceiverPhone = "0912345678",
                ShippingAddress = "456 Lê Lợi, Quận 3, TP.HCM",
                Note = "Đổi ý không mua nữa",
                Status = OrderStatus.Cancelled,
                PaymentMethod = PaymentMethod.COD,
                CreatedAt = DateTime.Now.AddDays(-5),
                UpdatedAt = DateTime.Now.AddDays(-4),
                OrderDetails = new List<OrderDetail>
                {
                    new() { ProductId = products[3].Id, ProductName = products[3].Name, ProductImage = products[3].MainImageUrl, UnitPrice = products[3].DiscountPrice ?? products[3].Price, Quantity = 1, SubTotal = products[3].DiscountPrice ?? products[3].Price },
                    new() { ProductId = products[4].Id, ProductName = products[4].Name, ProductImage = products[4].MainImageUrl, UnitPrice = products[4].DiscountPrice ?? products[4].Price, Quantity = 1, SubTotal = products[4].DiscountPrice ?? products[4].Price },
                }
            };
            order3.TotalAmount = order3.OrderDetails.Sum(od => od.SubTotal);

            await context.Orders.AddRangeAsync(order1, order2, order3);
            await context.SaveChangesAsync();
        }
    }
}
