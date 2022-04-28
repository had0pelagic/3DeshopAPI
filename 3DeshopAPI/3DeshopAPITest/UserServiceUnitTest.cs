using _3DeshopAPI.Models.Settings;
using _3DeshopAPI.Services;
using _3DeshopAPI.Services.Interfaces;
using AutoMapper;
using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _3DeshopAPITest
{
    public class UserServiceUnitTest
    {
        private IUserService _userService;
        private Mock<IMapper> _mapper;
        private Mock<IHttpContextAccessor> _contextAccessor;
        private Mock<IBalanceService> _balanceService;
        private Mock<IOptions<DefaultFileSettings>> _fileSettings;
        private Context _context;


        [SetUp]
        public void Setup()
        {
            var mapperMock = new Mock<IMapper>();
            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            var balanceServiceMock = new Mock<IBalanceService>();
            var fileSettingsMock = new Mock<IOptions<DefaultFileSettings>>();

            var dbContextOptions = new DbContextOptionsBuilder<Context>()
                .UseSqlServer("Server=localhost\\SQLEXPRESS;Database=master;Trusted_Connection=True;");
            _context = new Context(dbContextOptions.Options);
            _context.Database.EnsureCreated();

            _userService = new UserService(
                mapperMock.Object,
                contextAccessorMock.Object,
                _context,
                balanceServiceMock.Object,
                fileSettingsMock.Object);
        }

        [Test]
        public async Task GetAllUsersTest()
        {
            //arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.GetAllUsers())
             .ReturnsAsync(new List<User> { new User() { Email = "a@", Username = "b" } });
            //act
            var item = await _userService.GetAllUsers();
            //assert
            Assert.IsNotEmpty(item);
        }

        //[Test]
        //public async Task GetUserTest()
        //{
        //    //arrange
        //    var userServiceMock = new Mock<IUserService>();
        //    userServiceMock.Setup(x => x.GetAllUsers())
        //     .ReturnsAsync(new List<User> { new User() { Email = "a@", Username = "b" } });
        //    //act
        //    var item = await _userService.GetUser();
        //    //assert


        //    foreach (var i in item)
        //    {
        //        System.Console.WriteLine(i.Username);
        //    }

        //    Assert.IsNotEmpty(item);
        //}
    }
}