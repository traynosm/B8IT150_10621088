using core_strength_yoga_products_api.Data.Contexts;
using core_strength_yoga_products_api.Extensions;
using core_strength_yoga_products_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Web.Http;
using core_strength_yoga_products_api.Model;
using Microsoft.AspNetCore.Authorization;

namespace core_strength_yoga_products_api.Controllers
{
    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("api/v1/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly CoreStrengthYogaProductsApiDbContext _context;

        public ProductsController(ILogger<ProductsController> logger, CoreStrengthYogaProductsApiDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Microsoft.AspNetCore.Mvc.HttpGet("{id}")]
        public async Task<ActionResult<Product>> Get(int id)
        {
            var product = await _context.Products
                .IncludeAllRelated()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound(); 
            
            return product;
        }

        [Microsoft.AspNetCore.Mvc.HttpGet()]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            var products = await _context.Products
                .IncludeAllRelated()
                .ToListAsync();

            if (products == null) return NotFound();

            return products;
        }

        [Microsoft.AspNetCore.Mvc.HttpGet("ByCategory/{id}")]
        public ActionResult<IEnumerable<Product>> ByProductCategory(int id)
        {
            var products = _context.Products.SelectOnCategory(id);

            if (products == null) return NotFound();

            return products.ToList();
        }

        [Microsoft.AspNetCore.Mvc.HttpGet("ByType/{id}")]
        public ActionResult<IEnumerable<Product>> ByProductType(int id)
        {
            var products = _context.Products.SelectOnType(id);

            if (products == null) return NotFound();

            return products.ToList();
        }

        [Microsoft.AspNetCore.Mvc.HttpGet(
            "FilterOnAttribute/ProductCategory={categoryId}/ProductType={productTypeId}/Colour={colourId}/Size={sizeId}/Gender={genderId}")]
        public ActionResult<IEnumerable<Product>> FilterOnAttribute(
            [FromUri] int categoryId = 0, int productTypeId = 0, int colourId = 0, int sizeId = 0, int genderId = 0)
        {
            var products = _context.Products.SelectOnCategory(categoryId);


            if (products == null) return NotFound();

            products = products
                .SelectOnType(_context.Products, productTypeId)
                .SelectOnColourAttribute(_context.Products, colourId)
                .SelectOnSizeAttribute(_context.Products, sizeId)
                .SelectOnGenderAttribute(_context.Products, genderId);

            return products.Any() ? products.ToList() : new List<Product>();
        }

        [Microsoft.AspNetCore.Mvc.HttpGet("Search/{query}")]
        public ActionResult<IEnumerable<Product>> Search([FromUri] string query)
        {
            var products = _context.Products
                .IncludeAllRelated()
                .Where(p => 
                    p.Name.ToLower().Contains(query) ||
                    p.ProductCategory.ProductCategoryName.ToLower().Contains(query.ToLower()) ||
                    p.ProductType.ProductTypeName.ToLower().Contains(query.ToLower()));

            if (products == null) return NotFound();

            return products.Any() ? products.ToList() : new List<Product>();
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public async Task<ActionResult<Product>> Post(Product product)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'DbContext.Products' is null.");
            }

            if(_context.Products.Any(p => p.Name == product.Name))
            {
                return Problem($"Product with name='{product.Name}' already exists!");
            }

            if (product.ProductType.Id > 0)
            {
                _context.ProductTypes.Attach(product.ProductType);
            }

            if (product.ProductCategory.Id > 0)
            {
               _context.ProductCategories.Attach(product.ProductCategory);
            }

            if (product.Image.Id > 0)
            {
                _context.Images.Attach(product.Image);
            }
            //else
            //{
            //    Image image = new Image();
            //    image.ImageName = product.Image.ImageName;
            //    image.Alt = product.Image.Alt;
            //    image.Path = product.Image.Path;
            //}

            foreach (var productAttribute in product.ProductAttributes)
            {
                if(productAttribute.Id > 0)
                {
                    _context.ProductAttributes.Attach(productAttribute);
                }
                else
                {
                    _context.Add(productAttribute);
                }
            }

