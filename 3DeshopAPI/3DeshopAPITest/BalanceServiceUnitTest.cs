using _3DeshopAPI.Exceptions;
using _3DeshopAPI.Extensions;
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
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _3DeshopAPI.Models;

namespace _3DeshopAPITest;

public class BalanceServiceUnitTest
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

        _fileSettings = Options.Create(new DefaultFileSettings { Image = "10000000-0000-0000-0000-000000000000" });

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
    /// Register a new user
    /// returns his current balance, which is 0
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task GetUserBalance_BalanceIs0_Test()
    {
        using var transaction = _context.Database.BeginTransaction();

        var user = new User
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

    /// <summary>
    /// Register a new user
    /// add to balance 100 credits
    /// check users balance
    /// returns user current balance, which is 100
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task GetUserBalance_BalanceIs100_Test()
    {
        using var transaction = _context.Database.BeginTransaction();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = Faker.Internet.Email(),
            FirstName = Faker.Name.First(),
            LastName = Faker.Name.Last(),
            Password = Faker.Phone.Number(),
            Username = Faker.Internet.UserName()
        };

        var topUpModel = new TopUpModel
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

    /// <summary>
    /// Register a new user
    /// add to balance 100 credits
    /// checks if returned balance is not null
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task BalanceTopUp_Success_Test()
    {
        using var transaction = _context.Database.BeginTransaction();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = Faker.Internet.Email(),
            FirstName = Faker.Name.First(),
            LastName = Faker.Name.Last(),
            Password = Faker.Phone.Number(),
            Username = Faker.Internet.UserName()
        };

        var topUpModel = new TopUpModel
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

    /// <summary>
    /// Registers owner and buyer users
    /// add 100 credits to buyers balance
    /// upload new product
    /// buyer purchases created product
    /// checks if balance is not null
    /// checks if balance product id is equal to uploaded product id
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task PayForProduct_Success_Test()
    {
        using var transaction = _context.Database.BeginTransaction();

        var userOwner = new User
        {
            Id = Guid.NewGuid(),
            Email = Faker.Internet.Email(),
            FirstName = Faker.Name.First(),
            LastName = Faker.Name.Last(),
            Password = Faker.Phone.Number(),
            Username = Faker.Internet.UserName()
        };

        var userBuyer = new User
        {
            Id = Guid.NewGuid(),
            Email = Faker.Internet.Email(),
            FirstName = Faker.Name.First(),
            LastName = Faker.Name.Last(),
            Password = Faker.Phone.Number(),
            Username = Faker.Internet.UserName()
        };

        var topUpModel = new TopUpModel
        {
            Amount = 100,
            UserId = userBuyer.Id,
        };

        var productUploadModel = new ProductUploadModel
        {
            About = new ProductAboutModel
            {
                Description = Faker.Internet.UserName(),
                Name = Faker.Internet.UserName(),
                Price = 10,
                Downloads = 0,
                IsActive = true,
                UploadDate = DateTime.Now,
                VideoLink = "none"
            },
            Categories = new List<Guid> {
                new Guid("7074D187-E72F-4FA0-B1B3-3B08103FF73B") },
            Files = new List<FileModel> {
                new FileModel {
                    Data = Array.Empty<byte>(),
                    Name = "file.obj",
                    Format = ".obj",
                    Size = 20 } },
            Formats = new List<Guid> {
                new Guid("60D7BB69-7DAF-4B9C-A033-133EE30326C3") },
            Images = new List<ImageModel> { new ImageModel {
                Data = Array.Empty<byte>(),
                Name = "image.jpg",
                Format = ".jpg",
                Size = 20 } },
            Specifications = new ProductSpecificationsModel
            {
                Animation = false,
                Materials = true,
                Rig = false,
                Textures = true
            },
            UserId = userOwner.Id
        };

        var payForProductModel = new PayForProductModel
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
            Assert.AreEqual(product.Id, balance.Product.Id);
            transaction.Rollback();
        }
        catch
        {
            transaction.Rollback();
        }
    }

    /// <summary>
    /// Registers owner and buyer users
    /// add 1 credit to buyers balance
    /// upload new product with the price of 999999 credits
    /// buyer purchases created product
    /// checks if correct exception is thrown
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task PayForProduct_BalanceNotEnough_Test()
    {
        using var transaction = _context.Database.BeginTransaction();

        var userOwner = new User
        {
            Id = Guid.NewGuid(),
            Email = Faker.Internet.Email(),
            FirstName = Faker.Name.First(),
            LastName = Faker.Name.Last(),
            Password = Faker.Phone.Number(),
            Username = Faker.Internet.UserName()
        };

        var userBuyer = new User
        {
            Id = Guid.NewGuid(),
            Email = Faker.Internet.Email(),
            FirstName = Faker.Name.First(),
            LastName = Faker.Name.Last(),
            Password = Faker.Phone.Number(),
            Username = Faker.Internet.UserName()
        };

        var topUpModel = new TopUpModel
        {
            Amount = 1,
            UserId = userBuyer.Id,
        };

        var productUploadModel = new ProductUploadModel
        {
            About = new ProductAboutModel
            {
                Description = Faker.Internet.UserName(),
                Name = Faker.Internet.UserName(),
                Price = 999999,
                Downloads = 0,
                IsActive = true,
                UploadDate = DateTime.Now,
                VideoLink = "none"
            },
            Categories = new List<Guid> { _productCategory },
            Files = new List<FileModel> {
                new FileModel {
                    Data = Array.Empty<byte>(),
                    Name = "file.obj",
                    Format = ".obj",
                    Size = 20 } },
            Formats = new List<Guid> { _productFormat },
            Images = new List<ImageModel>() { new ImageModel {
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

        var payForProductModel = new PayForProductModel
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
            await _balanceService.PayForProduct(payForProductModel);
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Assert.AreEqual(ex.Message, ErrorCodes.NotEnoughBalance.GetEnumDescription());
        }
    }

    /// <summary>
    /// Registers owner 
    /// add 10 credits to buyers balance
    /// upload new product 
    /// owner tries to purchase created product
    /// checks if correct exception is thrown
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task PayForProduct_OwnerUnableToBuyHisProduct_Test()
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

        var topUpModel = new TopUpModel
        {
            Amount = 10,
            UserId = userOwner.Id,
        };

        var productUploadModel = new ProductUploadModel
        {
            About = new ProductAboutModel
            {
                Description = Faker.Internet.UserName(),
                Name = Faker.Internet.UserName(),
                Price = 1,
                Downloads = 0,
                IsActive = true,
                UploadDate = DateTime.Now,
                VideoLink = "none"
            },
            Categories = new List<Guid>() {
                new Guid("7074D187-E72F-4FA0-B1B3-3B08103FF73B") },
            Files = new List<FileModel> {
                new FileModel() {
                    Data = Array.Empty<byte>(),
                    Name = "file.obj",
                    Format = ".obj",
                    Size = 20 } },
            Formats = new List<Guid> {
                new Guid("60D7BB69-7DAF-4B9C-A033-133EE30326C3") },
            Images = new List<ImageModel>() { new ImageModel {
                Data = Array.Empty<byte>(),
                Name = "image.jpg",
                Format = ".jpg",
                Size = 20 } },
            Specifications = new ProductSpecificationsModel
            {
                Animation = false,
                Materials = true,
                Rig = false,
                Textures = true
            },
            UserId = userOwner.Id
        };

        var payForProductModel = new PayForProductModel
        {
            ProductId = Guid.NewGuid(),
            UserId = userOwner.Id,
        };

        try
        {
            await _userService.RegisterUser(userOwner);
            await _balanceService.BalanceTopUp(topUpModel);
            var product = await _productService.UploadProduct(productUploadModel);
            payForProductModel.ProductId = product.Id;
            await _balanceService.PayForProduct(payForProductModel);
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Assert.AreEqual(ex.Message, ErrorCodes.OwnerUnableToBuyProduct.GetEnumDescription());
        }
    }

    /// <summary>
    /// Registers owner and buyer users
    /// add 10 credits to buyers balance
    /// upload new product 
    /// buyer purchases created product
    /// buyer purchases the same created product the second time
    /// checks if correct exception is thrown
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task PayForProduct_DuplicateBuy_Test()
    {
        using var transaction = _context.Database.BeginTransaction();

        var userOwner = new User
        {
            Id = Guid.NewGuid(),
            Email = Faker.Internet.Email(),
            FirstName = Faker.Name.First(),
            LastName = Faker.Name.Last(),
            Password = Faker.Phone.Number(),
            Username = Faker.Internet.UserName()
        };

        var userBuyer = new User
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
            Amount = 10,
            UserId = userBuyer.Id,
        };

        var productUploadModel = new ProductUploadModel
        {
            About = new ProductAboutModel()
            {
                Description = Faker.Internet.UserName(),
                Name = Faker.Internet.UserName(),
                Price = 1,
                Downloads = 0,
                IsActive = true,
                UploadDate = DateTime.Now,
                VideoLink = "none"
            },
            Categories = new List<Guid> { _productCategory },
            Files = new List<FileModel> {
                new FileModel {
                    Data = Array.Empty<byte>(),
                    Name = "file.obj",
                    Format = ".obj",
                    Size = 20 } },
            Formats = new List<Guid> { _productFormat },
            Images = new List<ImageModel> { new ImageModel {
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

        var payForProductModel = new PayForProductModel
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
            await _balanceService.PayForProduct(payForProductModel);
            await _balanceService.PayForProduct(payForProductModel);
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Assert.AreEqual(ex.Message, ErrorCodes.DuplicateBuy.GetEnumDescription());
        }
    }
}