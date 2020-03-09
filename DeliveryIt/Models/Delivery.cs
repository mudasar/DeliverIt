using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverIt.Models
{
    public class Delivery
    {
        public int Id { get; set; }

        [Required]
        public DeliveryStatus Status { get; set; }

        
        public int RecipientId { get; set; }
        public virtual User Recipient { get; set; }

        
        public int SenderId { get; set; }
        public virtual Partner Sender { get; set; }

        
        public int AccessWindowId { get; set; }
        
        public virtual AccessWindow AccessWindow { get; set; }

        [Required]
        public long OrderId { get; set; }
    }
}
