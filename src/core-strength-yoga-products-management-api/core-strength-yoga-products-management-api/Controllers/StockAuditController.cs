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
            "/StartDateTime={startDateTime}" +
            "/EndDateTime={endDateTime}" +
            "/ProductId={productId}" +
            "/ProductTypeId={productTypeId}" +
            "/Username={username}")]
        public async Task<ActionResult<IEnumerable<StockAudit>>> FilterReport(
            string startDateTime, string endDateTime, int productId, int productTypeId, string username)
        {
            var start = DateTime.Parse(startDateTime);
            var end = DateTime.Parse(endDateTime);
            var usern = "";
            var stockAudits = new List<StockAudit>();

            if (username != "unknown")
            {
                usern = username.Replace("%20", " ");
            }

            if (productId > 0)
            {
                stockAudits = _context.StockAudits
                    .Where(s => s.ProductId == productId).ToList();
            }
            else if (productId == 0 && productTypeId > 0)
            {
                var productsByTypeId = _context.Products.SelectOnType(productTypeId);
                var productIdsForProductTypeId = productsByTypeId.Select(p => p.Id).ToList();
                stockAudits = _context.StockAudits
                    .Where(s => productIdsForProductTypeId.Contains(s.ProductId)).ToList();
            }
            else
            {
                stockAudits = _context.StockAudits.ToList();
            }

            if (!string.IsNullOrEmpty(usern))
            {
                stockAudits = stockAudits
                    .Where(p => p.Username == usern && p.ChangedAt >= start && p.ChangedAt <= end).ToList();
            }
            else
            {
                stockAudits = stockAudits
                    .Where(p => p.ChangedAt >= start && p.ChangedAt <= end).ToList();

            }

            if (stockAudits == null) return NotFound();

            return stockAudits.ToList();
        }
    }
}
