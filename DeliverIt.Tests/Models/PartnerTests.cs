using DeliveryIt.Models;
using System;
using Xunit;

namespace DeliverIt.Tests
{
    public class PartnerTests
    {
        [Fact]
        public void CanCreatePartnerModel()
        {
            var partner = new Partner();
            partner.Name = "Ikea";
            partner.Id = 1;

            Assert.NotNull(partner);
            Assert.Equal("Ikea", partner.Name);
        }
    }
}
