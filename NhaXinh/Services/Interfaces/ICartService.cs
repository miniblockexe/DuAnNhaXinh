using NhaXinh.Models;

namespace NhaXinh.Services.Interfaces
{
    public interface ICartService
    {
        List<CartItem> GetCart();
        void AddToCart(Product product, int quantity = 1);
        void UpdateQuantity(int productId, int quantity);
        void RemoveFromCart(int productId);
        void ClearCart();
        decimal GetTotalAmount();
        int GetTotalItems();
    }
}
