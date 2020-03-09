using DeliverIt.Data;
using DeliverIt.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverIt.Services
{
    public class DeliveryService : IDeliveryService
    {

        private readonly DeliverItContext db;
        public DeliveryService(DeliverItContext context)
        {
            this.db = context;
        }

        public async Task<IList<Delivery>> GetAllDeliveries()
        {
            return await db.Deliveries.ToListAsync<Delivery>();
        }
        public async Task<bool> DeliveryExists(int id)
        {
            return await db.Deliveries.AnyAsync(x => x.Id == id);
        }
        public async Task<Delivery> RemoveDelivery(int id)
        {
            var delivery = await this.GetDeliveryById(id);
            db.Deliveries.Remove(delivery);
            await db.SaveChangesAsync();
            return delivery;
        }
        public async Task<Delivery> GetDeliveryById(int id)
        {
            return await db.Deliveries.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<Delivery> CreateDelivery(Delivery delivery)
        {
            db.Deliveries.Add(delivery);
            await db.SaveChangesAsync();
            return delivery;
        }
        public async Task<Delivery> UpdateDelivery(Delivery delivery)
        {
            db.Deliveries.Update(delivery);
            await db.SaveChangesAsync();
            return delivery;
        }

        public async Task<bool> DeliveryExists(long orderId)
        {
            return await db.Deliveries.AnyAsync(x => x.OrderId == orderId);
        }
    }
}
