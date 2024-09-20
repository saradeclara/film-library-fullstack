using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using api.Data;
using api.Enums;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace api.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserRepository> _logger;
        public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ResultWithData<List<User>>> GetAllUsersAsync(string? sortBy, bool isDescending, int pageNumber, int pageSize)
        {
            var query = _context.Users.AsQueryable();

            // if sortBy is available, apply querying, then pagination
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                if (!isValidSortColumn(sortBy))
                {
                    return new ResultWithData<List<User>>(Result.InvalidSortColumn, null);
                }
                // sortBy & isDescending
                query = ApplyUserSorting(query, sortBy, isDescending);
            }
            // if sortBy is not available, go straight to pagination
            // pagination
            query = ApplyPagination(query, pageNumber, pageSize);

            var users = await query.ToListAsync();

            return new ResultWithData<List<User>>(Result.Success, users);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            var foundUser = await _context.Users.FirstOrDefaultAsync(user => user.Id == id);

            if (foundUser == null)
            {
                return null;
            }

            return foundUser;
        }

        private bool isValidSortColumn(string sortBy)
        {
            var allowedColumns = new[] { "id", "username", "email" };
            return allowedColumns.Contains(sortBy.ToLower());
        }

        private IQueryable<User> ApplyUserSorting(
            IQueryable<User> query,
            string sortBy,
            bool isDescending
            )
        {
            switch (sortBy.ToLower())
            {
                case "id":
                    query = isDescending ? query.OrderByDescending(user => user.Id) : query.OrderBy(user => user.Id);
                    break;
                case "username":
                    query = isDescending ? query.OrderByDescending(user => user.UserName) : query.OrderBy(user => user.UserName);
                    break;
                case "email":
                    query = isDescending ? query.OrderByDescending(user => user.Email) : query.OrderBy(user => user.Email);
                    break;
                default:
                    throw new ArgumentException("Invalid sort column.");
            }

            return query;
        }

        private IQueryable<User> ApplyPagination(IQueryable<User> query, int pageNumber, int pageSize)
        {
            var takeNumber = pageSize;
            var skipNumber = (pageNumber - 1) * pageSize;

            return query.Skip(skipNumber).Take(takeNumber);
        }
    }
}