using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.User;
using api.Enums;
using api.Interfaces;
using api.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<UserController> _logger;
        public UserController(IUserRepository userRepo, IMapper mapper, ILogger<UserController> logger)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll(
            [FromQuery] string? sortBy,
            [FromQuery] bool isDescending = false,
            [FromQuery][Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0.")] int pageNumber = 1,
            [FromQuery][Range(1, 100, ErrorMessage = "Page size must be between 1 and 100.")] int pageSize = 10)
        {
            var resultWithData = await _userRepo.GetAllUsersAsync(sortBy, isDescending, pageNumber, pageSize);

            _logger.LogInformation("resultWithData: {resultWithData}", resultWithData.Result);

            return resultWithData.Result switch
            {
                Result.Success => Ok(_mapper.Map<List<UserDto>>(resultWithData.Data)),
                Result.Failure => StatusCode(500, "An error occurred while fetching users"),
                _ => StatusCode(500, "An unexpected error occurred")
            };
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var foundUser = await _userRepo.GetUserByIdAsync(id);
            _logger.LogInformation("from route: {id}", id);
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