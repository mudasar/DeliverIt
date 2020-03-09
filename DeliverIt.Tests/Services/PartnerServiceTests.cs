using System;
using DeliverIt.Data;
using DeliverIt.Models;
using DeliverIt.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DeliverIt.Tests.Services
{
    public class PartnerServiceTests
    {
        private readonly DeliverItContext dbContext;
        private readonly IPartnerService partnerService;


        public PartnerServiceTests()
        {
            var options = new DbContextOptionsBuilder<DeliverItContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
                .Options;
            dbContext = new DeliverItContext(options);
            partnerService = new PartnerService(dbContext);
            dbContext.Partners.Add(new Partner
            {
                Name = "Ikea"
            });
            dbContext.SaveChanges();
        }

        [Fact]
        public async void CanGetAllPartners()
        {
            var users = await partnerService.GetAllPartners();
            Assert.NotNull(users);
            Assert.NotEmpty(users);
        }

        [Fact]
        public async void CanGetPartnerById()
        {
            var partner = await partnerService.GetPartnerById(1);
            Assert.NotNull(partner);
            Assert.NotNull(partner.Name);
        }

        [Fact]
        public async void ReturnNullIfPartnerNotFound()
        {
            var partner = await partnerService.GetPartnerById(-1);
            Assert.Null(partner);
        }

        [Fact]
        public async void CanCheckIfPartnerExistsByPartnerId()
        {

            var exists = await partnerService.PartnerExists(1);
            Assert.True(exists);
        }

        [Fact]
        public async void CanCheckIfPartnerExistsByPartnerName()
        {

            var exists = await partnerService.PartnerExists("Ikea");
            Assert.True(exists);
        }

        [Fact]
        public async void ReturnFalseIfNoPartnerById()
        {
            var exists = await partnerService.PartnerExists(-1);
            Assert.False(exists);
        }

        [Fact]
        public async void ReturnFalseIfNoPartnerByName()
        {
            var exists = await partnerService.PartnerExists("Google");
            Assert.False(exists);
        }

        [Fact]
        public async void CanRemovePartner()
        {
            var newPartner = await partnerService.CreatePartner(new Partner
            {
                Name = "Msn"
            });

            var partner = await partnerService.RemovePartner(newPartner.Id);
            Assert.NotNull(partner);
        }

        [Fact]
        public async void ThrowsIfCannotRemovePartner()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                var partner = await partnerService.RemovePartner(-1);
                Assert.NotNull(partner);
            });
        }

        [Fact]
        public async void CanAddPartner()
        {
            var partner = new Partner
            {
                Name = "Amazon"
            };

            var newPartner = await partnerService.CreatePartner(partner);
            Assert.NotNull(newPartner);
            Assert.Equal("Amazon", newPartner.Name);
        }

        [Fact]
        public async void CannotAddPartnerWithEmptyDetails()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                var partner = new Partner
                {

                };

                var newPartner = await partnerService.CreatePartner(partner);
                Assert.Null(newPartner);
            });
        }

        [Fact]
        public async void CannotAddDuplicatePartner()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                var partner = new Partner
                {
                    Name = "Ikea"
                };

                var newPartner1 = await partnerService.CreatePartner(partner);
                Assert.NotNull(newPartner1);
                var newPartner = await partnerService.CreatePartner(partner);
                Assert.Null(newPartner);
            });
        }

        [Fact]
        public async void CanUpdatePartnerDetails()
        {
            var partner = new Partner
            {
                Name = "Ikea1"
            };

            var newPartner1 = await partnerService.CreatePartner(partner);
            Assert.NotNull(newPartner1);
            newPartner1.Name = "Ikea2";

            var updated = await partnerService.UpdatePartner(newPartner1);
            Assert.NotNull(updated);
            Assert.NotEqual("Ikea1", updated.Name);
            Assert.Equal("Ikea2", updated.Name);
        }

        [Fact]
        public async void CannotUpdateToDuplicatePartner()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                var partner = new Partner
                {
                    Name = "Ikea3"
                };

                var newPartner1 = await partnerService.CreatePartner(partner);
                Assert.NotNull(newPartner1);

                var prtner2 = new Partner
                {
                    Name = "Ikea4"
                };

                var newPartner2 = await partnerService.CreatePartner(partner);
                Assert.NotNull(newPartner2);

                newPartner2.Name = "Ikea3";

                var updated = await partnerService.UpdatePartner(newPartner2);
                Assert.Null(updated);
            });
        }
    }
}