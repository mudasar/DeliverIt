using AutoMapper;
using DeliverIt.Controllers;
using DeliverIt.Data;
using DeliverIt.Models;
using DeliverIt.Services;
using DeliverIt.ViewModels.Delivery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Xunit;

namespace DeliverIt.Tests.Controllers
{
    public class DeliveryControllerTests
    {
        private IMapper mapper;
        private IDeliveryService deliveryService;
        private DeliverItContext dbContext;

        public DeliveryControllerTests()
        {
            var config = new MapperConfiguration(
                cfg =>
                {
                    cfg.AddProfile<TestMapperProfile>();

                });
            mapper = config.CreateMapper();
            var options = new DbContextOptionsBuilder<DeliverItContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
                .Options;
            mapper = config.CreateMapper();
            dbContext = new DeliverItContext(options);
            deliveryService = new DeliveryService(dbContext);
            dbContext.Add<Delivery>(new Delivery
            {
               
                OrderId = 1,
                Status = DeliveryStatus.Created,
                AccessWindow = new AccessWindow
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(2)
                },
                Sender = new Partner
                {
                    Name = "Ikea"
                },
                Recipient = new User
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@google.com",
                    Address = "Test Street, London",
                    Phone = "0845345"
                }
            });
            dbContext.Add<Delivery>(new Delivery
            {
               
                OrderId = 10,
                Status = DeliveryStatus.Approved,
                AccessWindow = new AccessWindow
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(2)
                },
                
            });
            dbContext.Add<Delivery>(new Delivery
            {
               
                OrderId = 11,
                Status = DeliveryStatus.Approved,
                AccessWindow = new AccessWindow
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(2)
                },
                
            });
            dbContext.Add<Delivery>(new Delivery
            {
               
                OrderId = 13,
                Status = DeliveryStatus.Approved,
                AccessWindow = new AccessWindow
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(2)
                },
                
            });
             dbContext.Add<Delivery>(new Delivery
            {
               
                OrderId = 9,
                Status = DeliveryStatus.Completed,
                AccessWindow = new AccessWindow
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(2)
                },
                
            });
             dbContext.Add<Delivery>(new Delivery
            {
               
                OrderId = 10,
                Status = DeliveryStatus.Expired,
                AccessWindow = new AccessWindow
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(2)
                },
                
            });
            dbContext.SaveChanges();
        }

        [Fact()]
        public async void GetAllDeliveriesTest()
        {
            var controller = new DeliveryController(deliveryService, mapper);
            var response = await controller.GetAll();
            var okResult = response as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var deliveries = okResult.Value as IEnumerable<DeliveryViewModel>;
            Assert.NotEmpty(deliveries);
        }

        [Fact()]
        public async void GetSingleDeliveryTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var response = await controller.Get(2);
            var okResult = response as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var delivery = okResult.Value as DeliveryViewModel;
            Assert.Equal(2, delivery.Id);
        }


        [Fact()]
        public async void GetReturnValidResponseIfDeliveryNotFoundTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var response = await controller.Get(-5);
            var notFoundResult = response as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact()]
        public async void PostTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var model = new CreateDeliveryViewModel()
            {
                OrderId = 30,
                RecipientId = 1,
                PartnerId = 1,
                StartTime = DateTime.Now.AddDays(2).AddHours(7),
                EndTime = DateTime.Now.AddDays(2).AddHours(10),

            };
            var response = await controller.Post(model);
            var okResult = response as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var delivery = okResult.Value as DeliveryViewModel;
            Assert.NotEqual(1, delivery.Id);
        }

        [Fact()]
        public async void PostCannotAddDeliveryForSameOrderTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var model = new CreateDeliveryViewModel()
            {
                OrderId = 1,
            };

            var response = await controller.Post(model);
            var okResult = response as BadRequestObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(400, okResult.StatusCode);
            Assert.NotNull(okResult.Value);
        }

        [Fact()]
        public async void PostCannotAddDeliveryForForInValidStartEndTimeTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var model = new CreateDeliveryViewModel()
            {
                OrderId = 5,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(-1)
            };

            var response = await controller.Post(model);
            var okResult = response as BadRequestObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(400, okResult.StatusCode);
            Assert.NotNull(okResult.Value);
        }

        [Fact()]
        public async void PostCannotAddDeliveryForForExpiredEndTimeTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var model = new CreateDeliveryViewModel()
            {
                OrderId = 5,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(-5)
            };

            var response = await controller.Post(model);
            var okResult = response as BadRequestObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(400, okResult.StatusCode);
            Assert.NotNull(okResult.Value);
        }

        [Fact()]
        public async void PutTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var model = new UpdateDeliveryViewModel()
            {
                Id = 1,
                Status = DeliveryStatus.Approved,
                StartTime = DateTime.Now.AddDays(2).AddHours(7),
                EndTime = DateTime.Now.AddDays(2).AddHours(10),
            };
            var response = await controller.Put(1, model);
            var okResult = response as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var delivery = okResult.Value as DeliveryViewModel;
            Assert.Equal(model.Status, delivery.Status);
        }

        [Fact()]
        public async void PutReturnValidResponseIfDeliveryNotFoundTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var response = await controller.Put(-5, new UpdateDeliveryViewModel()
            {
                Id = -5,
                StartTime = DateTime.Now.AddDays(2).AddHours(7),
                EndTime = DateTime.Now.AddDays(2).AddHours(10),
            });
            var notFoundResult = response as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact()]
        public async void PutReturnValidResponseIfDeliveryNotFoundDueToIdMismatchTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var response = await controller.Put(5, new UpdateDeliveryViewModel() { Id = 4 });
            var notFoundResult = response as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact()]
        public async void PutCannotAddDeliveryForForInValidStartEndTimeTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var model = new UpdateDeliveryViewModel()
            {
                Id = 1,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(-1)
            };

            var response = await controller.Put(1, model);
            var okResult = response as BadRequestObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(400, okResult.StatusCode);
            Assert.NotNull(okResult.Value);
        }

        [Fact()]
        public async void PutCannotAddDeliveryForForExpiredEndTimeTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var model = new UpdateDeliveryViewModel()
            {
                Id = 1,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(-5)
            };

            var response = await controller.Put(1, model);
            var okResult = response as BadRequestObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(400, okResult.StatusCode);
            Assert.NotNull(okResult.Value);
        }


        [Fact()]
        public async void ApproveDeliveryTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var response = await controller.ApproveDelivery(1);
            var okResult = response as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var delivery = okResult.Value as DeliveryViewModel;
            Assert.Equal(DeliveryStatus.Approved, delivery.Status);
        }

        [Fact()]
        public async void CompleteDeliveryTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var response = await controller.CompleteDelivery(2);
            var okResult = response as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var delivery = okResult.Value as DeliveryViewModel;
            Assert.Equal(DeliveryStatus.Completed, delivery.Status);
        }

        [Fact()]
        public async void CancelDeliveryTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var response = await controller.CancelDelivery(3);
            var okResult = response as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var delivery = okResult.Value as DeliveryViewModel;
            Assert.Equal(DeliveryStatus.Cancelled, delivery.Status);
        }

        [Fact()]
        public async void ExpireDeliveryTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var response = await controller.ExpireDelivery(4);
            var okResult = response as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var delivery = okResult.Value as DeliveryViewModel;
            Assert.Equal(DeliveryStatus.Expired, delivery.Status);
        }

        [Fact()]
        public async void UpdateDeliveryStatusCannotSetStatusToCompletedIfNotApprovedTest()
        {
            var controller = new DeliveryController(deliveryService, mapper);
            var response = await controller.CompleteDelivery(5);
            var badResult = response as BadRequestObjectResult;
            Assert.NotNull(badResult);
            Assert.Equal(400, badResult.StatusCode);
            Assert.NotNull(badResult.Value);
        }

        [Fact()]
        public async void UpdateDeliveryStatusCannotSetStatusToApprovedIfNotStatusIsCreatedTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var response = await controller.ApproveDelivery(5);
            var badResult = response as BadRequestObjectResult;
            Assert.NotNull(badResult);
            Assert.Equal(400, badResult.StatusCode);
            Assert.NotNull(badResult.Value);
        }

        [Fact()]
        public async void UpdateDeliveryStatusCanNotSetStatusToCancelIfStatusIsNotCompletedOrExpiredTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var response = await controller.CancelDelivery(6);
            var badResult = response as BadRequestObjectResult;
            Assert.NotNull(badResult);
            Assert.Equal(400, badResult.StatusCode);
            Assert.NotNull(badResult.Value);
        }

        [Fact()]
        public async void ApproveDeliveryStatusReturnValidResponseIfDeliveryNotFoundTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var response = await controller.ApproveDelivery(-9);
            var notFoundResult = response as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact()]
        public async void CompleteDeliveryStatusReturnValidResponseIfDeliveryNotFoundTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var response = await controller.CompleteDelivery(-10);
            var notFoundResult = response as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact()]
        public async void CancelDeliveryStatusReturnValidResponseIfDeliveryNotFoundTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var response = await controller.CancelDelivery(-1);
            var notFoundResult = response as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact()]
        public async void ExpireDeliveryStatusReturnValidResponseIfDeliveryNotFoundTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var response = await controller.ExpireDelivery(-2);
            var notFoundResult = response as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }


        [Fact()]
        public async void DeleteTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var response = await controller.Delete(1);
            var okResult = response as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var delivery = okResult.Value as Delivery;
            Assert.Equal(1, delivery.Id);
        }

        [Fact()]
        public async void DeleteReturnValidResponseIfDeliveryNotFoundTest()
        {
            var controller = new DeliveryController(deliveryService, mapper);
            var response = await controller.Delete(-1);
            var notFoundResult = response as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);

        }
    }
}