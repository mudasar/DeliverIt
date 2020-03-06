using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryIt.Models
{
    public class Delivery
    {
        public int Id { get; set; }
        public DeliveryStatus Status { get; set; }
        public User Recipient { get; set; }
        public Partner Sender { get; set; }
        public AccessWindow AccessWindow { get; set; }
        public long OrderId { get; set; }
    }
}
