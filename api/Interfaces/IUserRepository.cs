using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace api.Interfaces
{
    public interface IUserRepository
    {
        Task<IActionResult> GetUserById(string id);
    }
}