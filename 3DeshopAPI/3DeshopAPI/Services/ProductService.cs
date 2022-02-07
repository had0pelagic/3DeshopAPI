using _3DeshopAPI.Exceptions;
using _3DeshopAPI.Services.Interfaces;
using Domain.Product;
using Infrastructure;

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

        public Task<List<Product>> GetAllProducts()
        {
            throw new InvalidClientOperationException(ErrorCodes.ProductNotFound);
        }

        public Task<Product?> GetProduct(Guid id)
        {
            throw new InvalidClientOperationException(ErrorCodes.ProductNotFound);
        }
    }
}
