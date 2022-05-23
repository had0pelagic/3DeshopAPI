using _3DeshopAPI.Exceptions;
using _3DeshopAPI.Extensions;
using _3DeshopAPI.Models.Settings;
using _3DeshopAPI.Models.User;
using _3DeshopAPI.Services;
using _3DeshopAPI.Services.Interfaces;
using AutoMapper;
using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace _3DeshopAPITest;

public class UserServiceUnitTest
{
    private IUserService _userService;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _contextAccessor;
    private Context _context;
    private IBalanceService _balanceService;
    private IOptions<DefaultFileSettings> _fileSettings;


    [SetUp]
    public void Setup()
    {
        var dbContextOptions = new DbContextOptionsBuilder<Context>()
            .UseSqlServer("Server=localhost\\SQLEXPRESS;Database=DB_TEST;Trusted_Connection=True;");
        _context = new Context(dbContextOptions.Options);
        _context.Database.EnsureCreated();
        _fileSettings = Options.Create(new DefaultFileSettings { Image = "10000000-0000-0000-0000-000000000000" });

        _balanceService = new BalanceService(_context);
        _userService = new UserService(
            _mapper,
            _contextAccessor,
            _context,
            _balanceService,
            _fileSettings);
    }

    /// <summary>
    /// Registers 2 users with the same username
    /// checks if correct exception is thrown
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task RegisterUser_UserAlreadyExists_Test()
    {
        using var transaction = _context.Database.BeginTransaction();

        var user = new User
        {
            Email = Faker.Internet.Email(),
            FirstName = Faker.Name.First(),
            LastName = Faker.Name.Last(),
            Password = Faker.Phone.Number(),
            Username = Faker.Internet.UserName()
        };

        try
        {
            await _userService.RegisterUser(user);
            await _userService.RegisterUser(user);
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Assert.AreEqual(ex.Message, ErrorCodes.InvalidUsername.GetEnumDescription());
        }
    }

    /// <summary>
    /// Registers user
    /// checks if returned user is not null
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task RegisterUser_Success_Test()
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
            var response = await _userService.GetUser(user.Id);

            Assert.IsNotNull(response);
            transaction.Rollback();
        }
        catch
        {
            transaction.Rollback();
        }
    }

    /// <summary>
    /// Tries to change user password with random user id
    /// checks if correct exception is thrown
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task ChangePassword_UserNotFound_Test()
    {
        using var transaction = _context.Database.BeginTransaction();

        try
        {
            await _userService.ChangePassword(Guid.NewGuid(), null);
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Assert.AreEqual(ex.Message, ErrorCodes.UserNotFound.GetEnumDescription());
        }
    }

    /// <summary>
    /// Registers user
    /// tries to change password, with null values instead of password
    /// checks if correct exception is thrown
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task ChangePassword_EmptyPassword_Test()
    {
        using var transaction = _context.Database.BeginTransaction();

        var model = new UserPasswordModel
        {
            ConfirmPassword = null,
            Password = null
        };

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
            await _userService.ChangePassword(user.Id, model);
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Assert.AreEqual(ex.Message, ErrorCodes.EmptyPassword.GetEnumDescription());
        }
    }

    /// <summary>
    /// Registers user
    /// changes password
    /// checks if correct response is returned
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task ChangePassword_Success_Test()
    {
        using var transaction = _context.Database.BeginTransaction();
        NoContentResult result = new NoContentResult();

        var model = new UserPasswordModel
        {
            ConfirmPassword = "111111111111111",
            Password = "111111111111111"
        };

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
            var response = await _userService.ChangePassword(user.Id, model);
            result = response as NoContentResult;

            Assert.AreEqual(result.StatusCode, StatusCodes.Status204NoContent);
            transaction.Rollback();
        }
        catch
        {
            transaction.Rollback();
        }
    }

    /// <summary>
    /// Registers new user
    /// updates user information
    /// checks if correct response is returned
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task UpdateUser_Success_Test()
    {
        using var transaction = _context.Database.BeginTransaction();
        NoContentResult result = new NoContentResult();

        var model = new UserUpdateModel
        {
            Email = Faker.Internet.Email(),
            FirstName = Faker.Name.First(),
            Image = null,
            LastName = Faker.Name.Last(),
            Username = Faker.Internet.UserName()
        };

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
            var response = await _userService.UpdateUser(user.Id, model);
            result = response as NoContentResult;

            Assert.AreEqual(result.StatusCode, StatusCodes.Status204NoContent);
            transaction.Rollback();
        }
        catch
        {
            transaction.Rollback();
        }
    }

    /// <summary>
    /// Tries to update user with random user id
    /// checks if correct exception is thrown
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task UpdateUser_UserNotFound_Test()
    {
        using var transaction = _context.Database.BeginTransaction();

        try
        {
            await _userService.UpdateUser(Guid.NewGuid(), null);
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Assert.AreEqual(ex.Message, ErrorCodes.UserNotFound.GetEnumDescription());
        }
    }

    /// <summary>
    /// Registers 3 different users
    /// returns all users
    /// checks if returned user count is equal to 3
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task GetAllUsers_Success_Test()
    {
        using var transaction = _context.Database.BeginTransaction();

        var user = new User
        {
            Email = Faker.Internet.Email(),
            FirstName = Faker.Name.First(),
            LastName = Faker.Name.Last(),
            Password = Faker.Phone.Number(),
            Username = Faker.Internet.UserName()
        };

        try
        {
            user.Id = Guid.NewGuid();
            user.Username = Faker.Internet.UserName();
            await _userService.RegisterUser(user);

            user.Id = Guid.NewGuid();
            user.Username = Faker.Internet.UserName();
            await _userService.RegisterUser(user);

            user.Id = Guid.NewGuid();
            user.Username = Faker.Internet.UserName();
            await _userService.RegisterUser(user);

            var users = await _userService.GetAllUsers();

            Assert.AreEqual(users.Count, 3);
            transaction.Rollback();

        }
        catch
        {
            transaction.Rollback();
        }
    }
}