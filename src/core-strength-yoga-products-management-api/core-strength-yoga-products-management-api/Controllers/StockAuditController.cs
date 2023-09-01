using core_strength_yoga_products_api.Data.Contexts;
using core_strength_yoga_products_api.Extensions;
using core_strength_yoga_products_api.Migrations;
using core_strength_yoga_products_api.Model;
using core_strength_yoga_products_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;
using System.Web.Http;

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
        [Microsoft.AspNetCore.Mvc.HttpGet("{productId}")]
        public async Task<ActionResult<IEnumerable<StockAudit>>> Get(int productId)
        {
            var stockAudits = _context.StockAudits
                .Where(p => p.ProductId == productId).ToList();

            if (stockAudits == null) return NotFound();

            return stockAudits;
        }

        //[Microsoft.AspNetCore.Mvc.HttpGet("SearchByProductType/{productTypeId}")]
        //public async Task<ActionResult<IEnumerable<StockAudit>>> SearchByTypeId(int productTypeId)
        //{
        //    var stockAudits = _context.StockAudits.
        //        Where(p => p. == productTypeId).ToList();

        //    if (stockAudits == null) return NotFound();

        //    return stockAudits.ToList();
        //}

        [Microsoft.AspNetCore.Mvc.HttpGet(
            "FilterReport" +
            "/Username={username}" +
            "/StartDateTime={startDateTime}" +
            "/EndDateTime={endDateTime}" +
            "/ProductTypeId={productTypeId}")]
        public async Task<ActionResult<IEnumerable<StockAudit>>> FilterReport(
            string startDateTime, string endDateTime, int productTypeId, string username)
        {
            var start = DateTime.Parse(startDateTime);
            var end = DateTime.Parse(endDateTime);
            var usern = "";
            var prodTypeId = 0;
            var productsByTypeId = _context.Products.SelectOnType(productTypeId);

            var productIds = productsByTypeId.Select(p => p.Id).ToList();

            var stockAuditsByProductTypeId = _context.StockAudits
                .Where(s => productIds.Contains(s.ProductId));


            if (username != "unknown")
            {
                usern = username.Replace("%20", " ");
            }
            if (productTypeId != 0 )
            {
                prodTypeId = productTypeId;
            }

            var stockAudits = new List<StockAudit>();
            if (string.IsNullOrEmpty(usern) && prodTypeId == 0) 
            {
                stockAudits = _context.StockAudits.
                    Where(p => p.ChangedAt >= start && p.ChangedAt <= end).ToList();
            }
            else if (string.IsNullOrEmpty(usern) && prodTypeId > 0)
            {
                stockAudits = stockAuditsByProductTypeId
                    .Where(p => p.ChangedAt >= start && p.ChangedAt <= end).ToList();
            }
            else if (!string.IsNullOrEmpty(usern) && productTypeId == 0) 
            {
                stockAudits = _context.StockAudits
                    .Where(p => p.Username == usern && p.ChangedAt >= start && p.ChangedAt <= end).ToList();
            }
            else if (!string.IsNullOrEmpty(usern) && prodTypeId > 0)
            {
                stockAudits = stockAuditsByProductTypeId
                    .Where(p => p.Username == usern && p.ChangedAt >= start && p.ChangedAt <= end).ToList();
            }
            if (stockAudits == null) return NotFound();

            return stockAudits.ToList();
        }
    }
}
