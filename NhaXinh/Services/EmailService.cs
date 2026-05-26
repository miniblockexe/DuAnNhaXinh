using System.Net.Mail;
using System.Net;
using NhaXinh.Services.Interfaces;

namespace NhaXinh.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        private string SmtpHost => _configuration["EmailSettings:SmtpHost"] ?? "";
        private int SmtpPort => int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
        private string SmtpUser => _configuration["EmailSettings:Username"] ?? "";
        private string SmtpPassword => _configuration["EmailSettings:Password"] ?? "";
        private string SenderName => _configuration["EmailSettings:SenderName"] ?? "Nhà Xinh Shop";
        private string AdminEmail => _configuration["EmailSettings:AdminEmail"] ?? SmtpUser;

        public EmailService(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        private async Task<string> LoadTemplateAsync(string templateFileName)
        {
            var templatePath = Path.Combine(
                _env.ContentRootPath,
                "Services",
                "EmailTemplates",
                templateFileName
            );

            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"Không tìm thấy email template: {templateFileName}");

            return await File.ReadAllTextAsync(templatePath);
        }

        public async Task<(bool Success, string Message)> SendEmailAsync(
            string toEmail,
            string subject,
            string htmlBody)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                return (false, "Địa chỉ email người nhận không hợp lệ.");

            try
            {
                using var client = new SmtpClient(SmtpHost, SmtpPort)
                {
                    Credentials = new NetworkCredential(SmtpUser, SmtpPassword),
                    EnableSsl = true
                };

                using var message = new MailMessage
                {
                    From = new MailAddress(SmtpUser, SenderName),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

                message.To.Add(toEmail);

                await client.SendMailAsync(message);
                return (true, "Gửi email thành công.");
            }
            catch (SmtpException ex)
            {
                return (false, $"Lỗi SMTP: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi khi gửi email: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> SendOrderConfirmationAsync(
            string toEmail,
            string receiverName,
            string orderCode,
            decimal totalAmount)
        {
            try
            {
                var body = await LoadTemplateAsync("OrderConfirmation.html");

                body = body
                    .Replace("{{ReceiverName}}", receiverName)
                    .Replace("{{OrderCode}}", orderCode)
                    .Replace("{{TotalAmount}}", $"{totalAmount:N0} đ");

                var subject = $"[Nhà Xinh] Xác nhận đơn hàng #{orderCode}";
                return await SendEmailAsync(toEmail, subject, body);
            }
            catch (FileNotFoundException ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> SendNewOrderNotificationAsync(
            string orderCode,
            string customerName,
            decimal totalAmount)
        {
            try
            {
                var body = await LoadTemplateAsync("NewOrderNotification.html");

                body = body
                    .Replace("{{OrderCode}}", orderCode)
                    .Replace("{{CustomerName}}", customerName)
                    .Replace("{{TotalAmount}}", $"{totalAmount:N0} đ");

                var subject = $"[Nhà Xinh Admin] Đơn hàng mới #{orderCode}";
                return await SendEmailAsync(AdminEmail, subject, body);
            }
            catch (FileNotFoundException ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
