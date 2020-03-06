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
              
            }
        };

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

            var delivery = new Delivery()
            {
                Id = deliveries.Count + 1,

            };
            deliveries.Add(delivery);
            // TODO: use automapper

            return Ok(delivery);
        }

        // PUT: api/Delivery/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateDeliveryViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound($"Delivery with id {id} was not found");
            }

            if (deliveries.Any(x => x.Id == id))
            {
                var delivery = deliveries.FirstOrDefault(x => x.Id == id);

  

                return Ok(delivery);
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
