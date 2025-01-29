using Core.Exceptions;
using Core.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserServices.DTOs;
using UserServices.Models;
using UserServices.Repositories;
using UserServices.Validators;
using static UserServices.Models.jwtSettings;

public class UserService
{
    private readonly IRepository<User> _repository;
    private readonly jwtSettings _jwtSettings;

    public UserService(IRepository<User> repository, IOptions<jwtSettings> options)
    {
        _repository = repository;
        _jwtSettings = options.Value;
    }

    public async Task<IEnumerable<UserDTO>> GetUsersAsync()
    {
        var users = await _repository.GetAllAsync();
        return users.Select(user => new UserDTO
        {
            Name = user.Name,
            Email = user.Email
        });
    }
    public async Task<RegisterUserDTO> CreateUserAsync(RegisterUserDTO userDto)
    {
        var validator = new RegisterDTOValidator();
        var validationResult = await validator.ValidateAsync(userDto);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.FirstOrDefault()?.ErrorMessage;
            throw new ValidationException(errors);
        }

        var existingUser = await GetUserByEmailAsync(userDto.Email); // Método para obtener usuario por email
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

        return new RegisterUserDTO
        {
            
            Name = user.Name,
            Email = user.Email
        };
    }

    public async Task<User> LoginUserAsync(LoginUserDTO loginDTO)
    {
        var user = await GetUserByEmailAsync(loginDTO.Email); 
        if (user == null || user.Password != loginDTO.Password)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        return user;
    }

    
    public async Task<User> GetUserByEmailAsync(string email)
    {
        var allUsers = await _repository.GetAllAsync();
        return allUsers.FirstOrDefault(user => user.Email == email);
    }

    public async Task DeleteUserAsync(int id)
    {
       
        var user = await _repository.GetByIdAsync(id);
        if (user == null)
        {
           
            throw new NotFoundException($"User with ID {id} not found.");
        }

     
        await _repository.DeleteAsync(id);
    }

    public async Task<UpdateUserDTO> UpdateUserAsync(int id, UpdateUserDTO updateDTO)
    {
        var existingUser = await _repository.GetByIdAsync(id);
        if (existingUser != null)
        {
            var validator = new UpdateDTOValidator();
            var validationResult = await validator.ValidateAsync(updateDTO);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors.FirstOrDefault()?.ErrorMessage);
            }

            existingUser.Name = updateDTO.Name ?? existingUser.Name;
            existingUser.Email = updateDTO.Email ?? existingUser.Email;
            existingUser.UpdatedAt = DateTime.Now;

           
            await _repository.UpdateAsync(existingUser);

            
            return new UpdateUserDTO
            {
                Name = existingUser.Name,
                Email = existingUser.Email
            };
        }
        throw new NotFoundException($"User with ID {id} not found.");
    }

    public string GenerateJwtToken(User user)
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