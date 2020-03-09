using DeliverIt.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace DeliverIt.ViewModels.Delivery
{
    public class UpdateDeliveryViewModel
    {
        public int Id { get; set; }
       
        public DeliveryStatus Status { get; set; }
        [Required]
        public DateTime StartTime { get;  set; }
        [Required]
        public DateTime EndTime { get; set; }
    }
}