namespace NhaXinh.Services.Interfaces
{
    public interface IEmailService
    {
        Task<(bool Success, string Message)> SendEmailAsync(
            string toEmail,
            string subject,
            string htmlBody);

        Task<(bool Success, string Message)> SendOrderConfirmationAsync(
            string toEmail,
            string receiverName,
            string orderCode,
            decimal totalAmount,
            string phone = "",
            string address = "",
            string paymentMethod = "");

        Task<(bool Success, string Message)> SendNewOrderNotificationAsync(
            string orderCode,
            string customerName,
            decimal totalAmount,
            int orderId = 0);
    }
}