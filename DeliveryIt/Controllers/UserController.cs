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
using DeliverIt.ViewModels;
using DeliverIt.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class UserController : ControllerBase
    {

        private readonly IUserService userService;
        private readonly IMapper mapper;
        private readonly AppSettings appSettings;


        public UserController(IUserService userService, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            this.mapper = mapper;
            this.userService = userService;
            this.appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Authenticate([FromBody] UserLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await userService.AuthenticateUser(model.Email, model.Password);
            if (user != null)
            {
                // Generate Jwt token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim(ClaimTypes.Role, "user")
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return Ok(new { token= tokenHandler.WriteToken(token)});
            }
            else
            {
                return Unauthorized("Incorrect Email or Password");
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await userService.UserExists(model.Email))
            {
                return BadRequest("User already exists");
            }

            var user = mapper.Map<User>(model);
            user = await userService.CreateUser(user);
            if (user != null)
            {
                return Ok(user);
            }
            else
            {
                return BadRequest("Unable to register please contact System admin");
            }
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await userService.UserExists(model.Email))
            {
                return BadRequest("User already exists");
            }
            var newUser = mapper.Map<User>(model);
            var user = await userService.CreateUser(newUser);
            return Ok(user);
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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
                user.Password = model.Password;

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
