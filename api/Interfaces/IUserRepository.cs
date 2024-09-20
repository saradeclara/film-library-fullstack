using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Helpers;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Interfaces
{
    public interface IUserRepository
    {
        Task<ResultWithData<List<User>>> GetAllUsersAsync(string? sortBy, bool isDescending, int pageNumber, int pageSize);
        Task<User?> GetUserByIdAsync(int id);
    }
}