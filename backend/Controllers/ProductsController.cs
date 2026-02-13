using backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        // Mock data
        private static readonly List<Product> MockProducts = new()
        {
            new Product { Id = 1, Name = "Laptop", Description = "High-performance laptop for developers", Price = 999.99m, Stock = 15 },
            new Product { Id = 2, Name = "Monitor", Description = "27-inch 4K UHD display", Price = 449.99m, Stock = 32 },
            new Product { Id = 3, Name = "Keyboard", Description = "Mechanical RGB keyboard", Price = 129.99m, Stock = 50 },
            new Product { Id = 4, Name = "Mouse", Description = "Wireless ergonomic mouse", Price = 79.99m, Stock = 45 },
            new Product { Id = 5, Name = "USB-C Hub", Description = "7-in-1 USB-C hub with multiple ports", Price = 49.99m, Stock = 28 }
        };

        /// <summary>
        /// Get all products
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetProducts()
        {
            return Ok(MockProducts);
        }

        /// <summary>
        /// Get a product by ID
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult<Product> GetProduct(int id)
        {
            var product = MockProducts.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
    }
}
