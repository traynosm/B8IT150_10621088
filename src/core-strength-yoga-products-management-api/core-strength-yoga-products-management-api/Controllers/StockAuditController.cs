using core_strength_yoga_products_api.Data.Contexts;
using core_strength_yoga_products_api.Model;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;

namespace core_strength_yoga_products_api.Controllers
{
    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("api/v1/[controller]")]
    

    public class StockAuditController : ControllerBase
    {
        private readonly ILogger<StockAuditController> _logger;
        private readonly CoreStrengthYogaProductsApiDbContext _context;

        public StockAuditController(ILogger<StockAuditController> logger, CoreStrengthYogaProductsApiDbContext context) 
        {
            _logger = logger;
            _context = context;
        }
        [HttpGet()]
        public ActionResult<IEnumerable<StockAudit>> Get()
        {
            var stockAudits = _context.StockAudits.ToList();

            if (stockAudits == null) return NotFound();

            return stockAudits;
        }
        [HttpGet("{productId}")]
        public ActionResult<IEnumerable<StockAudit>> Get(int productId)
        {
            var stockAudits = _context.StockAudits
                .Where(p => p.ProductId == productId).ToList();

            if (stockAudits == null) return NotFound();

            return stockAudits;
        }
    }
}
