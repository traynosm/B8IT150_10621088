using core_strength_yoga_products_api.Data.Contexts;
using core_strength_yoga_products_api.Extensions;
using core_strength_yoga_products_api.Migrations;
using core_strength_yoga_products_api.Model;
using core_strength_yoga_products_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            "/EndDateTime={endDateTime}")]
        public async Task<ActionResult<IEnumerable<StockAudit>>> FilterReport(
            string username, 
            string startDateTime, string endDateTime)
        {
            var start = DateTime.Parse(startDateTime);
            var end = DateTime.Parse(endDateTime);
            var usern = username.Replace("%20", " ");
            
            var stockAudits = _context.StockAudits.
                Where(p => p.Username == usern && p.ChangedAt >= start && p.ChangedAt <= end).ToList();

            if (stockAudits == null) return NotFound();

            return stockAudits.ToList();
        }
        //public static List<Product> SelectOnProductType(this List<StockAudit> stockAudits, DbSet<StockAudit> set, int id)
        //{
        //    var ids = stockAudits
        //         .Where(p => p.ProductId == id)
        //         .Select(p => p.Id)
        //         .ToList();

        //    return id > 0 && stockAudits.Any() ? set
        //     .IncludeAllRelated()
        //     .Where(p => ids.Contains(p.Id))
        //         .ToList() :
        //     stockAudits;
        //}

        //[Microsoft.AspNetCore.Mvc.HttpGet("SearchByDatrange/{dateTime}")]


    }
}
