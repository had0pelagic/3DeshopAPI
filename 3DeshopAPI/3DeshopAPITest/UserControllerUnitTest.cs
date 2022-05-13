using _3DeshopAPI.Auth.Interfaces;
using _3DeshopAPI.Controllers;
using _3DeshopAPI.Exceptions;
using _3DeshopAPI.Extensions;
using _3DeshopAPI.Models.Balance;
using _3DeshopAPI.Models.Settings;
using _3DeshopAPI.Models.User;
using _3DeshopAPI.Services;
using _3DeshopAPI.Services.Interfaces;
using AutoMapper;
using Domain;
using Domain.Balance;
using Domain.Product;
using Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _3DeshopAPITest
{
    public class UserControllerUnitTest
    {
        private IUserService _userService;
        private Mock<IUserService> _userServiceMock;

        private Mock<IAuthService> _authServiceMock;
        private Mock<IBalanceService> _balanceServiceMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IConfiguration> _configMock;

        private Mock<Context> _contextMock;
        private Mock<IHttpContextAccessor> _contextAccessorMock;
        private Mock<DefaultFileSettings> _fileSettingsMock;

        [SetUp]
        public void Setup()
        {
            //_userServiceMock = new Mock<IUserService>();
            //_authServiceMock = new Mock<IAuthService>();
            //_balanceServiceMock = new Mock<IBalanceService>();
            //_mapperMock = new Mock<IMapper>();
            //_configMock = new Mock<IConfiguration>();
            //_fileSettingsMock = new Mock<DefaultFileSettings>();

            //_userService = new UserService(_mapperMock.Object, _contextAccessorMock.Object, _contextMock.Object, _balanceServiceMock.Object, _fileSettingsMock);
        }

        [Test]
        public async Task GetUserBalance_NotNull_Test()
        {
            //arrange
            //var userListNotMapped = new List<User>() {
            //    new User() {
            //        Email = "aaaaa@",
            //        FirstName = "jooooo",
            //        LastName = "eeeee",
            //        Image = null,
            //        Username = "joeeeee1" } };

            //var userList = new List<UserModel>() {
            //    new UserModel() {
            //        Email = "aaaaa@",
            //        FirstName = "jooooo",
            //        LastName = "eeeee",
            //        Image = null,
            //        Username = "joeeeee1" } };

            //var b = _userServiceMock.Setup(x => x.GetAllUsers()).ReturnsAsync(userListNotMapped);

            ////act
            //var users = await _userController.GetUsers();

            //var okResult = users.Result as OkObjectResult;
            //var value = okResult.Value.GetType().GetField("_source");

            ////assert
            //Assert.IsNotNull(users);
            //Assert.AreEqual(users, userListNotMapped);
        }
    }
}
