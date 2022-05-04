using _3DeshopAPI.Exceptions;
using _3DeshopAPI.Models.Product;
using _3DeshopAPI.Models.User;
using _3DeshopAPI.Services.Interfaces;
using AutoMapper;
using Domain.Product;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace _3DeshopAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly ILogger<ProductService> _logger;
        private readonly IUserService _userService;
        private readonly IBalanceService _balanceService;
        private readonly IMapper _mapper;
        private readonly Context _context;

        public ProductService(ILogger<ProductService> logger, IUserService userService, IBalanceService balanceService, IMapper mapper, Context context)
        {
            _logger = logger;
            _userService = userService;
            _balanceService = balanceService;
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

            return products.Select(x => ProductToProductDisplayModel(x).Result).ToList();
        }

        /// <summary>
        /// Gets all products by given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<List<ProductDisplayModel>> GetProductsByName(string name)
        {
            var products = await _context.Products
                .Include(i => i.About)
                .Where(x => x.About.Name.Contains(name))
                .AsNoTracking()
                .ToListAsync();

            return products.Select(x => ProductToProductDisplayModel(x).Result).ToList();
        }

        /// <summary>
        /// Gets all products by given criteria
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<List<ProductDisplayModel>> GetProductsByCriteria(ProductFindByCriteriaModel model)
        {
            var productsByName = await _context.Products
                .Include(x => x.About)
                .Include(x => x.Specifications)
                .Where(x => x.About.Name.Contains(model.Name))
                .AsNoTracking()
                .ToListAsync();

            var containsValidCategoryIds = model.Categories.Any(x => Guid.TryParse(x, out _));

            if (containsValidCategoryIds)
            {
                productsByName = await GetProductsByCategories(model, productsByName);
            }

            var containsValidFormatIds = model.Formats.Any(x => Guid.TryParse(x, out _));

            if (containsValidFormatIds)
            {
                productsByName = await GetProductsByFormats(model, productsByName);
            }

            var unselectedSpecifications = model.Specifications
                .GetType()
                .GetProperties()
                .Where(x => x.PropertyType == typeof(bool))
                .Select(x => (bool)x.GetValue(model.Specifications))
                .All(x => x.Equals(false));

            if (!unselectedSpecifications)
            {
                productsByName = GetProductsBySpecifications(model, productsByName);
            }

            return productsByName.Select(x => ProductToProductDisplayModel(x).Result).ToList();
        }

        /// <summary>
        /// Returns all user uploaded products
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidClientOperationException"></exception>
        public async Task<List<ProductTableDisplayModel>> GetUserProducts(Guid userId)
        {
            var user = await _userService.GetUser(userId);

            if (user == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.UserNotFound);
            }

            var products = await _context.Products
                .Include(i => i.About)
                .Where(x => x.UserId == userId)
                .AsNoTracking()
                .ToListAsync();

            return products.Select(x => ProductToProductTableDisplayModel(x).Result).ToList();
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

            var userId = _userService.GetCurrentUser().Id;
            var purchasedIds = await _balanceService.GetPurchasedProductIds(userId);
            var isBoughtByUser = purchasedIds.Contains(id);

            return await ProductToProductModel(product, isBoughtByUser);
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

            await _context.Products.AddAsync(newProduct);
            await SetProductFiles(newProduct, model.Files);
            await SetProductCategories(newProduct, model.Categories);
            await SetProductFormats(newProduct, model.Formats);
            await SetProductImages(newProduct, model.Images);

            await _context.SaveChangesAsync();

            return newProduct;
        }

        /// <summary>
        /// Maps Product model to ProductModel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ProductModel> ProductToProductModel(Product model, bool isBoughtByUser = false)
        {
            return new ProductModel
            {
                About = _mapper.Map<ProductAboutModel>(model.About),
                Specifications = _mapper.Map<ProductSpecificationsModel>(model.Specifications),
                User = new UserDisplayModel()
                {
                    Id = model.UserId,
                    Username = await _userService.GetUsername(model.UserId)
                },
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
        public async Task<ProductDisplayModel> ProductToProductDisplayModel(Product model)
        {
            return new ProductDisplayModel
            {
                Id = model.Id,
                Name = model.About.Name,
                User = new UserDisplayModel()
                {
                    Id = model.UserId,
                    Username = await _userService.GetUsername(model.UserId)
                },
                Price = model.About.Price,
                Downloads = model.About.Downloads,
                Image = await GetProductImage(model.Id),
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

            return products.Select(x => ProductToProductDisplayModel(x).Result).ToList();
        }

        /// <summary>
        /// Finds products by given categories
        /// </summary>
        /// <param name="model"></param>
        /// <param name="products"></param>
        /// <returns></returns>
        private async Task<List<Product>> GetProductsByCategories(ProductFindByCriteriaModel model, List<Product> products)
        {
            var categoriesToGuid = model.Categories
                .ConvertAll(x => Guid.Parse(x));

            var productIdsByCategory = await _context.ProductCategories
                .Include(x => x.Product)
                .Include(x => x.Category)
                .Where(x => categoriesToGuid.Contains(x.Category.Id))
                .Select(x => x.Product.Id)
                .ToListAsync();

            products = products
                .Where(x => productIdsByCategory.Contains(x.Id))
                .ToList();

            return products;
        }

        /// <summary>
        /// Finds products by given formats
        /// </summary>
        /// <param name="model"></param>
        /// <param name="products"></param>
        /// <returns></returns>
        private async Task<List<Product>> GetProductsByFormats(ProductFindByCriteriaModel model, List<Product> products)
        {
            var formatsToGuid = model.Formats
                .ConvertAll(x => Guid.Parse(x));

            var productIdsByFormat = await _context.ProductFormats
                .Include(x => x.Product)
                .Include(x => x.Format)
                .Where(x => formatsToGuid.Contains(x.Format.Id))
                .Select(x => x.Product.Id)
                .ToListAsync();

            products = products
                .Where(x => productIdsByFormat.Contains(x.Id))
                .ToList();

            return products;
        }
        /// <summary>
        /// Finds products by given specifications
        /// </summary>
        /// <param name="model"></param>
        /// <param name="products"></param>
        /// <returns></returns>
        private List<Product> GetProductsBySpecifications(ProductFindByCriteriaModel model, List<Product> products)
        {
            if (model.Specifications.Textures)
            {
                products = products
                .Where(x => x.Specifications.Textures)
                .ToList();
            }

            if (model.Specifications.Materials)
            {
                products = products
                .Where(x => x.Specifications.Materials)
                .ToList();
            }

            if (model.Specifications.Animation)
            {
                products = products
                .Where(x => x.Specifications.Animation)
                .ToList();
            }

            if (model.Specifications.Rig)
            {
                products = products
                .Where(x => x.Specifications.Rig)
                .ToList();
            }

            return products;
        }

        /// <summary>
        /// Maps Product model to ProductTableDisplayModel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task<ProductTableDisplayModel> ProductToProductTableDisplayModel(Product model)
        {
            return new ProductTableDisplayModel
            {
                Id = model.Id,
                Name = model.About.Name,
            };
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
        private async Task<List<ImageModel>> GetProductImages(Guid id)
        {
            return await _context.ProductImages
                .Where(i => i.Product.Id == id)
                .Include(i => i.Image)
                .Select(i => _mapper.Map<ImageModel>(i.Image))
                .ToListAsync();
        }

        /// <summary>
        /// Returns single product image
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<ImageModel> GetProductImage(Guid id)
        {
            var images = await GetProductImages(id);

            //return !images.Any() ? new byte[] : images.FirstOrDefault().Data;
            return !images.Any() ? null : images.FirstOrDefault();
        }

        /// <summary>
        /// Sets product files
        /// </summary>
        /// <param name="product"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task SetProductFiles(Product product, List<FileModel> files)
        {
            try
            {
                foreach (var file in files)
                {
                    var newFile = _mapper.Map<Domain.Product.File>(file);
                    await _context.Files.AddAsync(newFile);
                    await _context.ProductFiles.AddAsync(new ProductFiles()
                    {
                        File = newFile,
                        Product = product
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
                    {
                        throw new InvalidClientOperationException(ErrorCodes.CategoryNotFound);
                    }

                    await _context.ProductCategories.AddAsync(new ProductCategories
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

                    await _context.ProductFormats.AddAsync(new ProductFormats()
                    {
                        Product = product,
                        Format = foundFormat
                    });
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
        private async Task SetProductImages(Product product, List<ImageModel> images)
        {
            try
            {
                foreach (var image in images)
                {
                    var newImage = _mapper.Map<Image>(image);
                    await _context.Images.AddAsync(newImage);
                    await _context.ProductImages.AddAsync(new ProductImages()
                    {
                        Image = newImage,
                        Product = product
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
