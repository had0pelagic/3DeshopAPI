using _3DeshopAPI.Controllers;
using _3DeshopAPI.Exceptions;
using _3DeshopAPI.Extensions;
using _3DeshopAPI.Models.Balance;
using _3DeshopAPI.Services.Interfaces;
using AutoMapper;
using Domain.Balance;
using Domain.Product;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace _3DeshopAPITest
{
    public class BalanceControllerUnitTest
    {
        private BalanceController _balanceController;
        private Mock<IBalanceService> _balanceServiceMock;
        private Mock<IMapper> _mapperMock;

        [SetUp]
        public void Setup()
        {
            _balanceServiceMock = new Mock<IBalanceService>();
            _mapperMock = new Mock<IMapper>();
            _balanceController = new BalanceController(_mapperMock.Object, _balanceServiceMock.Object);
        }

        [Test]
        public async Task GetUserBalance_NotNull_Test()
        {
            //arrange
            _balanceServiceMock.Setup(x => x.GetUserBalance(It.IsAny<Guid>()))
                .ReturnsAsync(new UserBalanceModel() { Balance = 100 });

            //act
            var balance = await _balanceController.GetUserBalance(It.IsAny<Guid>());

            var okResult = balance.Result as OkObjectResult;
            var value = okResult.Value as UserBalanceModel;

            //assert
            Assert.IsNotNull(value.Balance);
        }

        [Test]
        public async Task GetUserBalance_BalanceIs100_Test()
        {
            //arrange
            _balanceServiceMock.Setup(x => x.GetUserBalance(It.IsAny<Guid>()))
                .ReturnsAsync(new UserBalanceModel() { Balance = 100 });

            //act
            var balance = await _balanceController.GetUserBalance(It.IsAny<Guid>());

            var okResult = balance.Result as OkObjectResult;
            var value = okResult.Value as UserBalanceModel;

            //assert
            Assert.AreEqual(value.Balance, 100);
        }

        [Test]
        public async Task BalanceTopUp_IsNotNull_Test()
        {
            //arrange
            var balanceHistoryModel = new BalanceHistory()
            {
                Balance = 100,
                From = new Domain.User(),
                To = new Domain.User() { Id = It.IsAny<Guid>() },
                Id = It.IsAny<Guid>(),
                IsPending = false,
                IsTopUp = true,
                LastTime = new DateTime(),
                Order = null,
                Product = null
            };

            var topUpModel = new TopUpModel()
            {
                Amount = 100,
                UserId = It.IsAny<Guid>()
            };

            _balanceServiceMock.Setup(x => x.BalanceTopUp(topUpModel))
                .ReturnsAsync(balanceHistoryModel);

            //act
            var balance = await _balanceController.BalanceTopUp(topUpModel);

            //assert
            Assert.IsNotNull(balance);
        }

        [Test]
        public async Task BalanceTopUp_IsBalance100_Test()
        {
            //arrange
            var balanceHistoryModel = new BalanceHistory()
            {
                Balance = 100,
                From = new Domain.User(),
                To = new Domain.User() { Id = It.IsAny<Guid>() },
                Id = It.IsAny<Guid>(),
                IsPending = false,
                IsTopUp = true,
                LastTime = new DateTime(),
                Order = null,
                Product = null
            };

            var topUpModel = new TopUpModel()
            {
                Amount = 100,
                UserId = It.IsAny<Guid>()
            };

            _balanceServiceMock.Setup(x => x.BalanceTopUp(topUpModel))
                .ReturnsAsync(balanceHistoryModel);

            //act
            var balance = await _balanceController.BalanceTopUp(topUpModel);

            //assert
            Assert.AreEqual(balance.Balance, 100);
        }

        [Test]
        public async Task PayForProduct_IsNotNull_Test()
        {
            //arrange
            var payForProductModel = new PayForProductModel()
            {
                ProductId = It.IsAny<Guid>(),
                UserId = It.IsAny<Guid>(),
            };

            var balanceHistoryModel = new BalanceHistory()
            {
                Balance = It.IsAny<double>(),
                From = new Domain.User(),
                To = new Domain.User() { Id = It.IsAny<Guid>() },
                Id = It.IsAny<Guid>(),
                IsPending = false,
                IsTopUp = true,
                LastTime = new DateTime(),
                Order = null,
                Product = null
            };

            _balanceServiceMock.Setup(x => x.PayForProduct(payForProductModel))
                .ReturnsAsync(balanceHistoryModel);

            //act
            var balance = await _balanceController.PayForProduct(payForProductModel);

            //assert
            Assert.IsNotNull(balance);
        }

        [Test]
        public async Task PayForProduct_IsCorrectProduct_Test()
        {
            //arrange
            var payForProductModel = new PayForProductModel()
            {
                ProductId = new Guid("8A65D274-D463-4D6B-1FBC-08DA34134566"),
                UserId = It.IsAny<Guid>(),
            };

            var balanceHistoryModel = new BalanceHistory()
            {
                Balance = It.IsAny<double>(),
                From = new Domain.User(),
                To = new Domain.User() { Id = It.IsAny<Guid>() },
                Id = It.IsAny<Guid>(),
                IsPending = false,
                IsTopUp = true,
                LastTime = new DateTime(),
                Order = null,
                Product = new Product() { Id = new Guid("8A65D274-D463-4D6B-1FBC-08DA34134566") }
            };

            _balanceServiceMock.Setup(x => x.PayForProduct(payForProductModel))
                .ReturnsAsync(balanceHistoryModel);

            //act
            var balance = await _balanceController.PayForProduct(payForProductModel);

            //assert
            Assert.AreEqual(balanceHistoryModel.Product.Id.ToString(), "8A65D274-D463-4D6B-1FBC-08DA34134566".ToLower());
        }

        [Test]
        public async Task PayForProduct_OwnerIsUnableToBuyProduct_Test()
        {
            //arrange
            var correctException = false;

            var payForProductModel = new PayForProductModel()
            {
                ProductId = new Guid("8A65D274-D463-4D6B-1FBC-08DA34134566"),
                UserId = new Guid("1A65D274-D463-4D6B-1FBC-08DA34134566"),
            };

            var balanceHistoryModel = new BalanceHistory()
            {
                Balance = It.IsAny<double>(),
                From = new Domain.User() { Id = new Guid("1A65D274-D463-4D6B-1FBC-08DA34134566") },
                To = new Domain.User() { Id = It.IsAny<Guid>() },
                Id = It.IsAny<Guid>(),
                IsPending = false,
                IsTopUp = true,
                LastTime = new DateTime(),
                Order = null,
                Product = new Product() { Id = new Guid("8A65D274-D463-4D6B-1FBC-08DA34134566") }
            };

            _balanceServiceMock.Setup(x => x.PayForProduct(payForProductModel))
                .Throws(new InvalidClientOperationException(ErrorCodes.OwnerUnableToBuyProduct));

            //act
            try
            {
                var balance = await _balanceController.PayForProduct(payForProductModel);
            }
            catch (Exception ex)
            {
                if (ex.Message == ErrorCodes.OwnerUnableToBuyProduct.GetEnumDescription())
                    correctException = true;
            }

            //assert
            Assert.IsTrue(correctException);
        }

        [Test]
        public async Task PayForProduct_BalanceNotEnough_Test()
        {
            //arrange
            var correctException = false;

            var payForProductModel = new PayForProductModel()
            {
                ProductId = new Guid("8A65D274-D463-4D6B-1FBC-08DA34134566"),
                UserId = new Guid("1A65D274-D463-4D6B-1FBC-08DA34134566"),
            };

            var balanceHistoryModel = new BalanceHistory()
            {
                Balance = It.IsAny<double>(),
                From = new Domain.User() { Id = new Guid("BA65D274-D463-4D6B-1FBC-08DA34134566") },
                To = new Domain.User() { Id = It.IsAny<Guid>() },
                Id = It.IsAny<Guid>(),
                IsPending = false,
                IsTopUp = true,
                LastTime = new DateTime(),
                Order = null,
                Product = new Product() { Id = new Guid("8A65D274-D463-4D6B-1FBC-08DA34134566"), About = new About() { Price = 99999999 } }
            };

            _balanceServiceMock.Setup(x => x.PayForProduct(payForProductModel))
                .Throws(new InvalidClientOperationException(ErrorCodes.NotEnoughBalance));

            //act
            try
            {
                var balance = await _balanceController.PayForProduct(payForProductModel);
            }
            catch (Exception ex)
            {
                if (ex.Message == ErrorCodes.NotEnoughBalance.GetEnumDescription())
                    correctException = true;
            }

            //assert
            Assert.IsTrue(correctException);
        }

    }
}
