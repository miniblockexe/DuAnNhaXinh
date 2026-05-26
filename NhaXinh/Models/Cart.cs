namespace NhaXinh.Models
{
    public class Cart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();

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

        public decimal SubTotal => UnitPrice * Quantity;
    }
}