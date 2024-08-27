using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.User;
using api.Helpers;
using api.Interfaces;
using api.Models;
using AutoMapper;
using BCrypt.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace api.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly string _secretKey;
        public AuthRepository(ApplicationDbContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _secretKey = configuration["Jwt:Key"]
                ?? throw new ArgumentNullException(nameof(configuration), "JWT Secret Key not configured");

        }
        public async Task<User?> CreateNewUser(CreateUserDto createUserDto)
        {
            // check if email is already in db
            var currentUser = await UserExists(createUserDto.Email);

            if (currentUser)
            {
                return null;
            }

            var hashedPassword = PasswordHasher.HashPassword(createUserDto.Password);

            // create new user
            var newUser = new User { UserName = createUserDto.Email, Email = createUserDto.Email, PasswordHash = hashedPassword };

            // add to db
            await _context.Users.AddAsync(newUser);

            // save changes
            await _context.SaveChangesAsync();

            // return new user
            return newUser;

        }

        public async Task<LoginResponseDto?> LoginUser(LoginUserDto loginUserDto)
        {
            // check if user exists
            var currentUser = await _context.Users.FirstOrDefaultAsync(user => user.Email == loginUserDto.Email);

            // if user does not exist, return conflict
            if (currentUser == null)
            {
                return null;
            }

            // Check if PasswordHash exists
            if (string.IsNullOrEmpty(currentUser.PasswordHash))
            {
                return null;
            }

            // verify password
            if (!PasswordHasher.VerifyPassword(loginUserDto.Password, currentUser.PasswordHash))
            {
                return null;
            }

            // if user does exist and password is verified, generate jwt
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.Email, loginUserDto.Email)
                    }
                ),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(securityToken);

            var currentUserDto = _mapper.Map<UserDto>(currentUser);

            return new LoginResponseDto
            {
                User = currentUserDto,
                Token = jwt,
            };
        }

        public async Task<bool> UserExists(string email)
        {
            return await _context.Users.AnyAsync(user => user.Email == email);
        }
    }
}