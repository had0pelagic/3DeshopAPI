using _3DeshopAPI.Exceptions;
using _3DeshopAPI.Services.Interfaces;
using Domain.Product;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _3DeshopAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IUserService _userService;
        private readonly Context _context;

        public ProductService(ILogger<UserService> logger, IUserService userService, Context context)
        {
            _logger = logger;
            _userService = userService;
            _context = context;
        }

        /// <summary>
        /// Get all products, returns only front page data about product
        /// </summary>
        /// <returns></returns>
        public async Task<List<Product>> GetAllProducts()
        {
            var products = await _context.Products
                .Include(i => i.About)
                .AsNoTracking()
                .ToListAsync();

            return products;
        }

        /// <summary>
        /// Get product by id, returns full information about product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="InvalidClientOperationException"></exception>
        public async Task<Product> GetProduct(Guid id)
        {
            var product = await _context.Products
                .Include(i => i.About)
                .Include(i => i.Specifications)
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id);

            if (product == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.ProductNotFound);
            }

            return product;
        }

        /// <summary>
        /// Upload product
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task UploadProduct(Product model)
        {
            var user = await _userService.GetUser(model.UserId);

            if (user == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.UserNotFound);
            }

            _context.Products.Add(model);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Sets product category
        /// </summary>
        /// <param name="product"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public async Task<IActionResult> SetProductCategory(Guid productId, Guid categoryId)
        {
            var product = await _context.Products.FindAsync(productId);
            var category = await _context.Categories.FindAsync(categoryId);

            if (product == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.ProductNotFound);
            }
            if (category == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.CategoryNotFound);
            }

            var productCategory = new ProductCategories()
            {
                Product = product,
                Category = category
            };

            _context.ProductCategories.Add(productCategory);
            await _context.SaveChangesAsync();

            return new NoContentResult();
        }

        /// <summary>
        /// Sets product format
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="formatId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidClientOperationException"></exception>
        public async Task<IActionResult> SetProductFormat(Guid productId, Guid formatId)
        {
            var product = await _context.Products.FindAsync(productId);
            var format = await _context.Formats.FindAsync(formatId);

            if (product == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.ProductNotFound);
            }
            if (format == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.FormatNotFound);
            }

            var productFormat = new ProductFormats()
            {
                Product = product,
                Format = format
            };

            _context.ProductFormats.Add(productFormat);
            await _context.SaveChangesAsync();

            return new NoContentResult();
        }

        /// <summary>
        /// Adds and sets product image
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="InvalidClientOperationException"></exception>
        public async Task<IActionResult> SetProductImage(Guid productId, Image model)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.ProductNotFound);
            }

            _context.Images.Add(model);

            var productImages = new ProductImages()
            {
                Image = model,
                Product = product
            };

            _context.ProductImages.Add(productImages);
            await _context.SaveChangesAsync();

            return new NoContentResult();
        }

        /// <summary>
        /// Adds and sets product comment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> AddProductComment(Guid productId, Guid userId, Comment model)
        {
            var product = await _context.Products.FindAsync(productId);
            var user = await _userService.GetUser(userId);

            if (product == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.ProductNotFound);
            }
            if (user == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.UserNotFound);
            }

            var comment = new Comment()
            {
                Product = product,
                User = user,
                Description = model.Description,
                Created = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return new NoContentResult();
        }
    }
}
