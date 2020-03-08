using DeliveryIt.Models;
using System;

namespace DeliveryIt.ViewModels.Delivery
{
    public class UpdateDeliveryViewModel
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public DeliveryStatus Status { get; set; }
        public DateTime StartTime { get;  set; }
        public DateTime EndTime { get; set; }
    }
}