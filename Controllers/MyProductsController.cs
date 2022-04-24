#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIUppgift.Entities;
using WebAPIUppgift.Filters;
using WebAPIUppgift.Models;

namespace WebAPI_Examinationsuppgift.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [MyApiKey]
    public class MyProductsController : ControllerBase
    {
        private readonly DataContext _context;

        public MyProductsController(DataContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<ActionResult> PostProduct(MyCreateProductModel model)
        {

            Category? category = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryName == model.CategoryName);
            
            if (category == null)
            {
                category = new Category
                {
                    CategoryName = model.CategoryName,
                };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }


            Product product = new Product()
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                CategoryId = (await _context.Categories.FirstOrDefaultAsync(x => x.CategoryId == category.CategoryId)).CategoryId
            };

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            return new OkObjectResult(product);

        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadProductModel>>> GetAllProducts()
        {
            List<ReadProductModel> allProducts = new List<ReadProductModel>();

            foreach (Product product in await _context.Products.Include(x => x.Categories).ToListAsync())
            {
                allProducts.Add(new ReadProductModel()
                {
                    ProductId = product.ProductId,
                    CategoryName = product.Categories.CategoryName,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price

                });
            }

            return allProducts;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReadProductModel>> GetProduct(int id)

        {
            Product product = await _context.Products
                .Include(x => x.Categories)
                .FirstOrDefaultAsync(x => x.ProductId == id);


            if (product == null)
            {
                return NotFound();
            }

            return new ReadProductModel
            {
                ProductId = product.ProductId,
                CategoryName = product.Categories.CategoryName,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price

            };
            
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, MyCreateProductModel model)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            Category? category = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryName == model.CategoryName);

            if (category == null)
            {
                category = new Category
                {
                    CategoryName = model.CategoryName,
                };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.CategoryId = category.CategoryId;

            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return new OkObjectResult(product);

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.Include(x => x.Products).FirstOrDefaultAsync(x => x.CategoryId == product.CategoryId);

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            if (category.Products.Count == 0)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }


            return NoContent();

        }
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
