using Dressed.Shared.DTOs;

namespace Design.Service.Services;

public interface IDesignService
{
    Task<DesignResponse?> CreateDesign(int designerId, CreateDesignRequest request);
    Task<DesignResponse?> GetDesign(int id);
    Task<List<DesignResponse>> GetDesignerDesigns(int designerId);
    Task<List<DesignResponse>> GetAllDesigns(string? category = null);
    Task<bool> UpdateDesignStatus(int id, string status);
    Task<bool> DeleteDesign(int id);
}
