using AutoMapper;
using DeliverIt.Controllers;
using DeliverIt.Data;
using DeliverIt.Models;
using DeliverIt.Services;
using DeliverIt.ViewModels.Delivery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using Xunit;

namespace DeliverIt.Tests.Controllers
{
    public class DeliveryControllerTests
    {
        private IMapper mapper;
        private IDeliveryService deliveryService;
        private DeliverItContext dbContext;


        private DeliverItContext dbSqlite;

        public DeliveryControllerTests()
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

            if (File.Exists("deliveryittest.db"))
            {
                File.Delete("deliveryittest.db");
            }



            var sqliteOptions = new DbContextOptionsBuilder<DeliverItContext>().UseSqlite("Data Source=deliveryittest.db", builder =>
            {

            }).Options;
            mapper = config.CreateMapper();
            dbContext = new DeliverItContext(options);
            dbSqlite = new DeliverItContext(sqliteOptions);
            dbSqlite.Database.EnsureDeleted();
            dbSqlite.Database.Migrate();

            deliveryService = new DeliveryService(dbContext);
            dbContext.Add<Delivery>(new Delivery
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
            dbContext.Add<Delivery>(new Delivery
            {

                OrderId = 10,
                Status = DeliveryStatus.Approved,
                AccessWindow = new AccessWindow
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(2)
                },

            });
            dbContext.Add<Delivery>(new Delivery
            {

                OrderId = 11,
                Status = DeliveryStatus.Approved,
                AccessWindow = new AccessWindow
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(2)
                },

            });
            dbContext.Add<Delivery>(new Delivery
            {

                OrderId = 13,
                Status = DeliveryStatus.Approved,
                AccessWindow = new AccessWindow
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(2)
                },

            });
            dbContext.Add<Delivery>(new Delivery
            {

                OrderId = 9,
                Status = DeliveryStatus.Completed,
                AccessWindow = new AccessWindow
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(2)
                },

            });
            dbContext.Add<Delivery>(new Delivery
            {

                OrderId = 10,
                Status = DeliveryStatus.Expired,
                AccessWindow = new AccessWindow
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(2)
                },

            });
            dbContext.SaveChanges();
        }

        [Fact()]
        public async void GetAllDeliveriesTest()
        {
            var controller = new DeliveryController(deliveryService, mapper);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new System.Security.Claims.ClaimsPrincipal()
                }
            };

            var response = await controller.GetAll();
            var okResult = response as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var deliveries = okResult.Value as IEnumerable<DeliveryViewModel>;
            Assert.NotEmpty(deliveries);
        }

        [Fact()]
        public async void GetSingleDeliveryTest()
        {
            var delivery = new Delivery
            {
                OrderId = 10,
                Status = DeliveryStatus.Approved,
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
            };
            dbContext.Add(delivery);
            dbContext.SaveChanges();

            var controller = new DeliveryController(deliveryService, mapper); ;
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new System.Security.Claims.ClaimsPrincipal(new ClaimsIdentity(new Claim[]{
                        new Claim(ClaimTypes.NameIdentifier, "1")
                        }))
                }
            };

            var response1 = await controller.Get(delivery.Id);
            var okResult1 = response1 as OkObjectResult;
            Assert.NotNull(okResult1);
            Assert.Equal(200, okResult1.StatusCode);
            Assert.NotNull(okResult1.Value);

            var delivery1 = okResult1.Value as DeliveryViewModel;
            Assert.Equal(delivery.Id, delivery1.Id);
        }


        [Fact()]
        public async void GetReturnValidResponseIfDeliveryNotFoundTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var response = await controller.Get(-5);
            var notFoundResult = response as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }


        [Fact()]
        public async void PostWithRelatedDataTest()
        {
            var relatedDeliveryService = new DeliveryService(dbSqlite);
            dbSqlite.Partners.Add(new Partner()
            {
                Name = "Ikea",
                Password = "password"
            });
            dbSqlite.Users.Add(new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@google.com",
                Address = "Test Street, London",
                Phone = "0845345",
                Password = "password"
            });
            dbSqlite.SaveChanges();

            var controller = new DeliveryController(relatedDeliveryService, mapper);
            var model = new CreateDeliveryViewModel()
            {
                OrderId = 30,
                RecipientId = 1,
                PartnerId = 1,
                StartTime = DateTime.Now.AddDays(2).AddHours(7),
                EndTime = DateTime.Now.AddDays(2).AddHours(10),

            };
            var response = await controller.Post(model);
            var okResult = response as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var delivery = okResult.Value as DeliveryViewModel;
            Assert.NotEqual(0, delivery.Id);
            Assert.NotNull(delivery.AccessWindow);
            Assert.NotNull(delivery.Recipient);
            Assert.NotNull(delivery.Order);
        }

        [Fact()]
        public async void PostCannotAddDeliveryForSameOrderTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var model = new CreateDeliveryViewModel()
            {
                OrderId = 1,
            };

            var response = await controller.Post(model);
            var okResult = response as BadRequestObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(400, okResult.StatusCode);
            Assert.NotNull(okResult.Value);
        }

        [Fact()]
        public async void PostCannotAddDeliveryForForInValidStartEndTimeTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var model = new CreateDeliveryViewModel()
            {
                OrderId = 5,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(-1)
            };

            var response = await controller.Post(model);
            var okResult = response as BadRequestObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(400, okResult.StatusCode);
            Assert.NotNull(okResult.Value);
        }

        [Fact()]
        public async void PostCannotAddDeliveryForForExpiredEndTimeTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var model = new CreateDeliveryViewModel()
            {
                OrderId = 5,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(-5)
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
            var controller = new DeliveryController(deliveryService, mapper); ;
            var model = new UpdateDeliveryViewModel()
            {
                Id = 1,
                Status = DeliveryStatus.Approved,
                StartTime = DateTime.Now.AddDays(2).AddHours(7),
                EndTime = DateTime.Now.AddDays(2).AddHours(10),
            };
            var response = await controller.Put(1, model);
            var okResult = response as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var delivery = okResult.Value as DeliveryViewModel;
            Assert.Equal(Enum.GetName(typeof(DeliveryStatus), model.Status), delivery.Status);
        }

        [Fact()]
        public async void PutReturnValidResponseIfDeliveryNotFoundTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var response = await controller.Put(-5, new UpdateDeliveryViewModel()
            {
                Id = -5,
                StartTime = DateTime.Now.AddDays(2).AddHours(7),
                EndTime = DateTime.Now.AddDays(2).AddHours(10),
            });
            var notFoundResult = response as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact()]
        public async void PutReturnValidResponseIfDeliveryNotFoundDueToIdMismatchTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var response = await controller.Put(5, new UpdateDeliveryViewModel() { Id = 4 });
            var notFoundResult = response as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact()]
        public async void PutCannotAddDeliveryForForInValidStartEndTimeTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var model = new UpdateDeliveryViewModel()
            {
                Id = 1,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(-1)
            };

            var response = await controller.Put(1, model);
            var okResult = response as BadRequestObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(400, okResult.StatusCode);
            Assert.NotNull(okResult.Value);
        }

        [Fact()]
        public async void PutCannotAddDeliveryForForExpiredEndTimeTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var model = new UpdateDeliveryViewModel()
            {
                Id = 1,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(-5)
            };

            var response = await controller.Put(1, model);
            var okResult = response as BadRequestObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(400, okResult.StatusCode);
            Assert.NotNull(okResult.Value);
        }


        [Fact()]
        public async void ApproveDeliveryTest()
        {
            var delivery = new Delivery
            {
                OrderId = 10,
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
            };
            dbContext.Add(delivery);
            dbContext.SaveChanges();

            var controller = new DeliveryController(deliveryService, mapper);
            var response1 = await controller.ApproveDelivery(delivery.Id);
            var okResult1 = response1 as OkObjectResult;
            Assert.NotNull(okResult1);
            Assert.Equal(200, okResult1.StatusCode);
            Assert.NotNull(okResult1.Value);

            var delivery1 = okResult1.Value as DeliveryViewModel;
            Assert.Equal(DeliveryStatus.Approved, Enum.Parse<DeliveryStatus>(delivery1.Status, true));
        }

        [Fact()]
        public async void CompleteDeliveryTest()
        {
            var delivery = new Delivery
            {
                OrderId = 10,
                Status = DeliveryStatus.Approved,
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
            };
            dbContext.Add(delivery);
            dbContext.SaveChanges();


            var controller = new DeliveryController(deliveryService, mapper); ;

            var response1 = await controller.CompleteDelivery(delivery.Id);
            var okResult1 = response1 as OkObjectResult;
            Assert.NotNull(okResult1);
            Assert.Equal(200, okResult1.StatusCode);
            Assert.NotNull(okResult1.Value);

            var delivery1 = okResult1.Value as DeliveryViewModel;
            Assert.Equal(DeliveryStatus.Completed, Enum.Parse<DeliveryStatus>(delivery1.Status, true));
        }

        [Fact()]
        public async void CancelDeliveryTest()
        {
            var delivery = new Delivery
            {
                OrderId = 10,
                Status = DeliveryStatus.Approved,
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
            };
            dbContext.Add(delivery);
            dbContext.SaveChanges();

            var controller = new DeliveryController(deliveryService, mapper); ;
            var response = await controller.CancelDelivery(delivery.Id);
            var okResult = response as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var delivery1 = okResult.Value as DeliveryViewModel;
            Assert.Equal(DeliveryStatus.Cancelled, Enum.Parse<DeliveryStatus>(delivery1.Status, true));
        }

        [Fact()]
        public async void ExpireDeliveryTest()
        {
            var delivery = new Delivery
            {
                OrderId = 10,
                Status = DeliveryStatus.Approved,
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
            };
            dbContext.Add(delivery);
            dbContext.SaveChanges();
            var controller = new DeliveryController(deliveryService, mapper); ;
            var response = await controller.ExpireDelivery(delivery.Id);
            var okResult = response as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var delivery1 = okResult.Value as DeliveryViewModel;
            Assert.Equal(DeliveryStatus.Expired, Enum.Parse<DeliveryStatus>(delivery1.Status, true));
        }

        [Fact()]
        public async void UpdateDeliveryStatusCannotSetStatusToCompletedIfNotApprovedTest()
        {
            var delivery = new Delivery
            {
                OrderId = 10,
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
            };
            dbContext.Add(delivery);
            dbContext.SaveChanges();

            var controller = new DeliveryController(deliveryService, mapper);
            var response = await controller.CompleteDelivery(delivery.Id);
            var badResult = response as BadRequestObjectResult;
            Assert.NotNull(badResult);
            Assert.Equal(400, badResult.StatusCode);
            Assert.NotNull(badResult.Value);
        }

        [Fact()]
        public async void UpdateDeliveryStatusCannotSetStatusToApprovedIfNotStatusIsCreatedTest()
        {
            var delivery = new Delivery
            {
                OrderId = 10,
                Status = DeliveryStatus.Expired,
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
            };
            dbContext.Add(delivery);
            dbContext.SaveChanges();
            var controller = new DeliveryController(deliveryService, mapper); ;
            var response = await controller.ApproveDelivery(delivery.Id);
            var badResult = response as BadRequestObjectResult;
            Assert.NotNull(badResult);
            Assert.Equal(400, badResult.StatusCode);
            Assert.NotNull(badResult.Value);
        }

        [Fact()]
        public async void UpdateDeliveryStatusCanNotSetStatusToCancelIfStatusIsNotCompletedOrExpiredTest()
        {
            var delivery = new Delivery
            {
                OrderId = 10,
                Status = DeliveryStatus.Expired,
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
            };
            dbContext.Add(delivery);
            dbContext.SaveChanges();
            var controller = new DeliveryController(deliveryService, mapper); ;
            var response = await controller.CancelDelivery(delivery.Id);
            var badResult = response as BadRequestObjectResult;
            Assert.NotNull(badResult);
            Assert.Equal(400, badResult.StatusCode);
            Assert.NotNull(badResult.Value);
        }

        [Fact()]
        public async void ApproveDeliveryStatusReturnValidResponseIfDeliveryNotFoundTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var response = await controller.ApproveDelivery(-9);
            var notFoundResult = response as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact()]
        public async void CompleteDeliveryStatusReturnValidResponseIfDeliveryNotFoundTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var response = await controller.CompleteDelivery(-10);
            var notFoundResult = response as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact()]
        public async void CancelDeliveryStatusReturnValidResponseIfDeliveryNotFoundTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var response = await controller.CancelDelivery(-1);
            var notFoundResult = response as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact()]
        public async void ExpireDeliveryStatusReturnValidResponseIfDeliveryNotFoundTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var response = await controller.ExpireDelivery(-2);
            var notFoundResult = response as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }


        [Fact()]
        public async void DeleteTest()
        {
            var controller = new DeliveryController(deliveryService, mapper); ;
            var response = await controller.Delete(1);
            var okResult = response as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var delivery = okResult.Value as Delivery;
            Assert.Equal(1, delivery.Id);
        }

        [Fact()]
        public async void DeleteReturnValidResponseIfDeliveryNotFoundTest()
        {
            var controller = new DeliveryController(deliveryService, mapper);
            var response = await controller.Delete(-1);
            var notFoundResult = response as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);

        }
    }
}