using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeliveryIt.Models;
using DeliveryIt.ViewModels;
using DeliveryIt.ViewModels.Delivery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryIt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly List<Delivery> deliveries = new List<Delivery>()
        {
            new Delivery()
            {
                Id = 1,
                OrderId = 1,

                Status =DeliveryStatus.Created,
                Sender = new Partner()
                {
                    Id = 1,
                    Name = "IKea"

                },
                Recipient = new User()
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Phone = "0766574747",
                    Email = "john.doe@google.com",
                    Address = "123 Street, London"
                },
                AccessWindow = new AccessWindow()
                {
                    StartTime = DateTime.Now.AddHours(5),
                    EndTime = DateTime.Now.AddHours(7),
                }
            }
        };

        public DeliveryController()
        {

        }

        // GET: api/Delivery
        [HttpGet]
        public async Task<IEnumerable<Delivery>> Get()
        {
            return deliveries;
        }

        // GET: api/Delivery/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<IActionResult> Get(int id)
        {
            if (deliveries.Any(x => x.Id == id))
            {
                var delivery = deliveries.FirstOrDefault(x => x.Id == id);

                // TODO use automapper to map the objects

                var model = new DeliveryViewModel();
                model.Id = delivery.Id;
                model.Status = delivery.Status;
                model.Order = new OrderViewModel
                {
                    OrderNumber = delivery.OrderId.ToString(),
                    Sender = delivery.Sender.Name
                };
                model.Recipient = new ViewModels.User.UserViewModel
                {
                    Name = delivery.Recipient.Name,
                    Address = delivery.Recipient.Address,
                    PhoneNumber = delivery.Recipient.Phone,
                };
                model.AccessWindow = delivery.AccessWindow;

                return Ok(model);
            }
            else
            {
                return NotFound($"Delivery with id {id} was not found");
            }

        }

        // POST: api/Delivery
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateDeliveryViewModel model)
        {
            if (deliveries.Any(x => x.OrderId == model.OrderId))
            {
                return BadRequest("Delivery for this order already exists");
            }

            if (DateTime.Now > model.EndTime)
            {
                return BadRequest($"{nameof(model.EndTime)} is already expired");
            }

            if (model.StartTime > model.EndTime)
            {
                return BadRequest($"{nameof(model.StartTime)} cannot be greater than {nameof(model.EndTime)}");
            }

            var delivery = new Delivery()
            {
                Id = deliveries.Count + 1,
                Status = DeliveryStatus.Created,
                AccessWindow = new AccessWindow
                {
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                }
            };

            deliveries.Add(delivery);
            // TODO: use automapper
            var deliveryModel = new DeliveryViewModel
            {
                Id = delivery.Id
            };
            return Ok(deliveryModel);
        }

        // PUT: api/Delivery/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateDeliveryViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound($"Delivery with id {id} was not found");
            }

            if (DateTime.Now > model.EndTime)
            {
                return BadRequest($"{nameof(model.EndTime)} is already expired");
            }

            if (model.StartTime > model.EndTime)
            {
                return BadRequest($"{nameof(model.StartTime)} cannot be greater than {nameof(model.EndTime)}");
            }

            if (deliveries.Any(x => x.Id == id))
            {
                var delivery = deliveries.FirstOrDefault(x => x.Id == id);
                delivery.Status = model.Status;
                delivery.AccessWindow = new AccessWindow
                {
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                };

                // TODO: use automapper
                var deliveryModel = new DeliveryViewModel
                {
                    Id = delivery.Id,
                    Status = delivery.Status
                };
                return Ok(deliveryModel);
            }
            else
            {
                return NotFound($"Delivery with id {id} was not found");
            }
        }

        // PUT: api/Delivery/5
        [HttpPut("update-status/{id}")]
        public async Task<IActionResult> UpdateDeliveryStatus(int id, [FromBody] DeliveryStatus status)
        {
            if (deliveries.Any(x => x.Id == id))
            {
                var delivery = deliveries.FirstOrDefault(x => x.Id == id);
                delivery.Status = status;

                // TODO: use automapper
                var deliveryModel = new DeliveryViewModel
                {
                    Id = delivery.Id,
                    Status = delivery.Status
                };
                return Ok(deliveryModel);
            }
            else
            {
                return NotFound($"Delivery with id {id} was not found");
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (deliveries.Any(x => x.Id == id))
            {
                var delivery = deliveries.FirstOrDefault(x => x.Id == id);
                deliveries.Remove(delivery);
                // TODO: add automapper
                return Ok(delivery);
            }
            else
            {
                return NotFound($"Delivery with id {id} was not found");
            }
        }
    }
}
