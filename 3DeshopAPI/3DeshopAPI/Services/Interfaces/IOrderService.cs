using _3DeshopAPI.Models.Order;
using Domain.Order;

namespace _3DeshopAPI.Services.Interfaces
{
    public interface IOrderService
    {
        public Task<List<Order>> GetOrders();
        public Task<Order> GetOrder(Guid id);
        public Task<OrderDisplayModel> GetDisplayOrder(Guid id);
        public Task<Order> PostOrder(OrderUploadModel model);
        public Task<List<Offer>> GetOffers();
        public Task<Offer> GetOffer(Guid id);
        public Task<Offer> PostOffer(OfferUploadModel model);
        public Task<List<Offer>> GetOrderOffers(Guid orderId);
        public Task<Job> ApproveOffer(Guid userId, Guid offerId, Guid orderId);
        public Task<List<Order>> GetInactiveOrders();
    }
}
