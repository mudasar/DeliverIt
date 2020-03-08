using DeliveryIt.Controllers;
using DeliveryIt.Models;
using DeliveryIt.ViewModels.Delivery;
using Microsoft.AspNetCore.Mvc;
using System;
using Xunit;

namespace DeliverIt.Tests.Controllers
{
    public class DeliveryControllerTests
    {
        [Fact()]
        public async void GetAllDeliveriesTest()
        {
            var controller = new DeliveryController();
            var deliveries = await controller.Get();
            Assert.NotEmpty(deliveries);
        }

        [Fact()]
        public async void GetSingleDeliveryTest()
        {
            var controller = new DeliveryController();
            var response = await controller.Get(1);
            var okResult = response as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var delivery = okResult.Value as DeliveryViewModel;
            Assert.Equal(1, delivery.Id);
        }


        [Fact()]
        public async void GetReturnValidResponseIfDeliveryNotFoundTest()
        {
            var controller = new DeliveryController();
            var response = await controller.Get(5);
            var notFoundResult = response as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact()]
        public async void PostTest()
        {
            var controller = new DeliveryController();
            var model = new CreateDeliveryViewModel()
            {
                OrderId = 3,
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
            var controller = new DeliveryController();
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
            var controller = new DeliveryController();
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
            var controller = new DeliveryController();
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
            var controller = new DeliveryController();
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
            var controller = new DeliveryController();
            var response = await controller.Put(5, new UpdateDeliveryViewModel()
            {
                Id = 5,
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
            var controller = new DeliveryController();
            var response = await controller.Put(5, new UpdateDeliveryViewModel() { Id = 4 });
            var notFoundResult = response as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact()]
        public async void PutCannotAddDeliveryForForInValidStartEndTimeTest()
        {
            var controller = new DeliveryController();
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
            var controller = new DeliveryController();
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
        public async void UpdateDeliveryStatusToApprovedTest()
        {
            var controller = new DeliveryController();
            var response = await controller.UpdateDeliveryStatus(5, DeliveryStatus.Approved);
            var okResult = response as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var delivery = okResult.Value as DeliveryViewModel;
            Assert.Equal(DeliveryStatus.Approved, delivery.Status);
        }

        [Fact()]
        public async void UpdateDeliveryStatusToCompletedIfApprovedTest()
        {
            Assert.True(false);
            var controller = new DeliveryController();
            var response = await controller.UpdateDeliveryStatus(5, DeliveryStatus.Completed);
            var okResult = response as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var delivery = okResult.Value as DeliveryViewModel;
            Assert.Equal(DeliveryStatus.Completed, delivery.Status);
        }

        [Fact()]
        public async void UpdateDeliveryStatusCannotSetStatusToCompletedIfNotApprovedTest()
        {
            Assert.True(false);
        }

        [Fact()]
        public async void UpdateDeliveryStatusCannotSetStatusToApprovedIfNotStatusIsCreatedTest()
        {
            Assert.True(false);
        }

        [Fact()]
        public async void UpdateDeliveryStatusCanSetStatusToCancelIfNotStatusIsNotCompletedOrExpiredTest()
        {
            Assert.True(false);
        }

        [Fact()]
        public async void UpdateDeliveryStatusReturnValidResponseIfDeliveryNotFoundTest()
        {
            var controller = new DeliveryController();
            var response = await controller.UpdateDeliveryStatus(5, DeliveryStatus.Approved);
            var notFoundResult = response as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }


        [Fact()]
        public async void DeleteTest()
        {
            var controller = new DeliveryController();
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
            var controller = new DeliveryController();
            var response = await controller.Delete(5);
            var notFoundResult = response as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);

        }
    }
}