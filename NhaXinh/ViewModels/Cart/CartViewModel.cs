using NhaXinh.Models;

namespace NhaXinh.ViewModels.Cart
{
    public class CartViewModel
    {
        public List<CartItem> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public int TotalItems { get; set; }
    }
}
