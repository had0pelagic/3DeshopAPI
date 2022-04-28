using _3DeshopAPI.Exceptions;
using _3DeshopAPI.Extensions;
using _3DeshopAPI.Services;
using _3DeshopAPI.Services.Interfaces;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace _3DeshopAPITest
{
    public class BalanceServiceUnitTest
    {
        private IBalanceService _balanceService;
        private Context _context;

        [SetUp]
        public void Setup()
        {
            var dbContextOptions = new DbContextOptionsBuilder<Context>()
                .UseSqlServer("Server=localhost\\SQLEXPRESS;Database=master;Trusted_Connection=True;");
            _context = new Context(dbContextOptions.Options);
            _context.Database.EnsureCreated();

            _balanceService = new BalanceService(_context);
        }

        [Test]
        public async Task GetUserBalanceTest()
        {
            //arrange
            //act
            var balance = await _balanceService.GetUserBalance(new Guid("274264F5-186E-4F42-CFD3-08DA05888877"));
            //assert
            Assert.IsNotNull(balance);
        }

        [Test]
        public async Task GetUserBalanceIsUserExistsTest()
        {
            //arrange
            //act
            var ex = Assert.ThrowsAsync<InvalidClientOperationException>(
                async () => await _balanceService.GetUserBalance(new Guid("274264F5-186E-4F42-CFD3-08DA05881877")));
            var errorCode = ErrorCodes.UserNotFound.GetEnumDescription();
            //assert
            Assert.That(ex.Message, Is.EqualTo(errorCode));
        }
    }
}
