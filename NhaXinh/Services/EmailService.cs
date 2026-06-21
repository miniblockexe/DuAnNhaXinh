using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using NhaXinh.Models;
using NhaXinh.Services.Interfaces;

namespace NhaXinh.Services
{
    public class EmailSettings
    {
        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 587;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string SenderName { get; set; } = "Nhà Xinh Shop";
        public string? AdminEmail { get; set; }
        public string? BaseUrl { get; set; }
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IOptions<EmailSettings> options,
            IWebHostEnvironment env,
            ILogger<EmailService> logger)
        {
            _settings = options.Value;
            _env = env;
            _logger = logger;
        }

        public async Task<(bool Success, string Message)> SendEmailAsync(
            string toEmail, string subject, string htmlBody)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                return (false, "Địa chỉ email người nhận không hợp lệ.");

            try
            {
                using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
                {
                    Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                    EnableSsl = true
                };

                using var message = new MailMessage
                {
                    From = new MailAddress(_settings.Username, _settings.SenderName),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

                message.To.Add(toEmail);
                await client.SendMailAsync(message);

                _logger.LogInformation("Gửi email thành công tới {ToEmail}", toEmail);
                return (true, "Gửi email thành công.");
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "Lỗi SMTP khi gửi tới {ToEmail}", toEmail);
                return (false, $"Lỗi SMTP: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi email tới {ToEmail}", toEmail);
                return (false, $"Lỗi khi gửi email: {ex.Message}");
            }
        }
        public async Task<(bool Success, string Message)> SendOrderConfirmationAsync(
            string toEmail,
            string receiverName,
            string orderCode,
            decimal totalAmount,
            string phone = "",
            string address = "",
            string paymentMethod = "")
        {
            try
            {
                var body = await LoadTemplateAsync("OrderConfirmation.html");

                var orderHistoryUrl = string.IsNullOrEmpty(_settings.BaseUrl)
                    ? "/Order/History"
                    : $"{_settings.BaseUrl.TrimEnd('/')}/Order/History";

                body = body
                    .Replace("{{ReceiverName}}", receiverName)
                    .Replace("{{OrderCode}}", orderCode)
                    .Replace("{{TotalAmount}}", $"{totalAmount:N0} ₫")
                    .Replace("{{Phone}}", phone)
                    .Replace("{{Address}}", address)
                    .Replace("{{PaymentMethod}}", paymentMethod)
                    .Replace("{{OrderHistoryUrl}}", orderHistoryUrl);

                return await SendEmailAsync(
                    toEmail,
                    $"[Nhà Xinh] Xác nhận đơn hàng #{orderCode}",
                    body);
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError(ex, "Không tìm thấy template OrderConfirmation.html");
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> SendNewOrderNotificationAsync(
            string orderCode,
            string customerName,
            decimal totalAmount,
            int orderId = 0)
        {
            var adminEmail = _settings.AdminEmail;
            if (string.IsNullOrWhiteSpace(adminEmail))
            {
                _logger.LogWarning("SendNewOrderNotification: AdminEmail chưa cấu hình.");
                return (false, "AdminEmail chưa được cấu hình.");
            }

            try
            {
                var body = await LoadTemplateAsync("NewOrderNotification.html");

                var path = orderId == 0
                   ? "/Admin/Order"
                   : $"/Admin/Order/Detail/{orderId}";

                var adminOrderUrl = string.IsNullOrEmpty(_settings.BaseUrl)
                    ? path
                    : $"{_settings.BaseUrl.TrimEnd('/')}{path}";

                body = body
                    .Replace("{{OrderCode}}", orderCode)
                    .Replace("{{CustomerName}}", customerName)
                    .Replace("{{TotalAmount}}", $"{totalAmount:N0} ₫")
                    .Replace("{{AdminOrderUrl}}", adminOrderUrl);

                return await SendEmailAsync(
                    adminEmail,
                    $"[Nhà Xinh Admin] Đơn hàng mới #{orderCode}",
                    body);
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError(ex, "Không tìm thấy template NewOrderNotification.html");
                return (false, ex.Message);
            }
        }


        private async Task<string> LoadTemplateAsync(string templateFileName)
        {
            var path = Path.Combine(
                _env.ContentRootPath, "Services", "EmailTemplates", templateFileName);

            if (!File.Exists(path))
                throw new FileNotFoundException(
                    $"Không tìm thấy email template: {templateFileName}");

            return await File.ReadAllTextAsync(path);
        }
    }
}