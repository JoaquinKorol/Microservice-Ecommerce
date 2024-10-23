using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using UserServices.DTOs;
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
                return CreatedAtAction(nameof(Register), new { id = newUser.Id }, newUser); // 201 Created
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message }); // 400 Bad Request para errores de validación
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message }); // 409 Conflict para email en uso
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message }); // 500 Internal Server Error para cualquier otro error
            }
        }

        //POST: user/login
        [HttpPost("login")]
        public async Task<ActionResult<User>> Login([FromBody] LoginDTO loginDTO)
        {

            try
            {
                var user = await _userService.LoginUserAsync(loginDTO);
                var jwtToken = _userService.GetToken(user); // Genera el token JWT aquí

                return Ok(new { UserId = user.Id, UserName = user.Name, jwtToken }); // Devuelve el token
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message }); // Manejo de excepción para credenciales no válidas
            }
            catch (Exception ex)
            {
                // Aquí puedes registrar el error (por ejemplo, en la consola o en un archivo de log)
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, new { Message = "An unexpected error occurred." }); // Devuelve un error 500
            }
        }

        // user/delete/id
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<User>> Delete(int id)
        {
            
            await _userService.DeleteUserAsync(id);
            return Ok("Usuario borrado correctamente");
        }

        [HttpPut("update/{id}")]
        public async Task<ActionResult<User>> Update(int id, [FromBody] UpdateDTO updateDTO)
        {
            var updatedUser = await _userService.UpdateUserAsync(id, updateDTO);

            if (updatedUser == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            return Ok(updatedUser);
        }

        [Authorize]
        [HttpGet("protected-endpoint")]
        public IActionResult GetProtectedData()
        {
            return Ok("This is a protected endpoint");
        }
    }
}
