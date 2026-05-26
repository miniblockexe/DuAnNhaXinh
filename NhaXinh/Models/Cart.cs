namespace NhaXinh.Models
{
    // ⚠️ KHÔNG có DbSet — Lưu trong Session dưới dạng JSON
    // Dùng SessionHelper để đọc/ghi object này vào Session

    public class Cart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        // Tính toán — không lưu, gọi mỗi lần cần
        public decimal TotalAmount => Items.Sum(i => i.SubTotal);
        public int TotalItems => Items.Sum(i => i.Quantity);
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImage { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }

        // Tính toán — không lưu
        public decimal SubTotal => UnitPrice * Quantity;
    }
}