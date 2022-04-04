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
        public Task<List<Order>> GetInactiveOrders();
        public Task<List<Job>> GetUserJobs(Guid id);
    }
}
