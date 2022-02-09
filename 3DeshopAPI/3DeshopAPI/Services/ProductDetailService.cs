using _3DeshopAPI.Services.Interfaces;
using Domain.Product;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace _3DeshopAPI.Services
{
    public class ProductDetailService : IProductDetailService
    {
        private readonly ILogger<ProductService> _logger;
        private readonly Context _context;

        public ProductDetailService(ILogger<ProductService> logger, IUserService userService, Context context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        /// <returns></returns>
        public async Task<List<Category>> GetAllCategories()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .ToListAsync();

            return categories;
        }

        /// <summary>
        /// Get all formats
        /// </summary>
        /// <returns></returns>
        public async Task<List<Format>> GetAllFormats()
        {
            var formats = await _context.Formats
                .AsNoTracking()
                .ToListAsync();

            return formats;
        }
    }
}
