using core_strength_yoga_products_api.Controllers;
using core_strength_yoga_products_api.Data.Contexts;
using core_strength_yoga_products_api.Extensions;
using core_strength_yoga_products_api.Interfaces;
using core_strength_yoga_products_api.Model;
using core_strength_yoga_products_api.Models;

namespace core_strength_yoga_products_api.Data
{
    public class DataGenerator : IDataGenerator
    {
        private readonly CoreStrengthYogaProductsApiDbContext _context;
        private IEnumerable<Product> _products;
        private IEnumerable<Customer> _customers;
        private readonly ProductsController _productsController;
        private Random _random = new Random();

        public DataGenerator(CoreStrengthYogaProductsApiDbContext context,
            ProductsController productsController)
        {
            _context = context;
            _products = GetProducts();
            _customers = GetCustomers();
            _productsController = productsController;
        }

        public async Task Generate(int numberOfDays)
        {
            var productIds = _products.Select(p => p.Id).ToArray();

            for (int i = numberOfDays; i < 0; i++)
            {
                var dateTime = DateTime.Now.AddDays(i);
                await UpdateStockLevels(dateTime);

                var ordersToday = _random.Next(0, 10);

                for (int j = 0; j < ordersToday; j++)
                {
                    var productId = _random.Next(0, productIds.Length-1);
                    var product = _products.FirstOrDefault(p =>
                        p.Id == productIds[productId])!;

                    var productAttrIds = product.ProductAttributes.Select(p => 
                        p.Id).ToArray();

                    var productAttrId = _random.Next(0, productAttrIds.Length-1);
                 
                    var productAttribute = product.ProductAttributes
                        .FirstOrDefault(p => p.Id == productAttrIds[productAttrId])!;

                    var qty = _random.Next(1, productAttribute.StockLevel);

                    var totalCost = BasketItem.CalculateTotalItemCost(
                                    product.FullPrice,
                                    productAttribute.PriceAdjustment,
                                    qty);

                    var customer = _customers.FirstOrDefault(c => c.Id == 1)!;
                    var customerName = customer.CustomerDetail.FirstName + ' ' + 
                                        customer.CustomerDetail.Surname;

                    var order = new Order()
                    {
                        CustomerId = 1,
                        ShippingAddressId = 1,
                        DateOfSale = dateTime,
                        IsPaid = false,
                        OrderTotal = totalCost,
                        Items = new List<BasketItem>
                        {
                            new BasketItem
                            {
                                CustomerId = 1,
                                ProductId = product.Id,
                                ProductAttributeId = productAttribute.Id,
                                Quantity = qty,
                                TotalCost = totalCost,
                            }
                        }
                    };

                    await _context.Orders.AddAsync(order);
                    _context.SaveChanges();

                    var stockAudit = new StockAudit()
                    {
                        ChangedAt = dateTime,
                        ProductId = product.Id,
                        ProductAttributeId = productAttribute.Id,
                        Username = customerName,
                        OldStockLevel = productAttribute.StockLevel,
                        NewStockLevel = productAttribute.StockLevel - qty,
                        StockLevelChange = StockAudit.CalculateStockChange(
                            productAttribute.StockLevel - qty, productAttribute.StockLevel),
                        OrderId = order.Id,
                    };

                    _context.StockAudits.Add(stockAudit);
                    await _context.SaveChangesAsync();

                    productAttribute.StockLevel = productAttribute.StockLevel - qty;
                }
            }
        }

        private async Task UpdateStockLevels(DateTime dateTime)
        {
            foreach (var product in _products)
            {
                foreach (var attr in product.ProductAttributes)
                {
                    if (attr.StockLevel <= 50)
                    {
                        var stockAudit = new StockAudit()
                        {
                            ChangedAt = dateTime,
                            ProductId = product.Id,
                            ProductAttributeId = attr.Id,
                            Username = "ProductExecutive",
                            OldStockLevel = attr.StockLevel,
                            NewStockLevel = 100,
                            StockLevelChange = StockAudit.CalculateStockChange(100, attr.StockLevel),
                        };
                        _context.StockAudits.Add(stockAudit);
                        await _context.SaveChangesAsync();
                    }
                    attr.StockLevel = 100;
                }
                await _productsController.Put(product);
            }
        }

        private IEnumerable<Product> GetProducts()
        {
            return _context.Products
                .IncludeAllRelated()
                .ToList();
        }

        private IEnumerable<Customer> GetCustomers()
        {
            return _context.Customers
                .IncludeAllRelated()
                .ToList();
        }

    }

}
