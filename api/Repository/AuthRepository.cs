using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.User;
using api.Enums;
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

        public async Task<(LoginResult Result, string? Token)> LoginUser(LoginUserDto loginUserDto)
        {
            // check if user exists
            var currentUser = await _context.Users.FirstOrDefaultAsync(user => user.Email == loginUserDto.Email);

            // if user does not exist, return conflict
            if (currentUser == null)
            {
                return (LoginResult.UserNotFound, null);
            }

            // Check if PasswordHash exists
            if (string.IsNullOrEmpty(currentUser.PasswordHash))
            {
                return (LoginResult.InvalidCredentials, null);
            }

            // verify password
            if (!PasswordHasher.VerifyPassword(loginUserDto.Password, currentUser.PasswordHash))
            {
                return (LoginResult.InvalidPassword, null);
            }

            // check account is not locked
            if (currentUser.IsLocked)
            {
                return (LoginResult.AccountLocked, null);
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

            return (LoginResult.Success, jwt);
        }

        public async Task<bool> UserExists(string email)
        {
            return await _context.Users.AnyAsync(user => user.Email == email);
        }
    }
}