using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.User;
using api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
        public async Task<IActionResult> Create(CreateUserDto createUserDto)
        {
            // check dto is valid
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                // invoke service
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
            catch (Exception _ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "An unexpected error occurred while creating the user" });
            }
        }





        // [HttpPost("login")]
    }
}