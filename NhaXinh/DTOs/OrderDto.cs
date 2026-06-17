using NhaXinh.Models;

namespace NhaXinh.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverPhone { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string? Note { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public string StatusLabel => Status switch
        {
            OrderStatus.Pending => "Chờ xác nhận",
            OrderStatus.Confirmed => "Đã xác nhận",
            OrderStatus.Shipping => "Đang giao hàng",
            OrderStatus.Delivered => "Đã giao",
            OrderStatus.Cancelled => "Đã huỷ",
            _ => "Không xác định"
        };
        public PaymentMethod PaymentMethod { get; set; }
        public string PaymentLabel => PaymentMethod == PaymentMethod.COD
            ? "Thanh toán khi nhận hàng"
            : "Chuyển khoản ngân hàng";
        public DateTime CreatedAt { get; set; }
        public List<OrderDetailDto> Details { get; set; } = new();
    }
    public class OrderDetailDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImage { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
    }
}
