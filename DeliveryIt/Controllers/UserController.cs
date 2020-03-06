using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeliveryIt.Models;
using DeliveryIt.ViewModels;
using DeliveryIt.ViewModels.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryIt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly List<User> users = new List<User>()
        {
            new User()
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Address = "Test Street, London",
                Email = "john.doe@google.com",
                Phone  = "4475123456"
            }
        };

        // GET: api/User
        [HttpGet]
        public async Task<IEnumerable<User>> Get()
        {
            return users;
        }

        // GET: api/User/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<IActionResult> Get(int id)
        {
            if (users.Any(x => x.Id == id))
            {
                var user = users.FirstOrDefault(x => x.Id == id);
                return Ok(user);
            }
            else
            {
                return NotFound($"User with id {id} was not found");
            }

        }

        // POST: api/User
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateUserViewModel model)
        {
            if (users.Any(x => x.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest("User already exists");
            }

            var user = new User()
            {
                Id = users.Count + 1,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address
            };
            users.Add(user);
            return Ok(user);
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateUserViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound($"User with id {id} was not found");
            }

            if (users.Any(x => x.Id == id))
            {
                var user = users.FirstOrDefault(x => x.Id == id);

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.Phone = model.Phone;
                user.Address = model.Address;


                return Ok(user);
            }
            else
            {
                return NotFound($"User with id {id} was not found");
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (users.Any(x => x.Id == id))
            {
                var user = users.FirstOrDefault(x => x.Id == id);
                users.Remove(user);
                return Ok(user);
            }
            else
            {
                return NotFound($"User with id {id} was not found");
            }
        }
    }
}
