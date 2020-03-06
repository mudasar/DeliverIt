using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeliveryIt.Models;
using DeliveryIt.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryIt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnerController : ControllerBase
    {
        // GET: api/Partner
        [HttpGet]
        public async Task<IEnumerable<Partner>> Get()
        {
            return new List<Partner>()
            {
                new Partner()
            };
        }

        // GET: api/Partner/5
        [HttpGet("{id}", Name = "Get")]
        public async  Task<Partner> Get(int id)
        {
            return new Partner(){Id = 1};
        }

        // POST: api/Partner
        [HttpPost]
        public async Task<Partner> Post([FromBody] CreatePartnerViewModel model)
        {

            return new Partner() { Id = 1, Name = model.Name};
        }

        // PUT: api/Partner/5
        [HttpPut("{id}")]
        public async Task<Partner> Put(int id, [FromBody] UpdatePartnerViewModel model)
        {
            return new Partner() { Id = id, Name = model.Name };
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<Partner> Delete(int id)
        {
            return new Partner(){Id = id};
        }
    }
}
