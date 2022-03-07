﻿using _3DeshopAPI.Exceptions;
using _3DeshopAPI.Models.Product;
using _3DeshopAPI.Services.Interfaces;
using AutoMapper;
using Domain.Product;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Services.Interfaces;

namespace _3DeshopAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly ILogger<ProductService> _logger;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly Context _context;
        private readonly IPaymentService _paymentService;

        public ProductService(ILogger<ProductService> logger, IUserService userService, IPaymentService paymentService, IMapper mapper, Context context)
        {
            _logger = logger;
            _userService = userService;
            _paymentService = paymentService;
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all products, returns only front page data about product
        /// </summary>
        /// <returns></returns>
        public async Task<List<ProductDisplayModel>> GetAllProducts()
        {
            var products = await _context.Products
                .Include(i => i.About)
                .AsNoTracking()
                .ToListAsync();

            return products.Select(x => ToProductDisplayModel(x).Result).ToList();
        }

        /// <summary>
        /// Get product by id, returns full information about product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="InvalidClientOperationException"></exception>
        public async Task<ProductModel?> GetProduct(Guid id)
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

            var payments = await _paymentService.GetPayments();
            var userId = _userService.GetCurrentUser().Id;
            var isBoughtByUser = payments.Any(x => x.UserId == userId && x.ProductId == id);

            return await ToProductModel(product, isBoughtByUser);
        }

        /// <summary>
        /// Upload product
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Product> UploadProduct(ProductUploadModel model)
        {
            var user = await _userService.GetUser(model.UserId);

            if (user == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.UserNotFound);
            }

            var newProduct = _mapper.Map<Product>(model);

            _context.Products.Add(newProduct);
            await SetProductCategories(newProduct, model.Categories);
            await SetProductFormats(newProduct, model.Formats);
            SetProductImages(newProduct, model.Images);

            await _context.SaveChangesAsync();

            return newProduct;
        }

        /// <summary>
        /// Maps Product model to ProductModel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ProductModel> ToProductModel(Product model, bool isBoughtByUser = false)
        {
            return new ProductModel
            {
                About = _mapper.Map<ProductAboutModel>(model.About),
                Specifications = _mapper.Map<ProductSpecificationsModel>(model.Specifications),
                UserId = model.UserId,
                Categories = await GetProductCategories(model.Id),
                Formats = await GetProductFormats(model.Id),
                Images = await GetProductImages(model.Id),
                IsBoughtByUser = isBoughtByUser
            };
        }

        /// <summary>
        /// Maps Product model to ProductDisplayModel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ProductDisplayModel> ToProductDisplayModel(Product model)
        {
            return new ProductDisplayModel
            {
                Id = model.Id,
                Name = model.About.Name,
                Username = await _userService.GetUsername(model.UserId),
                Price = model.About.Price,
                Downloads = model.About.Downloads,
                ImageUrl = await GetProductImageUrl(model.Id),
                Categories = await GetProductCategories(model.Id)
            };
        }

        /// <summary>
        /// Returns all user bought products
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<ProductDisplayModel>> GetAllPurchases(Guid id, List<Guid> productIds)
        {
            var products = await _context.Products
                .Include(i => i.About)
                .Where(x => productIds.Contains(x.Id))
                .AsNoTracking()
                .ToListAsync();

            return products.Select(x => ToProductDisplayModel(x).Result).ToList();
        }

        /// <summary>
        /// Gets product categories
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<List<ProductCategoryModel>> GetProductCategories(Guid id)
        {
            return await _context.ProductCategories
                .Where(i => i.Product.Id == id)
                .Include(i => i.Category)
                .Select(i => _mapper.Map<ProductCategoryModel>(i.Category))
                .ToListAsync();
        }

        /// <summary>
        /// Get product formats
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<List<ProductFormatModel>> GetProductFormats(Guid id)
        {
            return await _context.ProductFormats
                .Where(i => i.Product.Id == id)
                .Include(i => i.Format)
                .Select(i => _mapper.Map<ProductFormatModel>(i.Format))
                .ToListAsync();
        }

        /// <summary>
        /// Gets product images
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<List<ProductImageModel>> GetProductImages(Guid id)
        {
            return await _context.ProductImages
                .Where(i => i.Product.Id == id)
                .Include(i => i.Image)
                .Select(i => _mapper.Map<ProductImageModel>(i.Image))
                .ToListAsync();
        }

        /// <summary>
        /// Returns single product image
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<string> GetProductImageUrl(Guid id)
        {
            var images = await GetProductImages(id);

            return !images.Any() ? "NoImage" : images.FirstOrDefault().Data;
        }

        /// <summary>
        /// Sets product categories
        /// </summary>
        /// <param name="product"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        private async Task SetProductCategories(Product product, List<Guid> categories)
        {
            try
            {
                foreach (var categoryId in categories)
                {
                    var foundCategory = await _context.Categories.FindAsync(categoryId);

                    if (foundCategory == null)
                        throw new InvalidClientOperationException(ErrorCodes.CategoryNotFound);

                    _context.ProductCategories.Add(new ProductCategories
                    {
                        Product = product,
                        Category = foundCategory
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Sets product formats
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="formatId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidClientOperationException"></exception>
        private async Task SetProductFormats(Product product, List<Guid> formats)
        {
            try
            {
                foreach (var formatId in formats)
                {
                    var foundFormat = await _context.Formats.FindAsync(formatId);

                    if (foundFormat == null)
                    {
                        throw new InvalidClientOperationException(ErrorCodes.FormatNotFound);
                    }

                    var productFormat = new ProductFormats()
                    {
                        Product = product,
                        Format = foundFormat
                    };

                    _context.ProductFormats.Add(productFormat);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Adds and sets product image
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="InvalidClientOperationException"></exception>
        private void SetProductImages(Product product, List<ProductImageModel> images)
        {
            try
            {
                foreach (var image in images)
                {
                    var newImage = _mapper.Map<Image>(image);

                    _context.Images.Add(newImage);

                    var productImages = new ProductImages()
                    {
                        Image = newImage,
                        Product = product
                    };

                    _context.ProductImages.Add(productImages);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
