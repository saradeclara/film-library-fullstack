using System;
using System.Collections.Generic;
using System.Linq;
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

namespace api.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public AuthRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<User?> CreateNewUser(CreateUserDto createUserDto)
        {
            // check if email is already in db
            var isUserExist = await UserExists(createUserDto.Email);

            if (isUserExist)
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

        public async Task<User?> GetUserById(string id)
        {
            var foundUser = await _context.Users.FindAsync(id);

            if (foundUser == null)
            {
                return null;
            }

            return foundUser;
        }

        public async Task<bool> UserExists(string email)
        {
            return await _context.Users.AnyAsync(user => user.Email == email);
        }
    }
}