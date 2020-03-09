using System;
using System.ComponentModel.DataAnnotations;

namespace DeliverIt.ViewModels.Delivery
{
    public class CreateDeliveryViewModel
    {
        [Required]
        public int RecipientId { get; set; }
        [Required]
        public long OrderId { get; set; }
        [Required]
        public int PartnerId { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
    }
}
