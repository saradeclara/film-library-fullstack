using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.User;
using api.Enums;
using api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthRepository userRepo, IMapper mapper, ILogger<AuthController> logger)
        {
            _authRepo = userRepo;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Create([FromBody] CreateUserDto createUserDto)
        {
            // check dto is valid
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var createdUserModel = await _authRepo.CreateNewUser(createUserDto);

                if (createdUserModel == null)
                {
                    return Conflict("A user with this email already exists");
                }

                // map model to dto to return to user
                var createdUserDto = _mapper.Map<UserDto>(createdUserModel);
                _logger.LogInformation("User created succesfully: {Email}", createdUserDto.Email);

                return CreatedAtAction(
                    nameof(UserController.GetById),
                    "User",
                    new { id = createdUserModel?.Id },
                    createdUserDto
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating user");
                return StatusCode(500, "An error occurred while processing your request.");
            }

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
        {

            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login attempt");
                return StatusCode(500, "An error occurred while processing your request.");
            }


        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var logoutResult = await _authRepo.LogoutUser();

            if (logoutResult != LogoutResult.Success)
            {
                _logger.LogError("User was not logged out");
                return Ok("User was not logged out successfully");
            }
            _logger.LogInformation("User was logged out");
            return Ok("User was logged out successfully");
        }
    }
}