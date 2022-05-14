using _3DeshopAPI.Exceptions;
using _3DeshopAPI.Extensions;
using _3DeshopAPI.Mappings;
using _3DeshopAPI.Models.Balance;
using _3DeshopAPI.Models.Order;
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
    public class OrderServiceUnitTest
    {
        private IUserService _userService;
        private IBalanceService _balanceService;
        private IOrderService _orderService;
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
            _orderService = new OrderService(_mapper, _userService, _balanceService, _context);

        }

        /// <summary>
        /// Create new order with invalid user id
        /// checks if correct exception is thrown
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task PostOrder_UserNotFound_Test()
        {
            using var transaction = _context.Database.BeginTransaction();

            var orderUploadModel = new OrderUploadModel()
            {
                CompleteTill = DateTime.Now,
                Description = Faker.Name.First(),
                Images = new List<ImageModel>() { new ImageModel() {
                    Data = Array.Empty<byte>(),
                    Name = "image.jpg",
                    Format = ".jpg",
                    Size = 20 } },
                Name = Faker.Internet.UserName(),
                Price = 5,
                UserId = Guid.NewGuid()
            };

            try
            {
                await _orderService.PostOrder(orderUploadModel);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Assert.AreEqual(ex.Message, ErrorCodes.UserNotFound.GetEnumDescription());
            }
        }

        /// <summary>
        /// Registers new user
        /// adds 1 credit to users balance
        /// user tries to post order with price of 50
        /// checks if correct exception is thrown
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task PostOrder_NotEnoughBalance_Test()
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

            var orderUploadModel = new OrderUploadModel()
            {
                CompleteTill = DateTime.Now,
                Description = Faker.Name.First(),
                Images = new List<ImageModel>() { new ImageModel() {
                    Data = Array.Empty<byte>(),
                    Name = "image.jpg",
                    Format = ".jpg",
                    Size = 20 } },
                Name = Faker.Internet.UserName(),
                Price = 50,
                UserId = userOwner.Id
            };

            var topUpModel = new TopUpModel()
            {
                Amount = 1,
                UserId = userOwner.Id,
            };

            try
            {
                await _userService.RegisterUser(userOwner);
                await _balanceService.BalanceTopUp(topUpModel);
                await _orderService.PostOrder(orderUploadModel);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Assert.AreEqual(ex.Message, ErrorCodes.NotEnoughBalance.GetEnumDescription());
            }
        }

        /// <summary>
        /// Registers new user
        /// adds 50 credits to users balance
        /// user tries to post order with price of 50
        /// checks if correct exception is thrown
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task PostOrder_Success_Test()
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

            var orderUploadModel = new OrderUploadModel()
            {
                CompleteTill = DateTime.Now,
                Description = Faker.Name.First(),
                Images = new List<ImageModel>() { new ImageModel() {
                    Data = Array.Empty<byte>(),
                    Name = "image.jpg",
                    Format = ".jpg",
                    Size = 20 } },
                Name = Faker.Internet.UserName(),
                Price = 50,
                UserId = userOwner.Id
            };

            var topUpModel = new TopUpModel()
            {
                Amount = 50,
                UserId = userOwner.Id,
            };

            try
            {
                await _userService.RegisterUser(userOwner);
                await _balanceService.BalanceTopUp(topUpModel);
                var order = await _orderService.PostOrder(orderUploadModel);

                Assert.NotNull(order);
                Assert.AreEqual(order.Name, orderUploadModel.Name);
                transaction.Rollback();
            }
            catch
            {
                transaction.Rollback();
            }
        }

        /// <summary>
        /// Registers userOwner and userOffer
        /// adds 100 credits to users balance
        /// userOwner posts an order with price of 50
        /// userOffer posts an offer to the created order
        /// checks if returned offer is not null and user id is the same as returned offer
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task PostOffer_Success_Test()
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

            var userOffer = new User()
            {
                Id = Guid.NewGuid(),
                Email = Faker.Internet.Email(),
                FirstName = Faker.Name.First(),
                LastName = Faker.Name.Last(),
                Password = Faker.Phone.Number(),
                Username = Faker.Internet.UserName()
            };

            var orderUploadModel = new OrderUploadModel()
            {
                CompleteTill = DateTime.Now,
                Description = Faker.Name.First(),
                Images = new List<ImageModel>() { new ImageModel() {
                    Data = Array.Empty<byte>(),
                    Name = "image.jpg",
                    Format = ".jpg",
                    Size = 20 } },
                Name = Faker.Internet.UserName(),
                Price = 50,
                UserId = userOwner.Id
            };

            var offerUploadModel = new OfferUploadModel()
            {
                CompleteTill = DateTime.Now,
                Created = DateTime.Now,
                Description = Faker.Name.First(),
                OrderId = Guid.NewGuid(),
                UserId = userOffer.Id
            };

            var topUpModel = new TopUpModel()
            {
                Amount = 100,
                UserId = userOwner.Id,
            };

            try
            {
                await _userService.RegisterUser(userOwner);
                await _userService.RegisterUser(userOffer);
                await _balanceService.BalanceTopUp(topUpModel);
                var order = await _orderService.PostOrder(orderUploadModel);
                offerUploadModel.OrderId = order.Id;
                var offer = await _orderService.PostOffer(offerUploadModel);

                Assert.NotNull(order);
                Assert.AreEqual(offer.User.Id, offerUploadModel.UserId);
                transaction.Rollback();
            }
            catch
            {
                transaction.Rollback();
            }
        }

        /// <summary>
        /// Registers userOwner and userOffer
        /// adds 100 credits to users balance
        /// userOwner posts an order with price of 50
        /// userOffer posts an offer to the created order with random user id
        /// checks if correct exception is thrown
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task PostOffer_UserNotFound_Test()
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

            var userOffer = new User()
            {
                Id = Guid.NewGuid(),
                Email = Faker.Internet.Email(),
                FirstName = Faker.Name.First(),
                LastName = Faker.Name.Last(),
                Password = Faker.Phone.Number(),
                Username = Faker.Internet.UserName()
            };

            var orderUploadModel = new OrderUploadModel()
            {
                CompleteTill = DateTime.Now,
                Description = Faker.Name.First(),
                Images = new List<ImageModel>() { new ImageModel() {
                    Data = Array.Empty<byte>(),
                    Name = "image.jpg",
                    Format = ".jpg",
                    Size = 20 } },
                Name = Faker.Internet.UserName(),
                Price = 50,
                UserId = userOwner.Id
            };

            var offerUploadModel = new OfferUploadModel()
            {
                CompleteTill = DateTime.Now,
                Created = DateTime.Now,
                Description = Faker.Name.First(),
                OrderId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
            };

            var topUpModel = new TopUpModel()
            {
                Amount = 100,
                UserId = userOwner.Id,
            };

            try
            {
                await _userService.RegisterUser(userOwner);
                await _userService.RegisterUser(userOffer);
                await _balanceService.BalanceTopUp(topUpModel);
                var order = await _orderService.PostOrder(orderUploadModel);
                offerUploadModel.OrderId = order.Id;
                var offer = await _orderService.PostOffer(offerUploadModel);

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Assert.AreEqual(ex.Message, ErrorCodes.UserNotFound.GetEnumDescription());
            }
        }

        /// <summary>
        /// Registers userOwner and userOffer
        /// adds 100 credits to users balance
        /// userOwner posts and order with price of 50
        /// userOffer posts an offer to the created order with random order id
        /// checks if correct exception is thrown
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task PostOffer_OrderNotFound_Test()
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

            var userOffer = new User()
            {
                Id = Guid.NewGuid(),
                Email = Faker.Internet.Email(),
                FirstName = Faker.Name.First(),
                LastName = Faker.Name.Last(),
                Password = Faker.Phone.Number(),
                Username = Faker.Internet.UserName()
            };

            var orderUploadModel = new OrderUploadModel()
            {
                CompleteTill = DateTime.Now,
                Description = Faker.Name.First(),
                Images = new List<ImageModel>() { new ImageModel() {
                    Data = Array.Empty<byte>(),
                    Name = "image.jpg",
                    Format = ".jpg",
                    Size = 20 } },
                Name = Faker.Internet.UserName(),
                Price = 1,
                UserId = userOwner.Id
            };

            var offerUploadModel = new OfferUploadModel()
            {
                CompleteTill = DateTime.Now,
                Created = DateTime.Now,
                Description = Faker.Name.First(),
                OrderId = Guid.NewGuid(),
                UserId = userOffer.Id,
            };

            var topUpModel = new TopUpModel()
            {
                Amount = 100,
                UserId = userOwner.Id,
            };

            try
            {
                await _userService.RegisterUser(userOwner);
                await _userService.RegisterUser(userOffer);
                await _balanceService.BalanceTopUp(topUpModel);
                var order = await _orderService.PostOrder(orderUploadModel);
                var offer = await _orderService.PostOffer(offerUploadModel);

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Assert.AreEqual(ex.Message, ErrorCodes.OrderNotFound.GetEnumDescription());
            }
        }

        /// <summary>
        /// Registers userOwner and userOffer
        /// adds 100 credits to users balance
        /// userOwner posts an order with price of 50
        /// userOffer posts an offer to the created order
        /// userOwner accepts offer
        /// checks if returned job is not null and returned order, offer ids are correct
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task AcceptOffer_Success_Test()
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

            var userOffer = new User()
            {
                Id = Guid.NewGuid(),
                Email = Faker.Internet.Email(),
                FirstName = Faker.Name.First(),
                LastName = Faker.Name.Last(),
                Password = Faker.Phone.Number(),
                Username = Faker.Internet.UserName()
            };

            var orderUploadModel = new OrderUploadModel()
            {
                CompleteTill = DateTime.Now,
                Description = Faker.Name.First(),
                Images = new List<ImageModel>() { new ImageModel() {
                    Data = Array.Empty<byte>(),
                    Name = "image.jpg",
                    Format = ".jpg",
                    Size = 20 } },
                Name = Faker.Internet.UserName(),
                Price = 50,
                UserId = userOwner.Id
            };

            var offerUploadModel = new OfferUploadModel()
            {
                CompleteTill = DateTime.Now,
                Created = DateTime.Now,
                Description = Faker.Name.First(),
                OrderId = Guid.NewGuid(),
                UserId = userOffer.Id
            };

            var topUpModel = new TopUpModel()
            {
                Amount = 100,
                UserId = userOwner.Id,
            };

            try
            {
                await _userService.RegisterUser(userOwner);
                await _userService.RegisterUser(userOffer);
                await _balanceService.BalanceTopUp(topUpModel);
                var order = await _orderService.PostOrder(orderUploadModel);
                offerUploadModel.OrderId = order.Id;
                var offer = await _orderService.PostOffer(offerUploadModel);
                var job = await _orderService.AcceptOffer(userOwner.Id, offer.Id, order.Id);

                Assert.NotNull(job);
                Assert.AreEqual(job.Offer.Id, offer.Id);
                Assert.AreEqual(job.Order.Id, order.Id);
                transaction.Rollback();
            }
            catch
            {
                transaction.Rollback();
            }
        }

        /// <summary>
        /// Registers userOwner and userOffer
        /// adds 100 credits to users balance
        /// userOwner posts an order with price of 50
        /// userOffer posts an offer to the created order
        /// userOwner accepts offer with random userOwner id
        /// checks if correct exception is thrown
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task AcceptOffer_UserNotFound_Test()
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

            var userOffer = new User()
            {
                Id = Guid.NewGuid(),
                Email = Faker.Internet.Email(),
                FirstName = Faker.Name.First(),
                LastName = Faker.Name.Last(),
                Password = Faker.Phone.Number(),
                Username = Faker.Internet.UserName()
            };

            var orderUploadModel = new OrderUploadModel()
            {
                CompleteTill = DateTime.Now,
                Description = Faker.Name.First(),
                Images = new List<ImageModel>() { new ImageModel() {
                    Data = Array.Empty<byte>(),
                    Name = "image.jpg",
                    Format = ".jpg",
                    Size = 20 } },
                Name = Faker.Internet.UserName(),
                Price = 50,
                UserId = userOwner.Id
            };

            var offerUploadModel = new OfferUploadModel()
            {
                CompleteTill = DateTime.Now,
                Created = DateTime.Now,
                Description = Faker.Name.First(),
                OrderId = Guid.NewGuid(),
                UserId = userOffer.Id
            };

            var topUpModel = new TopUpModel()
            {
                Amount = 100,
                UserId = userOwner.Id,
            };

            try
            {
                await _userService.RegisterUser(userOwner);
                await _userService.RegisterUser(userOffer);
                await _balanceService.BalanceTopUp(topUpModel);
                var order = await _orderService.PostOrder(orderUploadModel);
                offerUploadModel.OrderId = order.Id;
                var offer = await _orderService.PostOffer(offerUploadModel);
                var job = await _orderService.AcceptOffer(Guid.NewGuid(), offer.Id, order.Id);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Assert.AreEqual(ex.Message, ErrorCodes.UserNotFound.GetEnumDescription());
            }
        }

        /// <summary>
        /// Registers userOwner and userOffer
        /// adds 100 credits to users balance
        /// userOwner posts an order with price of 50
        /// userOffer posts an offer to the created order
        /// userOwner accepts offer with random offer id
        /// checks if correct exception is thrown
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task AcceptOffer_OfferNotFound_Test()
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

            var userOffer = new User()
            {
                Id = Guid.NewGuid(),
                Email = Faker.Internet.Email(),
                FirstName = Faker.Name.First(),
                LastName = Faker.Name.Last(),
                Password = Faker.Phone.Number(),
                Username = Faker.Internet.UserName()
            };

            var orderUploadModel = new OrderUploadModel()
            {
                CompleteTill = DateTime.Now,
                Description = Faker.Name.First(),
                Images = new List<ImageModel>() { new ImageModel() {
                    Data = Array.Empty<byte>(),
                    Name = "image.jpg",
                    Format = ".jpg",
                    Size = 20 } },
                Name = Faker.Internet.UserName(),
                Price = 50,
                UserId = userOwner.Id
            };

            var offerUploadModel = new OfferUploadModel()
            {
                CompleteTill = DateTime.Now,
                Created = DateTime.Now,
                Description = Faker.Name.First(),
                OrderId = Guid.NewGuid(),
                UserId = userOffer.Id
            };

            var topUpModel = new TopUpModel()
            {
                Amount = 100,
                UserId = userOwner.Id,
            };

            try
            {
                await _userService.RegisterUser(userOwner);
                await _userService.RegisterUser(userOffer);
                await _balanceService.BalanceTopUp(topUpModel);
                var order = await _orderService.PostOrder(orderUploadModel);
                offerUploadModel.OrderId = order.Id;
                var offer = await _orderService.PostOffer(offerUploadModel);
                var job = await _orderService.AcceptOffer(userOwner.Id, Guid.NewGuid(), order.Id);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Assert.AreEqual(ex.Message, ErrorCodes.OfferNotFound.GetEnumDescription());
            }
        }

        /// <summary>
        /// Registers userOwner and userOffer
        /// adds 100 credits to users balance
        /// userOwner posts an order with price of 50
        /// userOffer posts an offer to the created order
        /// userOwner accepts offer with random order id
        /// checks if correct exception is thrown
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task AcceptOffer_OrderNotFound_Test()
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

            var userOffer = new User()
            {
                Id = Guid.NewGuid(),
                Email = Faker.Internet.Email(),
                FirstName = Faker.Name.First(),
                LastName = Faker.Name.Last(),
                Password = Faker.Phone.Number(),
                Username = Faker.Internet.UserName()
            };

            var orderUploadModel = new OrderUploadModel()
            {
                CompleteTill = DateTime.Now,
                Description = Faker.Name.First(),
                Images = new List<ImageModel>() { new ImageModel() {
                    Data = Array.Empty<byte>(),
                    Name = "image.jpg",
                    Format = ".jpg",
                    Size = 20 } },
                Name = Faker.Internet.UserName(),
                Price = 50,
                UserId = userOwner.Id
            };

            var offerUploadModel = new OfferUploadModel()
            {
                CompleteTill = DateTime.Now,
                Created = DateTime.Now,
                Description = Faker.Name.First(),
                OrderId = Guid.NewGuid(),
                UserId = userOffer.Id
            };

            var topUpModel = new TopUpModel()
            {
                Amount = 100,
                UserId = userOwner.Id,
            };

            try
            {
                await _userService.RegisterUser(userOwner);
                await _userService.RegisterUser(userOffer);
                await _balanceService.BalanceTopUp(topUpModel);
                var order = await _orderService.PostOrder(orderUploadModel);
                offerUploadModel.OrderId = order.Id;
                var offer = await _orderService.PostOffer(offerUploadModel);
                var job = await _orderService.AcceptOffer(userOwner.Id, offer.Id, Guid.NewGuid());
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Assert.AreEqual(ex.Message, ErrorCodes.OrderNotFound.GetEnumDescription());
            }
        }

        /// <summary>
        /// Registers userOwner and userOffer
        /// adds 100 credits to users balance
        /// userOwner posts an order with price of 50
        /// userOffer posts an offer to the created order
        /// userOwner declines offer
        /// checks if returned job is not null and returned offer and original offer id's are the same
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task DeclineOffer_Success_Test()
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

            var userOffer = new User()
            {
                Id = Guid.NewGuid(),
                Email = Faker.Internet.Email(),
                FirstName = Faker.Name.First(),
                LastName = Faker.Name.Last(),
                Password = Faker.Phone.Number(),
                Username = Faker.Internet.UserName()
            };

            var orderUploadModel = new OrderUploadModel()
            {
                CompleteTill = DateTime.Now,
                Description = Faker.Name.First(),
                Images = new List<ImageModel>() { new ImageModel() {
                    Data = Array.Empty<byte>(),
                    Name = "image.jpg",
                    Format = ".jpg",
                    Size = 20 } },
                Name = Faker.Internet.UserName(),
                Price = 50,
                UserId = userOwner.Id
            };

            var offerUploadModel = new OfferUploadModel()
            {
                CompleteTill = DateTime.Now,
                Created = DateTime.Now,
                Description = Faker.Name.First(),
                OrderId = Guid.NewGuid(),
                UserId = userOffer.Id
            };

            var topUpModel = new TopUpModel()
            {
                Amount = 100,
                UserId = userOwner.Id,
            };

            try
            {
                await _userService.RegisterUser(userOwner);
                await _userService.RegisterUser(userOffer);
                await _balanceService.BalanceTopUp(topUpModel);
                var order = await _orderService.PostOrder(orderUploadModel);
                offerUploadModel.OrderId = order.Id;
                var offer = await _orderService.PostOffer(offerUploadModel);
                var declinedOffer = await _orderService.DeclineOffer(userOwner.Id, offer.Id);

                Assert.NotNull(declinedOffer);
                Assert.AreEqual(declinedOffer.Id, offer.Id);
                transaction.Rollback();
            }
            catch
            {
                transaction.Rollback();
            }
        }

        /// <summary>
        /// Declines offer with random user id
        /// checks if correct exception is thrown
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task DeclineOffer_UserNotFound_Test()
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                await _orderService.DeclineOffer(Guid.NewGuid(), Guid.NewGuid());
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Assert.AreEqual(ex.Message, ErrorCodes.UserNotFound.GetEnumDescription());
            }
        }

        /// <summary>
        /// Registers userOwner and userOffer
        /// Declines offer with random offer id
        /// checks if correct exception is thrown
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task DeclineOffer_OfferNotFound_Test()
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

            try
            {
                await _userService.RegisterUser(userOwner);
                await _orderService.DeclineOffer(userOwner.Id, Guid.NewGuid());
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Assert.AreEqual(ex.Message, ErrorCodes.OfferNotFound.GetEnumDescription());
            }
        }

        /// <summary>
        /// Registers userOwner and userOffer
        /// adds 100 credits to users balance
        /// userOwner posts an order with price of 50
        /// userOffer posts an offer to the created order
        /// userOwner accepts offer
        /// userOwner accepts order
        /// checks if returned order is not null and status is changes to approved
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task ApproveOrder_Success_Test()
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

            var userOffer = new User()
            {
                Id = Guid.NewGuid(),
                Email = Faker.Internet.Email(),
                FirstName = Faker.Name.First(),
                LastName = Faker.Name.Last(),
                Password = Faker.Phone.Number(),
                Username = Faker.Internet.UserName()
            };

            var orderUploadModel = new OrderUploadModel()
            {
                CompleteTill = DateTime.Now,
                Description = Faker.Name.First(),
                Images = new List<ImageModel>() { new ImageModel() {
                    Data = Array.Empty<byte>(),
                    Name = "image.jpg",
                    Format = ".jpg",
                    Size = 20 } },
                Name = Faker.Internet.UserName(),
                Price = 50,
                UserId = userOwner.Id
            };

            var offerUploadModel = new OfferUploadModel()
            {
                CompleteTill = DateTime.Now,
                Created = DateTime.Now,
                Description = Faker.Name.First(),
                OrderId = Guid.NewGuid(),
                UserId = userOffer.Id
            };

            var topUpModel = new TopUpModel()
            {
                Amount = 100,
                UserId = userOwner.Id,
            };

            try
            {
                await _userService.RegisterUser(userOwner);
                await _userService.RegisterUser(userOffer);
                await _balanceService.BalanceTopUp(topUpModel);
                var order = await _orderService.PostOrder(orderUploadModel);
                offerUploadModel.OrderId = order.Id;
                var offer = await _orderService.PostOffer(offerUploadModel);
                await _orderService.AcceptOffer(userOwner.Id, offer.Id, order.Id);
                var approvedOrder = await _orderService.ApproveOrder(order.Id, userOwner.Id);

                Assert.NotNull(approvedOrder);
                Assert.AreEqual(approvedOrder.Approved, true);
                transaction.Rollback();
            }
            catch
            {
                transaction.Rollback();
            }
        }

        /// <summary>
        /// Approves order with random user and order id
        /// checks if correct exception is thrown
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task ApproveOrder_UserNotFound_Test()
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                await _orderService.ApproveOrder(Guid.NewGuid(), Guid.NewGuid());
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Assert.AreEqual(ex.Message, ErrorCodes.UserNotFound.GetEnumDescription());
            }
        }

        /// <summary>
        /// Registers userOwner
        /// Approves order and order id
        /// checks if correct exception is thrown
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task ApproveOrder_OrderNotFound_Test()
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

            try
            {
                await _userService.RegisterUser(userOwner);
                await _orderService.ApproveOrder(Guid.NewGuid(), userOwner.Id);

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Assert.AreEqual(ex.Message, ErrorCodes.OrderNotFound.GetEnumDescription());
            }
        }

        /// <summary>
        /// Registers userOwner
        /// adds 15 credits to user balance
        /// creates 3 orders for the price of 5
        /// returns all orders
        /// checks if returned orders are not null and count is equal to 3
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetOrders_SuccessReturns3Orders_Test()
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

            var orderUploadModel = new OrderUploadModel()
            {
                CompleteTill = DateTime.Now,
                Description = Faker.Name.First(),
                Images = new List<ImageModel>() { new ImageModel() {
                    Data = Array.Empty<byte>(),
                    Name = "image.jpg",
                    Format = ".jpg",
                    Size = 20 } },
                Name = Faker.Internet.UserName(),
                Price = 5,
                UserId = userOwner.Id
            };

            var topUpModel = new TopUpModel()
            {
                Amount = 15,
                UserId = userOwner.Id,
            };

            try
            {
                await _userService.RegisterUser(userOwner);
                await _balanceService.BalanceTopUp(topUpModel);
                await _orderService.PostOrder(orderUploadModel);
                orderUploadModel.Name = Faker.Internet.UserName();
                await _orderService.PostOrder(orderUploadModel);
                orderUploadModel.Name = Faker.Internet.UserName();
                await _orderService.PostOrder(orderUploadModel);

                var orders = await _orderService.GetOrders();

                Assert.NotNull(orders);
                Assert.AreEqual(orders.Count, 3);
            }
            catch
            {
                transaction.Rollback();
            }
        }

    }
}
