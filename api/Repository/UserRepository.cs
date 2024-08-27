using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace api.Repository
{
    public class UserRepository : IUserRepository
    {
        public Task<IActionResult> GetUserById(string id)
        {
            throw new NotImplementedException();
        }
    }
}