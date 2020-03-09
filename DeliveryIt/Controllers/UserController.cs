using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DeliverIt.Models;
using DeliverIt.Services;
using DeliverIt.ViewModels;
using DeliverIt.ViewModels.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DeliverIt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService userService;
        private readonly IMapper mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            this.mapper = mapper;
            this.userService = userService;
        }

        // GET: api/User
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await this.userService.GetAllUsers());
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (await userService.UserExists(id))
            {
                var user = await userService.GetUserById(id);
                return Ok(user);
            }
            else
            {
                return NotFound($"User with id {id} was not found");
            }

        }

        // POST: api/User
        /// <summary>
        /// Create a new User
        /// </summary>
        /// <param name="model"></param>
        /// <returns>A new User Object</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]     // Created
        [ProducesResponseType(StatusCodes.Status400BadRequest)]  // BadRequest
        public async Task<IActionResult> Post([FromBody] CreateUserViewModel model)
        {
            if (await userService.UserExists(model.Email))
            {
                return BadRequest("User already exists");
            }
            var newUser = new User()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address
            };
            var user = await userService.CreateUser(newUser);
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

            if (await userService.UserExists(id))
            {
                var user = await userService.GetUserById(id);

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.Phone = model.Phone;
                user.Address = model.Address;

                var updatedUser = await userService.UpdateUser(user);
                return Ok(updatedUser);
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
            if (await userService.UserExists(id))
            {
                var user = await userService.RemoveUser(id);
                return Ok(user);
            }
            else
            {
                return NotFound($"User with id {id} was not found");
            }
        }
    }
}
