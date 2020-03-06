using DeliveryIt.Controllers;
using DeliveryIt.ViewModels;
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
            var partner = await controller.Get(1);
            Assert.NotNull(partner);
            Assert.Equal(1, partner.Id);
        }

        [Fact()]
        public async void PostTest()
        {
            var controller = new PartnerController();
            var model = new CreatePartnerViewModel()
            {
                Name = "Ikea"
            };
            var partner = await controller.Post(model);
            Assert.NotNull(partner);
            Assert.Equal(1, partner.Id);
            Assert.Equal(model.Name, partner.Name);
        }

        [Fact()]
        public async void PutTest()
        {
            var controller = new PartnerController();
            var model = new UpdatePartnerViewModel()
            {
                Id = 5,
                Name = "Hello"
            };
            var partner = await controller.Put(5, model);
            Assert.NotNull(partner);
            Assert.Equal(1, partner.Id);
            Assert.Equal(model.Name, partner.Name);
        }

        [Fact()]
        public async void DeleteTest()
        {
            var controller = new PartnerController();
            var partner = await controller.Delete(5);
            Assert.NotNull(partner);
            Assert.Equal(5, partner.Id);
        }
    }
}