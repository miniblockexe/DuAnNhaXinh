
namespace NhaXinh.Extensions
{
    public static class RouteConfig
    {
        public static WebApplication MapProjectRoutes(this WebApplication app)
        {
            app.MapControllerRoute("areas",
                "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

            app.MapControllerRoute("product-list",
                "san-pham",
                new { controller = "Product", action = "Index" });

            app.MapControllerRoute("product-detail",
                "san-pham/{slug}",
                new { controller = "Product", action = "Detail" });

            app.MapControllerRoute("news-list",
                "tin-tuc",
                new { controller = "News", action = "Index" });

            app.MapControllerRoute("news-detail",
                "tin-tuc/{slug}",
                new { controller = "News", action = "Detail" });

            app.MapControllerRoute("default",
                "{controller=Home}/{action=Index}/{id?}");

            return app;
        }
    }
}