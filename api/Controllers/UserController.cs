using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.User;
using api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;
        public UserController(IUserRepository userRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _mapper = mapper;
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetById(string id)
        {
            var foundUser = await _userRepo.GetUserById(id);

            if (foundUser == null)
            {
                return NotFound();
            }

            // map model to dto to return to user
            var foundUserDto = _mapper.Map<UserDto>(foundUser);

            return Ok(foundUserDto);
        }
    }
}