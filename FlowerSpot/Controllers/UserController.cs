using FlowerSpot.Models;
using Microsoft.AspNetCore.Mvc;
using FlowerSpot.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Net.Http.Json;
using Newtonsoft.Json.Linq;

namespace FlowerSpot.Controllers
{
    // We can have something like [Route("[controller]")], but I like having my endpoints in lowercase
    [ApiController]
    [Route("users")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<User>> RegisterUser(User user, [FromServices] IUserService userService)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await userService.GetUserByUsernameAsync(user.Username);
                if (existingUser != null)
                {
                    return BadRequest("Username is already taken.");
                }
                try
                {
                    var createdUser = await userService.CreateUserAsync(user);
                    return CreatedAtAction(nameof(RegisterUser), new { id = createdUser.Id }, createdUser);
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to save user to database");
                }
            }
            return BadRequest(ModelState);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id, [FromServices] IUserService userService)
        {
            var user = await userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<User>> UpdateUser(int id, [FromBody] JObject user)
        {
            var userToUpdate = await _userService.GetUserByIdAsync(id);

            if (userToUpdate == null)
            {
                return NotFound();
            }
            bool updatePass = false;
            userToUpdate.Username = (string)user["username"] ?? userToUpdate.Username;
            userToUpdate.Email = (string)user["email"] ?? userToUpdate.Email;
            if (user.GetValue("password") != null)
            {
                userToUpdate.Password = (string)user["password"];
                updatePass = true;
            }

            if (!TryValidateModel(userToUpdate))
            {
                return BadRequest(ModelState);
            }

            var updatedUser = await _userService.UpdateUserAsync(userToUpdate, updatePass);

            if (updatedUser == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update user in database");
            }

            return Ok(updatedUser);
        }


        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

    }
}
