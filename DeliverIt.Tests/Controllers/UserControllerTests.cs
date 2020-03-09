using System;
using System.Collections.Generic;
using AutoMapper;
using DeliverIt.Controllers;
using DeliverIt.Data;
using DeliverIt.Models;
using DeliverIt.Services;
using DeliverIt.ViewModels;
using DeliverIt.ViewModels.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DeliverIt.Tests.Controllers
{
    public class UserControllerTests
    {
        private IMapper mapper;
        private IUserService userService;
        private DeliverItContext dbContext;

        public UserControllerTests()
        {
            var config = new MapperConfiguration(
                cfg =>
                {
                    cfg.AddProfile<TestMapperProfile>();

                });
            var options = new DbContextOptionsBuilder<DeliverItContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
                .Options;
            mapper = config.CreateMapper();
            dbContext = new DeliverItContext(options);
            userService = new UserService(dbContext);
            dbContext.Users.Add(new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@google.com"
            });
            dbContext.SaveChanges();
        }

        [Fact()]
        public async void GetAllUsersTest()
        {
            var controller = new UserController(userService, mapper);
            var response = await controller.GetAll();
            var okResult = response as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var users = okResult.Value as IEnumerable<User>;
            Assert.NotEmpty(users);
        }

        [Fact()]
        public async void GetSingleUserTest()
        {

            var controller = new UserController(userService, mapper);
            var response = await controller.Get(1);
            var okResult = response as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var user = okResult.Value as User;
            Assert.Equal(1, user.Id);
        }


        [Fact()]
        public async void GetReturnValidResponseIfUserNotFoundTest()
        {
            var controller = new UserController(userService, mapper);
            var response = await controller.Get(-5);
            var notFoundResult = response as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact()]
        public async void PostTest()
        {
            var controller = new UserController(userService, mapper);
            var model = new CreateUserViewModel()
            {
                FirstName = "John",
                LastName = "Doe",

            };
            var response = await controller.Post(model);
            var okResult = response as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var user = okResult.Value as User;
            Assert.Equal(model.FirstName, user.FirstName);
        }

        [Fact()]
        public async void PostCannotAddUserWithSameEmailTest()
        {
            var controller = new UserController(userService, mapper);
            var model = new CreateUserViewModel()
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@google.com"
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
            var controller = new UserController(userService, mapper);
            var model = new UpdateUserViewModel()
            {
                Id = 1,
                FirstName = "Alpha",
                LastName = "Doe"
            };
            var response = await controller.Put(1, model);
            var okResult = response as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var user = okResult.Value as User;
            Assert.Equal(model.FirstName, user.FirstName);
        }

        [Fact()]
        public async void PutReturnValidResponseIfUserNotFoundTest()
        {
            var controller = new UserController(userService, mapper);
            var response = await controller.Put(-5, new UpdateUserViewModel() { Id = -5 });
            var notFoundResult = response as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact()]
        public async void PutReturnValidResponseIfUserNotFoundDueToIdMismatchTest()
        {
            var controller = new UserController(userService, mapper);
            var response = await controller.Put(5, new UpdateUserViewModel() { Id = 4 });
            var notFoundResult = response as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact()]
        public async void DeleteTest()
        {
            var controller = new UserController(userService, mapper);
            var response = await controller.Delete(1);
            var okResult = response as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var user = okResult.Value as User;
            Assert.Equal(1, user.Id);
        }

        [Fact()]
        public async void DeleteReturnValidResponseIfUserNotFoundTest()
        {
            var controller = new UserController(userService, mapper);
            var response = await controller.Delete(-5);
            var notFoundResult = response as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);

        }
    }
}