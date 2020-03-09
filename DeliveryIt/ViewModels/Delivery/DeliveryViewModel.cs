using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeliverIt.Models;
using DeliverIt.ViewModels.User;

namespace DeliverIt.ViewModels.Delivery
{
    public class DeliveryViewModel
    {
        public int Id { get; set; }
        public DeliveryStatus Status { get; set; }
        public AccessWindowViewModel AccessWindow { get; set; }
        public UserViewModel Recipient { get; set; }
        public OrderViewModel Order { get; set; }

    }
}
