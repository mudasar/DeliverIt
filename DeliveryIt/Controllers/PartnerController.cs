using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeliveryIt.Models;
using DeliveryIt.ViewModels;
using DeliveryIt.ViewModels.Partner;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryIt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnerController : ControllerBase
    {
        private readonly List<Partner> _partners = new List<Partner>()
        {
            new Partner()
            {
                Id = 1,
                Name = "Ikea",

            }
        };

        // GET: api/Partner
        [HttpGet]
        public async Task<IEnumerable<Partner>> Get()
        {
            return _partners;
        }

        // GET: api/Partner/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<IActionResult> Get(int id)
        {
            if (_partners.Any(x => x.Id == id))
            {
                var user = _partners.FirstOrDefault(x => x.Id == id);
                return Ok(user);
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
            if (_partners.Any(x => x.Name.Equals(model.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest("Partner already exists");
            }

            var user = new Partner()
            {
                Id = _partners.Count + 1,
                Name = model.Name,

            };
            _partners.Add(user);
            return Ok(user);
        }

        // PUT: api/Partner/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdatePartnerViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound($"Partner with id {id} was not found");
            }

            if (_partners.Any(x => x.Id == id))
            {
                var partner = _partners.FirstOrDefault(x => x.Id == id);

                partner.Name = model.Name;

                return Ok(partner);
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
            if (_partners.Any(x => x.Id == id))
            {
                var user = _partners.FirstOrDefault(x => x.Id == id);
                _partners.Remove(user);
                return Ok(user);
            }
            else
            {
                return NotFound($"Partner with id {id} was not found");
            }
        }
    }
}
