using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserServices.DTOs;
using UserServices.Models;
using UserServices.Repositories;
using UserServices.Validators;
using static UserServices.Models.jwtSettings;

namespace UserServices.Services
{
    public class UserService
    {
        private readonly IUserRepository _repository;
        private readonly JwtSettings _jwtSettings;
        public UserService(IUserRepository repository, IOptions<JwtSettings> options) 
        {
            _repository = repository;
            _jwtSettings = options.Value;
        }

        public async Task<User> CreateUserAsync(RegisterDTO userDto)
        {
            var validator = new RegisterDTOValidator();
            var validationResult = await validator.ValidateAsync(userDto);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.FirstOrDefault()?.ErrorMessage;
                throw new ValidationException(errors);
            }

            var existingUser = await _repository.GetByEmailAsync(userDto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("The email is already in use.");
            }
            var user = new User
            {
                Name = userDto.Name,
                Email = userDto.Email,
                Password = userDto.Password,
                CreatedAt = DateTime.UtcNow,
            };
            await _repository.AddAsync(user);
            return user;
        }

        public async Task<User> LoginUserAsync(LoginDTO loginDTO)
        {
            var user = await _repository.GetByEmailAsync(loginDTO.Email);
            if (user == null || user.Password != loginDTO.Password)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            return user; 
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user != null)
            {
                await _repository.DeleteAsync(user);
            }
        }   

        public async Task<User> UpdateUserAsync(int id, UpdateDTO updateDTO) 
        {
            return await _repository.UpdateAsync(id, updateDTO);
        }

        public string GetToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("Name", user.Name)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = _jwtSettings.Audience, 
                Issuer = _jwtSettings.Issuer
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


    }
}
