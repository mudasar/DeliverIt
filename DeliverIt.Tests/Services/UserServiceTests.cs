using DeliverIt.Data;
using DeliverIt.Models;
using DeliverIt.Services;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DeliverIt.Tests.Services
{
    public class UserServiceTests
    {
        private DeliverItContext dbContext;
        private IUserService userService;
        

        public UserServiceTests()
        {
            var options = new DbContextOptionsBuilder<DeliverItContext>().Options;
            var initialEntities = new[]
            {
                new User { Id = 1, FirstName = "John", LastName = "Doe" },
                new User { Id = 2, FirstName = "Jane", LastName = "Doe" },
            };
            var dbContextMock = new DbContextMock<DeliverItContext>(options);
            var usersDbSetMock = dbContextMock.CreateDbSetMock(x => x.Users, initialEntities);

            dbContext = dbContextMock.Object;
            userService = new UserService(dbContext);
        }

        [Fact]
        public async void CanGetUserById()
        {
            var user = await userService.GetUserById(1);
            Assert.NotNull(user);
            Assert.NotNull(user.FirstName);
        }
    }
}
