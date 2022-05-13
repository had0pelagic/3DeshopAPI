using _3DeshopAPI.Mappings;
using _3DeshopAPI.Models.Balance;
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
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _3DeshopAPITest
{
    public class BalanceServiceUnitTest
    {
        private IUserService _userService;
        private IProductService _productService;
        private IBalanceService _balanceService;
        private IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private Context _context;
        private IOptions<DefaultFileSettings> _fileSettings;


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

        [Test]
        public async Task GetUserBalance_BalanceIs0_Test()
        {
            using var transaction = _context.Database.BeginTransaction();

            var user = new User()
            {
                Id = Guid.NewGuid(),
                Email = Faker.Internet.Email(),
                FirstName = Faker.Name.First(),
                LastName = Faker.Name.Last(),
                Password = Faker.Phone.Number(),
                Username = Faker.Internet.UserName()
            };

            try
            {
                await _userService.RegisterUser(user);
                var balance = await _balanceService.GetUserBalance(user.Id);

                Assert.AreEqual(balance.Balance, 0);
                transaction.Rollback();
            }
            catch
            {
                transaction.Rollback();
            }
        }

        [Test]
        public async Task GetUserBalance_BalanceIs100_Test()
        {
            using var transaction = _context.Database.BeginTransaction();

            var user = new User()
            {
                Id = Guid.NewGuid(),
                Email = Faker.Internet.Email(),
                FirstName = Faker.Name.First(),
                LastName = Faker.Name.Last(),
                Password = Faker.Phone.Number(),
                Username = Faker.Internet.UserName()
            };

            var topUpModel = new TopUpModel()
            {
                Amount = 100,
                UserId = user.Id,
            };

            try
            {
                await _userService.RegisterUser(user);
                await _balanceService.BalanceTopUp(topUpModel);
                var balance = await _balanceService.GetUserBalance(user.Id);

                Assert.AreEqual(balance.Balance, 100);
                transaction.Rollback();
            }
            catch
            {
                transaction.Rollback();
            }
        }

        [Test]
        public async Task BalanceTopUp_Success_Test()
        {
            using var transaction = _context.Database.BeginTransaction();

            var user = new User()
            {
                Id = Guid.NewGuid(),
                Email = Faker.Internet.Email(),
                FirstName = Faker.Name.First(),
                LastName = Faker.Name.Last(),
                Password = Faker.Phone.Number(),
                Username = Faker.Internet.UserName()
            };

            var topUpModel = new TopUpModel()
            {
                Amount = 100,
                UserId = user.Id,
            };

            try
            {
                await _userService.RegisterUser(user);
                var balance = await _balanceService.BalanceTopUp(topUpModel);

                Assert.IsNotNull(balance);
                transaction.Rollback();
            }
            catch
            {
                transaction.Rollback();
            }
        }

        [Test]
        public async Task PayForProduct_Success_Test()
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

            var userBuyer = new User()
            {
                Id = Guid.NewGuid(),
                Email = Faker.Internet.Email(),
                FirstName = Faker.Name.First(),
                LastName = Faker.Name.Last(),
                Password = Faker.Phone.Number(),
                Username = Faker.Internet.UserName()
            };

            var topUpModel = new TopUpModel()
            {
                Amount = 100,
                UserId = userBuyer.Id,
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
                Categories = new List<Guid>() {
                    new Guid("7074D187-E72F-4FA0-B1B3-3B08103FF73B") },
                Files = new List<FileModel>() {
                    new FileModel() {
                        Data = Array.Empty<byte>(),
                        Name = "file.obj",
                        Format = ".obj",
                        Size = 20 } },
                Formats = new List<Guid>() {
                    new Guid("60D7BB69-7DAF-4B9C-A033-133EE30326C3") },
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

            var payForProductModel = new PayForProductModel()
            {
                ProductId = Guid.NewGuid(),
                UserId = userBuyer.Id,
            };

            try
            {
                await _userService.RegisterUser(userOwner);
                await _userService.RegisterUser(userBuyer);
                await _balanceService.BalanceTopUp(topUpModel);
                var product = await _productService.UploadProduct(productUploadModel);
                payForProductModel.ProductId = product.Id;
                var balance = await _balanceService.PayForProduct(payForProductModel);

                Assert.IsNotNull(balance);
                transaction.Rollback();
            }
            catch
            {
                transaction.Rollback();
            }
        }
    }
}
