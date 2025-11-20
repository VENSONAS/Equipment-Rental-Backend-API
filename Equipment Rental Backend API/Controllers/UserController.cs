using Microsoft.AspNetCore.Mvc;
using Mapster;
using Service;
using Domain.Models;
using Equipment_Rental_Backend_API.Dtos.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equipment_Rental_Backend_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetAllUsers();
            var userDtos = users.Adapt<IEnumerable<UserDto>>();
            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            var userDto = user.Adapt<UserDto>();
            return Ok(userDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> UpdateUser(int id, UpdateUserDto userDto)
        {
            var user = userDto.Adapt<User>();
            var updatedUser = await _userService.UpdateUser(id, user);

            if (updatedUser == null)
                return NotFound();

            var updatedUserDto = updatedUser.Adapt<UserDto>();
            return Ok(updatedUserDto);
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto createUserDto)
        {
            var user = createUserDto.Adapt<User>();
            var createdUser = await _userService.CreateUser(user);
            var createdUserDto = createdUser.Adapt<UserDto>();

            return CreatedAtAction(
                nameof(GetUserById),
                new { id = createdUserDto.Id },
                createdUserDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUser(id);
            return NoContent();
        }
    }
}
