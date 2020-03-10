using DeliverIt.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeliverIt.Services
{
    public interface IDeliveryService
    {
        Task<IList<Delivery>> GetAllDeliveries();
        Task<bool> DeliveryExists(int id);
        Task<bool> DeliveryExists(long orderId);
        Task<Delivery> RemoveDelivery(int id);
        Task<Delivery> GetDeliveryById(int id);


        Task<Delivery> CreateDelivery(Delivery delivery);
        Task<Delivery> UpdateDelivery(Delivery delivery);
        Task<IList<Delivery>> GetAllDeliveriesForPartner(int partnerId);
        Task<IList<Delivery>> GetAllDeliveriesForUser(int useId);
    }
}
