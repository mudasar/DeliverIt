using DeliveryIt.Controllers;
using DeliveryIt.Models;
using DeliveryIt.ViewModels;
using DeliveryIt.ViewModels.Partner;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DeliverIt.Tests.Controllers
{
    public class PartnerControllerTests
    {
        [Fact()]
        public async void GetAllPartnersTest()
        {
            var controller = new PartnerController();
            var partners = await controller.Get();
            Assert.NotEmpty(partners);
        }

        [Fact()]
        public async void GetSinglePartnerTest()
        {
            var controller = new PartnerController();
            var response = await controller.Get(1);
            var okResult = response as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var partner = okResult.Value as Partner;
            Assert.Equal(1, partner.Id);
        }


        [Fact()]
        public async void GetReturnValidResponseIfPartnerNotFoundTest()
        {
            var controller = new PartnerController();
            var response = await controller.Get(5);
            var notFoundResult = response as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact()]
        public async void PostTest()
        {
            var controller = new PartnerController();
            var model = new CreatePartnerViewModel()
            {
                Name = "Amazon",

            };
            var response = await controller.Post(model);
            var okResult = response as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var partner = okResult.Value as Partner;
            Assert.Equal(model.Name, partner.Name);
        }

        [Fact()]
        public async void PostCannotAddPartnerWithSameEmailTest()
        {
            var controller = new PartnerController();
            var model = new CreatePartnerViewModel()
            {
                Name = "Ikea",
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
            var controller = new PartnerController();
            var model = new UpdatePartnerViewModel()
            {
                Id = 1,
                Name = "Amazon",
            };
            var response = await controller.Put(1, model);
            var okResult = response as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var partner = okResult.Value as Partner;
            Assert.Equal(model.Name, partner.Name);
        }

        [Fact()]
        public async void PutReturnValidResponseIfPartnerNotFoundTest()
        {
            var controller = new PartnerController();
            var response = await controller.Put(5, new UpdatePartnerViewModel() { Id = 5 });
            var notFoundResult = response as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact()]
        public async void PutReturnValidResponseIfPartnerNotFoundDueToIdMismatchTest()
        {
            var controller = new PartnerController();
            var response = await controller.Put(5, new UpdatePartnerViewModel() { Id = 4 });
            var notFoundResult = response as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact()]
        public async void DeleteTest()
        {
            var controller = new PartnerController();
            var response = await controller.Delete(1);
            var okResult = response as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var partner = okResult.Value as Partner;
            Assert.Equal(1, partner.Id);
        }

        [Fact()]
        public async void DeleteReturnValidResponseIfPartnerNotFoundTest()
        {
            var controller = new PartnerController();
            var response = await controller.Delete(5);
            var notFoundResult = response as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);

        }
    }
}