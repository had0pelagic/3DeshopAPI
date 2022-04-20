using _3DeshopAPI.Exceptions;
using _3DeshopAPI.Models.Balance;
using _3DeshopAPI.Models.Order;
using _3DeshopAPI.Models.Product;
using _3DeshopAPI.Models.User;
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
        private readonly IBalanceService _balanceService;
        private readonly Context _context;

        public OrderService(ILogger<ProductService> logger, IMapper mapper, IUserService userService, IBalanceService balanceService, Context context)
        {
            _logger = logger;
            _mapper = mapper;
            _userService = userService;
            _balanceService = balanceService;
            _context = context;
        }

        /// <summary>
        /// Returns all orders
        /// </summary>
        /// <returns></returns>
        public async Task<List<Order>> GetOrders()
        {
            var orders = await _context.Orders
                .Include(x => x.User)
                .ToListAsync();

            return orders;
        }

        /// <summary>
        /// Returns all orders associated with user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<Order>> GetUserOrders(Guid id)
        {
            var orders = await _context.Orders
                .Include(x => x.User)
                .Where(x => x.User.Id == id)
                .ToListAsync();

            return orders;
        }

        /// <summary>
        /// Returns all inactive orders
        /// </summary>
        /// <returns></returns>
        public async Task<List<InactiveOrderDisplayModel>> GetInactiveOrders()
        {
            var jobs = await GetJobs();
            var activeJobIds = jobs
                .Select(x => x.Order.Id)
                .ToList();
            var orders = _context.Orders
                .Include(x => x.User)
                .Where(x => !activeJobIds.Contains(x.Id))
                .ToList();

            return orders.Select(x => OrderToInactiveOrderDisplayModel(x).Result).ToList();
        }

        /// <summary>
        /// Returns order by given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Order?> GetOrder(Guid id)
        {
            var order = await _context.Orders
                .Include(x => x.User)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.OrderNotFound);
            }

            return order;
        }

        /// <summary>
        /// Returns OrderDisplayModel by given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="InvalidClientOperationException"></exception>
        public async Task<OrderDisplayModel?> GetDisplayOrder(Guid id)
        {
            var order = await _context.Orders
                .Include(x => x.User)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.OrderNotFound);
            }

            return await OrderToOrderDisplayModel(order);
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

            var isBalanceEnough = await _balanceService.IsBalanceEnough(model.UserId, model.Price);

            if (!isBalanceEnough)
            {
                throw new InvalidClientOperationException(ErrorCodes.NotEnoughBalance);
            }

            var order = _mapper.Map<Order>(model);
            order.Created = DateTime.UtcNow;
            order.User = user;
            _context.Entry(order).State = EntityState.Modified;

            await _context.Orders.AddAsync(order);
            await SetOrderImages(order, model.Images);
            await _context.SaveChangesAsync();

            return order;
        }

        /// <summary>
        /// Removes user order
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidClientOperationException"></exception>
        public async Task<Order> RemoveOrder(Guid userId, Guid orderId)
        {
            var user = await _userService.GetUser(userId);

            if (user == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.UserNotFound);
            }

            var order = await _context.Orders
                .Include(x => x.User)
                .Where(x => x.Id == orderId)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.OrderNotFound);
            }

            if (user.Id != order.User.Id)
            {
                throw new InvalidClientOperationException(ErrorCodes.UnauthorizedForAction);
            }

            var isActive = await IsOrderJobActive(order.Id);

            if (isActive == true)
            {
                throw new InvalidClientOperationException(ErrorCodes.CantRemoveOrderIsActive);
            }

            await _balanceService.RemoveBalanceHistoryByOrder(orderId);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return order;
        }

        /// <summary>
        /// Sets job progress and writes progress comment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="InvalidClientOperationException"></exception>
        public async Task<Job> SetJobProgress(JobProgressModel model)
        {
            var user = await _userService.GetUser(model.UserId);

            if (user == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.UserNotFound);
            }

            var order = await _context.Orders.FindAsync(model.OrderId);

            if (order == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.OrderNotFound);
            }

            var job = await GetJob(model.Id);

            if (job == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.JobNotFound);
            }

            job.Progress = model.Progress;
            _context.Entry(job).State = EntityState.Modified;

            var jobProgress = new JobProgress()
            {
                Created = DateTime.UtcNow,
                Description = model.Comment,
                JobId = model.Id,
                UserId = model.UserId,
                Progress = model.Progress
            };
            await _context.Progresses.AddAsync(jobProgress);
            await _context.SaveChangesAsync();

            return job;
        }

        /// <summary>
        /// Sets job progress and uploads given files
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="InvalidClientOperationException"></exception>
        public async Task<Job> SetJobCompletion(JobCompletionModel model)
        {
            var user = await _userService.GetUser(model.UserId);

            if (user == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.UserNotFound);
            }

            var order = await _context.Orders.FindAsync(model.OrderId);

            if (order == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.OrderNotFound);
            }

            var job = await GetJob(model.Id);

            if (job == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.JobNotFound);
            }

            job.Progress = model.Progress;
            job.NeedChanges = false;
            _context.Entry(job).State = EntityState.Modified;

            var jobProgress = new JobProgress()
            {
                Created = DateTime.UtcNow,
                Description = model.Comment,
                JobId = model.Id,
                UserId = model.UserId,
                Progress = model.Progress
            };
            await _context.Progresses.AddAsync(jobProgress);
            await SetOrderFiles(order, model.Files);
            await _context.SaveChangesAsync();

            return job;
        }

        /// <summary>
        /// Sets job status to inactive
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="InvalidClientOperationException"></exception>
        public async Task<Job> WorkerAbandonJob(AbandonJobModel model)
        {
            var user = await _userService.GetUser(model.UserId);

            if (user == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.UserNotFound);
            }

            var job = await _context.Jobs
                .Include(x => x.Offer)
                .Include(x => x.Offer.User)
                .FirstOrDefaultAsync(x => x.Id == model.JobId);

            if (job == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.JobNotFound);
            }

            if (job.Offer.User.Id != model.UserId)
            {
                throw new InvalidClientOperationException(ErrorCodes.UnauthorizedForAction);
            }

            //set to inactive or remove?
            job.Active = false;
            _context.Entry(job).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return job;
        }

        /// <summary>
        /// Returns all progresses associated with given job
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="InvalidClientOperationException"></exception>
        public async Task<List<JobProgressDisplayModel>> GetJobProgress(Guid userId, Guid orderId)
        {
            var user = await _userService.GetUser(userId);

            if (user == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.UserNotFound);
            }

            var order = await _context.Orders.FindAsync(orderId);

            if (order == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.OrderNotFound);
            }

            var jobs = await _context.Jobs
                .Include(x => x.Order)
                .ToListAsync();
            var job = jobs
                .Where(x => x.Order.Id == orderId)
                .FirstOrDefault();

            if (job == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.JobNotFound);
            }

            var progresses = await _context.Progresses
                .Where(x => x.JobId == job.Id)
                .ToListAsync();

            return progresses.Select(x => JobProgressToJobProgressDisplayModel(x).Result).ToList();

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
            var order = await _context.Offers
                .Include(x => x.User)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

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
            offer.Created = DateTime.UtcNow;
            offer.User = user;
            _context.Entry(offer).State = EntityState.Modified;

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
        public async Task<List<OfferDisplayModel>> GetOrderOffers(Guid orderId)
        {
            var offers = await _context.OrderOffers
                .Include(x => x.Order)
                .Include(x => x.Offer)
                .Include(x => x.Offer.User)
                .Where(x => x.Order.Id == orderId)
                .Select(x => _mapper.Map<OfferDisplayModel>(x.Offer))
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
        public async Task<Job> AcceptOffer(Guid userId, Guid offerId, Guid orderId)
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
                Order = order,
                Created = DateTime.UtcNow
            };

            await _context.Jobs.AddAsync(job);
            var orderModel = new PayForOrderModel()
            {
                OrderId = orderId,
                From = userId,
                To = offer.User.Id
            };
            await _balanceService.PayForOrder(orderModel);
            await _context.SaveChangesAsync();

            return job;
        }

        /// <summary>
        /// Declines offer, removes offer from database
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="offerId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidClientOperationException"></exception>
        public async Task<Offer> DeclineOffer(Guid userId, Guid offerId)
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

            _context.Offers.Remove(offer);
            await _context.SaveChangesAsync();

            return offer;
        }

        /// <summary>
        /// Approves order
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidClientOperationException"></exception>
        public async Task<Order> ApproveOrder(Guid orderId, Guid userId)
        {
            var user = await _userService.GetUser(userId);

            if (user == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.UserNotFound);
            }

            var order = await GetOrder(orderId);

            if (order == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.OrderNotFound);
            }

            order.Approved = true;
            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var workerId = await _context.Jobs
                .Include(x => x.Order)
                .Include(x => x.Offer)
                .Where(x => x.Order.Id == order.Id)
                .Select(x => x.Offer.User.Id)
                .FirstAsync();
            await _balanceService.PayForCompletedOrder(workerId, order.Id);

            return order;
        }

        /// <summary>
        /// Returns job by given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Job?> GetJob(Guid id)
        {
            var job = await _context.Jobs
                .Include(x => x.Offer)
                .FirstOrDefaultAsync(x => x.Id == id);

            return job;
        }

        /// <summary>
        /// Checks if order job is active
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<bool?> IsOrderJobActive(Guid orderId)
        {
            var job = await _context.Jobs
                .Include(x => x.Order)
                .FirstOrDefaultAsync(x => x.Order.Id == orderId);

            return job?.Active;
        }

        /// <summary>
        /// Creates new job progress and sets job as needed for changes
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidClientOperationException"></exception>
        public async Task<Job> RequestJobChanges(ChangeRequestModel model)
        {
            var job = await _context.Jobs
                .Include(x => x.Order)
                .Include(x => x.Offer)
                .Include(x => x.Offer.User)
                .Where(x => x.Order.Id == model.OrderId)
                .FirstOrDefaultAsync();

            if (job == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.JobNotFound);
            }

            var jobProgress = new JobProgress()
            {
                Created = DateTime.UtcNow,
                UserId = job.Offer.User.Id,
                Description = model.Description,
                JobId = job.Id,
                Progress = 0,
            };

            await _context.Progresses.AddAsync(jobProgress);

            job.NeedChanges = true;
            job.Progress = 0;
            _context.Entry(job).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return job;
        }

        /// <summary>
        /// Checks if user is the owner of the given order
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidClientOperationException"></exception>
        public async Task<bool> IsOrderOwner(Guid userId, Guid orderId)
        {
            var order = await GetOrder(orderId);

            if (order == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.OrderNotFound);
            }

            if (order.User.Id == userId)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns all existing jobs
        /// </summary>
        /// <returns></returns>
        public async Task<List<Job>> GetJobs()
        {
            var jobs = await _context.Jobs
                .Include(x => x.Order)
                .Include(x => x.Offer)
                .ToListAsync();

            return jobs;
        }

        /// <summary>
        /// Returns all existing jobs
        /// </summary>
        /// <returns></returns>
        public async Task<List<JobDisplayModel>> GetUserJobs(Guid id)
        {
            var jobs = await _context.Jobs
                .Include(x => x.Order)
                .Include(x => x.Order.User)
                .Include(x => x.Offer)
                .Include(x => x.Offer.User)
                .Select(x => _mapper.Map<JobDisplayModel>(x))
                .ToListAsync();
            var userJobs = jobs
                .Where(x => x.Offer.User.Id == id)
                .ToList();

            return userJobs;
        }

        /// <summary>
        /// Maps Order to OrderDisplayModel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<OrderDisplayModel> OrderToOrderDisplayModel(Order model)
        {
            return new OrderDisplayModel
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                User = _mapper.Map<UserDisplayModel>(model.User),
                CompleteTill = model.CompleteTill,
                Created = model.Created,
                Images = await GetOrderImages(model.Id),
                Approved = model.Approved
            };
        }

        /// <summary>
        /// Maps order to InactiveOrderDisplayModel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<InactiveOrderDisplayModel> OrderToInactiveOrderDisplayModel(Order model)
        {
            return new InactiveOrderDisplayModel
            {
                Id = model.Id,
                Approved = model.Approved,
                CompleteTill = model.CompleteTill,
                Created = model.Created,
                Description = model.Description,
                Name = model.Name,
                Price = model.Price,
                User = await _userService.GetDisplayUser(model.User.Id)
            };
        }

        /// <summary>
        /// Maps JobProgress to JobProgressDisplayModel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task<JobProgressDisplayModel> JobProgressToJobProgressDisplayModel(JobProgress model)
        {
            return new JobProgressDisplayModel
            {
                Id = model.Id,
                Created = model.Created,
                Description = model.Description,
                JobId = model.JobId,
                Progress = model.Progress,
                User = new UserDisplayModel()
                {
                    Id = model.UserId,
                    Username = await _userService.GetUsername(model.UserId)
                }
            };
        }
        /// <summary>
        /// Returns all images associated with order
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<List<ImageModel>> GetOrderImages(Guid id)
        {
            return await _context.OrderImages
                .Where(i => i.Order.Id == id)
                .Include(i => i.Image)
                .Select(i => _mapper.Map<ImageModel>(i.Image))
                .ToListAsync();
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

        /// <summary>
        /// Sets job files
        /// </summary>
        /// <param name="job"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task SetOrderFiles(Order order, List<FileModel> files)
        {
            try
            {
                foreach (var file in files)
                {
                    var newFile = _mapper.Map<Domain.Product.File>(file);
                    await _context.Files.AddAsync(newFile);
                    await _context.OrderFiles.AddAsync(new OrderFiles()
                    {
                        File = newFile,
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
