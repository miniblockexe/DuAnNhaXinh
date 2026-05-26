using System.Text.Json;
using NhaXinh.Models;
using NhaXinh.Services.Interfaces;

namespace NhaXinh.Services
{
    public class CartService : ICartService
    {
        private const string CartSessionKey = "ShoppingCart";

        private readonly IHttpContextAccessor _httpContextAccessor;

        private ISession Session => _httpContextAccessor.HttpContext!.Session;

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public List<CartItem> GetCart()
        {
            var json = Session.GetString(CartSessionKey);

            if (string.IsNullOrEmpty(json))
                return new List<CartItem>();

            return JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            var json = JsonSerializer.Serialize(cart);
            Session.SetString(CartSessionKey, json);
        }

        public void AddToCart(Product product, int quantity = 1)
        {
            if (quantity <= 0) return;

            var cart = GetCart();
            var existing = cart.FirstOrDefault(x => x.ProductId == product.Id);

            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductImage = product.MainImageUrl,
                    UnitPrice = product.DiscountPrice ?? product.Price,
                    Quantity = quantity
                });
            }

            SaveCart(cart);
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.ProductId == productId);

            if (item == null) return;

            if (quantity <= 0)
                cart.Remove(item);
            else
                item.Quantity = quantity;

            SaveCart(cart);
        }

        public void RemoveFromCart(int productId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.ProductId == productId);

            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }
        }

        public void ClearCart()
        {
            Session.Remove(CartSessionKey);
        }

        public decimal GetTotalAmount()
        {
            return GetCart().Sum(x => x.SubTotal);
        }

        public int GetTotalItems()
        {
            return GetCart().Sum(x => x.Quantity);
        }
    }
}
