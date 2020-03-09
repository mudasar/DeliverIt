using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DeliverIt.Models;
using DeliverIt.Services;
using DeliverIt.ViewModels.Partner;
using Microsoft.AspNetCore.Mvc;

namespace DeliverIt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnerController : ControllerBase
    {
        private readonly IPartnerService partnerService;
        private readonly IMapper mapper;
        public PartnerController(IPartnerService partnerService, IMapper mapper)
        {
            this.mapper = mapper;
            this.partnerService = partnerService;
        }


        // GET: api/Partner
        [HttpGet]
        public async Task<IEnumerable<Partner>> GetAll()
        {
            return await this.partnerService.GetAllPartners();
        }

        // GET: api/Partner/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (await this.partnerService.PartnerExists(id))
            {
                var partner = await partnerService.GetPartnerById(id);
                return Ok(partner);
            }
            else
            {
                return NotFound($"Partner with id {id} was not found");
            }
        }

        // POST: api/Partner
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreatePartnerViewModel model)
        {
            if (await partnerService.PartnerExists(model.Name))
            {
                return BadRequest("Partner already exists");
            }

            var newPartner = new Partner()
            {
                Name = model.Name,

            };
            var partner = await partnerService.CreatePartner(newPartner);
            return Ok(partner);
        }

        // PUT: api/Partner/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdatePartnerViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound($"Partner with id {id} was not found");
            }

            if (await partnerService.PartnerExists(id))
            {
                var partner = await partnerService.GetPartnerById(id);

                partner.Name = model.Name;
                var updatedPartner = await partnerService.UpdatePartner(partner);
                return Ok(updatedPartner);
            }
            else
            {
                return NotFound($"Partner with id {id} was not found");
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await partnerService.PartnerExists(id))
            {
                var partner = await partnerService.RemovePartner(id);
                return Ok(partner);
            }
            else
            {
                return NotFound($"Partner with id {id} was not found");
            }
        }
    }
}
