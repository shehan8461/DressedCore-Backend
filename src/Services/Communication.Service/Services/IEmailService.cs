namespace Communication.Service.Services;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string toEmail, string toName, string subject, string body);
    Task<bool> SendDesignNotificationAsync(string supplierEmail, string supplierName, string designTitle, int designId);
    Task<bool> SendQuoteNotificationAsync(string designerEmail, string designerName, string designTitle, int quoteId);
    Task<bool> SendOrderConfirmationAsync(string email, string name, string orderNumber, decimal amount);
}
