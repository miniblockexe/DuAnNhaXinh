namespace NhaXinh.Models
{
    public enum OrderStatus
    {
        Pending = 0,      
        Confirmed = 1,      // Đã xác nhận
        Shipping = 2,       // Đang giao hàng
        Delivered = 3,      // Đã giao thành công
        Cancelled = 4       // Đã huỷ
    }

    public enum PaymentMethod
    {
        COD = 0,            // Thanh toán khi nhận hàng
        BankTransfer = 1    // Chuyển khoản ngân hàng
    }

    public class Order
    {
        public int Id { get; set; }

        // NX + năm + số thứ tự — ví dụ: NX20240001
        public string OrderCode { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;     // FK → ApplicationUser
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverPhone { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string? Note { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.COD;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation
        public ApplicationUser User { get; set; } = null!;
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}