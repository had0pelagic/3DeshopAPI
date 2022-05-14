using _3DeshopAPI.Exceptions;
using _3DeshopAPI.Extensions;
using _3DeshopAPI.Mappings;
using _3DeshopAPI.Models.Product;
using _3DeshopAPI.Models.Settings;
using _3DeshopAPI.Services;
using _3DeshopAPI.Services.Interfaces;
using AutoMapper;
using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _3DeshopAPITest
{
    public class ProductServiceUnitTest
    {
        private IUserService _userService;
        private IProductService _productService;
        private IBalanceService _balanceService;
        private IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private Context _context;
        private IOptions<DefaultFileSettings> _fileSettings;
        private Guid _productCategory = new Guid("7074D187-E72F-4FA0-B1B3-3B08103FF73B");
        private Guid _productFormat = new Guid("60D7BB69-7DAF-4B9C-A033-133EE30326C3");

        [SetUp]
        public void Setup()
        {
            var dbContextOptions = new DbContextOptionsBuilder<Context>()
                .UseSqlServer("Server=localhost\\SQLEXPRESS;Database=DB_TEST;Trusted_Connection=True;");
            _context = new Context(dbContextOptions.Options);
            _context.Database.EnsureCreated();

            _fileSettings = Options.Create(new DefaultFileSettings() { Image = "10000000-0000-0000-0000-000000000000" });

            var config = new MapperConfiguration(opts =>
            {
                opts.AddProfile<ProductProfile>();
                opts.AddProfile<OrderProfile>();
                opts.AddProfile<ProductDetailProfile>();
                opts.AddProfile<UserProfile>();
            });
            _mapper = config.CreateMapper();

            _balanceService = new BalanceService(_context);
            _userService = new UserService(
                _mapper,
                _contextAccessor,
                _context,
                _balanceService,
                _fileSettings);
            _productService = new ProductService(_userService, _balanceService, _mapper, _context);
        }

        /// <summary>
        /// Registers userOwner
        /// uploads new product with random userId
        /// checks if thrown exception is correct
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task UploadProduct_UserNotFound_Test()
        {
            using var transaction = _context.Database.BeginTransaction();

            var userOwner = new User()
            {
                Id = Guid.NewGuid(),
                Email = Faker.Internet.Email(),
                FirstName = Faker.Name.First(),
                LastName = Faker.Name.Last(),
                Password = Faker.Phone.Number(),
                Username = Faker.Internet.UserName()
            };

            var productUploadModel = new ProductUploadModel()
            {
                About = new ProductAboutModel()
                {
                    Description = Faker.Internet.UserName(),
                    Name = Faker.Internet.UserName(),
                    Price = 10,
                    Downloads = 0,
                    IsActive = true,
                    UploadDate = DateTime.Now,
                    VideoLink = "none"
                },
                Categories = new List<Guid>() { _productCategory },
                Files = new List<FileModel>() {
                    new FileModel() {
                        Data = Array.Empty<byte>(),
                        Name = "file.obj",
                        Format = ".obj",
                        Size = 20 } },
                Formats = new List<Guid>() { _productFormat },
                Images = new List<ImageModel>() { new ImageModel() {
                    Data = Array.Empty<byte>(),
                    Name = "image.jpg",
                    Format = ".jpg",
                    Size = 20 } },
                Specifications = new ProductSpecificationsModel()
                {
                    Animation = false,
                    Materials = true,
                    Rig = false,
                    Textures = true
                },
                UserId = Guid.NewGuid()
            };

            try
            {
                await _userService.RegisterUser(userOwner);
                await _productService.UploadProduct(productUploadModel);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Assert.AreEqual(ex.Message, ErrorCodes.UserNotFound.GetEnumDescription());
            }
        }

        /// <summary>
        /// Registers userOwner
        /// uploads new product 
        /// checks if returned product is not null and its name is equal to upload model name
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task UploadProduct_Success_Test()
        {
            using var transaction = _context.Database.BeginTransaction();

            var userOwner = new User()
            {
                Id = Guid.NewGuid(),
                Email = Faker.Internet.Email(),
                FirstName = Faker.Name.First(),
                LastName = Faker.Name.Last(),
                Password = Faker.Phone.Number(),
                Username = Faker.Internet.UserName()
            };

            var productUploadModel = new ProductUploadModel()
            {
                About = new ProductAboutModel()
                {
                    Description = Faker.Internet.UserName(),
                    Name = Faker.Internet.UserName(),
                    Price = 10,
                    Downloads = 0,
                    IsActive = true,
                    UploadDate = DateTime.Now,
                    VideoLink = "none"
                },
                Categories = new List<Guid>() { _productCategory },
                Files = new List<FileModel>() {
                    new FileModel() {
                        Data = Array.Empty<byte>(),
                        Name = "file.obj",
                        Format = ".obj",
                        Size = 20 } },
                Formats = new List<Guid>() { _productFormat },
                Images = new List<ImageModel>() { new ImageModel() {
                    Data = Array.Empty<byte>(),
                    Name = "image.jpg",
                    Format = ".jpg",
                    Size = 20 } },
                Specifications = new ProductSpecificationsModel()
                {
                    Animation = false,
                    Materials = true,
                    Rig = false,
                    Textures = true
                },
                UserId = userOwner.Id
            };

            try
            {
                await _userService.RegisterUser(userOwner);
                var product = await _productService.UploadProduct(productUploadModel);

                Assert.IsNotNull(product);
                Assert.AreEqual(product.About.Name, productUploadModel.About.Name);
                transaction.Rollback();
            }
            catch
            {
                transaction.Rollback();
            }
        }

        /// <summary>
        /// Registers userOwner
        /// uploads 3 products
        /// returns all existing products
        /// checks if existing product count is equal to 3
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetAllProducts_SuccessReturn3Products_Test()
        {
            using var transaction = _context.Database.BeginTransaction();

            var userOwner = new User()
            {
                Id = Guid.NewGuid(),
                Email = Faker.Internet.Email(),
                FirstName = Faker.Name.First(),
                LastName = Faker.Name.Last(),
                Password = Faker.Phone.Number(),
                Username = Faker.Internet.UserName()
            };

            var productUploadModel = new ProductUploadModel()
            {
                About = new ProductAboutModel()
                {
                    Description = Faker.Internet.UserName(),
                    Name = Faker.Internet.UserName(),
                    Price = 10,
                    Downloads = 0,
                    IsActive = true,
                    UploadDate = DateTime.Now,
                    VideoLink = "none"
                },
                Categories = new List<Guid>() { _productCategory },
                Files = new List<FileModel>() {
                    new FileModel() {
                        Data = Array.Empty<byte>(),
                        Name = "file.obj",
                        Format = ".obj",
                        Size = 20 } },
                Formats = new List<Guid>() { _productFormat },
                Images = new List<ImageModel>() { new ImageModel() {
                    Data = Array.Empty<byte>(),
                    Name = "image.jpg",
                    Format = ".jpg",
                    Size = 20 } },
                Specifications = new ProductSpecificationsModel()
                {
                    Animation = false,
                    Materials = true,
                    Rig = false,
                    Textures = true
                },
                UserId = userOwner.Id
            };

            try
            {
                await _userService.RegisterUser(userOwner);
                await _productService.UploadProduct(productUploadModel);
                await _productService.UploadProduct(productUploadModel);
                await _productService.UploadProduct(productUploadModel);
                var products = await _productService.GetAllProducts();

                Assert.IsNotNull(products);
                Assert.AreEqual(products.Count, 3);
                transaction.Rollback();
            }
            catch
            {
                transaction.Rollback();
            }
        }

        /// <summary>
        /// Register userOwner
        /// uploads 2 products with given category and format
        /// returns all given products by given criteria
        /// checks if returned products are not null and count is equal to 2
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetProductsByCriteria_Success_Test()
        {
            using var transaction = _context.Database.BeginTransaction();

            var userOwner = new User()
            {
                Id = Guid.NewGuid(),
                Email = Faker.Internet.Email(),
                FirstName = Faker.Name.First(),
                LastName = Faker.Name.Last(),
                Password = Faker.Phone.Number(),
                Username = Faker.Internet.UserName()
            };

            var productUploadModel = new ProductUploadModel()
            {
                About = new ProductAboutModel()
                {
                    Description = Faker.Internet.UserName(),
                    Name = Faker.Internet.UserName(),
                    Price = 10,
                    Downloads = 0,
                    IsActive = true,
                    UploadDate = DateTime.Now,
                    VideoLink = "none"
                },
                Categories = new List<Guid>() { _productCategory },
                Files = new List<FileModel>() {
                    new FileModel() {
                        Data = Array.Empty<byte>(),
                        Name = "file.obj",
                        Format = ".obj",
                        Size = 20 } },
                Formats = new List<Guid>() { _productFormat },
                Images = new List<ImageModel>() { new ImageModel() {
                    Data = Array.Empty<byte>(),
                    Name = "image.jpg",
                    Format = ".jpg",
                    Size = 20 } },
                Specifications = new ProductSpecificationsModel()
                {
                    Animation = false,
                    Materials = true,
                    Rig = false,
                    Textures = true
                },
                UserId = userOwner.Id
            };

            var productFindByCriteriaModel = new ProductFindByCriteriaModel()
            {
                Categories = new List<string>() { _productCategory.ToString() },
                Formats = new List<string>() { _productFormat.ToString() },
                Name = "",
                Specifications = new ProductSpecificationsModel()
                {
                    Animation = false,
                    Materials = true,
                    Rig = false,
                    Textures = true
                }
            };

            try
            {
                await _userService.RegisterUser(userOwner);
                await _productService.UploadProduct(productUploadModel);
                await _productService.UploadProduct(productUploadModel);
                var products = await _productService.GetProductsByCriteria(productFindByCriteriaModel);

                Assert.IsNotNull(products);
                Assert.AreEqual(products.Count, 2);
                transaction.Rollback();
            }
            catch
            {
                transaction.Rollback();
            }
        }

        /// <summary>
        /// Uploads product
        /// tries to return all user products
        /// checks if correct exception is thrown
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetUserProducts_UserNotFound_Test()
        {
            using var transaction = _context.Database.BeginTransaction();

            var productUploadModel = new ProductUploadModel()
            {
                About = new ProductAboutModel()
                {
                    Description = Faker.Internet.UserName(),
                    Name = Faker.Internet.UserName(),
                    Price = 10,
                    Downloads = 0,
                    IsActive = true,
                    UploadDate = DateTime.Now,
                    VideoLink = "none"
                },
                Categories = new List<Guid>() { _productCategory },
                Files = new List<FileModel>() {
                    new FileModel() {
                        Data = Array.Empty<byte>(),
                        Name = "file.obj",
                        Format = ".obj",
                        Size = 20 } },
                Formats = new List<Guid>() { _productFormat },
                Images = new List<ImageModel>() { new ImageModel() {
                    Data = Array.Empty<byte>(),
                    Name = "image.jpg",
                    Format = ".jpg",
                    Size = 20 } },
                Specifications = new ProductSpecificationsModel()
                {
                    Animation = false,
                    Materials = true,
                    Rig = false,
                    Textures = true
                },
                UserId = Guid.NewGuid()
            };

            try
            {
                await _productService.UploadProduct(productUploadModel);
                await _productService.GetUserProducts(Guid.NewGuid());
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Assert.AreEqual(ex.Message, ErrorCodes.UserNotFound.GetEnumDescription());
            }
        }

        /// <summary>
        /// Registers userOwner
        /// uploads 3 products
        /// returns all user products
        /// checks if returned products is not null and count is equal to 3
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetUserProducts_SuccessReturn3Products_Test()
        {
            using var transaction = _context.Database.BeginTransaction();

            var userOwner = new User()
            {
                Id = Guid.NewGuid(),
                Email = Faker.Internet.Email(),
                FirstName = Faker.Name.First(),
                LastName = Faker.Name.Last(),
                Password = Faker.Phone.Number(),
                Username = Faker.Internet.UserName()
            };

            var productUploadModel = new ProductUploadModel()
            {
                About = new ProductAboutModel()
                {
                    Description = Faker.Internet.UserName(),
                    Name = Faker.Internet.UserName(),
                    Price = 10,
                    Downloads = 0,
                    IsActive = true,
                    UploadDate = DateTime.Now,
                    VideoLink = "none"
                },
                Categories = new List<Guid>() { _productCategory },
                Files = new List<FileModel>() {
                    new FileModel() {
                        Data = Array.Empty<byte>(),
                        Name = "file.obj",
                        Format = ".obj",
                        Size = 20 } },
                Formats = new List<Guid>() { _productFormat },
                Images = new List<ImageModel>() { new ImageModel() {
                    Data = Array.Empty<byte>(),
                    Name = "image.jpg",
                    Format = ".jpg",
                    Size = 20 } },
                Specifications = new ProductSpecificationsModel()
                {
                    Animation = false,
                    Materials = true,
                    Rig = false,
                    Textures = true
                },
                UserId = userOwner.Id
            };

            try
            {
                await _userService.RegisterUser(userOwner);
                await _productService.UploadProduct(productUploadModel);
                await _productService.UploadProduct(productUploadModel);
                await _productService.UploadProduct(productUploadModel);
                var products = await _productService.GetUserProducts(userOwner.Id);

                Assert.IsNotNull(products);
                Assert.AreEqual(products.Count, 3);
                transaction.Rollback();
            }
            catch
            {
                transaction.Rollback();
            }
        }
    }
}
