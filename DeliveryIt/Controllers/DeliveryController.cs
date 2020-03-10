using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DeliverIt.Helpers;
using DeliverIt.Models;
using DeliverIt.Services;
using DeliverIt.ViewModels;
using DeliverIt.ViewModels.Delivery;
using DeliverIt.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
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

        /// <summary>
        /// Load all the deliveries in system if user is logged in it 'll filter for logged in user or partner id
        /// </summary>
        /// <returns></returns>
        // GET: api/Delivery
        [Authorize(Roles = "user,partner")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IList<Delivery> deliveries;
            if (User.HasRole("partner"))
            {
                var partnerId = User.GetId();
                deliveries = await deliveryService.GetAllDeliveriesForPartner(int.Parse(partnerId));
            }
            else if (User.HasRole("user"))
            {
                var userId = User.GetId();
                deliveries = await deliveryService.GetAllDeliveriesForUser(int.Parse(userId));
            }
            else
            {
                deliveries = await deliveryService.GetAllDeliveries();
            }

            return Ok(mapper.Map<List<DeliveryViewModel>>(deliveries));
        }

        // GET: api/Delivery/5
        [Authorize(Roles = "user,partner")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            //if (!User.Identity.IsAuthenticated)
            //{
            //    return Unauthorized($"You are not authorized to get this resource");
            //}
            if (await deliveryService.DeliveryExists(id))
            {
                var delivery = await deliveryService.GetDeliveryById(id);
                return Ok(MakeDeliveryViewModel(delivery));
            }
            else
            {
                return NotFound($"Delivery with id {id} was not found");
            }

        }

        private DeliveryViewModel MakeDeliveryViewModel(Delivery delivery)
        {
            return mapper.Map<DeliveryViewModel>(delivery);

        }

        /// <summary>
        /// Create a new Delivery
        /// only partners can create a new delivey
        /// </summary>
        /// <param name="model">Returns DeliveryViewModel Resource</param>
        /// <returns></returns>
        // POST: api/Delivery
        [Authorize(Roles = "partner")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateDeliveryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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
            // TODO: needs a better strategy
            var deliveryModel = mapper.Map<DeliveryViewModel>(await deliveryService.GetDeliveryById(delivery.Id));
            return Ok(deliveryModel);
        }

        /// <summary>
        ///   Updates a Delivery
        ///   can only update AccessWindow and Delivery Status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        // PUT: api/Delivery/5
        [Authorize(Roles = "partner")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateDeliveryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

        /// <summary>
        ///  Approve a delivery
        ///  only user can approve
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "user")]
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

        /// <summary>
        /// A User or Partner can Cancel a delivery
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "user,partner")]
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

        /// <summary>
        /// A partner can complete a delivery
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "partner")]
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

        /// <summary>
        /// This method will only be used by a service worker for automatic expiration
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Only a partner can remove a delivery
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "partner")]
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
            return MakeDeliveryViewModel(delivery);
        }
    }
}
