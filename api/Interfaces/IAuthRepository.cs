using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.User;
using api.Enums;
using api.Models;

namespace api.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> CreateNewUser(CreateUserDto createUserDto);
        Task<(LoginResult Result, string? Token)> LoginUser(LoginUserDto loginUserDto);
        Task<bool> UserExists(string email);
    }
}