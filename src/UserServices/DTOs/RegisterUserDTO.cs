﻿using System.ComponentModel.DataAnnotations;

namespace UserServices.DTOs
{
    public class RegisterUserDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
