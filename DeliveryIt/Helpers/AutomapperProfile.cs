using AutoMapper;
using DeliveryIt.Models;
using DeliveryIt.ViewModels.Delivery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryIt.Helpers
{
    public class AutomapperProfile: Profile
    {
        public AutomapperProfile()
        {
            CreateMap<Delivery, DeliveryViewModel>();
        }
    }
}
