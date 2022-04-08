using _3DeshopAPI.Models.Order;
using Domain.Order;

namespace _3DeshopAPI.Services.Interfaces
{
    public interface IOrderService
    {
        public Task<List<Order>> GetOrders();
        public Task<List<Order>> GetUserOrders(Guid id);
        public Task<Order> GetOrder(Guid id);
        public Task<OrderDisplayModel> GetDisplayOrder(Guid id);
        public Task<Order> PostOrder(OrderUploadModel model);
        public Task<Order> RemoveOrder(Guid userId, Guid orderId);
        public Task<Job> SetJobProgress(JobProgressModel model);
        public Task<Job> SetJobCompletion(JobCompletionModel model);
        public Task<Job> WorkerAbandonJob(AbandonJobModel model);
        public Task<List<JobProgress>> GetJobProgress(Guid userId, Guid orderId);
        public Task<List<Offer>> GetOffers();
        public Task<Offer> GetOffer(Guid id);
        public Task<Offer> PostOffer(OfferUploadModel model);
        public Task<List<Offer>> GetOrderOffers(Guid orderId);
        public Task<Job> AcceptOffer(Guid userId, Guid offerId, Guid orderId);
        public Task<Offer> DeclineOffer(Guid userId, Guid offerId);
        public Task<List<InactiveOrderDisplayModel>> GetInactiveOrders();
        public Task<List<Job>> GetUserJobs(Guid id);
        public Task<Order> ApproveOrder(Guid orderId, Guid userId);
        public Task<bool?> IsOrderJobActive(Guid orderId);
        public Task<Job> RequestJobChanges(Guid jobId);
        public Task<bool> IsOrderOwner(Guid userId, Guid orderId);
    }
}
