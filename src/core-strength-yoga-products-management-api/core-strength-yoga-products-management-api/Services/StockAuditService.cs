using core_strength_yoga_products_api.Data.Contexts;
using core_strength_yoga_products_api.Interfaces;
using core_strength_yoga_products_api.Model;

namespace core_strength_yoga_products_api.Services
{
    public class StockAuditService : IStockAuditService
    {
        private readonly ILogger<StockAuditService> _logger;
        private readonly CoreStrengthYogaProductsApiDbContext _context;

        public StockAuditService(ILogger<StockAuditService> logger,
            CoreStrengthYogaProductsApiDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<bool> SaveStockAudit(StockAudit stockAudit)
        {
            _context.StockAudits.Add(stockAudit);
            await _context.SaveChangesAsync();

            return true;
        }

    }
}
