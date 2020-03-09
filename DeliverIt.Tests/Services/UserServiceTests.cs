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
        private readonly DeliverItContext dbContext;
        private readonly IUserService userService;


        public UserServiceTests()
        {
            var options = new DbContextOptionsBuilder<DeliverItContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
                .Options;
            dbContext = new DeliverItContext(options);
            userService = new UserService(dbContext);
            dbContext.Users.Add(new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@google.com"
            });
            // dbContext.Users.Add();
            dbContext.SaveChanges();
        }

        [Fact]
        public async void CanGetAllUsers()
        {
            var users = await userService.GetAllUsers();
            Assert.NotNull(users);
            Assert.NotEmpty(users);
        }

        [Fact]
        public async void CanGetUserById()
        {
            var user = await userService.GetUserById(1);
            Assert.NotNull(user);
            Assert.NotNull(user.FirstName);
        }

        [Fact]
        public async void ReturnNullIfUserNotFound()
        {
            var user = await userService.GetUserById(-1);
            Assert.Null(user);
        }

        [Fact]
        public async void CanCheckIfUserExistsByUserId()
        {

            var exists = await userService.UserExists(1);
            Assert.True(exists);
        }

        [Fact]
        public async void CanCheckIfUserExistsByUserEmail()
        {

            var exists = await userService.UserExists("john.doe@google.com");
            Assert.True(exists);
        }

        [Fact]
        public async void ReturnFalseIfNoUserById()
        {
            var exists = await userService.UserExists(-1);
            Assert.False(exists);
        }

        [Fact]
        public async void ReturnFalseIfNoUserByEmail()
        {
            var exists = await userService.UserExists("nouser@google.com");
            Assert.False(exists);
        }

        [Fact]
        public async void CanRemoveUser()
        {
            var newUser = await userService.CreateUser(new User
            {
                FirstName = "To be",
                LastName = "Deleted",
                Email = "to.be.deleted@google.com"
            });

            var user = await userService.RemoveUser(newUser.Id);
            Assert.NotNull(user);
        }

        [Fact]
        public async void ThrowsIfCannotRemoveUser()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
             {
                 var user = await userService.RemoveUser(-1);
                 Assert.NotNull(user);
             });
        }

        [Fact]
        public async void CanAddUser()
        {
            var user = new User
            {
                FirstName = "New User",
                LastName = "User",
                Email = "Some.User@google.com"
            };

            var newUser = await userService.CreateUser(user);
            Assert.NotNull(newUser);
            Assert.Equal(user.FirstName, newUser.FirstName);
        }

        [Fact]
        public async void CannotAddUserWithEmptyDetails()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                var user = new User
                {

                };

                var newUser = await userService.CreateUser(user);
                Assert.Null(newUser);
            });
        }

        [Fact]
        public async void CannotAddDuplicateUser()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
             {
                 var user = new User
                 {
                     FirstName = "New User",
                     LastName = "User",
                     Email = "Some.User.1@google.com"
                 };

                 var newUser1 = await userService.CreateUser(user);
                 Assert.NotNull(newUser1);
                 var newUser = await userService.CreateUser(user);
                 Assert.Null(newUser);
             });
        }

        [Fact]
        public async void CanUpdateUserDetails()
        {
            var user = new User
            {
                FirstName = "New User",
                LastName = "User",
                Email = "Some.User.2@google.com"
            };

            var newUser1 = await userService.CreateUser(user);
            Assert.NotNull(newUser1);
            newUser1.FirstName = "Updated";

            var updated = await userService.UpdateUser(newUser1);
            Assert.NotNull(updated);
            Assert.NotEqual("New User", updated.FirstName);
            Assert.Equal(user.LastName, updated.LastName);
        }

        [Fact]
        public async void CannotUpdateToDuplicateUser()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                var user = new User
                {
                    FirstName = "New User",
                    LastName = "User",
                    Email = "Some.User.1@google.com"
                };

                var newUser1 = await userService.CreateUser(user);
                Assert.NotNull(newUser1);

                var user2 = new User
                {
                    FirstName = "New User",
                    LastName = "User",
                    Email = "Some.User.2@google.com"
                };

                var newUser2 = await userService.CreateUser(user);
                Assert.NotNull(newUser2);

                newUser2.Email = "Some.User.1@google.com";

                var updated = await userService.UpdateUser(newUser2);
                Assert.Null(updated);
            });
        }


    }
}
