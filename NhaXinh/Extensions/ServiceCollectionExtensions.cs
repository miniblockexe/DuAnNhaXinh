using NhaXinh.Repositories.Interfaces;
using NhaXinh.Repositories;
using NhaXinh.Services.Interfaces;
using NhaXinh.Services;

namespace NhaXinh.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services)
        {
            services.AddScoped<IBannerRepository, BannerRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<INewsRepository, NewsRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductImageRepository, ProductImageRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IBannerService, BannerService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<INewsService, NewsService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IProductService, ProductService>();

            return services;
        }
    }
}
