using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeliveryIt.Models;
using DeliveryIt.ViewModels.User;

namespace DeliveryIt.ViewModels.Delivery
{
    public class DeliveryViewModel
    {
        public int Id { get; set; }
        public DeliveryStatus Status { get; set; }
        public AccessWindow AccessWindow { get; set; }
        public UserViewModel Recipient { get; set; }
        public OrderViewModel Order { get; set; }

    }
}
