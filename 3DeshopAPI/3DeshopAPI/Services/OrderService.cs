using _3DeshopAPI.Exceptions;
using _3DeshopAPI.Models.Order;
using _3DeshopAPI.Models.Product;
using _3DeshopAPI.Services.Interfaces;
using AutoMapper;
using Domain.Order;
using Domain.Product;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace _3DeshopAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly ILogger<ProductService> _logger;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly Context _context;

        public OrderService(ILogger<ProductService> logger, IMapper mapper, IUserService userService, Context context)
        {
            _logger = logger;
            _mapper = mapper;
            _userService = userService;
            _context = context;
        }

        /// <summary>
        /// Returns all orders
        /// </summary>
        /// <returns></returns>
        public async Task<List<Order>> GetOrders()
        {
            var orders = await _context.Orders.ToListAsync();

            return orders;
        }

        /// <summary>
        /// Returns all inactive orders
        /// </summary>
        /// <returns></returns>
        public async Task<List<Order>> GetInactiveOrders()
        {
            var jobs = await GetJobs();
            var activeJobIds = jobs
                .Select(x => x.Order.Id)
                .ToList();
            var orders = _context.Orders
                .Where(x => !activeJobIds.Contains(x.Id))
                .ToList();

            return orders;
        }

        /// <summary>
        /// Returns offer by given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Order?> GetOrder(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);

            return order;
        }

        /// <summary>
        /// Uploads new offer
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="InvalidClientOperationException"></exception>
        public async Task<Order> PostOrder(OrderUploadModel model)
        {
            var user = await _userService.GetUser(model.UserId);

            if (user == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.UserNotFound);
            }

            var order = _mapper.Map<Order>(model);

            await _context.Orders.AddAsync(order);
            await SetOrderImages(order, model.Images);
            await _context.SaveChangesAsync();

            return order;
        }

        /// <summary>
        /// Returns all offers
        /// </summary>
        /// <returns></returns>
        public async Task<List<Offer>> GetOffers()
        {
            var orders = await _context.Offers.ToListAsync();

            return orders;
        }

        /// <summary>
        /// Returns offer by given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Offer> GetOffer(Guid id)
        {
            var order = await _context.Offers.FindAsync(id);

            return order;
        }

        /// <summary>
        /// Uploads new offer
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="InvalidClientOperationException"></exception>
        public async Task<Offer> PostOffer(OfferUploadModel model)
        {
            var user = await _userService.GetUser(model.UserId);

            if (user == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.UserNotFound);
            }

            var order = await GetOrder(model.OrderId);

            if (order == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.OrderNotFound);
            }

            //check if order is being processed (approved)

            var offer = _mapper.Map<Offer>(model);
            await _context.Offers.AddAsync(offer);
            await _context.OrderOffers.AddAsync(new OrderOffers()
            {
                Offer = offer,
                Order = order
            });
            await _context.SaveChangesAsync();

            return offer;
        }

        /// <summary>
        /// Returns all offers associated with selected order
        /// </summary>
        /// <returns></returns>
        public async Task<List<Offer>> GetOrderOffers(Guid orderId)
        {
            var offers = await _context.OrderOffers
                .Include(x => x.Order)
                .Include(x => x.Offer)
                .Where(x => x.Order.Id == orderId)
                .Select(x => x.Offer)
                .ToListAsync();

            return offers;
        }

        /// <summary>
        /// Approves offer by creating new active job
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="offerId"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidClientOperationException"></exception>
        public async Task<Job> ApproveOffer(Guid userId, Guid offerId, Guid orderId)
        {
            var user = await _userService.GetUser(userId);

            if (user == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.UserNotFound);
            }

            var offer = await GetOffer(offerId);

            if (offer == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.OfferNotFound);
            }

            var order = await GetOrder(orderId);

            if (order == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.OrderNotFound);
            }

            var job = new Job()
            {
                Offer = offer,
                Order = order
            };

            await _context.Jobs.AddAsync(job);
            await _context.SaveChangesAsync();

            return job;
        }

        public async Task<Job> GetJob(Guid id)
        {
            var job = await _context.Jobs.FindAsync(id);

            return job;
        }

        public async Task<List<Job>> GetJobs()
        {
            var jobs = await _context.Jobs
                .Include(x => x.Order)
                .Include(x => x.Offer)
                .ToListAsync();

            return jobs;
        }

        /// <summary>
        /// Sets all order images
        /// </summary>
        /// <param name="order"></param>
        /// <param name="images"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task SetOrderImages(Order order, List<ImageModel> images)
        {
            try
            {
                foreach (var image in images)
                {
                    var newImage = _mapper.Map<Image>(image);
                    await _context.Images.AddAsync(newImage);
                    await _context.OrderImages.AddAsync(new OrderImages()
                    {
                        Image = newImage,
                        Order = order
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
