using TscLoanManagement.TSCDB.Application.DTOs.PurchaseSourceDocuments;

namespace TscLoanManagement.TSCDB.Application.Interfaces
{
    public interface IPurchaseSourceDocumentService
    {
        Task<IEnumerable<PurchaseSourceDocumentDto>> GetByPurchaseSourceIdAsync(int sourceId);
        Task<IEnumerable<PurchaseSourceDocumentDto>> CreateManyAsync(PurchaseSourceDocumentCreateManyDto dto);
    }
}
