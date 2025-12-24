using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Communication.Service.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _senderEmail;
    private readonly string _senderName;
    private readonly bool _enableSsl;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
        _smtpServer = configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
        _smtpPort = int.Parse(configuration["EmailSettings:SmtpPort"] ?? "587");
        _senderEmail = configuration["EmailSettings:SenderEmail"] ?? "noreply@dressed.com";
        _senderName = configuration["EmailSettings:SenderName"] ?? "Dressed™ Platform";
        _enableSsl = bool.Parse(configuration["EmailSettings:EnableSsl"] ?? "true");
    }

    public async Task<bool> SendEmailAsync(string toEmail, string toName, string subject, string body)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_senderName, _senderEmail));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body
            };
            message.Body = bodyBuilder.ToMessageBody();

            // In development mode, just log the email instead of sending
            if (_configuration["ASPNETCORE_ENVIRONMENT"] == "Development")
            {
                Console.WriteLine($"[EMAIL] To: {toEmail}, Subject: {subject}");
                Console.WriteLine($"[EMAIL] Body: {body}");
                return true;
            }

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpServer, _smtpPort, _enableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
            
            var username = _configuration["EmailSettings:Username"];
            var password = _configuration["EmailSettings:Password"];
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                await client.AuthenticateAsync(username, password);
            }

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Email send error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> SendDesignNotificationAsync(string supplierEmail, string supplierName, string designTitle, int designId)
    {
        var subject = $"New Design Available: {designTitle}";
        var body = $@"
            <html>
            <body>
                <h2>Hello {supplierName},</h2>
                <p>A new design that matches your categories has been posted on Dressed™ Platform.</p>
                <h3>Design: {designTitle}</h3>
                <p>Log in to your account to view the design details and submit your quote.</p>
                <p><a href='http://localhost:3000/supplier/designs/{designId}'>View Design</a></p>
                <br/>
                <p>Best regards,<br/>Dressed™ Team</p>
            </body>
            </html>";

        return await SendEmailAsync(supplierEmail, supplierName, subject, body);
    }

    public async Task<bool> SendQuoteNotificationAsync(string designerEmail, string designerName, string designTitle, int quoteId)
    {
        var subject = $"New Quote Received for: {designTitle}";
        var body = $@"
            <html>
            <body>
                <h2>Hello {designerName},</h2>
                <p>You have received a new quote for your design: <strong>{designTitle}</strong></p>
                <p>Log in to your account to review the quote and make a decision.</p>
                <p><a href='http://localhost:3000/designer/designs/quotes'>View Quotes</a></p>
                <br/>
                <p>Best regards,<br/>Dressed™ Team</p>
            </body>
            </html>";

        return await SendEmailAsync(designerEmail, designerName, subject, body);
    }

    public async Task<bool> SendOrderConfirmationAsync(string email, string name, string orderNumber, decimal amount)
    {
        var subject = $"Order Confirmation: {orderNumber}";
        var body = $@"
            <html>
            <body>
                <h2>Hello {name},</h2>
                <p>Your order has been confirmed!</p>
                <h3>Order Details:</h3>
                <ul>
                    <li>Order Number: {orderNumber}</li>
                    <li>Total Amount: ${amount:N2}</li>
                </ul>
                <p>You can track your order status in your dashboard.</p>
                <p><a href='http://localhost:3000/dashboard'>View Dashboard</a></p>
                <br/>
                <p>Thank you for using Dressed™ Platform!</p>
                <p>Best regards,<br/>Dressed™ Team</p>
            </body>
            </html>";

        return await SendEmailAsync(email, name, subject, body);
    }
}
