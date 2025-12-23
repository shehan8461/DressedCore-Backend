using Dressed.Shared.DTOs;

namespace Quote.Service.Services;

public interface IQuoteService
{
    Task<QuoteResponse?> CreateQuote(int supplierId, CreateQuoteRequest request);
    Task<QuoteResponse?> GetQuote(int id);
    Task<List<QuoteResponse>> GetQuotesByDesign(int designId);
    Task<List<QuoteResponse>> GetQuotesBySupplier(int supplierId);
    Task<bool> UpdateQuoteStatus(int id, string status);
    Task<bool> DeleteQuote(int id);
}
