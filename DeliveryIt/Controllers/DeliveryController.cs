using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DeliverIt.Models;
using DeliverIt.Services;
using DeliverIt.ViewModels;
using DeliverIt.ViewModels.Delivery;
using DeliverIt.ViewModels.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DeliverIt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly IDeliveryService deliveryService;
        private readonly IMapper mapper;
        public DeliveryController(IDeliveryService deliveryService, IMapper mapper)
        {
            this.mapper = mapper;
            this.deliveryService = deliveryService;
        }


        // GET: api/Delivery
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var deliveries = await deliveryService.GetAllDeliveries();
            return Ok(mapper.Map<List<DeliveryViewModel>>(deliveries));
        }

        // GET: api/Delivery/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (await deliveryService.DeliveryExists(id))
            {
                var delivery = await deliveryService.GetDeliveryById(id);
                var model = mapper.Map<DeliveryViewModel>(delivery);
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
            if (await deliveryService.DeliveryExists(model.OrderId))
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

            var newDelivery = mapper.Map<Delivery>(model);
          
            var delivery = await deliveryService.CreateDelivery(newDelivery);

            var deliveryModel = mapper.Map<DeliveryViewModel>(delivery);
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

            if (await deliveryService.DeliveryExists(id))
            {
                var delivery = await deliveryService.GetDeliveryById(id);
                delivery.Status = model.Status;
                delivery.AccessWindow.StartTime = model.StartTime;
                delivery.AccessWindow.EndTime = model.EndTime;
                delivery = await deliveryService.UpdateDelivery(delivery);
                var deliveryModel = mapper.Map<DeliveryViewModel>(delivery);
                return Ok(deliveryModel);
            }
            else
            {
                return NotFound($"Delivery with id {id} was not found");
            }
        }

        // PUT: api/Delivery/5
        [HttpPut("approve-delivery/{id}")]
        public async Task<IActionResult> ApproveDelivery(int id)
        {
            if (await deliveryService.DeliveryExists(id))
            {
                var delivery = await deliveryService.GetDeliveryById(id);
                if (delivery.Status == DeliveryStatus.Created)
                {
                    var deliveryModel = await UpdateDeliveryStatus(id, DeliveryStatus.Approved);
                    return Ok(deliveryModel);
                }
                return BadRequest($"Delivery cannot be approved");
            }
            else
            {
                return NotFound($"Delivery with id {id} was not found");
            }
        }



        // PUT: api/Delivery/5
        [HttpPut("cancel-delivery/{id}")]
        public async Task<IActionResult> CancelDelivery(int id)
        {
            if (await deliveryService.DeliveryExists(id))
            {
                var delivery = await deliveryService.GetDeliveryById(id);
                if (delivery.Status == DeliveryStatus.Approved || delivery.Status == DeliveryStatus.Created || delivery.Status == DeliveryStatus.Completed)
                {
                    var deliveryModel = await UpdateDeliveryStatus(id, DeliveryStatus.Cancelled);
                    return Ok(deliveryModel);
                }
                return BadRequest($"Delivery cannot be cannceled if expired");
            }
            else
            {
                return NotFound($"Delivery with id {id} was not found");
            }
        }


        [HttpPut("complete-delivery/{id}")]
        public async Task<IActionResult> CompleteDelivery(int id)
        {
            if (await deliveryService.DeliveryExists(id))
            {
                var delivery = await deliveryService.GetDeliveryById(id);
                if (delivery.Status == DeliveryStatus.Approved)
                {
                    var deliveryModel = await UpdateDeliveryStatus(id, DeliveryStatus.Completed);
                    return Ok(deliveryModel);
                }
                return BadRequest($"Delivery cannot be completed");
            }
            else
            {
                return NotFound($"Delivery with id {id} was not found");
            }
        }

        [HttpPut("expire-delivery/{id}")]
        public async Task<IActionResult> ExpireDelivery(int id)
        {
            if (await deliveryService.DeliveryExists(id))
            {
                var deliveryModel = await UpdateDeliveryStatus(id, DeliveryStatus.Expired);
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
            if (await deliveryService.DeliveryExists(id))
            {
                var delivery = await deliveryService.RemoveDelivery(id);
                return Ok(delivery);
            }
            else
            {
                return NotFound($"Delivery with id {id} was not found");
            }
        }


        private async Task<DeliveryViewModel> UpdateDeliveryStatus(int id, DeliveryStatus status)
        {
            var delivery = await deliveryService.GetDeliveryById(id);
            delivery.Status = status;
            delivery = await deliveryService.UpdateDelivery(delivery);
            var deliveryModel = mapper.Map<DeliveryViewModel>(delivery);
            return deliveryModel;
        }
    }
}
