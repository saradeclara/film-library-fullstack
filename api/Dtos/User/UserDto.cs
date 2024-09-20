using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.User
{
    public class UserDto
    {
        public string Email { get; set; } = string.Empty;
        public int Id { get; set; }
    }
}