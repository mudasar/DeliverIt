using AutoMapper;
using DeliverIt.Models;
using DeliverIt.ViewModels.Delivery;
using DeliverIt.ViewModels.Partner;
using DeliverIt.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverIt.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CreateUserViewModel, User>();
            CreateMap<UserRegisterViewModel, User>();
            CreateMap<UpdateUserViewModel, User>();
            CreateMap<User, UserViewModel>().ForMember(dest => dest.PhoneNumber, src => src.MapFrom(m => m.Phone));

            CreateMap<CreatePartnerViewModel, Partner>();
            CreateMap<UpdatePartnerViewModel, Partner>();
            CreateMap<Partner, PartnerViewModel>();


            CreateMap<CreateDeliveryViewModel, Delivery>()
                .ForMember(dest => dest.SenderId, src => src.MapFrom(m => m.PartnerId))
                .ForMember(dest => dest.AccessWindow, src => src.MapFrom(m => new AccessWindow
                {
                    StartTime = m.StartTime,
                    EndTime = m.EndTime
                }));
            CreateMap<UpdateDeliveryViewModel, Delivery>();
            CreateMap<AccessWindow, AccessWindowViewModel>();
            CreateMap<Delivery, OrderViewModel>()
                .ForMember(destinationMember: dest => dest.OrderNumber, src => src.MapFrom(m => m.OrderId.ToString()))
                .ForMember(dest => dest.Sender, src => src.MapFrom(m => m.Sender.Name));

            //CreateMap<UpdateDeliveryViewModel, AccessWindow>()
            //    .ForMember(dest => dest.StartTime, src => src.MapFrom(m => m.StartTime))
            //    .ForMember(dest => dest.EndTime, src => src.MapFrom(m => m.EndTime));

            CreateMap<Delivery, DeliveryViewModel>()
                .ForMember(dest => dest.Status, src => src.MapFrom(m => Enum.GetName(typeof(DeliveryStatus), m.Status)))
                .ForMember(dest => dest.Order, src => src.MapFrom(m => new OrderViewModel
                {
                    OrderNumber = m.OrderId.ToString(),
                    Sender = m.Sender.Name
                }));
        }
    }
}
