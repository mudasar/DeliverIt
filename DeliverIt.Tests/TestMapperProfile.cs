using AutoMapper;
using DeliverIt.Models;
using DeliverIt.ViewModels.Delivery;
using DeliverIt.ViewModels.Partner;
using DeliverIt.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeliverIt.Tests
{
    public class TestMapperProfile : Profile
    {
        public TestMapperProfile()
        {
            CreateMap<CreateUserViewModel, User>();
            CreateMap<UpdateUserViewModel, User>();
            CreateMap<User, UserViewModel>();

            CreateMap<CreatePartnerViewModel, Partner>();
            CreateMap<UpdatePartnerViewModel, Partner>();
            CreateMap<Partner, PartnerViewModel>();

            CreateMap<CreateDeliveryViewModel, Delivery>();
            CreateMap<UpdateDeliveryViewModel, Delivery>();
            CreateMap<AccessWindow, AccessWindowViewModel>();
            CreateMap<Delivery, OrderViewModel>()
                .ForMember(destinationMember: dest => dest.OrderNumber, src => src.MapFrom(m => m.OrderId.ToString()))
                .ForMember(dest => dest.Sender, src => src.MapFrom(m => m.Sender.Name));
            //CreateMap<UpdateDeliveryViewModel, AccessWindow>()
            //    .ForMember(dest => dest.StartTime, src => src.MapFrom(m => m.StartTime))
            //    .ForMember(dest => dest.EndTime, src => src.MapFrom(m => m.EndTime));
            CreateMap<Delivery, DeliveryViewModel>();
        }
    }
}
