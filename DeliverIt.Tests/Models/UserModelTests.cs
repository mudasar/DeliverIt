using DeliveryIt.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DeliverIt.Tests.Models
{
    public class UserModelTests
    {
        [Fact]
        public void CanCreateUserModel()
        {
            var user = new User();
            user.FirstName = "John";
            user.LastName = "Doe";
            user.Id = 1;

            Assert.NotNull(user);
            Assert.Equal("John Doe", user.Name);
        }

        [Fact]
        public void UserModelMustHaveFullName()
        {
            var user = new User();
            user.FirstName = "John";
            user.LastName = "Doe";
            user.Id = 1;

            Assert.NotNull(user.Name);
            Assert.Equal("John Doe", user.Name);
        }

        [Fact]
        public void UserModelMustHaveAddress()
        {
            var user = new User();
            user.Address = "hope street, London";
            user.Id = 1;

            Assert.NotNull(user.Address);
            Assert.Equal("hope street, London", user.Address);
        }

        [Fact]
        public void UserModelMustHavePhone()
        {
            var user = new User();
            user.Phone = "4471234567";
            user.Id = 1;

            Assert.NotNull(user.Phone);
            Assert.Equal("4471234567", user.Phone);
        }

        [Fact]
        public void UserModelMustHaveEmail()
        {
            var user = new User();
            user.Email = "john.doe@google.com";
            user.Id = 1;

            Assert.NotNull(user.Email);
            Assert.Equal("john.doe@google.com", user.Email);
        }
    }
}
