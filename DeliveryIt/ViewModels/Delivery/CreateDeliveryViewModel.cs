using System;

namespace DeliveryIt.ViewModels.Delivery
{
    public class CreateDeliveryViewModel
    {
        public int RecipientId { get; set; }
        public long OrderId { get; set; }

        public int PartnerId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
