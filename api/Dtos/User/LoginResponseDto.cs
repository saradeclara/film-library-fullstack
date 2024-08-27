using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Dtos.User
{
    public class LoginResponseDto
    {
        public UserDto? User { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}