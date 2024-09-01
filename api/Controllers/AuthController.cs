using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.User;
using api.Enums;
using api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Serilog;

namespace api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;
        private readonly IMapper _mapper;
        public AuthController(IAuthRepository userRepo, IMapper mapper)
        {
            _authRepo = userRepo;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Create([FromBody] CreateUserDto createUserDto)
        {
            // check dto is valid
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var createdUserModel = await _authRepo.CreateNewUser(createUserDto);

            if (createdUserModel == null)
            {
                return Conflict("A user with this email already exists");
            }

            // map model to dto to return to user
            var createdUserDto = _mapper.Map<UserDto>(createdUserModel);

            return CreatedAtAction(
                nameof(UserController.GetById),
                "User",
                new { id = createdUserModel?.Id },
                createdUserDto
                );
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
        {

            var (result, token) = await _authRepo.LoginUser(loginUserDto);

            switch (result)
            {
                case LoginResult.Success:
                    return Ok(new { Token = token });
                case LoginResult.AccountLocked:
                    return BadRequest("Account is locked.");
                case LoginResult.InvalidCredentials:
                    return BadRequest("Credentials provided are not valid");
                case LoginResult.InvalidPassword:
                    return BadRequest("Password provided was not valid.");
                case LoginResult.UserNotFound:
                    return BadRequest("User was not found.");
                default:
                    return BadRequest("An error occured.");
            }

        }
    }
}