using core_strength_yoga_products_api.Model;

namespace core_strength_yoga_products_api.Interfaces
{
    public interface IStockAuditService
    {
        Task<bool> SaveStockAudit(StockAudit stockAudit);
    }
}
