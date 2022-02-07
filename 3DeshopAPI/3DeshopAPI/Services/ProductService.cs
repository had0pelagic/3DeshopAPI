using _3DeshopAPI.Exceptions;
using _3DeshopAPI.Services.Interfaces;
using Domain.Product;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace _3DeshopAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly ILogger<UserService> _logger;
        private readonly Context _context;

        public ProductService(ILogger<UserService> logger, Context context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<List<Product>> GetAllProducts()
        {
            var products = await _context.Products
                .Include(i => i.About)
                .Include(i => i.User)
                .Include(i => i.Specifications)
                .AsNoTracking()
                .ToListAsync();

            return products;
        }

        public async Task<Product> GetProduct(Guid id)
        {
            var product = await _context.Products
                .Include(i => i.About)
                .Include(i => i.User)
                .Include(i => i.Specifications)
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id);

            if (product == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.ProductNotFound);
            }

            return product;
        }
    }
}