            _context.Products.Add(product);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(p => p.Id == product.Id))
                {
                    return NotFound();
                }
            }
            catch (Exception exception)
            {
                return StatusCode(500, exception.Message);
            }

            return RedirectToAction($"Get", new { product.Id });
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [Microsoft.AspNetCore.Mvc.HttpPut]
        public async Task<ActionResult<Product>> Put(Product productToUpdate)
        {
            if (productToUpdate.Id == 0)
            {
                return NotFound();
            }

            var savedProduct = _context.Products
                .IncludeAllRelated()
                .FirstOrDefault(p => p.Id == productToUpdate.Id);

            if (savedProduct != null)
            {
                RedirectToAction("Post", new { productToUpdate });
            }

            UpdateProductAttributes(productToUpdate, savedProduct!);
            productToUpdate = UpdateProductCategory(productToUpdate, savedProduct!);
            productToUpdate = UpdateProductType(productToUpdate, savedProduct!);
            productToUpdate = UpdateImage(productToUpdate, savedProduct!);

            _context.Entry(savedProduct!).CurrentValues.SetValues(productToUpdate);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(p => p.Id == productToUpdate.Id))
                {
                    return NotFound();
                }
            }
            catch (Exception exception)
            {
                return StatusCode(500, exception.Message);
            }

            return productToUpdate;
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [Microsoft.AspNetCore.Mvc.HttpGet("Delete/{productId}")]
        public async Task<IActionResult> Delete(int productid)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(productid);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private void UpdateProductAttributes(Product productToUpdate, Product savedProduct)
        {
            foreach (var productAttributeToUpdate in productToUpdate.ProductAttributes)
            {
                if(productAttributeToUpdate.ProductId == 0)
                {
                    productAttributeToUpdate.ProductId = savedProduct.Id;
                }
                
                var savedProductAttribute = savedProduct.ProductAttributes.SingleOrDefault(
                    pa => pa.Id == productAttributeToUpdate.Id);

                if (savedProductAttribute != null)
                {
                    _context.Entry(savedProductAttribute).CurrentValues.SetValues(productAttributeToUpdate);
                }
                else
                {
                    _context.ProductAttributes.Add(productAttributeToUpdate);
                }
            }
        }

        private Product UpdateProductCategory(Product productToUpdate, Product savedProduct)
        {
            if(productToUpdate.ProductCategory.Id == savedProduct.ProductCategory.Id)
            {
                productToUpdate.ProductCategory = savedProduct.ProductCategory;
                productToUpdate.ProductCategoryId = savedProduct.ProductCategoryId;

                return productToUpdate;
            }
            else
            {
                var updatedProductCategory = _context.ProductCategories.Find(
                    productToUpdate.ProductCategory.Id) ?? 
                    throw new NullReferenceException(
                        $"Could not find ProductCategory with Id=" +
                        $"{productToUpdate.ProductCategory.Id}");

                productToUpdate.ProductCategory = updatedProductCategory;
                productToUpdate.ProductCategoryId = updatedProductCategory.Id;
            }

             //_context.Entry(savedProduct.ProductCategory).CurrentValues
             //   .SetValues(productToUpdate.ProductCategory);
            
            return productToUpdate;
        }

        private Product UpdateProductType(Product productToUpdate, Product savedProduct)
        {
            if (productToUpdate.ProductType.Id == savedProduct.ProductType.Id)
            {
                productToUpdate.ProductType = savedProduct.ProductType;
                productToUpdate.ProductTypeId = savedProduct.ProductTypeId;

                return productToUpdate;
            }
            else
            {
                var updatedProductType = _context.ProductTypes.Find(
                    productToUpdate.ProductType.Id) ??
                    throw new NullReferenceException(
                        $"Could not find ProductType with Id=" +
                        $"{productToUpdate.ProductType.Id}");

                productToUpdate.ProductType = updatedProductType;
                productToUpdate.ProductTypeId = updatedProductType.Id;
            }

            //_context.Entry(savedProduct.ProductType).CurrentValues
            //   .SetValues(productToUpdate.ProductType);

            return productToUpdate;
        }

        private Product UpdateImage(Product productToUpdate, Product savedProduct)
        {
            if (productToUpdate.Image.Id == savedProduct.Image.Id)
            {
                productToUpdate.Image = savedProduct.Image;
                productToUpdate.ImageId = savedProduct.ImageId;

                return productToUpdate;
            }
            else
            {
                var updatedImage = _context.Images.Find(
                    productToUpdate.Image.Id) ??
                    throw new NullReferenceException(
                        $"Could not find Image with Id=" +
                        $"{productToUpdate.Image.Id}");

                productToUpdate.Image = updatedImage;
                productToUpdate.ImageId = updatedImage.Id;
            }

            _context.Entry(savedProduct.Image).CurrentValues
               .SetValues(productToUpdate.Image);

            return productToUpdate;
        }
    }
}