using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DeliverIt.Helpers;
using DeliverIt.Models;
using DeliverIt.Services;
using DeliverIt.ViewModels.Partner;
using DeliverIt.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DeliverIt.Controllers
{
    /// <summary>
    /// TOdo implement a admin user type authentication
    /// </summary>
    //[Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class PartnerController : ControllerBase
    {
        private readonly IPartnerService partnerService;
        private readonly IMapper mapper;
        private readonly AppSettings appSettings;

        public PartnerController(IPartnerService partnerService, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            this.mapper = mapper;
            this.partnerService = partnerService;
            this.appSettings = appSettings.Value;
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Authenticate([FromBody] PartnerLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var partner = await partnerService.AuthenticatePartner(model.Name, model.Password);
            if (partner != null)
            {
                // Generate Jwt token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, partner.Id.ToString()),
                        new Claim(ClaimTypes.Name, partner.Name),
                        new Claim(ClaimTypes.Role, "partner")
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return Ok(new { token = tokenHandler.WriteToken(token) });
            }
            else
            {
                return Unauthorized("Incorrect Email or Password");
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreatePartnerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await partnerService.PartnerExists(model.Name))
            {
                return BadRequest("Partner already exists");
            }

            var partner = mapper.Map<Partner>(model);
            partner = await partnerService.CreatePartner(partner);
            if (partner != null)
            {
                return Ok(partner);
            }
            else
            {
                return BadRequest("Unable to register please contact System admin");
            }
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            } 

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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            } 

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
