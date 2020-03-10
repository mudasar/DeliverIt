using AutoMapper;
using DeliverIt.Controllers;
using DeliverIt.Data;
using DeliverIt.Helpers;
using DeliverIt.Models;
using DeliverIt.Services;
using DeliverIt.ViewModels;
using DeliverIt.ViewModels.Partner;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Xunit;

namespace DeliverIt.Tests.Controllers
{
    public class PartnerControllerTests
    {
        private IMapper mapper;
        private IPartnerService partnerService;
        private DeliverItContext dbContext;
        private IOptions<AppSettings> appSettings;

        public PartnerControllerTests()
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
            partnerService = new PartnerService(dbContext);

            dbContext.Partners.Add(new Partner
            {
                Name = "Ikea",
                Password = "password"
            });
            dbContext.SaveChanges();
            appSettings = Options.Create<AppSettings>(new AppSettings { Secret = "Thisisaverylonglongsecret" });
        }


        [Fact()]
        public async void GetAllPartnersTest()
        {
            var controller = new PartnerController(partnerService, mapper, appSettings);
            var partners = await controller.GetAll();
            Assert.NotEmpty(partners);
        }

        [Fact()]
        public async void GetSinglePartnerTest()
        {
            var controller = new PartnerController(partnerService, mapper, appSettings);
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
            var controller = new PartnerController(partnerService, mapper, appSettings);
            var response = await controller.Get(-1);
            var notFoundResult = response as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async void AuthenticatePartner()
        {
            var controller = new PartnerController(partnerService, mapper, appSettings);
            var model = new PartnerLoginViewModel()
            {
                Name = "ikea",
                Password = "password",

            };
            var response = await controller.Authenticate(model);
            var okResult = response as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var token = okResult.Value as dynamic;
            Assert.NotNull(token);
        }

        [Fact()]
        public async void PostTest()
        {
            var controller = new PartnerController(partnerService, mapper, appSettings);
            var model = new CreatePartnerViewModel()
            {
                Name = "Amazon1",

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
            var controller = new PartnerController(partnerService, mapper, appSettings);
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
            var controller = new PartnerController(partnerService, mapper, appSettings);
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
            var controller = new PartnerController(partnerService, mapper, appSettings);
            var response = await controller.Put(-5, new UpdatePartnerViewModel() { Id = -5 });
            var notFoundResult = response as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact()]
        public async void PutReturnValidResponseIfPartnerNotFoundDueToIdMismatchTest()
        {
            var controller = new PartnerController(partnerService, mapper, appSettings);
            var response = await controller.Put(5, new UpdatePartnerViewModel() { Id = 4 });
            var notFoundResult = response as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact()]
        public async void DeleteTest()
        {
            var controller = new PartnerController(partnerService, mapper, appSettings);
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
            var controller = new PartnerController(partnerService, mapper, appSettings);
            var response = await controller.Delete(-1);
            var notFoundResult = response as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);

        }
    }
}