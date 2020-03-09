using System;
using DeliverIt.Data;
using DeliverIt.Models;
using DeliverIt.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DeliverIt.Tests.Services
{
    public class DeliveryServiceTests
    {

        private readonly DeliverItContext dbContext;
        private readonly IDeliveryService deliveryService;


        public DeliveryServiceTests()
        {
            var options = new DbContextOptionsBuilder<DeliverItContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
                .Options;
            dbContext = new DeliverItContext(options);
            deliveryService = new DeliveryService(dbContext);
            dbContext.Deliveries.Add(new Delivery()
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
            dbContext.SaveChanges();
        }

        [Fact]
        public async void CanGetAllDeliveries()
        {
            var deliveries = await deliveryService.GetAllDeliveries();
            Assert.NotNull(deliveries);
            Assert.NotEmpty(deliveries);
        }

        [Fact]
        public async void CanGetDeliveryById()
        {
            var delivery = await deliveryService.GetDeliveryById(1);
            Assert.NotNull(delivery);
            Assert.Equal(DeliveryStatus.Created, delivery.Status);
        }

        [Fact]
        public async void ReturnNullIfDeliveryNotFound()
        {
            var delivery = await deliveryService.GetDeliveryById(-1);
            Assert.Null(delivery);
        }

        [Fact]
        public async void CanCheckIfDeliveryExistsByDeliveryId()
        {

            var exists = await deliveryService.DeliveryExists(1);
            Assert.True(exists);
        }

        [Fact]
        public async void CanCheckIfDeliveryExistsByDeliveryOrderNumber()
        {

            var exists = await deliveryService.DeliveryExists(1);
            Assert.True(exists);
        }

        [Fact]
        public async void ReturnFalseIfNoDeliveryById()
        {
            var exists = await deliveryService.DeliveryExists(-1);
            Assert.False(exists);
        }

        [Fact]
        public async void ReturnFalseIfNoDeliveryByOrderNumber()
        {
            var exists = await deliveryService.DeliveryExists(-5);
            Assert.False(exists);
        }

        [Fact]
        public async void CanRemoveDelivery()
        {
            var newDelivery = await deliveryService.CreateDelivery(new Delivery
            {
               OrderId =  50,
               Status = DeliveryStatus.Approved
            });

            var delivery = await deliveryService.RemoveDelivery(newDelivery.Id);
            Assert.NotNull(delivery);
        }

        [Fact]
        public async void ThrowsIfCannotRemoveDelivery()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                var delivery = await deliveryService.RemoveDelivery(-1);
                Assert.NotNull(delivery);
            });
        }

        [Fact]
        public async void CanAddDelivery()
        {
            var delivery = new Delivery
            {
                OrderId = 51,
                Status = DeliveryStatus.Approved
            };

            var newDelivery = await deliveryService.CreateDelivery(delivery);
            Assert.NotNull(newDelivery);
            Assert.Equal(51, newDelivery.OrderId);
        }

        [Fact]
        public async void CannotAddDeliveryWithEmptyDetails()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                var delivery = new Delivery
                {

                };

                var newDelivery = await deliveryService.CreateDelivery(delivery);
                Assert.Null(newDelivery);
            });
        }

        [Fact]
        public async void CannotAddDuplicateDelivery()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                var delivery = new Delivery
                {
                    OrderId = 52,
                    Status = DeliveryStatus.Approved
                };

                var newDelivery1 = await deliveryService.CreateDelivery(delivery);
                Assert.NotNull(newDelivery1);
                var newDelivery = await deliveryService.CreateDelivery(delivery);
                Assert.Null(newDelivery);
            });
        }

        [Fact]
        public async void CanUpdateDeliveryDetails()
        {
            var delivery = new Delivery
            {
                OrderId = 53,
                Status = DeliveryStatus.Approved
            };

            var newDelivery1 = await deliveryService.CreateDelivery(delivery);
            Assert.NotNull(newDelivery1);
            newDelivery1.Status = DeliveryStatus.Completed;

            var updated = await deliveryService.UpdateDelivery(newDelivery1);
            Assert.NotNull(updated);
            Assert.NotEqual(DeliveryStatus.Approved, updated.Status);
            Assert.Equal(DeliveryStatus.Completed, updated.Status);
        }

        [Fact]
        public async void CannotUpdateToDuplicateDelivery()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                var delivery = new Delivery
                {
                    OrderId = 56,
                    Status = DeliveryStatus.Approved
                };

                var newDelivery1 = await deliveryService.CreateDelivery(delivery);
                Assert.NotNull(newDelivery1);

                var delivery2 = new Delivery
                {
                    OrderId = 57,
                    Status = DeliveryStatus.Approved
                };

                var newDelivery2 = await deliveryService.CreateDelivery(delivery);
                Assert.NotNull(newDelivery2);

                newDelivery2.OrderId = 56;

                var updated = await deliveryService.UpdateDelivery(newDelivery2);
                Assert.Null(updated);
            });
        }
    }
}