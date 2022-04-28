using _3DeshopAPI.Controllers;
using _3DeshopAPI.Models.Balance;
using _3DeshopAPI.Services.Interfaces;
using AutoMapper;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;

namespace _3DeshopAPITest
{
    public class BalanceControllerUnitTest
    {
        private BalanceController _balanceController;

        [SetUp]
        public void Setup()
        {
            var balanceServiceMock = new Mock<IBalanceService>();
            var mapperMock = new Mock<IMapper>();

            balanceServiceMock.Setup(x => x.GetUserBalance(It.IsAny<Guid>()))
                .ReturnsAsync(new UserBalanceModel() { Balance = 100 });

            _balanceController = new BalanceController(mapperMock.Object, balanceServiceMock.Object);
        }

        [Test]
        public async Task GetUserBalanceTest()
        {
            //arrange
            //act
            var balance = await _balanceController.GetUserBalance(It.IsAny<Guid>());

            var okResult = balance.Result as OkObjectResult;
            var value = okResult.Value as UserBalanceModel;
            //assert
            Assert.IsNotNull(value.Balance);
            Assert.AreEqual(value.Balance, 100);
        }
    }
}
