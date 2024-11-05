using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.ComponentModel.DataAnnotations;
using UserServices.DTOs;
using UserServices.Exceptions;
using UserServices.Models;
using UserServices.Repositories;
using UserServices.Services;

namespace UserServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }


        // user/register
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] RegisterDTO user)
        {
            try
            {
                var newUser = await _userService.CreateUserAsync(user);
                return CreatedAtAction(nameof(Register), new { id = newUser.Id }, newUser);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message }); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message }); 
            }
        }

        //POST: user/login
        [HttpPost("login")]
        public async Task<ActionResult<User>> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                var user = await _userService.LoginUserAsync(loginDTO);
                var jwtToken = _userService.GetToken(user); 

                return Ok(new { UserId = user.Id, UserName = user.Name, jwtToken }); 
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message }); 
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, new { Message = "An unexpected error occurred." }); 
            }
        }

        // user/delete/id
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<User>> Delete(int id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                return Ok("Usuario borrado correctamente");
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });

            }

        }

        //user/update/id
        [HttpPut("update/{id}")]
        public async Task<ActionResult<User>> Update(int id, [FromBody] UpdateDTO updateDTO)
        {
            try
            {
                var updatedUser = await _userService.UpdateUserAsync(id, updateDTO);
                return Ok(updatedUser);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });

            }
            catch (ValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { Message = "An error occurred while updating the user.", Details = ex.Message });
            }
        }

        
    }
}


