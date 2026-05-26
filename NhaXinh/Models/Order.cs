namespace NhaXinh.Models
{
    public enum OrderStatus
    {
        Pending = 0,      
        Confirmed = 1,    
        Shipping = 2,      
        Delivered = 3,     
        Cancelled = 4   
    }

    public enum PaymentMethod
    {
        COD = 0,           
        BankTransfer = 1   
    }

    public class Order
    {
        public int Id { get; set; }

        public string OrderCode { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;   
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